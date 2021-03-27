using System.Threading.Tasks;
using System;
using MyLogClass;
using SentinelLdkActivationToolCore.Controllers;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Xml.Linq;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using Telegram.Bot.Types.InputFiles;
using System.Linq;

namespace SentinelLdkActivationToolCore.Models.Commands
{
    public class GetActivationByPkCommand : Command
    {
        public override string Name => "getact";
        public override string Description => "this is command for geting activation by Product Key.";

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
            bool goNext = true;

            int maxFileSize = 8 * 1024 * 100; // 100Kb

            string myMessage = "";
            string pKey = "";
            string savingResult = "";
            string pathForSave = "";
            string fileName = "";
            string fileId = "";
            string fileData = "";
            string actXml = SentinelSettings.activationXmlString;

            var chatId = message.Chat.Id;
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Get Chat Id for response message: " + chatId);

            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to get Product Key from message: " + message.Caption);
            try { pKey = message.Caption.Split(" ")[1]; } catch { goNext = false; }
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Product Key is: " + pKey);

            if (String.IsNullOrEmpty(pKey) || !SentinelMethods.ProductKeyAndAidValidator(pKey))
            {
                // Return error message if PK is invalid
                myMessage = "Product Key is invalid (format)! Please check Product key and try again. ";
                goNext = false;
            }

            if (goNext) {

                fileName = SentinelMethods.FileNameBuilder(Name, true, pKey);
                pathForSave = SentinelMethods.PathBuilder(fileName, true);

                if (!String.IsNullOrEmpty(pathForSave))
                {
                    fileId = message.Document.FileId;
                    if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"File ID is: " + fileId);
                    var fileInfo = await botClient.GetFileAsync(fileId);
                    if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"File size is: " + fileInfo.FileSize.ToString() + " bits (Max file size is: " + maxFileSize.ToString() + " bit (((8 bit * 1024) byte * 100) Kb = 100Kb))");

                    if (fileInfo.FileSize < maxFileSize)
                    {
                        using (var saveFileStream = System.IO.File.Open(pathForSave, FileMode.Create))
                        {
                            await botClient.DownloadFileAsync(fileInfo.FilePath, saveFileStream);
                            savingResult = "OK";
                        }

                        if (savingResult == "OK")
                        {
                            fileData = await System.IO.File.ReadAllTextAsync(pathForSave);
                            
                            if (Startup.myAppSettings.LogIsEnabled) Log.Write("Incomming file(size: " + fileInfo.FileSize.ToString() + " bits) was saved in dir: " + pathForSave);
                        }
                        else
                        {
                            myMessage = "Problem with saving file on server. Saving result: " + savingResult + "\n\n";
                            if (Startup.myAppSettings.LogIsEnabled) Log.Write("Problem with saving file on server. Saving result: " + savingResult);
                            goNext = false;
                        }
                    }
                    else
                    {
                        myMessage = "Incomming file size is to big (more then 2 Mb): " + fileInfo.FileSize.ToString() + " bits\n\n";
                        if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"File size: " + fileInfo.FileSize.ToString() + " bits more then: " + maxFileSize.ToString());
                        goNext = false;
                    }
                }
                else
                {
                    goNext = false;
                }
            }

            if (goNext && savingResult == "OK") { 
                actXml = actXml.Replace("{PLACEHOLDER}", fileData);
                myHttpConnector = myHttpConnector.GetRequest("loginpk", HttpMethod.Post, null, new KeyValuePair<string, string>("productKey", pKey)); // TODO login first!
                if (myHttpConnector.httpClientResponseStatus == "OK")
                {
                    myHttpConnector = myHttpConnector.GetRequest(Name, HttpMethod.Post, pKey, new KeyValuePair<string, string>("activationXml", actXml), myHttpConnector); // TODO activation!
                    if (myHttpConnector.httpClientResponseStatus == "OK")
                    {
                        XDocument activationResultXml = XDocument.Parse(myHttpConnector.httpClientResponseStr);
                        fileName = SentinelMethods.FileNameBuilder(Name);
                        pathForSave = SentinelMethods.PathBuilder(fileName);
                        if (!String.IsNullOrEmpty(pathForSave))
                        {
                            XDocument v2cXml = XDocument.Parse(activationResultXml.Descendants("activationString").FirstOrDefault().Value);

                            savingResult = SentinelMethods.SaveFile(pathForSave, v2cXml.ToString());

                            if (savingResult == "OK")
                            {
                                using (var fileStream = new FileStream(pathForSave, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    await botClient.SendDocumentAsync(
                                        chatId: chatId,
                                        document: new InputOnlineFile(fileStream, fileName),
                                        caption: "Here is your license by Product Key: " + pKey + "\n" + "AID: " + activationResultXml.Descendants("AID").FirstOrDefault().Value
                                    );

                                    myMessage = "";
                                    if (Startup.myAppSettings.LogIsEnabled) Log.Write("License by Product Key: " + pKey + " | AID: " + activationResultXml.Descendants("AID").FirstOrDefault().Value);
                                }
                            }
                            else
                            {
                                myMessage = "Problem with saving file on server. Saving result: " + savingResult + "\n\n";
                                if (Startup.myAppSettings.LogIsEnabled) Log.Write("Problem with saving file on server. Saving result: " + savingResult);
                            }
                        }
                    }
                    else
                    {
                        myMessage = "Activation error: " + myHttpConnector.httpClientResponseStatus;
                        if (Startup.myAppSettings.LogIsEnabled) Log.Write("Activation error: " + myHttpConnector.httpClientResponseStatus);
                    }
                }
                else
                {
                    myMessage = "Login error: " + myHttpConnector.httpClientResponseStatus;
                    if (Startup.myAppSettings.LogIsEnabled) Log.Write("Login error: " + myHttpConnector.httpClientResponseStatus);
                }

                if (!String.IsNullOrEmpty(myMessage))
                {
                    if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to send response message - " + myMessage);
                    await botClient.SendTextMessageAsync(chatId, myMessage, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
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