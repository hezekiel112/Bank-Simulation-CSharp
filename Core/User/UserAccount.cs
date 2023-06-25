public class UserAccount {
    
    private DateTime TimeOfCreation { get; }
    private long AccountID { get; }
    private string Name { get; }

    public DateTime GetTimeOfCreation() {
        return TimeOfCreation;
    }

    public long GetAccountID() {
        return AccountID;
    }

    public string GetAccountName() {
        return Name;
    }

    public UserAccount(string name) {
        TimeOfCreation = DateTime.Now;
        AccountID = Random.Shared.NextInt64();
        Name = name;
    }
}