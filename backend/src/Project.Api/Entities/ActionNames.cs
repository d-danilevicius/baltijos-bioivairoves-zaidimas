namespace Project.Api.Entities;

public static class ActionNames
{
    public static class Login
    {
        public const string Success = "Login.Success";
        public const string Failed = "Login.Failed";
    }

    public const string Logout = "Logout";

    public static class Account
    {
        public const string Blocked = "Account.Blocked";
        public const string Unblocked = "Account.Unblocked";
    }
}
