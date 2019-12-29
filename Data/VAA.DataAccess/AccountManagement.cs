using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using VAA.CommonComponents;
using VAA.DataAccess.Interfaces;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;

namespace VAA.DataAccess
{
    public class AccountManagement : IAccount
    {
        readonly VAAEntities _context = new VAAEntities();

        public List<User> GetAllUsers()
        {
            var userData = (from u in _context.tUsers
                            select new
                            {
                                Id = u.UserID,
                                Username = u.Username,
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                City = u.City,
                                Country = u.Country,
                                Mobile = u.Mobile,
                                UserType = u.UserType,
                                ProfileImage = u.ProfileImage
                            }).ToList();

            return (from x in userData
                    select new User
                    {
                        Id = x.Id,
                        Username = x.Username,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        City = x.City,
                        Country = x.Country,
                        Mobile = x.Mobile,
                        UserType = x.UserType,
                        ProfileImage = x.ProfileImage,
                        IsBlocked = (from p in _context.tUserPermissions where p.UserID == x.Id select p.IsBlocked).FirstOrDefault(),
                    }).ToList();

        }
        public List<User> GetAllUsersByUserType(string usertype)
        {
            var userData = (from u in _context.tUsers
                            where u.UserType == usertype
                            select new
                            {
                                Id = u.UserID,
                                Username = u.Username,
                                FirstName = u.FirstName,
                                LastName = u.LastName
                            }).ToList();
            return (from x in userData
                    select new User
                    {
                        Id = x.Id,
                        Username = x.Username,
                        FirstName = x.FirstName,
                        LastName = x.LastName
                    }).ToList();
        }

        public User GetUserById(int userId)
        {
            var user = (from i in _context.tUsers
                        where i.UserID == userId
                        select new
                        {
                            Username = i.Username,
                            Id = i.UserID,
                            FirstName = i.FirstName,
                            LastName = i.LastName,
                            UserType = i.UserType,
                            Hash = i.Hash,
                            Department = i.Department,
                            Designation = i.Designation,
                            Address1 = i.address1,
                            Address2 = i.address2,
                            Address3 = i.address3,
                            City = i.City,
                            Postcode = i.postcode,
                            County = i.County,
                            Country = i.Country,
                            Mobile = i.Mobile,
                            Telephone = i.telephone,
                            ProfileImage = i.ProfileImage,
                            ModifiedAt = i.ModifiedAt,
                            LastLogonTime = i.LastLogonTime,
                            userPermission = (from p in _context.tUserPermissions where p.UserID == i.UserID select p).FirstOrDefault()
                        }).FirstOrDefault();

            User retUser = new User()
            {
                Username = user.Username,
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserType = user.UserType,
                Hash = user.Hash,
                Department = user.Department,
                Designation = user.Designation,
                Address1 = user.Address1,
                Address2 = user.Address2,
                Address3 = user.Address3,
                City = user.City,
                Postcode = user.Postcode,
                County = user.County,
                Country = user.Country,
                Mobile = user.Mobile,
                Telephone = user.Telephone,
                ProfileImage = user.ProfileImage,
                ModifiedAt = user.ModifiedAt,
                LastLogonTime = user.LastLogonTime,
            };

            if (user.userPermission != null)
            {
                retUser.PermissionId = user.userPermission.ID;
                retUser.IsSuper = user.userPermission.IsSuper == true ? true : false;
                retUser.CanImport = user.userPermission.CanImport == true ? true : false;
                retUser.CanExport = user.userPermission.CanExport == true ? true : false;
                retUser.IsBlocked = user.userPermission.IsBlocked == true ? true : false;
                retUser.CanCreateCampaign = user.userPermission.CanCreateCampaign == true ? true : false;
                retUser.CanDeleteCampaign = user.userPermission.CanDeleteCampaign == true ? true : false;
                retUser.CanViewPricing = user.userPermission.CanViewPricing == true ? true : false;
            }
            else
            {
                retUser.PermissionId = 0;
                retUser.IsSuper = false;
                retUser.CanImport = false;
                retUser.CanExport = false;
                retUser.IsBlocked = false;
                retUser.CanCreateCampaign = false;
                retUser.CanDeleteCampaign = false;
                retUser.CanViewPricing = false;
            }
            return retUser;
        }

