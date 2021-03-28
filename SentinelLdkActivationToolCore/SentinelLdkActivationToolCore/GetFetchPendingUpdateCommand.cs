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
    public class GetFetchPendingUpdateCommand : Command
    {
        public override string Name => "getfpu";
        public override string Description => "this is command for geting updates for exist key by C2V.";

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
            string savingResult = "";
            string pathForSave = "";
            string fileName = "";
            string fileId = "";
            string fileData = "";
            string targetXml = "";
            string haspId = "";

            var chatId = message.Chat.Id;
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Get Chat Id for response message: " + chatId);

            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Document)
            {
                // Return error message if C2V is not provided
                myMessage = "C2V is missing! Please do not forget add C2V and try again. ";
                goNext = false;
            }

            if (goNext)
            {
                fileName = SentinelMethods.FileNameBuilder(Name, true);
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

            if (goNext && savingResult == "OK")
            {
                targetXml = fileData;
                
                myHttpConnector = myHttpConnector.GetRequest(Name, HttpMethod.Post, null, new KeyValuePair<string, string>("targetXml", targetXml)); // TODO get updates!
                if (myHttpConnector.httpClientResponseStatus == "OK")
                {
                    XDocument v2cXml = XDocument.Parse(myHttpConnector.httpClientResponseStr);
                    haspId = v2cXml.Descendants("hasp").FirstOrDefault().Attributes("id").FirstOrDefault().Value.ToString();
                    fileName = SentinelMethods.FileNameBuilder(Name, key: haspId);
                    pathForSave = SentinelMethods.PathBuilder(fileName);
                    if (!String.IsNullOrEmpty(pathForSave))
                    {
                        savingResult = SentinelMethods.SaveFile(pathForSave, v2cXml.ToString());

                        if (savingResult == "OK")
                        {
                            using (var fileStream = new FileStream(pathForSave, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                await botClient.SendDocumentAsync(
                                    chatId: chatId,
                                    document: new InputOnlineFile(fileStream, fileName),
                                    caption: "Here is your updates for Protection Key ID: " + v2cXml.Descendants("hasp").FirstOrDefault().Attributes("id").FirstOrDefault().Value.ToString()
                                );

                                myMessage = "";
                                if (Startup.myAppSettings.LogIsEnabled) Log.Write("Updates for Protection Key ID: " + v2cXml.Descendants("hasp").FirstOrDefault().Attributes("id").FirstOrDefault().Value.ToString());
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
                    myMessage = "Get Fetch Pending Updates error: " + myHttpConnector.httpClientResponseStatus;
                    if (Startup.myAppSettings.LogIsEnabled) Log.Write("Get Fetch Pending Updates error: " + myHttpConnector.httpClientResponseStatus);
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