using VAA.DataAccess.Model;

namespace VAA.BusinessComponents.Interfaces
{
    public interface IAccountProcessor
    {
        bool ValidateLogin(string login, string password);
        int RegisterUser(User user);
        bool LogIn(string username, string password);
        bool LogOut(string username, string password);
        bool ChangePassword(string username, string oldpassword, string newpassword);
    }
}