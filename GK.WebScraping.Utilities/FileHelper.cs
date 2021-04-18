using System;
using System.IO;

namespace GK.WebScraping.Utilities
{
    public static class FileHelper
    {

        //Keep it private if possible...
        private static String ResourcesDirectory
        {
            get
            {
                switch (Environment.MachineName)
                {
                    case "GK-WS1":
                        return "C:\\Program Files (x86)\\GKMedia\\Services\\Resources";

                    case "GK_DESKTOP":
                        return "D:\\Projects\\GK.WebScraping\\Resources";

                    case "GOKHANWINL10":
                        return "C:\\Users\\Gökhan\\GK.WebScraping\\";
                    default:
                        throw new NotImplementedException();
                }

            }
        }
        public static String PagesDirectory
        {
            get
            {
                return FileHelper.ResourcesDirectory + "\\Pages";
            }
        }
    }
}
