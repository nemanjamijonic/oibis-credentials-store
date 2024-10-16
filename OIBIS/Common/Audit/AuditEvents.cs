using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Common.Audit
{

    public enum AuditEventTypes {
        AuthentificationSuccess = 1,
        AuthentificationFailed = 0,
        AuthorizationSuccess = 3,
        AuthorizationFailed = 2,
        DataBaseAccessSuccess = 5,
        DataBaseAccessFailed = 4,
        LogoutAttemptSuccess = 6,
        LogoutAttemptFailed = 7,
        LoginAttemptSuccess = 8,
        LoginAttemptFailed = 9
    }

    public class AuditEvents
    {

        private static ResourceManager resourceManager = null;
        private static object resourceLock = new object();

        private static ResourceManager ResourceMgr
        {
            get
            {
                lock (resourceLock)
                {
                    if (resourceManager == null)
                    {
                        resourceManager = new ResourceManager
                            (typeof(AuditEventFile).ToString(),
                            Assembly.GetExecutingAssembly());
                    }
                    return resourceManager;
                }
            }
        }



        public static string AuthentificationSuccess 
        {
            get 
            {
                return ResourceMgr.GetString(AuditEventTypes.AuthentificationSuccess.ToString());
            }
        }

        public static string AuthentificationFailed
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.AuthentificationFailed.ToString());
            }
        }

        public static string AuthorizationSuccess 
        {
            get 
            {
                return ResourceMgr.GetString (AuditEventTypes.AuthorizationSuccess.ToString());
            }
        }

        public static string AuthorizationFailed 
        {
            get 
            {
                return ResourceMgr.GetString(AuditEventTypes.AuthorizationFailed.ToString());
            }
        }

        public static string DataBaseAccessSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.DataBaseAccessSuccess.ToString());
            }
        }

        public static string DataBaseAccessFailed
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.DataBaseAccessFailed.ToString());
            }
        }

        public static string LogoutAttemptSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.LogoutAttemptSuccess.ToString());
            }

        }

        public static string LogoutAttemptFailed
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.LogoutAttemptFailed.ToString());
            }
        }

        public static string LoginAttemptFailed
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.LoginAttemptFailed.ToString());
            }
        }

        public static string LoginAttemptSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.LoginAttemptSuccess.ToString());
            }
        }
    }
}
