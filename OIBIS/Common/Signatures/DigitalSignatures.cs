using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.Signatures
{
    public class DigitalSignatures
    {
        public static byte[] GenerateDigitalSignature(byte[] data)
        {
            string signCertificate = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, signCertificate);

            return Create(data, certificateSign);
        }
        public static bool VerifyDigitalSignature(byte[] data, byte[] signature)
        {
            string clienName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);// Using when we use certificates
            string clientNameSign = clienName;
            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientNameSign);

            if (Verify(data, signature, certificate))
            {
                Console.WriteLine("Data signature is valid.\n");
                return true;
            }
            else
            {
                Console.WriteLine("Data signature is invalid.\n");
                return false;
            }
        }


        public static byte[] Create(byte[] data, X509Certificate2 certificate)
        {
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)certificate.PrivateKey;

            if (csp == null)
                throw new Exception("Valid certificate not found!");


            SHA1Managed sha1 = new SHA1Managed();
            byte[] dataByteHash = sha1.ComputeHash(data);

            byte[] signature = csp.SignHash(dataByteHash, CryptoConfig.MapNameToOID("SHA1"));
            return signature;
        }

        public static bool Verify(byte[] data, byte[] signature, X509Certificate2 certificate)
        {
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)certificate.PublicKey.Key;

            SHA1Managed sha1 = new SHA1Managed();
            byte[] hashdataBytesArray = sha1.ComputeHash(data);

            return csp.VerifyHash(hashdataBytesArray, CryptoConfig.MapNameToOID("SHA1"), signature);
        }
    }
}
