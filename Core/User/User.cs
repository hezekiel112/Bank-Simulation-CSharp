public abstract class User {

    public abstract UserEconomy GetEconomy();
    public abstract IEnumerable<object> FindAllValues();
    public abstract UserPlan GetPlan();
    public abstract UserAccount GetAccount();
}
