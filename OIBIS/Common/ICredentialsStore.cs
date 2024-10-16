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
    public interface ICredentialsStore
    {
        [OperationContract]
        [FaultContract(typeof(GroupException))]
        [FaultContract(typeof(UserException))]
        void CreateAccount(string username, string password, string appUser);

        [OperationContract]
        [FaultContract(typeof(GroupException))]
        [FaultContract(typeof(UserException))]
        void DeleteAccount(string username, string appUser);

        [OperationContract]
        [FaultContract(typeof(GroupException))]
        [FaultContract(typeof(UserException))]
        void LockAccount(string username, string appUser);

        [OperationContract]
        [FaultContract(typeof(GroupException))]
        [FaultContract(typeof(UserException))]
        void EnableAccount(string username, string appUser);

        [OperationContract]
        [FaultContract(typeof(GroupException))]
        [FaultContract(typeof(UserException))]
        void DisableAccount(string username, string appUser);
    }
}
