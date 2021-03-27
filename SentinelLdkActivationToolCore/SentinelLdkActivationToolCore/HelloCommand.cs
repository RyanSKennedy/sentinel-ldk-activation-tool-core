using System;
using System.Threading.Tasks;
using MyLogClass;
using SentinelLdkActivationToolCore.Controllers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SentinelLdkActivationToolCore.Models.Commands
{
    public class HelloCommand : Command
    {
        public override string Name => "hello";
        public override string Description => "this is command just for test.";

        public override bool Contains(Message message)
        {
            bool result;

            if (message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
            {
                result = !String.IsNullOrEmpty(message.Caption) && (message.Caption == ("/" + this.Name) || message.Caption.Contains("/" + this.Name + " ")) ? true : false;
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
            var userName = message.From.Username;

            if (message.Type == Telegram.Bot.Types.Enums.MessageType.Document && !String.IsNullOrEmpty(message.Caption) && message.Caption.Contains("/" + this.Name + " "))
            {
                userName = message.Caption.Split(' ', 2)[1];
            }
            else if (message.Type == Telegram.Bot.Types.Enums.MessageType.Text && message.Text.Contains(' '))
            {
                userName = message.Text.Split(' ', 2)[1];
            }

            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Get Chat Id for response message: " + chatId);

            var myMessage = "Hello " + userName + " I'm ASP.NET Core Bot: " + Startup.myAppSettings.AppVersion;

            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to send response message: " + myMessage);
            await botClient.SendTextMessageAsync(chatId, myMessage, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
    }
}