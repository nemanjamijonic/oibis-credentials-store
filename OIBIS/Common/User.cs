using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDisabled { get; set; }
        public string LockedTime { get; private set; }
        public string LoggedInTime { get; private set; }

        public User(string username, string password, bool isLocked, bool isDisabled, string lockedTime, string loggedInTime)
        {
            Username = username;
            Password = password;
            IsLocked = isLocked;
            IsDisabled = isDisabled;
            LockedTime = lockedTime;
            LoggedInTime = loggedInTime;
        }

        public void Lock()
        {
            IsLocked = true;
            LockedTime = DateTime.Now.ToString("HH:mm:ss");
            LoggedInTime = "";
        }

        public void Unlock()
        {
            IsLocked = false;
            LockedTime = "";
        }

        public void Disable()
        {
            IsDisabled = true;
            LoggedInTime = "";
            LockedTime = "";
        }

        public void Enable()
        {
            IsDisabled = false;
        }

        public void SetLoggedInTime(string timeReset)
        {
            LoggedInTime = timeReset;
        }

        public void SetLoggedInTime()
        {
            LoggedInTime = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}

