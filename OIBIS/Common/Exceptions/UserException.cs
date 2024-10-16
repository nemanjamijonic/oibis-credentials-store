using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    [DataContract]
    public class UserException
    {
        [DataMember]
        public string exceptionMessage;

        public UserException(string exception) { exceptionMessage = exception; }
    }
}
