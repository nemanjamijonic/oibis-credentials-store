using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class AuthenticationProxy : ChannelFactory<IAuthentificationService>, IAuthentificationService, IDisposable
    {
        IAuthentificationService factory;

        public AuthenticationProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public int CheckIn(string username)
        {
            return factory.CheckIn(username);
        }

        public int Login(string username, string password, string appUser)
        {
            return factory.Login(username, password, appUser);
        }

        public int Logout(string username,string appUser)
        {
            return factory.Logout(username,appUser);
        }
    }
}
