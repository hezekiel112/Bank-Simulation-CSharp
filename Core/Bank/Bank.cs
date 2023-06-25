public abstract class Bank {
    public abstract RegistredUserAccount GetUser(long id);
    public abstract int GetUsersCount();
    public abstract RegistredUserAccount RegisterNewUser(UserAccount account);
    public abstract bool DeleteUser(RegistredUserAccount account);
}