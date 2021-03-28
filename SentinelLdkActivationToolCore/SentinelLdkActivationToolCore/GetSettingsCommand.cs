using System;
using System.Threading.Tasks;
using MyLogClass;
using SentinelLdkActivationToolCore.Controllers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SentinelLdkActivationToolCore.Models.Commands
{
    public class GetSettingsCommand : Command
    {
        public override string Name => "getsettings";
        public override string Description => "this is internal command for geting list of settings (required master password).";

        public override bool Contains(Message message)
        {
            bool result;

            if (message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
            {
                result = !String.IsNullOrEmpty(message.Caption) ? ((message.Caption == ("/" + this.Name) || message.Caption.Contains("/" + this.Name + " ")) ? true : false) : false;
            }
            else if (message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                result = (message.Text == ("/" + this.Name) || message.Text.Contains("/" + this.Name + " ")) ? true : false;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Get Chat Id for response message: " + chatId);

            var myMessage = "";
            var messageForLogs = "";
            var password = "";

            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to get Password from message: " + message.Text);
            try
            {
                var tmpMass = message.Text.Split(" ", 1);
                foreach (string el in tmpMass)
                {
                    if (el.Contains("pass:") || el.Contains("password:") || el.Contains("p:")) password = el.Split(":")[1];
                }
            }
            catch { }
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Password is: " + password);

            var isMaster = false;
            if (password == AppSettings.MasterPassword) isMaster = true;

            if (password == AppSettings.MasterPassword || password == AppSettings.AdminPassword)
            {
                myMessage = "Here is list of" + (isMaster ? " FULL" : "") + " settings: \n\n";
                messageForLogs = "Here is list of" + (isMaster ? " FULL" : "") + " settings: ";
                myMessage += "//=======\n";
                myMessage += "\n";
                myMessage += "// Bot Settings -------\n";
                //myMessage += "//---\n";
                myMessage += "BotBuildVersion: " + Startup.myAppSettings.AppVersion + "\n";
                messageForLogs += "BotBuildVersion: " + Startup.myAppSettings.AppVersion + " | ";
                //myMessage += "//---\n";
                myMessage += "BotProtocol: " + ConfigurationManager.AppSetting["BotNetSettings:BotProtocol"] + "\n";
                messageForLogs += "BotProtocol: " + ConfigurationManager.AppSetting["BotNetSettings:BotProtocol"] + " | ";
                //myMessage += "//---\n";
                myMessage += "BotAddress: " + ConfigurationManager.AppSetting["BotNetSettings:BotAddress"] + "\n";
                messageForLogs += "BotAddress: " + ConfigurationManager.AppSetting["BotNetSettings:BotAddress"] + " | ";
                //myMessage += "//---\n";
                myMessage += "BotPort: " + ConfigurationManager.AppSetting["BotNetSettings:BotPort"] + "\n";
                messageForLogs += "BotPort: " + ConfigurationManager.AppSetting["BotNetSettings:BotPort"] + " | ";
                //myMessage += "//---\n";
                myMessage += "BotlUrl: " + AppSettings.Url + "\n";
                messageForLogs += "BotlUrl: " + AppSettings.Url + " | ";
                //myMessage += "//---\n";
                myMessage += "BotlHookPart: " + AppSettings.HookPart + "\n";
                messageForLogs += "BotlHookPart: " + AppSettings.HookPart + " | ";
                //myMessage += "//---\n";
                myMessage += "BotlFullUrl: " + string.Format(AppSettings.Url, AppSettings.HookPart) + "\n";
                messageForLogs += "BotlFullUrl: " + string.Format(AppSettings.Url, AppSettings.HookPart) + " | ";
                //myMessage += "//---\n";
                myMessage += "BotName: " + AppSettings.Name + "\n";
                messageForLogs += "BotName: " + AppSettings.Name + " | ";
                if (isMaster)
                {
                    //myMessage += "//---\n";
                    myMessage += "BotKey: " + AppSettings.Key + "\n";
                    messageForLogs += "BotKey: " + AppSettings.Key + " | ";
                }
                //myMessage += "//---\n";
                myMessage += "// End Bot Settings -------\n";
                myMessage += "\n";
                myMessage += "// EMS Settings -------\n";
                //myMessage += "//---\n";
                myMessage += "IgnoreSSLStatus: " + SentinelSettings.ignoreSslCertStatus.ToString() + "\n";
                messageForLogs += "IgnoreSSLStatus: " + SentinelSettings.ignoreSslCertStatus.ToString() + " | ";
                //myMessage += "//---\n";
                myMessage += "EmsProtocol: " + SentinelSettings.emsProtocol + "\n";
                messageForLogs += "EmsProtocol: " + SentinelSettings.emsProtocol + " | ";
                //myMessage += "//---\n";
                myMessage += "EmsAddress: " + SentinelSettings.emsAddress + "\n";
                messageForLogs += "EmsAddress: " + SentinelSettings.emsAddress + " | ";
                //myMessage += "//---\n";
                myMessage += "EmsPort: " + SentinelSettings.emsPort + "\n";
                messageForLogs += "EmsPort: " + SentinelSettings.emsPort + " | ";
                //myMessage += "//---\n";
                myMessage += "EmsBaseDir: " + SentinelSettings.emsBaseDir + "\n";
                messageForLogs += "EmsBaseDir: " + SentinelSettings.emsBaseDir + " | ";
                //myMessage += "//---\n";
                myMessage += "EmsWebServiceVersion: " + SentinelSettings.webServiceVersion + "\n";
                messageForLogs += "EmsWebServiceVersion: " + SentinelSettings.webServiceVersion + " | ";
                //myMessage += "//---\n";
                myMessage += "EmsUrl: " + SentinelMethods.UrlBuilder() + "\n";
                messageForLogs += "EmsUrl: " + SentinelMethods.UrlBuilder() + " | ";
                //myMessage += "//---\n";
                myMessage += "BatchCode: " + SentinelSettings.batchCode + "\n";
                messageForLogs += "BatchCode: " + SentinelSettings.batchCode + " | ";
                //myMessage += "//---\n";
                myMessage += "VendorId: " + SentinelSettings.vendorId + "\n";
                messageForLogs += "VendorId: " + SentinelSettings.vendorId + " | ";
                if (isMaster)
                {
                    //myMessage += "//---\n";
                    myMessage += "Vendor account login: " + SentinelSettings.vendorLogin + "\n";
                    messageForLogs += "Vendor account login: " + SentinelSettings.vendorLogin + " | ";
                    myMessage += "Vendor account password: " + SentinelSettings.vendorPassword + "\n";
                    messageForLogs += "Vendor account password: " + SentinelSettings.vendorPassword + " | ";
                }
                //myMessage += "//---\n";
                myMessage += "// End EMS Settings -------\n";
                myMessage += "\n";
                myMessage += "// Custom logging -------\n";
                //myMessage += "//---\n";
                myMessage += "LogsIsEnabled: " + Startup.myAppSettings.LogIsEnabled + "\n";
                messageForLogs += "LogsIsEnabled: " + Startup.myAppSettings.LogIsEnabled + " | ";
                //myMessage += "//---\n";
                myMessage += "LogsAutoClearIsEnabled: " + Startup.myAppSettings.LogsAutoClearIsEnabled.ToString() + "\n";
                messageForLogs += "LogsAutoClearIsEnabled: " + Startup.myAppSettings.LogsAutoClearIsEnabled.ToString() + " | ";
                //myMessage += "//---\n";
                myMessage += "LogsAutoClearDaysBeforeDelete: " + Startup.myAppSettings.LogsAutoClearDaysBeforeDelete + "\n";
                messageForLogs += "LogsAutoClearDaysBeforeDelete: " + Startup.myAppSettings.LogsAutoClearDaysBeforeDelete + " | ";
                //myMessage += "//---\n";
                myMessage += "// End Custom Logging -------\n";
                myMessage += "\n";
                myMessage += "// Storage Settings -------\n";
                //myMessage += "//---\n";
                myMessage += "StorageAutoClearIsEnabled: " + Startup.myAppSettings.StorageAutoClearIsEnabled.ToString() + "\n";
                messageForLogs += "StorageAutoClearIsEnabled: " + Startup.myAppSettings.StorageAutoClearIsEnabled.ToString() + " | ";
                //myMessage += "//---\n";
                myMessage += "StorageAutoClearDaysBeforeDelete: " + Startup.myAppSettings.StorageAutoClearDaysBeforeDelete + "\n";
                messageForLogs += "StorageAutoClearDaysBeforeDelete: " + Startup.myAppSettings.StorageAutoClearDaysBeforeDelete + " | ";
                //myMessage += "//---\n";
                myMessage += "// End Storage Settings -------\n";
                myMessage += "\n";
                myMessage += "// Test data -------\n";
                //myMessage += "//---\n";
                myMessage += "TestProductKey: " + SentinelSettings.testProductKey + "\n";
                messageForLogs += "TestProductKey: " + SentinelSettings.testProductKey + " | ";
                //myMessage += "//---\n";
                myMessage += "// End Test data -------\n";
                myMessage += "\n";
                myMessage += "//=======\n";
            }
            else
            {
                myMessage = "Wrong password, access denied!";
                messageForLogs = "Wrong password, access denied!";
            }

            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to send response message - " + messageForLogs);
            await botClient.SendTextMessageAsync(chatId, myMessage, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
    }
}