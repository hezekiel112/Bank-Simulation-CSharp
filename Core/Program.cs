using System;
using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.InteropServices.JavaScript;
using System.Text.RegularExpressions;
using System.Xml.Linq;

public class Program {

    static Dictionary<ICommandProcessor, DiagnosticCases> Diagnostic = new Dictionary<ICommandProcessor, DiagnosticCases>();

    static string input = string.Empty;

    static BankInformations? bankInfos;

    static Diagnostic Diag = new(Diagnostic);

    static void Main() {

        Command? cmdNotFound = null;

        Command[] commands = {
            new CommandBase("checkbank", () => {
                
                if (bankInfos != null) {
                    EvaluateBank(bankInfos);

                    return;
                }

                Diag.Add(new CommandBase("Missing bank", null, null), DiagnosticCases.MissingBank);

            }, "evaluate the bank you've created"),

            new CommandBase("addbank", () => {

                if (bankInfos != null) {
                    Diag.Add(new CommandBase("Cannot create two bank (bank profile update might come soon :-) ) ", null, null), DiagnosticCases.MissingBank);
                    return;
                }

                string[] args = input.Split(" ");

                switch (args.Length) {

                case > 3:
                        case < 3:
                    Diag.Add(new CommandBase("Invalid argument", null, null), DiagnosticCases.MissingEssentialArgument);
                    return;
                }

                bankInfos = new(args[1], args[2].FirstOrDefault(), DateTime.Now);

                EvaluateBank(bankInfos);

            }, "add a bank"),

            // create a new user into the user collection
            new CommandBase("adduser", () => {
                string[] args = input.Split(" ");

                if (bankInfos == null) {
                    Diag.Add(new CommandBase("Cannot create an account without a bank", null, null), DiagnosticCases.MissingBank);
                    return;
                }

                if (bankInfos.GetUsersNonRegistred().Count() > 0) {
                    for (int i = 0; i < bankInfos.GetUsersNonRegistred().Count(); i++)
                    {
                        if (bankInfos.GetUsersNonRegistred().ElementAt(i).GetAccountName() == args[1]) {
                            Diag.Add(new CommandBase("Cannot create an account without a bank", null, null), DiagnosticCases.MissingBank);
                            return;
                        }
                    }
                }

                if (bankInfos.GetUsers().Count() > 0) {
                    for (int i = 0; i < bankInfos.GetUsers().Count(); i++)
                    {
                        if (bankInfos.GetUsers().ElementAt(i).GetAccount().GetAccountName() == args[1]) {
                            Diag.Add(new CommandBase("Another account has the same name as this one", null, null), DiagnosticCases.DoubleAccount);
                            return;
                        }
                    }
                }

                switch (args.Length) {

                case > 2:
                        case < 2:
                    Diag.Add(new CommandBase("Invalid argument", null, null), DiagnosticCases.MissingEssentialArgument);
                    return;
                }

                bankInfos.NonRegistredUsers.Add(new UserAccount(args[1]), false);

                Console.WriteLine($"added {args[1]} to collection");

            }, "create an user profile without creating him a bank account"),
            
            // register an user from the collection into the bank
            new CommandBase("register", () => {

                if (bankInfos == null) {
                    Diag.Add(new CommandBase("Cannot register an account without a bank", null, null), DiagnosticCases.MissingBank);
                    return;
                }

                string[] args = input.Split(" ");

                if (bankInfos.GetUsers().Count() > 0) {
                    for (int i = 0; i < bankInfos.GetUsers().Count(); i++)
                    {
                        if (bankInfos.GetUsers().ElementAt(i).GetAccount().GetAccountName() == args[1]) {
                            Diag.Add(new CommandBase("This account is already registred", null, null), DiagnosticCases.IncoherentAction);
                            return;
                        }
                    }
                }

                if (bankInfos.GetUsersNonRegistred().Count() > 0) {
                    for (int i = 0; i < bankInfos.GetUsersNonRegistred().Count(); i++)
                    {
                        if (bankInfos.GetUsersNonRegistred().ElementAt(i).GetAccountName() == args[1]) {

                            bankInfos.RegisterNewUser(bankInfos.GetUsersNonRegistred().ElementAt(i));
                            bankInfos.NonRegistredUsers.Remove(bankInfos.GetUsersNonRegistred().ElementAt(i));

                            Console.WriteLine("registred user");
                            return;
                        }
                    }
                }

                Diag.Add(new CommandBase("this user does not exist (Does he have a bank account ?)", null, null), DiagnosticCases.IncoherentAction);

            }, "register the user into the bank user collection"),

            new CommandBase("checkuser", () => {
                string[] args = input.Split(" ");

                if (bankInfos == null) {
                    Diag.Add(new CommandBase("Cannot register an account without a bank", null, null), DiagnosticCases.MissingBank);
                    return;
                }

                if (bankInfos.GetUsers().Count() <= 0) {
                    Diag.Add(new CommandBase("No user registred", null, null), DiagnosticCases.IncoherentAction);
                }

                foreach (var acc in bankInfos.GetUsers()) {
                          if (args.Any(x => acc.GetAccount().GetAccountName() == x)) {
                              if (bankInfos.GetUser(acc.GetAccount().GetAccountID()) != null) {
                                  var user = bankInfos.GetUser(acc.GetAccount().GetAccountID());

                                 Console.WriteLine($"{acc.GetAccount().GetAccountName()}'s PLAN: {user.CurrentEconomy.CurrentPlan}\n" +
                                      $"\tSITUATION: {user.CurrentEconomy.CurrentSituation}\n" +
                                      $"\tBALANCE: ${user.CurrentEconomy.Money}");
                              }
                          }
                          else {
                              Diag.Add(new CommandBase("this user does not exist (Does he have a bank account ?)", null, null), DiagnosticCases.IncoherentAction);
                              return;
                          }
                    }

            }, "display commons informations about an user"),

            new CommandBase("addmoney", () => {
                string[] args = input.Split(" ");

                if (bankInfos == null) {
                    Diag.Add(new CommandBase("Cannot register an account without a bank", null, null), DiagnosticCases.MissingBank);
                    return;
                }

                if (args.Length <= 2 || args.Length <= 1) {
                    Diag.Add(new CommandBase("Missing argument", null, null), DiagnosticCases.MissingEssentialArgument);

                    return;
                }

                if (bankInfos.GetUsers().Count() > 0) {
                    for (int i = 0; i < bankInfos.GetUsers().Count(); i++)
                    {
                        if (args.Any(x => x == bankInfos.GetUsers().ElementAt(i).GetAccount().GetAccountName())) {
                            var user = bankInfos.GetUser(bankInfos.GetUsers().ElementAt(i).GetAccount().GetAccountID());

                            var money = long.Parse(string.Concat(args[2].Where(Char.IsDigit).ToArray()));

                            user.CurrentEconomy.Money += money;

                            user.CurrentEconomy.EvaluateSituation();

                            Console.WriteLine($"{user.Name} received {money} \n {user.CurrentEconomy.Money - money} <->  {user.CurrentEconomy.Money}");
                        }

                        else {
                            Diag.Add(new CommandBase("This user does not exist (Does he have a bank account ?)", null, null), DiagnosticCases.IncoherentAction);
                        }
                    }
                }
                else {
                    Diag.Add(new CommandBase("There's no user with a bank account", null, null), DiagnosticCases.IncoherentAction);
                    return;
                }
            }, "give an amount of money to an user - use minus (-) to decrease the amount, by default (+) is ignored"),

            new CommandBase("evaluate", () => {
                string[] name = input.Split(" ");

                if (bankInfos == null) {
                    Diag.Add(new CommandBase("Cannot evaluate an account without a bank", null, null), DiagnosticCases.MissingBank);
                    return;
                }

                if (bankInfos.GetUsers().Count() > 0) {
                    foreach (var acc in bankInfos.Users) {
                        if (name.Any(x => acc.Key.GetAccount().GetAccountName() == x)) {
                            EvaluateUser(acc.Key.GetAccount());
                        }
                    }
                }
                else {
                    return;
                }
            }, "shows all informations about an user"),
        };

        while (true) {

            Console.Write("> ");

            input = Console.ReadLine() ?? "";

            if (input == "exit")
                break;

            if (bankInfos != null) {
                Console.Title = bankInfos.Name + "'s Bank";
            }

            if (input != string.Empty) {

                for (int i = 0; i < commands.Length; i++) {
                    if (input.StartsWith(commands[i].Input)) {
                        commands[i].Recognize(input, commands[i]);
                    }
                }
            }

            if (input == "/help") {
                Console.WriteLine("If a command option or name start with '!' it means that it Is optional");
                foreach (var cmd in commands) {
                    Console.WriteLine("------------------------");

                    Console.WriteLine($"{cmd.Input} ; {cmd.Description}");

                    Console.WriteLine("------------------------");
                }

                if (bankInfos is null) {
                    Console.WriteLine($"start with : addbank [name] [!currency]");
                }

                continue;
            }

            if (input == "/program") {
                Console.WriteLine("Bank simulation ; application created by Hezek112\n\t\t  Version 1.0-stable");
            }

            if (Diagnostic.Count != 0) {
                Diagnostic.LastOrDefault().Key.FormatWarning(Diagnostic.LastOrDefault().Key, Diagnostic.LastOrDefault().Value);
                Diagnostic.Clear();
            }
        }
    }

