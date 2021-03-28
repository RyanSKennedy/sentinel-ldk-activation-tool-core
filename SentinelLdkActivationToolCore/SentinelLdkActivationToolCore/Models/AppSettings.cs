using System;
using System.IO;
using System.Reflection;
using MyLogClass;

namespace SentinelLdkActivationToolCore.Models
{
    public class AppSettings
    {
        public static string MasterPassword = ConfigurationManager.AppSetting["Access:MasterPassword"];

        public static string AdminPassword = ConfigurationManager.AppSetting["Access:AdminPassword"];

        public static string Url = ConfigurationManager.AppSetting["BotNetSettings:BotProtocol"] + "://" +
            ConfigurationManager.AppSetting["BotNetSettings:BotAddress"] + ":" +
            ConfigurationManager.AppSetting["BotNetSettings:BotPort"] + "/{0}";

        public static string HookPart = ConfigurationManager.AppSetting["BotNetSettings:HookPart"];

        public static string Name = ConfigurationManager.AppSetting["BotNetSettings:Name"];

        public static string Key = ConfigurationManager.AppSetting["BotNetSettings:Key"];

        public string WebHook { get; set; }

        public Log NewLog;

        public string AppVersion = typeof(Startup).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

        private string PathForLog { get; set; } = "";

        private bool LogsShouldBeEnabled { get; set; } = ConfigurationManager.AppSetting["CustomLogging:Enable"] == "true" ||
                                                            ConfigurationManager.AppSetting["CustomLogging:Enable"] == "True" ||
                                                            ConfigurationManager.AppSetting["CustomLogging:Enable"] == "1" ? true : false ;
        public bool LogIsEnabled { get; set; } = false;

        public bool LogsFileIsExist { get; set; } = false;

        public bool LogsAutoClearIsEnabled { get; set; } = ConfigurationManager.AppSetting["CustomLogging:AutoClear:Enable"] == "true" ||
                                                            ConfigurationManager.AppSetting["CustomLogging:AutoClear:Enable"] == "True" ||
                                                            ConfigurationManager.AppSetting["CustomLogging:AutoClear:Enable"] == "1" ? true : false;

        public int LogsAutoClearDaysBeforeDelete { get; set; } = Convert.ToInt32(ConfigurationManager.AppSetting["CustomLogging:AutoClear:DaysBeforeDelete"]);

        public bool StorageAutoClearIsEnabled { get; set; } = ConfigurationManager.AppSetting["StorageSettings:AutoClear:Enable"] == "true" ||
                                                            ConfigurationManager.AppSetting["StorageSettings:AutoClear:Enable"] == "True" ||
                                                            ConfigurationManager.AppSetting["StorageSettings:AutoClear:Enable"] == "1" ? true : false;

        public int StorageAutoClearDaysBeforeDelete { get; set; } = Convert.ToInt32(ConfigurationManager.AppSetting["StorageSettings:AutoClear:DaysBeforeDelete"]);

        public AppSettings(string fileName) {
            if (LogsShouldBeEnabled) {
                PathForLog = SentinelMethods.PathBuilder(fileName);
                var setLogResult = SetLogs(PathForLog);
                if (setLogResult == "OK")
                {
                    LogIsEnabled = true;
                    NewLog = new Log(PathForLog);
                }
            }

            WebHook = string.Format(Url, HookPart);
        }

        public AppSettings() {
            WebHook = string.Format(Url, HookPart);
        }

        private string SetLogs(string path)
        {
            var tmpStatus = "";

            // Create log file (if not exist) 
            //=============================================
            if (File.Exists(path))
            {
                LogsFileIsExist = true;
                tmpStatus = "OK";
            }
            else
            {
                try
                {
                    using (File.Create(path))
                    {
                        LogsFileIsExist = File.Exists(path);
                        tmpStatus = "OK";
                    }
                }
                catch (Exception ex) {
                    tmpStatus = "Can't create log file. Error: " + ex.Message;
                }
            }
            //=============================================

            return tmpStatus;
        }
    }
}
