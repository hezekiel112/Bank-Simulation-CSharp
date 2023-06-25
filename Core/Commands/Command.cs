public abstract class Command : ICommandProcessor {

    public virtual string? Input { get; }

    public virtual string? Description { get; }

    public virtual bool FormatOutput(ICommandProcessor command) {
        Console.Write($"[{command}] : ");

        return true;
    }

    public virtual bool FormatWarning(ICommandProcessor command, DiagnosticCases diagnosticCase) {

        var cmd = (CommandBase) command;

        switch (diagnosticCase) {
            case DiagnosticCases.CommandNotFound:
                Console.WriteLine($"{cmd.Input} -> [{diagnosticCase}] : \n\t    This command does not exist");
                break;

            case DiagnosticCases.MissingEssentialArgument:
                Console.WriteLine($"{cmd.Input} -> [{diagnosticCase}] : \n\t    This command has missing argument(s) or value(s)");
                break;

            case DiagnosticCases.MissingBank:
                Console.WriteLine($"{cmd.Input} -> [{diagnosticCase}] : \n\t    This command can not be executed without a bank");
                break;

            case DiagnosticCases.DoubleAccount:
                Console.WriteLine($"{cmd.Input} -> [{diagnosticCase}] : \n\t    This command handled this error because two account interfer with the process");
                break;

            case DiagnosticCases.IncoherentAction:
                Console.WriteLine($"{cmd.Input} -> [{diagnosticCase}] : \n\t    This command handled this error because an action interfer with the process");
                break;
        }

        return true;
    }

    public virtual void Recognize(string input, Command command) {
        FormatOutput(command);
    }
}