    public static void EvaluateBank(BankInformations bank) {

        Console.WriteLine("Start of evaluation");

        Console.WriteLine("------------------------");

        Console.WriteLine($"Bank : {bank.Name} ; Currency : {bank.Currency} ; Created at {bank.DateOfCreation}");

        Console.Write("\t");

        string nonRegistredUsersName = string.Empty ?? "none";
        string registredUsersName = string.Empty ?? "none";

        foreach (var nonRegUsers in bank.NonRegistredUsers) {
            if (bank.GetUsersNonRegistred().Length != 0 && !nonRegistredUsersName.Contains(nonRegUsers.Key.GetAccountName())) {
                nonRegistredUsersName += nonRegUsers.Key.GetAccountName() + ",";
            }
        }

        foreach (var regUsers in bank.Users) {
            if (bank.GetUsers().Length != 0 && !registredUsersName.Contains(regUsers.Key.GetAccount().GetAccountName())) {
                registredUsersName += regUsers.Key.GetAccount().GetAccountName() + ",";
            }
        }

        Console.WriteLine($"{bank.Name}'s User registry : \n" +
            $"NON REGISTRED USER COUNT : {bank.GetUsersNonRegistred().Length}\n" +
            $"REGISTRED USER COUNT : {bank.Users.Count}\n" +
            $"NON REGISTRED USERS :" +
            $" {nonRegistredUsersName}\n" +
            $"REGISTRED USERS :" +
            $" {registredUsersName}");

        Console.WriteLine("------------------------");

        Console.WriteLine("End of evaluation");
    }

