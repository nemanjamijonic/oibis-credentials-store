using AutentificationService;
using Common;
using Common.Audit;
using Common.Exceptions;
using Common.Signatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;

namespace AuthentificationService
{
    public class AuthentificationService : IAuthentificationService
    {
        LoginUsers loginUsers = new LoginUsers();

        public int Login(string username, string password,string appUser)
        {

            //RETURNS -4 IF SIGNATURE IS NOT VALID
            //RETURNS -3 IF USER IS DISABLED
            //RETURNS -2 IF USER IS LOCKED
            //RETURNS -1 IF USER DATA DOES NOT EXIST
            //RETURNS 0  IF USER PASSWORD IS NOT VALID
            //RETURNS 1  IF USER DATA IS VALID
            //RETURNS 2  IF USER IS ALREADY LOGGED IN

            if (Thread.CurrentPrincipal.IsInRole(Groups.GeneralUser))
            {

                CredentialsStoreProxy credentialsStoreProxy = CredentialsStoreProxy.SingletonInstance();
                Audit.AuthorizationSuccess(appUser, "Login");
                try
                {

                    //If user is already logged in
                    List<string> users = loginUsers.getLoginUsers();

                    for (int i = 0; i < users.Count(); i++)
                        if (users[i].Split('|')[0].Equals(username))
                            return 2;

                    //Encrypting data
                    byte[] outUsername = Cryptography.EncryptData(username, Cryptography.LoadKey(Cryptography.KeyLocation));
                    byte[] outPassword = Cryptography.EncryptData(password, Cryptography.LoadKey(Cryptography.KeyLocation));

                    //Digital signature generation 
                    byte[] data = new byte[outUsername.Length + outPassword.Length];
                    Buffer.BlockCopy(outUsername, 0, data, 0, outUsername.Length);
                    Buffer.BlockCopy(outPassword, 0, data, outUsername.Length, outPassword.Length);

                    byte[] signature = DigitalSignatures.GenerateDigitalSignature(data);

                    int ret = credentialsStoreProxy.ValidateCredentials(outUsername, outPassword, signature);

                    switch (ret)
                    {
                        case -4:
                            Console.WriteLine("Signature check failed, your data may be tampered with. Please contact your system administrator.\n");
                            Audit.AuthentificationFalied(appUser);
                            Audit.LoginAttemptFailed(username, "Signature check failed");
                            return -4;
                        case -3:
                            Console.WriteLine($"{username} is DISABLED. Please contact your system administrator.\n");
                            Audit.AuthentificationFalied(appUser);
                            Audit.LoginAttemptFailed(username, "User account is DISABLED");
                            return -3;
                        case -2:
                            Console.WriteLine($"{username} is LOCKED. Please contact your system administrator or wait some time and try again.\n");
                            Audit.AuthentificationFalied(appUser);
                            Audit.LoginAttemptFailed(username, "User account is LOCKED");
                            return -2;
                        case -1:
                            Console.WriteLine($"{username} does not exist in our Database. Please contact your system administrator.\n");
                            Audit.AuthentificationFalied(appUser);
                            Audit.LoginAttemptFailed(username, "User does not exist in DataBase");
                            return -1;
                        case 0:
                            Console.WriteLine($"Password for {username} is wrong.\n");
                            Audit.AuthentificationFalied(appUser);
                            Audit.LoginAttemptFailed(username, "Wrong password");
                            return 0;
                        default:
                            loginUsers.addUser(username);
                            Console.WriteLine($"{username} successfully logged in.\n");
                            Audit.AuthenticationSuccess(appUser);
                            Audit.LoginAttemptSuccess(username);
                            return 1;
                    }
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Client certificate check failed. Please contact your system administrator.\n");
                    Audit.LoginAttemptFailed(username, "Certificate check failed");
                    return -1;
                }
            }
            else
            {
                Audit.AuthorizationFailed(appUser, "Login", "Invalid Group Permission");
                throw new FaultException<GroupException>(new GroupException("Invalid Group permissions, please contact your system administrator if you think this is a mistake.\n"));
            }
        }

