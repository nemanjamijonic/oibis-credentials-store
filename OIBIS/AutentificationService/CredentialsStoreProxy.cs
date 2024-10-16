using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AutentificationService
{
    public class CredentialsStoreProxy : ChannelFactory<IValidationManagement>, IValidationManagement, IDisposable
    {

        private static CredentialsStoreProxy instance = null;

        IValidationManagement factory = null;

        public CredentialsStoreProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public CredentialsStoreProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            string clientName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, clientName); //Client public/private-key.PFX 

            factory = this.CreateChannel();
        }

        public static CredentialsStoreProxy SingletonInstance()
        {
            //If the instance is NULL, create one with the parameter bellow using the default Constructor for this class
            if (instance == null)
            {
                NetTcpBinding bindingCredentialsStore = new NetTcpBinding();
                bindingCredentialsStore.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
                X509Certificate2 serverCertificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, "credstore"); //Server public-key.CER 
                EndpointAddress credentialsStoreAddress = new EndpointAddress(new Uri("net.tcp://localhost:8082/CredentialsStore"),
                                                                              new X509CertificateEndpointIdentity(serverCertificate));
                instance = new CredentialsStoreProxy(bindingCredentialsStore, credentialsStoreAddress);
            }
            return instance;
        }

        public int ValidateCredentials(byte[] username, byte[] password, byte[] signature)
        {
            return factory.ValidateCredentials(username, password, signature);
        }

        public int ResetUserOnLogOut(byte[] username, byte[] signature)
        {
            return factory.ResetUserOnLogOut(username, signature);
        }

        public int CheckIn(byte[] username, byte[] signature)
        {
            return factory.CheckIn(username, signature);
        }
    }
}
