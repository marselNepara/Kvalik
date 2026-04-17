using MatyeApp.Models;

namespace MatyeApp.Services;

public static class SessionService
{
    public static User? currentUser { get; set; }
    public static bool isAuthenticated => currentUser != null;
    public static string currentRole => currentUser?.Role?.roleName ?? "Гость";
    public static void logout() => currentUser = null;
}
