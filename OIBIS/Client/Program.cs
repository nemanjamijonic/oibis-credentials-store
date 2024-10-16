using Common.Audit;
using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string authenticationServiceAddress = "net.tcp://localhost:8081/AuthentificationService";
            string credentialsStoreAddress = "net.tcp://localhost:8086/CredentialsStore";

            //WINDOWS AUTHENTICATION PROTOCOL INIT


            binding.Security.Mode = SecurityMode.Message;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            string fullUsername = WindowsIdentity.GetCurrent().Name;
            string[] splitName = fullUsername.Split('\\'); // Splitting by backslash

            string appUser = splitName[splitName.Length - 1];
        
            Console.WriteLine($"Currently used by [{WindowsIdentity.GetCurrent().User}] -> " + fullUsername + "\n");
            
            

            string username = "";
            string password = "";
            string option = "";

            using (AuthenticationProxy authenticationProxy = new AuthenticationProxy(binding, authenticationServiceAddress))
            {
                using (CredentialsStoreProxy credentialsStoreProxy = new CredentialsStoreProxy(binding, credentialsStoreAddress))
                {
                    do
                    {
                        Console.WriteLine("-----SERVER OPTIONS-----\n");
                        Console.WriteLine("1. Sign In");
                        Console.WriteLine("2. Create Account");
                        Console.WriteLine("3. Delete Account");
                        Console.WriteLine("4. Disable Account");
                        Console.WriteLine("5. Enable Account");
                        Console.WriteLine("6. Lock Account");
                        Console.WriteLine("7. Sign Out");
                        Console.Write("\n-> ");
                        option = Console.ReadLine();

                        switch (option)
                        {
                            case "1":
                                if (username == "")
                                {
                                    Console.Write("Your username: ");
                                    username = Console.ReadLine();
                                    Console.Write("Your password: ");
                                    password = Console.ReadLine();

                                    try
                                    {
                                        int ret = authenticationProxy.Login(username, password,appUser);

                                        switch (ret)
                                        {
                                            case -4:
                                                Console.WriteLine("Your data may be TAMPERED with. Please contact your system administrator.\n");
                                                username = "";
                                                break;
                                            case -3:
                                                Console.WriteLine($"\n{username} is DISABLED. Please contact your system administrator.\n");
                                                username = "";
                                                break;
                                            case -2:
                                                Console.WriteLine($"\n{username} is LOCKED. Please contact your system administrator or wait some time and try again.\n");
                                                username = "";
                                                break;
                                            case -1:
                                                Console.WriteLine($"\n{username} does not exist in our Database. Please contact your system administrator.\n");
                                                username = "";
                                                break;
                                            case 0:
                                                Console.WriteLine($"\n{username} password does not match please try again.\n");
                                                username = "";
                                                break;
                                            case 1:
                                                Console.WriteLine($"\n{username} successfully logged in.\n");
                                                Task t = Task.Factory.StartNew(() => DoCheck(authenticationProxy, ref username));
                                                break;
                                            case 2:
                                                Console.WriteLine($"\n{username} already logged in, try logging out first.\n");
                                                break;
                                        }
                                    }

                                    catch (FaultException<GroupException> ex)
                                    {
                                        Console.WriteLine(ex.Detail.exceptionMessage);
                                        username = "";
                                        break;
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                        username = "";
                                        break;
                                    }
                                }
                                else
                                    Console.WriteLine("You are already logged in, please log out first.\n");

                                break;


                            case "2":
                                try
                                {
                                    Console.Write("Account username: ");
                                    username = Console.ReadLine();
                                    Console.Write("Account password: ");
                                    password = Console.ReadLine();
                                    credentialsStoreProxy.CreateAccount(username, password, appUser);
                                    Console.WriteLine("Account successfully created.\n");
                                    username = "";
                                    break;

                                }
                                catch (FaultException<GroupException> ex)
                                {
                                    Console.WriteLine(ex.Detail.exceptionMessage);
                                    username = "";
                                    break;
                                }
                                catch (FaultException<UserException> ex)
                                {
                                    Console.WriteLine(ex.Detail.exceptionMessage);
                                    username = "";
                                    break;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    break;
                                }
                            case "3":
                                try
                                {
                                    Console.Write("Account username: ");
                                    username = Console.ReadLine();
                                    credentialsStoreProxy.DeleteAccount(username, appUser);
                                    Console.WriteLine("Account successfully deleted.\n");
                                    username = "";
                                    break;
                                }
                                catch (FaultException<GroupException> ex)
                                {
                                    Console.WriteLine(ex.Detail.exceptionMessage);
                                    username = "";
                                    break;
                                }
                                catch (FaultException<UserException> ex)
                                {
                                    Console.WriteLine(ex.Detail.exceptionMessage);
                                    username = "";
                                    break;
                                }
                            case "4":
                                try
                                {
                                    Console.Write("Account username: ");
                                    username = Console.ReadLine();
                                    credentialsStoreProxy.DisableAccount(username, appUser);
                                    Console.WriteLine("Account successfully disabled.\n");
                                    username = "";
                                    break;
                                }
                                catch (FaultException<GroupException> ex)
                                {
                                    Console.WriteLine(ex.Detail.exceptionMessage);
                                    username = "";
                                    break;
                                }
                                catch (FaultException<UserException> ex)
                                {
                                    Console.WriteLine(ex.Detail.exceptionMessage);
                                    username = "";
                                    break;
                                }
                            case "5":
                                try
                                {
                                    Console.Write("Account username: ");
                                    username = Console.ReadLine();
                                    credentialsStoreProxy.EnableAccount(username, appUser);
                                    Console.WriteLine("Account successfully enabled.\n");
                                    username = "";
                                    break;
                                }
                                catch (FaultException<GroupException> ex)
                                {
                                    Console.WriteLine(ex.Detail.exceptionMessage);
                                    username = "";
                                    break;
                                }
                                catch (FaultException<UserException> ex)
                                {
                                    Console.WriteLine(ex.Detail.exceptionMessage);
                                    username = "";
                                    break;
                                }
                            case "6":
                                try
                                {
                                    Console.Write("Account username: ");
                                    username = Console.ReadLine();
                                    credentialsStoreProxy.LockAccount(username, appUser);
                                    Console.WriteLine("Account successfully locked.\n");
                                    username = "";
                                    break;
                                }
                                catch (FaultException<GroupException> ex)
                                {
                                    Console.WriteLine(ex.Detail.exceptionMessage);
                                    username = "";
                                    break;
                                }
                                catch (FaultException<UserException> ex)
                                {
                                    Console.WriteLine(ex.Detail.exceptionMessage);
                                    username = "";
                                    break;
                                }
                            case "7":
                                try
                                {
                                    if (username != "")
                                    {
                                        authenticationProxy.Logout(username,appUser);
                                        username = "";
                                        Console.WriteLine("You have successfully logged out.\n");
                                    }
                                    else
                                        Console.WriteLine("You have to be logged in first.\n");
                                }
                                catch (FaultException<GroupException> ex)
                                {
                                    Console.WriteLine(ex.Detail.exceptionMessage);
                                    username = "";
                                    break;
                                }
                                catch (FaultException<UserException> ex)
                                {
                                    Console.WriteLine(ex.Detail.exceptionMessage);
                                    username = "";
                                    break;
                                }
                                break;
                            default:
                                Console.WriteLine("Ivalid option, please try again or contact your server administrator.\n");
                                break;
                        }

                    } while (option != "exit");

                }


            }

        }
        static void DoCheck(AuthenticationProxy proxy, ref string user)
        {
            while (proxy.State == CommunicationState.Opened)
            {
                int result = proxy.CheckIn(user);

                switch (result)
                {
                    case -4:
                        Console.WriteLine("\n{0} Your Account has been TIMED OUT for inactivity.\n", user);
                        Console.Write("->");
                        user = "";
                        return;
                    case -3:
                        Console.WriteLine("\n{0} Your Account has been DISABLED and logged out.\n", user);
                        Console.Write("->");
                        user = "";
                        return;
                    case -2:
                        Console.WriteLine("\n{0} Your Account has been LOCKED and logged out.\n", user);
                        Console.Write("->");
                        user = "";
                        return;
                    case -1:
                        Console.WriteLine("\n{0} Could not be checked, is MISSING from DB and logged out.\n", user);
                        Console.Write("->");
                        user = "";
                        return;
                    default:
                        Thread.Sleep(2500);
                        break;
                }
            }
        }
    }

}
