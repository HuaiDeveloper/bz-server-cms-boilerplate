namespace Shared;

public static class AuthRole
{
    public const string Admin = "Admin";
    public const string User = "User";

    public static string[] GetAuthRoles()
    {
        return new []
        {
            Admin,
            User
        };
    }
}