using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MyLogClass;
using SentinelLdkActivationToolCore.Controllers;
using SentinelLdkActivationToolCore.Models;

namespace SentinelLdkActivationToolCore
{
    public class SentinelMethods
    {
        public static string baseDirConstant { get; set; } = GetBaseDir();

        public SentinelMethods() { }

        public static string UrlBuilder(string action = null, string placeholder = null)
        {
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to build Url for action: " + (!String.IsNullOrEmpty(action) ? action : "null") + ", and placeholder: " + (!String.IsNullOrEmpty(placeholder) ? placeholder : "null"));
            var tmpUrl = SentinelSettings.emsProtocol +
                @"://" +
                SentinelSettings.emsAddress +
                @":" +
                SentinelSettings.emsPort +
                @"/" +
                SentinelSettings.emsBaseDir +
                @"/" +
                (String.IsNullOrEmpty(action) ? "" :
                SentinelSettings.webServiceVersion + @"/" +
                (String.IsNullOrEmpty(placeholder) ? action : action.Replace("{PLACEHOLDER}", placeholder)));

            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Prepared Url is: " + tmpUrl);
            return tmpUrl;
        }

        public static bool ProductKeyAndAidValidator(string data) {
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to validate data");
            var dataValid = false;
            
            string myPatern = @"\w{8}-\w{4}-\w{4}-\w{4}-\w{12}";
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Regex patern is: " + myPatern);

            Regex regex = new Regex(myPatern);
            if (regex.IsMatch(data)) {
                dataValid = true;
            }
            else {
                dataValid = false;
            }

            return dataValid;
        }

        public static string FileNameBuilder(string action, bool incomingFile = false, string key = null) {

            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to prepare filename...");

            var fileName = "";

            var currentTime = string.Format("{0:dd.MM.yyyy-HH.mm.ss.fff}", DateTime.Now);

            switch (action)
            {
                case "getact":
                case "getlic":
                    if (incomingFile) fileName = (String.IsNullOrEmpty(key) ? "" : key + "-") + "currentstate-" + currentTime + ".c2v";
                    else fileName = (String.IsNullOrEmpty(key) ? "" : key + "-") + "license-" + currentTime + ".v2c";
                    break;

                case "getinfo":
                    fileName = (String.IsNullOrEmpty(key) ? "" : key + "-") + "pkinfo-" + currentTime + ".xml";
                    break;

                case "getfpu":
                    if (incomingFile) fileName = (String.IsNullOrEmpty(key) ? "" : key + "-") + "currentstate-" + currentTime + ".c2v";
                    else fileName = (String.IsNullOrEmpty(key) ? "" : key + "-") + "pendingupdates-" + currentTime + ".v2cp";
                    break;

                case "dc2v":
                    if (incomingFile) fileName = (String.IsNullOrEmpty(key) ? "" : key + "-") + "currentstate-" + currentTime + ".c2v";
                    else fileName = (String.IsNullOrEmpty(key) ? "" : key + "-") + "decodedc2v-" + currentTime + ".xml";
                    break;

                case "ukey":
                    fileName = (String.IsNullOrEmpty(key) ? "" : key + "-") + "data-" + currentTime + ".xml";
                    break;

                default:
                    fileName = "unknown-" + currentTime + ".txt";
                    break;
            }

            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Prepared filename is: " + fileName);
            return fileName;
        }

        public static string GetBaseDir()
        {
            // Gate path to base dir of app
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to get base dir");
            var tmpBaseDirPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Base dir path is: " + tmpBaseDirPath);

            return tmpBaseDirPath;
        }