        public int Logout(string username, string appUser)
        {

            //RETURNS 0 IF LOGOUT IS SUCCESSFUL
            //RETURNS 1 IF LOGOUT IS NOT SUCCESSFUL

            if (Thread.CurrentPrincipal.IsInRole(Groups.GeneralUser))
            {

                Audit.AuthorizationSuccess(appUser, "Logout");
                List<string> users = loginUsers.getLoginUsers();

                for (int i = 0; i < users.Count(); i++)
                    if (users[i].Split('|')[0].Equals(username))
                        users.RemoveAt(i);

                loginUsers.updateLoginUsers(users);

                Console.WriteLine($"{username} successfully logged out.\n");

                CredentialsStoreProxy credentialsStoreProxy = CredentialsStoreProxy.SingletonInstance();
                try
                {
                    //Encrypting data
                    byte[] outUsername = Cryptography.EncryptData(username, Cryptography.LoadKey(Cryptography.KeyLocation));

                    byte[] signature = DigitalSignatures.GenerateDigitalSignature(outUsername);

                    int ret = credentialsStoreProxy.ResetUserOnLogOut(outUsername, signature);

                    switch (ret)
                    {
                        case -1:
                            Console.WriteLine("Signature check failed, your data may be tampered with. Please contact your system administrator.\n");
                            Audit.LogoutAttemptFailed(username, "Signature check failed");
                            return -1; //LOGOUT IS NOT SUCCESSFUL
                        case 0:
                            Console.WriteLine($"{username} logged out and user data in DB is successfully reset.\n");
                            Audit.LogoutAttemptSuccess(username);
                            Audit.AuthenticationSuccess(appUser);
                            break;
                    }

                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Client certificate check failed. Please contact your system administrator.\n");
                    Audit.LogoutAttemptFailed(username, "Certificate check failed");
                    return -1;
                }

                return 0; //LOGOUT IS SUCCESSFUL
            }
            else
            {
                Audit.AuthorizationFailed(appUser, "Logout","Invalid Group Permission");
                throw new FaultException<GroupException>(new GroupException("Invalid Group permissions, please contact your system administrator if you think this is a mistake.\n"));
            }
        }
        public int CheckIn(string username)
        {
            List<string> users = new List<string>();

            CredentialsStoreProxy credentialsStoreProxy = CredentialsStoreProxy.SingletonInstance();
            try
            {
                //Encrypting data
                byte[] outUsername = Cryptography.EncryptData(username, Cryptography.LoadKey(Cryptography.KeyLocation));

                //Digital signature generation 
                byte[] signature = DigitalSignatures.GenerateDigitalSignature(outUsername);

                int result = credentialsStoreProxy.CheckIn(outUsername, signature);

                switch (result)
                {
                    case -5:
                        users = loginUsers.getLoginUsers();
                        for (int i = 0; i < users.Count; i++)
                            if (users[i].Split('|')[0] == username)
                            {
                                users.RemoveAt(i);
                                loginUsers.updateLoginUsers(users);
                            }
                        Console.WriteLine("Signature check failed, your data may be TAMPERED with. Please contact your system administrator.\n");
                        return -5; //LOGOUT IS NOT SUCCESSFUL
                    case -4:

                        users = loginUsers.getLoginUsers();
                        for (int i = 0; i < users.Count; i++)
                            if (users[i].Split('|')[0] == username)
                            {
                                users.RemoveAt(i);
                                loginUsers.updateLoginUsers(users);
                            }
                        Console.WriteLine($"{username} -> checked, TIMED OUT.\n");
                        return -4; //LOGOUT IS NOT SUCCESSFUL
                    case -3:
                        users = loginUsers.getLoginUsers();
                        for (int i = 0; i < users.Count; i++)
                            if (users[i].Split('|')[0] == username)
                            {
                                users.RemoveAt(i);
                                loginUsers.updateLoginUsers(users);
                            }
                        Console.WriteLine($"{username} -> checked, DISABLED.\n");
                        return -3; //LOGOUT IS NOT SUCCESSFUL
                    case -2:
                        users = loginUsers.getLoginUsers();
                        for (int i = 0; i < users.Count; i++)
                            if (users[i].Split('|')[0] == username)
                            {
                                users.RemoveAt(i);
                                loginUsers.updateLoginUsers(users);
                            }
                        Console.WriteLine($"{username} -> checked, LOCKED.\n");
                        return -2; //LOGOUT IS NOT SUCCESSFUL
                    case -1:
                        users = loginUsers.getLoginUsers();
                        for (int i = 0; i < users.Count; i++)
                            if (users[i].Split('|')[0] == username)
                            {
                                users.RemoveAt(i);
                                loginUsers.updateLoginUsers(users);
                            }
                        Console.WriteLine($"{username} -> could not be checked, is MISSING from DB.\n");
                        return -1; //LOGOUT IS NOT SUCCESSFUL
                    case 0:
                        Console.WriteLine($"{username} -> checked, no changes.\n");
                        break;
                }
                return 0;
            }

            catch (InvalidOperationException)
            {
                Console.WriteLine("Client certificate check failed. Please contact your system administrator.\n");
                return -1;
            }
        }
    }
}
