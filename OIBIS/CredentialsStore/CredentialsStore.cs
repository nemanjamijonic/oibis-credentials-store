using Common;
using Common.Audit;
using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CredentialsStore
{
    public class CredentialsStore : ICredentialsStore
    {
        DataBase db = new DataBase();

        public void CreateAccount(string username, string password, string appUser)
        {
    
            if (Thread.CurrentPrincipal.IsInRole(Groups.AdminUser))
            {
                Audit.AuthorizationSuccess(appUser, "Create Account");
                User user = new User(username, (password.GetHashCode()).ToString(), false, false, string.Empty, string.Empty);
                List<User> users = db.getUsers();
                Console.WriteLine(users.Count());
                if (users.FindIndex(o => o.Username == username) == -1)
                {
                    users.Add(user);
                    db.addUsers(users);
                    Console.WriteLine($"Account - {username} successfully created.");
                }
                else
                    throw new FaultException<UserException>(new UserException("That username already exists, please try again.\n"));
            }
            else
            {
                Audit.AuthorizationFailed(appUser, "Create Account", "Invalid Group permission");
                throw new FaultException<GroupException>(new GroupException("Invalid Group permissions, please contact your system administrator if you think this is a mistake.\n"));
            }

        }

        public void DeleteAccount(string username, string appUser)
        {

            if (Thread.CurrentPrincipal.IsInRole(Groups.AdminUser))
            {
                Audit.AuthorizationSuccess(appUser, "Delete Account");
                List<User> users = db.getUsers();
                if ((users.FindIndex(o => o.Username == username) != -1))
                {
                    users.RemoveAt(users.FindIndex(o => o.Username == username));
                    db.addUsers(users);

                    Console.WriteLine($"Account - {username} successfully deleted.");
                }
                else
                    throw new FaultException<UserException>(new UserException("That username does not exists, please try again.\n"));
            }
            else {
                Audit.AuthorizationFailed(appUser, "Delete Account", "Invalid Group permission");
                throw new FaultException<GroupException>(new GroupException("Invalid Group permissions, please contact your system administrator if you think this is a mistake.\n"));
            }
        }

        public void DisableAccount(string username, string appUser)
        {

            if (Thread.CurrentPrincipal.IsInRole(Groups.AdminUser))
            {
                Audit.AuthorizationSuccess(appUser, "Delete Account");
                List<User> users = db.getUsers();
                if ((users.FindIndex(o => o.Username == username) != -1))
                {
                    users[users.FindIndex(o => o.Username == username)].IsDisabled = true;
                    db.addUsers(users);

                    Console.WriteLine($"Account - {username} successfully disabled.");
                }
                else
                    throw new FaultException<UserException>(new UserException("That username does not exists, please try again.\n"));
            }
            else
            {
                Audit.AuthorizationFailed(appUser, "Disable Account", "Invalid Group permission");
                throw new FaultException<GroupException>(new GroupException("Invalid Group permissions, please contact your system administrator if you think this is a mistake.\n"));
            }
        }

        public void EnableAccount(string username, string appUser)
        {

            if (Thread.CurrentPrincipal.IsInRole(Groups.AdminUser))
            {
                Audit.AuthorizationSuccess(appUser, "Enable Account");
                List<User> users = db.getUsers();
                if ((users.FindIndex(o => o.Username == username) != -1))
                {
                    users[users.FindIndex(o => o.Username == username)].IsDisabled = false;
                    users[users.FindIndex(o => o.Username == username)].IsLocked = false;

                    db.addUsers(users);

                    Console.WriteLine($"Account - {username} succesfully enabled.");
                }
                else
                    throw new FaultException<UserException>(new UserException("That username does not exists, please try again.\n"));
            }
            else
            {
                Audit.AuthorizationFailed(appUser, "Enable Account", "Invalid Group permission");
                throw new FaultException<GroupException>(new GroupException("Invalid Group permissions, please contact your system administrator if you think this is a mistake.\n"));
            }
        }

        public void LockAccount(string username, string appUser)
        {
  
            if (Thread.CurrentPrincipal.IsInRole(Groups.AdminUser))
            {
                Audit.AuthorizationSuccess(appUser, "Lock Account");
                List<User> users = db.getUsers();
                if ((users.FindIndex(o => o.Username == username) != -1))
                {
                    users[users.FindIndex(o => o.Username == username)].Lock();
                    db.addUsers(users);

                    Console.WriteLine($"Account - {username} succesfully locked.");
                }
                else
                    throw new FaultException<UserException>(new UserException("That username does not exists, please try again.\n"));
            }
            else
            {
                Audit.AuthorizationFailed(appUser, "Lock Account", "Invalid Group permission");
                throw new FaultException<GroupException>(new GroupException("Invalid Group permissions, please contact your system administrator if you think this is a mistake.\n"));
            }
        }
    }
}