    public static void EvaluateUser(RegistredUserAccount user) {
        
        Console.WriteLine("Start of evaluation");

        Console.WriteLine("------------------------");

        Console.WriteLine($"{user.CurrentAccount.GetAccountName()} : ID:({user.CurrentAccount.GetAccountID()}) at {user.CurrentAccount.GetTimeOfCreation()}");
        
        Console.Write("\t");

        Console.WriteLine($"Bank user account :" +
            $"  \t\t-- PLAN: {user.CurrentPlan}\n" +
            $"  \t\t-- FINANCIAL: ${user.CurrentEconomy.Money}\n" +
            $"  \t\t    \t  +SITUATION: {user.CurrentEconomy.CurrentSituation}");

        Console.WriteLine("------------------------");

        Console.WriteLine("End of evaluation");
    }

    public static void EvaluateUser(UserAccount user) {

        Console.WriteLine("Start of evaluation");

        Console.WriteLine("------------------------");

        Console.WriteLine($"{user.GetAccountName()} : ID:({user.GetAccountID()}) at {user.GetTimeOfCreation()}");

        Console.WriteLine("------------------------");

        Console.WriteLine("End of evaluation");
    }

    /*public static void EvaluateUsers(UserAccount[] accounts = null) {

        if (accounts == null) {
            accounts = new[] {
                new UserAccount("John Doe"),
                new UserAccount("Johnny Speed"),
                new UserAccount("Hezekiel"),
            };
        }

        foreach (var acc in accounts) {

            var bankAccount = bank.RegisterNewUser(acc);

            Console.WriteLine("Start of evaluation");

            Console.WriteLine("------------------------");

            Console.WriteLine($"Created account : {acc.GetAccountName()} (ID:{acc.GetAccountID()}) at {acc.GetTimeOfCreation()}");

            Console.Write("\t");

            Console.WriteLine($"Registred {acc.GetAccountName()}  at bank with the followings informations\n" +
                $"  \t\tPLAN: {bankAccount.CurrentPlan}");

            Console.WriteLine("------------------------");

            Console.WriteLine("End of evaluation");
        }
    }*/
}