        public User GetUserByUsername(string username)
        {
            var userData = (from user in _context.tUsers where user.Username == username select user).FirstOrDefault();

            if (userData != null)
            {
                return new User
                {
                    FirstName = userData.FirstName,
                    LastName = userData.LastName
                };
            }
            return null;
        }
        public string GetUserTypeByUserid(int userId)
        {
            var usertype = (from user in _context.tUsers where user.UserID == userId select user.UserType).FirstOrDefault();
            return usertype;
        }
        public User GetUserByUsernamePassword(string username, string hash)
        {
            return (from i in _context.tUsers
                    where i.Username == username && i.Hash == hash
                    select new User
                    {
                        Username = i.Username,
                        Id = i.UserID,
                        FirstName = i.FirstName,
                        LastName = i.LastName,
                        UserType = i.UserType,
                        Department = i.Department,
                        Designation = i.Designation,
                        Address1 = i.address1,
                        Address2 = i.address2,
                        Address3 = i.address3,
                        City = i.City,
                        Postcode = i.postcode,
                        County = i.County,
                        Country = i.Country,
                        Mobile = i.Mobile,
                        Telephone = i.telephone,
                        ProfileImage = i.ProfileImage,
                        ModifiedAt = i.ModifiedAt,
                        LastLogonTime = i.LastLogonTime
                    }).FirstOrDefault();
        }

        public User GetUserPermission(int userId)
        {
            throw new NotImplementedException();
        }



        public DateTime? GetUserLastLogonTime(int userId)
        {
            try
            {
                var userLastLoginTime = (from user in _context.tUsers where user.UserID == userId select user.LastLogonTime).FirstOrDefault();
                if (userLastLoginTime != null)
                {
                    return userLastLoginTime == null ? (DateTime)SqlDateTime.Null : userLastLoginTime;
                }
                return (DateTime)SqlDateTime.Null;
            }
            catch (Exception ex)
            {
                return (DateTime)SqlDateTime.Null;
            }
        }

