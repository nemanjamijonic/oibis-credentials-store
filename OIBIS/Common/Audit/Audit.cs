using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Audit
{
    public class Audit : IDisposable
    {

        private static EventLog customLog = null;
        const string SourceName = "CredentialsStore.Audit";
        const string LogName = "CredentialsStore";

        static Audit()
        {
            try
            {
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }
                customLog = new EventLog(LogName, Environment.MachineName, SourceName);
            }
            catch (Exception e)
            {
                customLog = null;
                Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
            }
        }

        public static void AuthenticationSuccess(string userName) 
        {
            if (customLog != null)
            {
                string AuthenticationSuccess = AuditEvents.AuthentificationSuccess;
                string message = String.Format(AuthenticationSuccess, userName);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.AuthentificationSuccess));
            }
        }

        public static void AuthentificationFalied(string error)
        {
            if (customLog != null)
            {
                string AuthenticationFailure = AuditEvents.AuthentificationFailed;
                string message = String.Format(AuthenticationFailure, error);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.AuthentificationFailed));
            }
        }

        public static void AuthorizationSuccess(string userName, string serviceName)
        {
            if (customLog != null)
            {
                string AuthorizationSuccess = AuditEvents.AuthorizationSuccess;
                string message = String.Format(AuthorizationSuccess, userName, serviceName);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.AuthorizationSuccess));
            }
        }

        public static void AuthorizationFailed(string userName, string serviceName, string reason)
        {
            if (customLog != null)
            {
                string AuthorizationFailed = AuditEvents.AuthorizationFailed;
                string message = String.Format(AuthorizationFailed, userName, serviceName, reason);

                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.AuthorizationFailed));
            }
        }

        public static void DataBaseAccessSuccess(string message) 
        {
            if (customLog != null)
            {
                string DataBaseAccessSuccess = AuditEvents.DataBaseAccessSuccess;
                string logMessage = String.Format(DataBaseAccessSuccess, message);
                customLog.WriteEntry(logMessage);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.DataBaseAccessSuccess));
            }
        }

        public static void DataBaseAccessFailed(string message)
        {
            if (customLog != null)
            {
                string DataBaseAccessFailed = AuditEvents.DataBaseAccessFailed;
                string logMessage = String.Format(DataBaseAccessFailed, message);
                customLog.WriteEntry(logMessage);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.DataBaseAccessFailed));
            }
        }

        public static void LogoutAttemptSuccess(string username)
        {
            if (customLog != null)
            {
                string LogoutAttemptSuccess = AuditEvents.LogoutAttemptSuccess;
                string logMessage = String.Format(LogoutAttemptSuccess, username);
                customLog.WriteEntry(logMessage);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.LogoutAttemptSuccess));
            }
        }

        public static void LogoutAttemptFailed(string username, string reason)
        {
            if (customLog != null)
            {
                string LogoutAttemptFailed = AuditEvents.LogoutAttemptFailed;
                string logMessage = String.Format(LogoutAttemptFailed, username,reason);
                customLog.WriteEntry(logMessage);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.LogoutAttemptFailed));
            }
        }

        public static void LoginAttemptSuccess(string username)
        {
            if (customLog != null)
            {
                string LoginAttemptSuccess = AuditEvents.LoginAttemptSuccess;
                string logMessage = String.Format(LoginAttemptSuccess, username);
                customLog.WriteEntry(logMessage);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.LoginAttemptSuccess));
            }
        }

        public static void LoginAttemptFailed(string username, string reason)
        {
            if (customLog != null)
            {
                string LoginAttemptFailed = AuditEvents.LoginAttemptFailed;
                string logMessage = String.Format(LoginAttemptFailed, username, reason);
                customLog.WriteEntry(logMessage);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.LoginAttemptFailed));
            }
        }

        public void Dispose()
        {
            if (customLog != null)
            {
                customLog.Dispose();
                customLog = null;
            }
        }
    }
}
