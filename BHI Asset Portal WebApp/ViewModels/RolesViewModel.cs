using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;


namespace BHI_Asset_Portal_WebApp.ViewModels
{
    public class RolesViewModel
    {
        [Key]
        public string Id { get; set; }
        [Display (Name ="User Name")]
        public string UserName { get; set; }
        public string Email { get; set; }
        [UIHint("Roles")]
        public List<IdentityRole> Roles { get; set; }
        public List<string> RoleIDs { get {
                if (roleIDs != null)
                    return roleIDs;
                else if (this.Roles != null)
                    return Roles.Select(m => m.Id).ToList();
                else return null;
            }
            set { roleIDs = value; } }
            

        public IEnumerable<SelectListItem> allRoles { get; set; }

        private List<string> roleIDs;
        
    }

    
    
}