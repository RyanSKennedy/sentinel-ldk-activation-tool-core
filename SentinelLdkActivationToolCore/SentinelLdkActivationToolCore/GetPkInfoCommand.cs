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

namespace SentinelLdkActivationToolCore.Models.Commands
{
    public class GetPkInfoCommand : Command
    {
        public override string Name => "getinfo";
        public override string Description => "this is command for geting info about Product Key.";

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
            var savingResult = "";
            var pathForSave = "";
            var fileName = "";

            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to get Product Key from message: " + message.Text);
            try { pKey = message.Text.Split(" ")[1]; } catch { }
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Product Key is: " + pKey);

            if (!SentinelMethods.ProductKeyAndAidValidator(pKey))
            {
                // Return error message if PK is invalid
                myMessage = "Product Key is invalid (format)! Please check Product key and try again. ";
            }
            else {
                // TODO some logics here...
                myHttpConnector = myHttpConnector.GetRequest("loginpk", HttpMethod.Post, null, new KeyValuePair<string, string>("productKey", pKey)); // TODO login first!
                if (myHttpConnector.httpClientResponseStatus == "OK") {
                    myHttpConnector.GetRequest(Name, HttpMethod.Get, pKey, new KeyValuePair<string, string>("productKey", pKey), myHttpConnector);

                    if (myHttpConnector.httpClientResponseStatus == "OK") {
                        var tmpXml = SentinelSettings.pkInfoXmlString;
                            tmpXml = tmpXml.Replace("{PLACEHOLDER}", myHttpConnector.httpClientResponseStr);
                        XDocument pkInfoXml = XDocument.Parse(tmpXml);
                        fileName = SentinelMethods.FileNameBuilder(Name);
                        pathForSave = SentinelMethods.PathBuilder(fileName);
                        if (!String.IsNullOrEmpty(pathForSave))
                        {
                            var tmp = SentinelSettings.pkInfoXmlString;
                            tmp = tmp.Replace("{PLACEHOLDER}", pkInfoXml.ToString());

                            savingResult = SentinelMethods.SaveFile(pathForSave, tmp);

                            if (savingResult == "OK")
                            {
                                myMessage = "File was saved in dir: " + pathForSave + "\n\n";
                            } else {
                                myMessage = "Problem with saving file on server. Saving result: " + savingResult + "\n\n";
                            }
                        }
                    } else {
                        myMessage = "Get Info error: " + myHttpConnector.httpClientResponseStatus;
                    }

                } else {
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
                        caption: "Here is info about your Product Key: " + pKey
                    );
                }
            } else {
                if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to send response message - " + myMessage);
                await botClient.SendTextMessageAsync(chatId, myMessage, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
        }
    }
}