using FotografciTakipWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace FotografciTakipWeb.App_Settings
{
    public class CustomRoleProvider : RoleProvider
    {
        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string eposta)
        {
            FotoTakipContext dbContext = new FotoTakipContext();
            if (!string.IsNullOrEmpty(eposta))
            {
                Kullanici kl = dbContext.Kullanicis.FirstOrDefault(x => x.Email == eposta);
                if (kl != null)
                {
                    //var rolAdi = dbcontext.Rols.FirstOrDefault(x => x.Id == kl.RolId);
                    string[] rolAdi1 = { kl.Rol.RolAdi };
                    //var rolAdi2 = kl.Rols.Select(x=>x.RolAdi).ToList();
                    //return kl.Rols == null ? new string[] { } : kl.Rols.Select(x => x.RolAdi).ToArray();
                    return rolAdi1;
                }
            }
            return new string[] { };
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}