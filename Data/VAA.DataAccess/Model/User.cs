using System;

namespace VAA.DataAccess.Model
{
    public class User
    {
        //user details
        public int Id { get; set; }
        public string Username { get; set; }
        public string Hash { get; set; }
        public string UserType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string Mobile { get; set; }
        public string Telephone { get; set; }
        public byte[] ProfileImage { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? LastLogonTime { get; set; }

        //Permissions
        public int PermissionId { get; set; }
        public bool IsSuper { get; set; }
        public bool CanImport { get; set; }
        public bool CanExport { get; set; }
        public bool ? IsBlocked { get; set; }
        public bool CanCreateCampaign { get; set; }
        public bool CanDeleteCampaign { get; set; }
        public bool CanViewPricing { get; set; }

        //Roles

      //  public int RoleId { get; set; }
       // public string RoleName { get; set; }

        //upper class
      //  public bool CanViewUpperClass { get; set; }
     //   public bool CanEditUpperClass { get; set; }
    //    public bool CanDeleteUpperClass { get; set; }
    //    public bool CanApproveUpperClass { get; set; }

        //PE
     //   public bool CanViewPremiumEconomy { get; set; }
     //   public bool CanEditPremiumEconomy { get; set; }
     //   public bool CanDeletePremiumEconomy { get; set; }
    //    public bool CanApprovePremiumEconomy { get; set; }

        //Eco
   //     public bool CanViewEconomy { get; set; }
//        public bool CanEditEconomy { get; set; }
    //    public bool CanDeleteEconomy { get; set; }
    //    public bool CanApproveEconomy { get; set; }

    }
}
