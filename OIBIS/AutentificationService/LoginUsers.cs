using Common.Audit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutentificationService
{
    public class LoginUsers
    {
        public void addUser(string user)
        {
            //Audit.DataBaseAccessSuccess("Add Login User");
            StreamWriter sw = new StreamWriter("loginUsers.txt", true, Encoding.ASCII);
            sw.WriteLine(user + '|' + DateTime.Now.ToString("HH:mm:ss"));
            sw.Close();

        }

        public void updateLoginUsers(List<string> users)
        {
           // Audit.DataBaseAccessSuccess("Update Login Users");
            StreamWriter sw = new StreamWriter("loginUsers.txt", false, Encoding.ASCII);
            foreach (string user in users)
                sw.WriteLine(user);
            sw.Close();
        }

        public void resetLoginUsers()
        {
            //Audit.DataBaseAccessSuccess("Reset Login Users");
            StreamWriter sw = new StreamWriter("loginUsers.txt", false, Encoding.ASCII);
            sw.Close();
        }

        public List<string> getLoginUsers()
        {
            //Audit.DataBaseAccessSuccess("Get Login Users");
            List<string> loginUsers = new List<string>();

            if (!File.Exists("loginUsers.txt"))
            {
                return loginUsers;
            }

            StreamReader sr = new StreamReader("loginUsers.txt", Encoding.ASCII);

            while (!sr.EndOfStream)
            {
                string user = sr.ReadLine();
                loginUsers.Add(user);
            }

            sr.Close();
            return loginUsers;
        }
    }
}
