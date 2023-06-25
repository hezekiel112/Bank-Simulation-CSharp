public class RegistredUserAccount : User {

    public long ID => CurrentAccount.GetAccountID();
    public string Name => CurrentAccount.GetAccountName();

    public UserEconomy CurrentEconomy;
    public UserPlan CurrentPlan { get; }
    public UserAccount CurrentAccount { get; }

    public RegistredUserAccount(UserPlan userPlan, UserEconomy userEconomy, UserAccount userAccount) : base() {
        CurrentPlan = userPlan;
        CurrentEconomy = userEconomy;
        CurrentAccount = userAccount;

        userEconomy.EvaluateSituation();
    }

    public override UserEconomy GetEconomy() {
        return CurrentEconomy;
    }

    public override IEnumerable<object> FindAllValues() {
        yield return ID.ToString();
        yield return Name;
        yield return CurrentPlan.ToString();
        yield return CurrentEconomy.ToString();
        yield return CurrentAccount.ToString();
    }

    public override UserPlan GetPlan() {
        return CurrentPlan;
    }

    public override UserAccount GetAccount() {
        return CurrentAccount;
    }
}