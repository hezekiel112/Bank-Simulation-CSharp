public class BankInformations : Bank {

    public Dictionary<User, long> Users = new Dictionary<User, long>();

    public int UsersCount  => Users.Count;

    public User[] GetUsers() => Users.Keys.ToArray();

    public string Name { get; }
    public char Currency { get; }
    public DateTime DateOfCreation { get; }

    public Dictionary<UserAccount, bool> NonRegistredUsers = new Dictionary<UserAccount, bool>();

    public UserAccount[] GetUsersNonRegistred() => NonRegistredUsers.Keys.ToArray();

    public override RegistredUserAccount GetUser(long id) {
        foreach (var u in Users) {

            var user = (RegistredUserAccount) u.Key;

            if (user.ID == id) 
                return user;
        }

        return null;
    }

    public override int GetUsersCount() {
        return UsersCount;
    }

    public override RegistredUserAccount RegisterNewUser(UserAccount account) {

        foreach (var u in Users) {

            if (u.Value == account.GetAccountID()) // prevent from exception of duplicated Value in dictionnary
                return null;
        }

        RegistredUserAccount newRegistredUser = new(UserPlan.LivretJeune, new(), account);

        Users.Add(newRegistredUser, newRegistredUser.ID);

        return newRegistredUser;
    }

    public override bool DeleteUser(RegistredUserAccount account) {

        var user = GetUser(account.ID);

        if (user != null) {

            Users.Remove(user);

            return true;
        }

        return false;
    }

    public BankInformations(string name, char currency, DateTime dateOfCreation) {
        Name = name;
        Currency = currency;
        DateOfCreation = dateOfCreation;
    }
}