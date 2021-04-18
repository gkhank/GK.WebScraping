using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.WebScraping.Model
{
    public static class Configurations
    {
        public static Boolean IsDevelopment { get { return Environment.MachineName != "GK-WS1"; } }
    }
}
