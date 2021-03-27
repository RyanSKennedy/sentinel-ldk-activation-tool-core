using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using MyLogClass;
using SentinelLdkActivationToolCore.Controllers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using System.Linq;

namespace SentinelLdkActivationToolCore.Models.Commands
{
    public class GetLicenseByAidCommand : Command
    {
        public override string Name => "getlic";
        public override string Description => "this is command for geting v2c by AID.";

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

            var myMessage = "";
            var pKey = "";
            var aid = "";
            var savingResult = "";
            var pathForSave = "";
            var fileName = "";

            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to get AID and Product Key from message: " + message.Text);
            try {
                var tmpMass = message.Text.Split(" ");
                foreach (string el in tmpMass) {
                    if (el.Contains("AID:") || el.Contains("aid:")) aid = el.Split(":")[1];
                    if (el.Contains("PK:") || el.Contains("pk:")) pKey = el.Split(":")[1];
                }
            } catch { }
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"AID is: " + aid + " | Product Key is: " + pKey);

            if (!SentinelMethods.ProductKeyAndAidValidator(aid) || !SentinelMethods.ProductKeyAndAidValidator(pKey))
            {
                // Return error message if AID or Product Key is invalid
                myMessage = "AID or Product Key is invalid (format)! Please check AID and Product Key and try again. ";
            }
            else
            {
                // TODO some logics here...
                myHttpConnector = myHttpConnector.GetRequest("loginpk", HttpMethod.Post, null, new KeyValuePair<string, string>("productKey", pKey)); // TODO login first!
                if (myHttpConnector.httpClientResponseStatus == "OK")
                {
                    myHttpConnector.GetRequest(Name, HttpMethod.Get, aid, new KeyValuePair<string, string>("aid", aid), myHttpConnector); // TODO get license by AID
                    if (myHttpConnector.httpClientResponseStatus == "OK")
                    {
                        XDocument response = XDocument.Parse(myHttpConnector.httpClientResponseStr);
                        
                        fileName = SentinelMethods.FileNameBuilder(Name);
                        pathForSave = SentinelMethods.PathBuilder(fileName);
                        if (!String.IsNullOrEmpty(pathForSave))
                        {
                            XDocument v2cXml = XDocument.Parse(response.Descendants("activationString").FirstOrDefault().Value);

                            savingResult = SentinelMethods.SaveFile(pathForSave, v2cXml.ToString());

                            if (savingResult == "OK")
                            {
                                myMessage = "File was saved in dir: " + pathForSave + "\n\n";
                            }
                            else
                            {
                                myMessage = "Problem with saving file on server. Saving result: " + savingResult + "\n\n";
                            }
                        }
                    }
                    else
                    {
                        myMessage = "Get license by AID error: " + myHttpConnector.httpClientResponseStatus;
                    }
                }
                else
                {
                    myMessage = "Login error: " + myHttpConnector.httpClientResponseStatus;
                }
            }

            if (savingResult == "OK")
            {
                using (var fileStream = new FileStream(pathForSave, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    await botClient.SendDocumentAsync(
                        chatId: chatId,
                        document: new InputOnlineFile(fileStream, fileName),
                        caption: "Here is your license getting by AID: " + aid
                    );
                }
            }
            else
            {
                if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to send response message - " + myMessage);
                await botClient.SendTextMessageAsync(chatId, myMessage, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
        }
    }
}