using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Groups
    {
        private static string generalUser = "AccountUsers";
        private static string adminUser = "AccountAdmins";

        public static string GeneralUser { get => generalUser; set => generalUser = value; }
        public static string AdminUser { get => adminUser; set => adminUser = value; }
    }
}
