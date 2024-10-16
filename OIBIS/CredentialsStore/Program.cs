using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using Common;
using System.ServiceModel.Security;
using System.ServiceModel.Description;
using Common.Audit;

namespace CredentialsStore
{
    internal class Program
    {
        static void Main(string[] args)
        {

            NetTcpBinding bindingClient = new NetTcpBinding();
            NetTcpBinding bindingAuthentificationService = new NetTcpBinding();
            string addressClient = "net.tcp://localhost:8086/CredentialsStore";
            string addressAuthentificationService = "net.tcp://localhost:8082/CredentialsStore";

            //WINDOWS AUTHENTICATION PROTOCOL INIT FOR CLIENT

            bindingClient.Security.Mode = SecurityMode.Message;
            bindingClient.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            bindingClient.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            ServiceHost hostCredentialsStore = new ServiceHost(typeof(CredentialsStore));
            ServiceHost hostAuthenticationServiceManagement = new ServiceHost(typeof(ValidationManagment));

            //CERTIFICATE SERVER CONFIGURATION INIT

            string serverName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            bindingAuthentificationService.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            hostAuthenticationServiceManagement.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            hostAuthenticationServiceManagement.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            hostAuthenticationServiceManagement.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, serverName); //Server public/private-key.PFX

            hostCredentialsStore.AddServiceEndpoint(typeof(ICredentialsStore), bindingClient, addressClient);
            hostAuthenticationServiceManagement.AddServiceEndpoint(typeof(IValidationManagement), bindingAuthentificationService, addressAuthentificationService);

            ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation = AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            hostCredentialsStore.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            hostCredentialsStore.Description.Behaviors.Add(newAudit);

            hostAuthenticationServiceManagement.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            hostAuthenticationServiceManagement.Description.Behaviors.Add(newAudit);

           
            hostCredentialsStore.Open();
               
            
            try
            {
                hostAuthenticationServiceManagement.Open();
                Audit.AuthenticationSuccess("CredentialsStore-AuthenitficationService");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Server certificate check failed. Please contact your system administrator.\n");
                Audit.AuthentificationFalied("Certificate check failed");
                hostCredentialsStore.Abort(); //To avoid CS server faulted state
                return;
            }

            Console.WriteLine($"Credentials store servis successfully started by [{WindowsIdentity.GetCurrent().User}] -> " + WindowsIdentity.GetCurrent().Name + ".\n");

            Console.ReadLine();

            hostCredentialsStore.Close();
            hostAuthenticationServiceManagement.Close();
        }
    }
}
