using System;
using System.IO;
using System.Reflection;
using MyLogClass;

namespace SentinelLdkActivationToolCore.Models
{
    public class AppSettings
    {
        public const string Url = "https://rsk-inc.ru:88/{0}";
        public const string HookPart = "api/message/update";
        public const string Name = "SentinelLdkSuperBot";
        public const string Key = "1502781432:AAGddoTRkW8gOTcmZj3zE00gvKgMv65xwYA";
        
        public string WebHook { get; set; }  
        public Log NewLog;

        public string AppVersion = typeof(Startup).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
        private string PathForLog { get; set; } = "";
        private bool LogsShouldBeEnabled { get; set; } = true;
        public bool LogIsEnabled { get; set; } = false;
        public bool LogsFileIsExist { get; set; } = false;

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
