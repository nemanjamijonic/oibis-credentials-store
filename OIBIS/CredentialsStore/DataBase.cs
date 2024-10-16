using Common;
using Common.Audit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredentialsStore
{

    public class DataBase
    {
        AccountPolicy ap = new AccountPolicy();

        public void addUser(User U)
        {
           Audit.DataBaseAccessSuccess("Succesfully Accessed Add User");
            string text = "";
            string locked;
            string disabled;

            if (U.IsLocked)
                locked = "Y";
            else locked = "N";

            if (U.IsDisabled)
                disabled = "Y";
            else disabled = "N";

            text = text + U.Username + "|" + U.Password.GetHashCode() + "|" + locked + "|" + disabled + "|" +
                          U.LockedTime + "|" + U.LoggedInTime;


            StreamWriter sw = new StreamWriter("db.txt", true, Encoding.ASCII);

            sw.WriteLine(text);
            sw.Close();
        }

        public void addUsers(List<User> users)
        {
            Audit.DataBaseAccessSuccess("Succesfully Accessed Add Users");
            string text = "";
            string locked;
            string disabled;

            foreach (User U in users)
            {

                if (U.IsLocked) locked = "Y"; else locked = "N";
                if (U.IsDisabled) disabled = "Y"; else disabled = "N";
                text = text + U.Username + "|" + U.Password + "|" + locked + "|" + disabled + "|" +
                    U.LockedTime + "|" + U.LoggedInTime + "\n";

            }
            StreamWriter sw = new StreamWriter("db.txt", false, Encoding.ASCII);
            sw.Write(text);
            Audit.DataBaseAccessSuccess("Succesfully Added Users");
            sw.Close();
        }

        public List<User> getUsers()
        {
            Audit.DataBaseAccessSuccess("Succesfully Accessed Get Users");
            List<User> ret = new List<User>();
            if (!File.Exists("db.txt"))
            {
                return ret;
            }
            StreamReader sr = new StreamReader("db.txt");
            string line = sr.ReadLine();
           
            while (line != null)
            {
                
                if (line != "" && line != "\n")
                {
                    string[] args = line.Split('|');

                    bool locked = false;
                    bool disabled = false;

                    if (args[2] == "Y")
                        locked = true;
                    if (args[3] == "Y")
                        disabled = true;


                    if (locked)
                        if (args[4] != string.Empty)
                        {
                            int vreme = ((int.Parse(args[4].Split(':')[0]) * 60) + (int.Parse(args[4].Split(':')[1]))) + ap.GetLockDuration();
                            if (vreme <= ((DateTime.Now.Hour * 60) + DateTime.Now.Minute))
                            {
                                locked = false;
                                args[4] = "";
                            }
                        }


                    if (args[5] != string.Empty)
                    {
                        int lVreme = ((int.Parse(args[5].Split(':')[0]) * 60) + (int.Parse(args[5].Split(':')[1]))) + ap.GetTimeOutInterval();
                        if (lVreme <= ((DateTime.Now.Hour * 60) + DateTime.Now.Minute))
                        {
                            disabled = true;
                            locked = false;
                            args[5] = "";
                        }
                    }

                    User U = new User(args[0], args[1], locked, disabled, args[4], args[5]);

                    ret.Add(U);
                    line = sr.ReadLine();
                }
                else
                    line = sr.ReadLine();
            }

            sr.Close();

            return ret;
        }
    }

}
