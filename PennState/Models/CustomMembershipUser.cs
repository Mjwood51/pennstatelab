using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace PennState.Models
{
    public class CustomMembershipUser : MembershipUser
    {
        #region User Properties  

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Tbl_Roles Roles { get; set; }

        #endregion

        public CustomMembershipUser(Tbl_Users user) : base("CustomMembership", user.UserName, user.Id, user.Email, string.Empty, string.Empty, true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now)
        {
            UserId = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Roles = user.Tbl_Roles;
        }
    }
}