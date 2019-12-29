using System;
using VAA.BusinessComponents.Interfaces;
using VAA.DataAccess.Model;

namespace VAA.BusinessComponents
{
    public class AccountProcessor :IAccountProcessor
    {
        public bool ValidateLogin(string login, string password)
        {
            throw new NotImplementedException();
        }

        public int RegisterUser(User user)
        {
            throw new NotImplementedException();
        }

        public bool LogIn(string username, string password)
        {
            throw new NotImplementedException();
        }

        public bool LogOut(string username, string password)
        {
            throw new NotImplementedException();
        }

        public bool ChangePassword(string username, string oldpassword, string newpassword)
        {
            throw new NotImplementedException();
        }
    }
}