        public bool CreateUser(User user, int currentLoggedInUserId)
        {
            try
            {
                if (user.Username == null) user.Username = "";
                if (user.FirstName == null) user.FirstName = "";
                if (user.LastName == null) user.LastName = "";
                if (user.UserType == null) user.UserType = "";
                if (user.Hash == null) user.Hash = "";
                if (user.Department == null) user.Department = "";
                if (user.Designation == null) user.Designation = "";
                if (user.Address1 == null) user.Address1 = "";
                if (user.Address2 == null) user.Address2 = "";
                if (user.Address3 == null) user.Address3 = "";
                if (user.City == null) user.City = "";
                if (user.County == null) user.County = "";
                if (user.Country == null) user.Country = "";
                if (user.Postcode == null) user.Postcode = "";
                if (user.Mobile == null) user.Mobile = "";
                if (user.Telephone == null) user.Telephone = "";

                tUsers addNewUsers = new tUsers()
                {
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserType = user.UserType,
                    Hash = EncryptionHelper.Encrypt(user.Hash),
                    Department = user.Department,
                    Designation = user.Designation,
                    address1 = user.Address1,
                    address2 = user.Address2,
                    address3 = user.Address3,
                    City = user.City,
                    County = user.County,
                    Country = user.Country,
                    postcode = user.Postcode,
                    Mobile = user.Mobile,
                    telephone = user.Telephone
                };
                _context.tUsers.Add(addNewUsers);
                _context.SaveChanges();

                var allPermission = (from permission in _context.tUserPermissions where permission.UserID == addNewUsers.UserID select permission).ToList();
                if (allPermission.Count == 1)
                {
                    var userp = (from u in allPermission where u.UserID == addNewUsers.UserID select u).FirstOrDefault();
                    if (userp != null)
                    {
                        userp.CanImport = user.CanImport;
                        userp.CanExport = user.CanExport;
                        userp.CanCreateCampaign = user.CanCreateCampaign;
                        userp.CanDeleteCampaign = user.CanDeleteCampaign;
                        userp.CanViewPricing = user.CanViewPricing;
                        userp.IsSuper = user.IsSuper;
                        userp.IsBlocked = user.IsBlocked;
                        _context.SaveChanges();
                    }
                    else
                    {
                        tUserPermissions addNewUserPermission = new tUserPermissions()
                        {
                            UserID = addNewUsers.UserID,
                            CanImport = user.CanImport,
                            CanExport = user.CanExport,
                            CanCreateCampaign = user.CanCreateCampaign,
                            CanDeleteCampaign = user.CanDeleteCampaign,
                            CanViewPricing = user.CanViewPricing,
                            IsSuper = user.IsSuper,
                            IsBlocked = user.IsBlocked
                        };
                        _context.tUserPermissions.Add(addNewUserPermission);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    if (allPermission.Count > 1)
                    {
                        _context.tUserPermissions.RemoveRange(allPermission);
                        _context.SaveChanges();
                    }
                    tUserPermissions addNewUserPermission = new tUserPermissions()
                    {
                        UserID = addNewUsers.UserID,
                        CanImport = user.CanImport,
                        CanExport = user.CanExport,
                        CanCreateCampaign = user.CanCreateCampaign,
                        CanDeleteCampaign = user.CanDeleteCampaign,
                        CanViewPricing = user.CanViewPricing,
                        IsSuper = user.IsSuper,
                        IsBlocked = user.IsBlocked
                    };
                    _context.tUserPermissions.Add(addNewUserPermission);
                    _context.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateMyAccount(User user)
        {
            try
            {
                if (user.Username == null) user.Username = "";
                if (user.FirstName == null) user.FirstName = "";
                if (user.LastName == null) user.LastName = "";
                if (user.UserType == null) user.UserType = "";
                if (user.Hash == null) user.Hash = "";
                if (user.Department == null) user.Department = "";
                if (user.Designation == null) user.Designation = "";
                if (user.Address1 == null) user.Address1 = "";
                if (user.Address2 == null) user.Address2 = "";
                if (user.Address3 == null) user.Address3 = "";
                if (user.City == null) user.City = "";
                if (user.County == null) user.County = "";
                if (user.Country == null) user.Country = "";
                if (user.Postcode == null) user.Postcode = "";
                if (user.Mobile == null) user.Mobile = "";
                if (user.Telephone == null) user.Telephone = "";

                var userData = (from u in _context.tUsers where u.UserID == user.Id select u).FirstOrDefault();
                if (userData != null)
                {
                    userData.Username = user.Username;
                    userData.FirstName = user.FirstName;
                    userData.LastName = user.LastName;
                    userData.UserType = user.UserType;
                    //userData.Hash = CommonComponents.EncryptionHelper.Encrypt(user.Hash);
                    userData.Hash = user.Hash;
                    userData.Department = user.Department;
                    userData.Designation = user.Designation;
                    userData.address1 = user.Address1;
                    userData.address2 = user.Address2;
                    userData.address3 = user.Address3;
                    userData.City = user.City;
                    userData.County = user.County;
                    userData.Country = user.Country;
                    userData.postcode = user.Postcode;
                    userData.Mobile = user.Mobile;
                    userData.telephone = user.Telephone;
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex) { return false; }
        }

        public bool UpdateUser(User user, int currentLoggedInUserId)
        {
            try
            {
                if (user.Username == null) user.Username = "";
                if (user.FirstName == null) user.FirstName = "";
                if (user.LastName == null) user.LastName = "";
                if (user.UserType == null) user.UserType = "";
                if (user.Hash == null) user.Hash = "";
                if (user.Department == null) user.Department = "";
                if (user.Designation == null) user.Designation = "";
                if (user.Address1 == null) user.Address1 = "";
                if (user.Address2 == null) user.Address2 = "";
                if (user.Address3 == null) user.Address3 = "";
                if (user.City == null) user.City = "";
                if (user.County == null) user.County = "";
                if (user.Country == null) user.Country = "";
                if (user.Postcode == null) user.Postcode = "";
                if (user.Mobile == null) user.Mobile = "";
                if (user.Telephone == null) user.Telephone = "";

                var userData = (from u in _context.tUsers where u.UserID == user.Id select u).FirstOrDefault();
                if (userData != null)
                {
                    userData.Username = user.Username;
                    userData.FirstName = user.FirstName;
                    userData.LastName = user.LastName;
                    userData.UserType = user.UserType;
                    userData.Hash = CommonComponents.EncryptionHelper.Encrypt(user.Hash);
                    userData.Department = user.Department;
                    userData.Designation = user.Designation;
                    userData.address1 = user.Address1;
                    userData.address2 = user.Address2;
                    userData.address3 = user.Address3;
                    userData.City = user.City;
                    userData.County = user.County;
                    userData.Country = user.Country;
                    userData.postcode = user.Postcode;
                    userData.Mobile = user.Mobile;
                    userData.telephone = user.Telephone;
                    _context.SaveChanges();

                    var allPermission = (from permission in _context.tUserPermissions where permission.UserID == userData.UserID select permission).ToList();
                    if (allPermission.Count == 1)
                    {
                        var userp = (from u in allPermission where u.UserID == userData.UserID select u).FirstOrDefault();
                        if (userp != null)
                        {
                            userp.CanImport = user.CanImport;
                            userp.CanExport = user.CanExport;
                            userp.CanCreateCampaign = user.CanCreateCampaign;
                            userp.CanDeleteCampaign = user.CanDeleteCampaign;
                            userp.CanViewPricing = user.CanViewPricing;
                            userp.IsSuper = user.IsSuper;
                            userp.IsBlocked = user.IsBlocked;
                            _context.SaveChanges();
                        }
                        else
                        {
                            tUserPermissions addNewUserPermission = new tUserPermissions()
                            {
                                UserID = userData.UserID,
                                CanImport = user.CanImport,
                                CanExport = user.CanExport,
                                CanCreateCampaign = user.CanCreateCampaign,
                                CanDeleteCampaign = user.CanDeleteCampaign,
                                CanViewPricing = user.CanViewPricing,
                                IsSuper = user.IsSuper,
                                IsBlocked = user.IsBlocked
                            };
                            _context.tUserPermissions.Add(addNewUserPermission);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        if (allPermission.Count > 1)
                        {
                            _context.tUserPermissions.RemoveRange(allPermission);
                            _context.SaveChanges();
                        }
                        tUserPermissions addNewUserPermission = new tUserPermissions()
                        {
                            UserID = userData.UserID,
                            CanImport = user.CanImport,
                            CanExport = user.CanExport,
                            CanCreateCampaign = user.CanCreateCampaign,
                            CanDeleteCampaign = user.CanDeleteCampaign,
                            CanViewPricing = user.CanViewPricing,
                            IsSuper = user.IsSuper,
                            IsBlocked = user.IsBlocked
                        };
                        _context.tUserPermissions.Add(addNewUserPermission);
                        _context.SaveChanges();
                    }


                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteUser(int userId)
        {
            try
            {
                var userData = (from user in _context.tUsers
                                join userPer in _context.tUserPermissions on user.UserID equals userPer.UserID
                                where user.UserID == userId
                                select new { user, userPer }).FirstOrDefault();
                if (userData != null)
                {
                    if (userData.userPer != null)
                    {
                        userData.userPer.IsBlocked = true;
                        _context.SaveChanges();
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int LogIn(string username, string password)
        {
            var userData = (from user in _context.tUsers where user.Username == username && user.Hash == password select user).FirstOrDefault();

            if (userData != null)
            {
                var isBlocked =
                (from up in _context.tUserPermissions where up.UserID == userData.UserID select up.IsBlocked)
                    .FirstOrDefault();

                if (isBlocked != null)
                {
                    if (Convert.ToBoolean(isBlocked))
                    {
                        return 0;
                    }

                }
                return userData.UserID;
            }
            return 0;
        }

        public bool UpdatePassword(string username, string password)
        {
            var userData = (from user in _context.tUsers where user.Username == username select user).FirstOrDefault();
            if (userData != null)
            {
                userData.Hash = EncryptionHelper.Encrypt(password);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool UploadUserImage(int userId, byte[] image)
        {
            try
            {
                var userdata = (from u in _context.tUsers where u.UserID == userId select u).FirstOrDefault();
                if (userdata != null)
                {
                    userdata.ProfileImage = image;
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //add with translation approver
        public bool AddUpdateApprovers(int olId, int classId, int apId, int catId, int tranId)
        {
            try
            {
                var approverData = (from tApprovers in _context.tApprovers where tApprovers.OriginLocationID == olId && tApprovers.ClassID == classId select tApprovers).FirstOrDefault();
                if (approverData == null) //add
                {
                    tApprovers newApprovers = new tApprovers
                    {
                        OriginLocationID = olId,
                        ClassID = classId,
                        VirginApproverID = apId,
                        CatererID = catId,
                        TranslatorID = tranId
                    };
                    _context.tApprovers.Add(newApprovers);
                    _context.SaveChanges();
                    return true;
                }
                else //update
                {
                    approverData.VirginApproverID = apId;
                    approverData.CatererID = catId;
                    approverData.TranslatorID = tranId;

                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //add without translation approver
        public bool AddUpdateApprovers(int olId, int classId, int apId, int catId)
        {
            try
            {
                var approverData = (from tApprovers in _context.tApprovers where tApprovers.OriginLocationID == olId && tApprovers.ClassID == classId select tApprovers).FirstOrDefault();
                if (approverData == null) //Add
                {
                    tApprovers newApprovers = new tApprovers
                    {
                        OriginLocationID = olId,
                        ClassID = classId,
                        VirginApproverID = apId,
                        CatererID = catId
                    };
                    _context.tApprovers.Add(newApprovers);
                    _context.SaveChanges();
                    return true;
                }
                else //update
                {
                    approverData.VirginApproverID = apId;
                    approverData.CatererID = catId;

                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<tApprovers> GetApproversByOrginAndClass(int selectedOrigin, int selectedClass)
        {
            var userData = (from u in _context.tApprovers
                            where u.OriginLocationID == selectedOrigin && u.ClassID == selectedClass
                            select new
                                {
                                    Id = u.ID,
                                    virginApprover = u.VirginApproverID,
                                    caterer = u.CatererID,
                                    Translator = u.TranslatorID
                                }).ToList();
            return (from x in userData
                    select new tApprovers
                    {
                        ID = x.Id,
                        VirginApproverID = x.virginApprover,
                        CatererID = x.caterer,
                        TranslatorID = x.Translator
                    }).ToList();
        }

        public List<User> GetEspVirginUsersByUserType()
        {
            var userData = (from u in _context.tUsers
                            where u.UserType == "Virgin" || u.UserType == "ESP"
                            select new
                            {
                                Id = u.UserID,
                                Username = u.Username,
                                FirstName = u.FirstName,
                                LastName = u.LastName
                            }).ToList();
            return (from x in userData
                    select new User
                    {
                        Id = x.Id,
                        Username = x.Username,
                        FirstName = x.FirstName,
                        LastName = x.LastName
                    }).ToList();
        }

      
    }
}
