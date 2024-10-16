using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class AccountPolicy
    {
        public int GetLockDuration()
        {

            StreamReader sw = new StreamReader("../../../Config.txt");
            string line = sw.ReadLine();
            sw.Close();
            return int.Parse(line.Split('|')[0].Split(':')[1]);
        }
        public int GetTimeOutInterval()
        {
            StreamReader sw = new StreamReader("../../../Config.txt");
            string line = sw.ReadLine();
            sw.Close();
            return int.Parse(line.Split('|')[1].Split(':')[1]);
        }
        public int GetFailedAttempts()
        {
            StreamReader sw = new StreamReader("../../../Config.txt");
            string line = sw.ReadLine();
            sw.Close();
            return int.Parse(line.Split('|')[2].Split(':')[1]);
        }
    }
}
