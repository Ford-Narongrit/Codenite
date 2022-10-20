public static class LocalUserData
{
    public static int id;
    public static string name;
    public static string email;
    public static string role;
    public static string auth_token;

    public static bool isLoggedIn { get { return auth_token != null; } }
    public static void LogOut()
    {
        name = null;
        auth_token = null;
    }
}
