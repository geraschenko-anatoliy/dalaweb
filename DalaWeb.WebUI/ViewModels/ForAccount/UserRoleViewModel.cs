using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DalaWeb.WebUI.ViewModels.ForAccount
{
    public class UserRoleViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string[] CurrentRoles { get; set; }
        public string[] AnotherRoles { get; set; }

        public UserRoleViewModel(int userId, string user, string[] currentRoles, string[] anotherRoles)
        {
            this.UserId = userId;
            this.UserName = user;
            this.CurrentRoles = currentRoles;
            this.AnotherRoles = anotherRoles;
        }
    }
}