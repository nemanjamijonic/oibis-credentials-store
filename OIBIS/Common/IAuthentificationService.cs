using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IAuthentificationService
    {
        [OperationContract]
        [FaultContract(typeof(GroupException))]
        int Login(string username, string password,string appUser);

        [OperationContract]
        [FaultContract(typeof(GroupException))]
        int Logout(string username, string appUser);

        [OperationContract]
        int CheckIn(string username);
    }
}
