using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class CredentialsStoreProxy : ChannelFactory<ICredentialsStore>, ICredentialsStore, IDisposable
    {
        ICredentialsStore factory;

        public CredentialsStoreProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public void CreateAccount(string username, string password, string appUser)
        {
            factory.CreateAccount(username, password,appUser);
        }

        public void DeleteAccount(string username, string appUser)
        {
            factory.DeleteAccount(username,appUser);
        }

        public void DisableAccount(string username, string appUser)
        {
            factory.DisableAccount(username,appUser);
        }

        public void EnableAccount(string username, string appUser)
        {
            factory.EnableAccount(username,appUser);
        }

        public void LockAccount(string username, string appUser)
        {
            factory.LockAccount(username,appUser);
        }
    }
}
