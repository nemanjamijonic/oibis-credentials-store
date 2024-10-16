using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using AutentificationService;
using AuthentificationService;
using Common;

namespace AuthentificationService
{
    public class Program
    {
        static void Main(string[] args)
        {

            string secretKey = Cryptography.GenerateKey();
            Cryptography.StoreKey(secretKey, Cryptography.KeyLocation);

            //AuthenticationService SERVER INIT

            NetTcpBinding bindingClient = new NetTcpBinding();
            string address = "net.tcp://localhost:8081/AuthentificationService";


            //WINDOWS AUTHENTICATION PROTOCOL INIT FOR CLIENT

            bindingClient.Security.Mode = SecurityMode.Message;
            bindingClient.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            bindingClient.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            ServiceHost host = new ServiceHost(typeof(AuthentificationService));

            ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation = AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            host.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            host.Description.Behaviors.Add(newAudit);

            host.AddServiceEndpoint(typeof(IAuthentificationService), bindingClient, address);
            host.Open();


            Console.WriteLine($"AuthenticationService is started: [{WindowsIdentity.GetCurrent().User}] ->  {WindowsIdentity.GetCurrent().Name}.\n");


            Console.ReadKey();

            //RESET CURRENTLY LOGGED IN USERS ON AS CLOSE

            LoginUsers loginUsers = new LoginUsers();
            loginUsers.resetLoginUsers();

            host.Close();
        }
    }
}
