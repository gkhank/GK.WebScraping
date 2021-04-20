using System;

namespace GK.WebScraping.Utilities
{
    public static class ApplicationPath
    {
        #region Keep private if possible
        private static String ResourcesDirectory
        {
            get
            {
                return ApplicationPath.RootDirectory + "\\Resources";
            }
        }

        private static String RootDirectory
        {
            get
            {
                switch (Environment.MachineName)
                {
                    case "GK-WS1":
                        return "C:\\Program Files (x86)\\GKMedia\\Services";

                    case "GK_DESKTOP":
                        return "D:\\Projects\\GK.WebScraping";

                    case "GOKHANWINL10":
                        return "C:\\Users\\Gökhan\\GK.WebScraping";
                    default:
                        throw new NotImplementedException();
                }

            }
        }
        #endregion

        public static String PagesDirectory
        {
            get
            {
                return ApplicationPath.ResourcesDirectory + "\\Pages";
            }
        }

        public static String ConfigDirectory
        {
            get
            {
                return ApplicationPath.ResourcesDirectory + "\\Config";
            }
        }
    }
}
