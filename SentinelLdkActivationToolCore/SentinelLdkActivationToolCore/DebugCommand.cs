using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using MyLogClass;
using SentinelLdkActivationToolCore.Controllers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SentinelLdkActivationToolCore.Models.Commands
{
    public class DebugCommand : Command
    {
        public override string Name => "debug";
        public override string Description => "get all data (path's, url's and etc...).";

        public HttpConnector myHttpConnector = new HttpConnector();

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

            var myMessage = "Here is my data: \n\n";
            myMessage += "//=======\n";
            myMessage += "//-------\n";
            var messageForLogs = "Here is my data: "; 
            myMessage += "BaseDir: " + SentinelMethods.GetBaseDir() + "\n";
            myMessage += "//-------\n";
            messageForLogs += "BaseDir: " + SentinelMethods.GetBaseDir() + " | ";
            myMessage += "Base EMS Url: " + SentinelMethods.UrlBuilder() + "\n";
            myMessage += "//-------\n";
            messageForLogs += "Base EMS Url: " + SentinelMethods.UrlBuilder() + " | ";
            myMessage += "Url for Login by PK: " + SentinelMethods.UrlBuilder(SentinelSettings.actionsList["loginpk"]) + "\n";
            myMessage += "//-------\n";
            messageForLogs += "Url for Login by PK: " + SentinelMethods.UrlBuilder(SentinelSettings.actionsList["loginpk"]) + " | ";
            myMessage += "Url for Get Info by PK: " + SentinelMethods.UrlBuilder(SentinelSettings.actionsList["getinfo"], SentinelSettings.testProductKey) + "\n";
            messageForLogs += "Url for Get Info by PK: " + SentinelMethods.UrlBuilder(SentinelSettings.actionsList["getinfo"], SentinelSettings.testProductKey);
            myMessage += "//-------\n";
            var myAuthData = SentinelSettings.authXmlString;
            myAuthData = myAuthData.Replace("{PLACEHOLDER_LOGIN}", SentinelSettings.vendorLogin).Replace("{PLACEHOLDER_PASSWORD}", SentinelSettings.vendorPassword);
            myHttpConnector = myHttpConnector.GetRequest("login", HttpMethod.Post, null, new KeyValuePair<string, string>("authenticationDetail", myAuthData)); // TODO vendor login
            XDocument response = XDocument.Parse(myHttpConnector.httpClientResponseStr);
            myMessage += "Test Login by Vendor - auth response: \n" + response.ToString() + "\n";
            myMessage += "//-------\n";
            myHttpConnector = myHttpConnector.GetRequest("logout", HttpMethod.Post, null, new KeyValuePair<string, string>("", ""), myHttpConnector); // TODO vendor logout
            myMessage += "Test Logout status: \n" + myHttpConnector.httpClientResponseStatus.ToString() + " (If \"204\" or \"NoContent\" - this is mean all correct!)\n";
            myMessage += "//-------\n";
            myMessage += "//=======\n";

            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to send response message - " + messageForLogs);
            await botClient.SendTextMessageAsync(chatId, myMessage, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
    }
}