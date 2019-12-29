using System;
using System.Collections.Generic;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;

namespace VAA.DataAccess.Interfaces
{
    /// <summary>
    /// Account related operations
    /// </summary>
    public interface IAccount
    {
        List<User> GetAllUsers();
        List<User> GetAllUsersByUserType(string usertype);
        User GetUserById(int userId);
        User GetUserByUsername(string username);
        User GetUserByUsernamePassword(string username, string hash);
        string GetUserTypeByUserid(int userId);
        User GetUserPermission(int userId);        

        DateTime? GetUserLastLogonTime(int userId);

        bool CreateUser(User user, int currentLoggedInUserId);
        bool UpdateMyAccount(User user);
        bool UpdateUser(User user, int currentLoggedInUserId);
        bool DeleteUser(int userId);

        int LogIn(string username, string password);

        bool UpdatePassword(string username, string password);

        bool UploadUserImage(int userId, byte[] image);
       
        //Approvers      
        bool AddUpdateApprovers(int olId, int classId, int apId, int catId, int tranId);
        bool AddUpdateApprovers(int olId, int classId, int apId, int catId);
        List<tApprovers> GetApproversByOrginAndClass(int selectedOrigin, int selectedClass);

        List<User> GetEspVirginUsersByUserType();

       
    }
}