        private static string PathBuilder(string baseDirPath, string fileName)
        {
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to Prepare full path for file: " + fileName);

            string fullPathToFile = "";
            var tmpDir = "";

            if (!String.IsNullOrEmpty(fileName))
            {
                if (fileName.StartsWith("log"))
                {
                    tmpDir = "logs";
                }
                else
                {
                    tmpDir = "tmp";
                }

                var currentYear = DateTime.Now.Year.ToString();
                var currentMonth = DateTime.Now.Month.ToString();

                try
                {
                    Directory.CreateDirectory(baseDirPath + Path.DirectorySeparatorChar + tmpDir + Path.DirectorySeparatorChar + currentYear + Path.DirectorySeparatorChar + currentMonth);
                }
                catch (Exception ex)
                {
                    if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Error: " + ex.Message);
                }

                fullPathToFile = baseDirPath + Path.DirectorySeparatorChar + tmpDir + Path.DirectorySeparatorChar + currentYear + Path.DirectorySeparatorChar + currentMonth + Path.DirectorySeparatorChar + fileName;
                if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Full path should be: " + fullPathToFile);
            }
            else {
                if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"File name is null or enpty!");
            }
            
            return fullPathToFile;
        }

        public static string PathBuilder(string fileName)
        {
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to Prepare full path for file: " + fileName);

            string fullPathToFile = "";
            string tmpDir = "";

            if (!String.IsNullOrEmpty(fileName)) {
                if (fileName.StartsWith("log"))
                {
                    tmpDir = "logs";
                }
                else
                {
                    tmpDir = "tmp";
                }

                var currentYear = DateTime.Now.Year.ToString();
                var currentMonth = DateTime.Now.Month.ToString(); 

                try
                {
                    Directory.CreateDirectory(baseDirConstant + Path.DirectorySeparatorChar + tmpDir + Path.DirectorySeparatorChar + currentYear + Path.DirectorySeparatorChar + currentMonth);
                }
                catch (Exception ex)
                {
                    if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Error: " + ex.Message);
                }

                fullPathToFile = baseDirConstant + Path.DirectorySeparatorChar + tmpDir + Path.DirectorySeparatorChar + currentYear + Path.DirectorySeparatorChar + currentMonth + Path.DirectorySeparatorChar + fileName;
                if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Full path should be: " + fullPathToFile);
            } else {
                if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"File name is null or enpty!");
            }

            return fullPathToFile;
        }

        public static string PathBuilder(string fileName, bool incomingFile)
        {
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to Prepare full path for save incoming file: " + fileName);

            string fullPathToFile = "";
            string tmpDir = "";

            if (!String.IsNullOrEmpty(fileName))
            {
                if (fileName.StartsWith("log"))
                {
                    tmpDir = "logs";
                }
                else
                {
                    if (incomingFile) {
                        tmpDir = "tmp-incoming";
                    } else {
                        tmpDir = "tmp";
                    }
                }

                var currentYear = DateTime.Now.Year.ToString();
                var currentMonth = DateTime.Now.Month.ToString();

                try
                {
                    Directory.CreateDirectory(baseDirConstant + Path.DirectorySeparatorChar + tmpDir + Path.DirectorySeparatorChar + currentYear + Path.DirectorySeparatorChar + currentMonth);
                }
                catch (Exception ex)
                {
                    if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Error: " + ex.Message);
                }

                fullPathToFile = baseDirConstant + Path.DirectorySeparatorChar + tmpDir + Path.DirectorySeparatorChar + currentYear + Path.DirectorySeparatorChar + currentMonth + Path.DirectorySeparatorChar + fileName;
                if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Full path should be: " + fullPathToFile);
            }
            else
            {
                if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"File name is null or enpty!");
            }

            return fullPathToFile;
        }

        public static string SaveFile(string fullPathToFile, object value)
        {
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to save data in to file: " + fullPathToFile);

            try
            {
                System.IO.File.WriteAllText(fullPathToFile, value.ToString(), System.Text.Encoding.UTF8);
                if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Data was saved successfully - status: OK");

                return "OK";
            }
            catch (Exception ex)
            {
                if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Can't create or append data in file. Error: " + ex.Message);

                return "Can't create or append data in file. Error: " + ex.Message;
            }
        }
    }
}
