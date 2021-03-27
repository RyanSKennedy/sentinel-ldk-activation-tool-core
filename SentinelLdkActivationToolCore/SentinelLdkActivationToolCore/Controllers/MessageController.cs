using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using SentinelLdkActivationToolCore.Models;
using System;
using MyLogClass;

namespace SentinelLdkActivationToolCore.Controllers
{
    [ApiController]
    [Route(AppSettings.HookPart)]
    public class MessageController : Controller
    {
        // GET api/values
        [HttpGet]
        public string Get()
        {
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Get Welcome message after open base url (GET request): 'api/message/update'");
            return "My service is working now! Build version: " + Startup.myAppSettings.AppVersion + " | Current DateTime: " + string.Format("{0:dd-MM-yyyy_HH-mm-ss-fff}", DateTime.Now);
        }

        // POST api/values
        [HttpPost]
        public async Task<OkResult> Post([FromBody]Update update)
        {
            if (update == null) {
                if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Update = null...");
                return Ok();
            }

            if (update.EditedMessage != null)
            {
                if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Edit of message is unsupported!");
                return Ok();
            }

            var commands = Bot.Commands;
            var message = update.Message;
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Get message from Bot by WebHook: " +
                (message.Type == Telegram.Bot.Types.Enums.MessageType.Text ? message.Text.ToString() :
                (message.Type == Telegram.Bot.Types.Enums.MessageType.Document ? message.Document.FileName.ToString() +
                " | Caption: " + (!String.IsNullOrEmpty(message.Caption) ? message.Caption : "Empty") :
                "Unknown" )) +
                " | MessageType: " + update.Type.ToString());

            var botClient = await Bot.GetBotClientAsync();

            var supportedCommand = false;

            foreach (var command in commands)
            {
                if (command.Contains(message))
                {
                    if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Chouse Bot command from Commands list and try to execute it: " + command.Name);

                    await command.Execute(message, botClient);
                    supportedCommand = true;
                    break;
                }
            }

            if (supportedCommand == false) {
                if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Message is unsupported (message update Id: " + update.Id.ToString() + ")! Type of message: " + message.Type.ToString());
                if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to Delete WebHooks");
                await botClient.DeleteWebhookAsync();
                if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to Get Updates and reset offset...");
                var myUpdates = await botClient.GetUpdatesAsync(offset: update.Id + 1);
                if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Prepare link for Set WebHooks: " + Startup.myAppSettings.WebHook);
                await botClient.SetWebhookAsync(Startup.myAppSettings.WebHook);

                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Message is unsupported!", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }

            return Ok();
        }
    }
}
