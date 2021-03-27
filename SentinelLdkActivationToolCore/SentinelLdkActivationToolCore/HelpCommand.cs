using System;
using System.Threading.Tasks;
using MyLogClass;
using SentinelLdkActivationToolCore.Controllers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SentinelLdkActivationToolCore.Models.Commands
{
    public class HelpCommand : Command
    {
        public override string Name => "help";
        public override string Description => "this is command for geting list of command.";

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

            var myMessage = "Here is list of commands: \n\n";
            var messageForLogs = "Here is list of commands: ";
            myMessage += "//=======\n";
            myMessage += "//-------\n";
            foreach (Command item in Bot.commandsList)
            {
                myMessage += @"/" + item.Name + " - " + item.Description + "\n";
                myMessage += "//-------\n";
                messageForLogs += @"/" + item.Name + " - " + item.Description + " | ";
            }
            myMessage += "//=======\n";

            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to send response message - " + messageForLogs);
            await botClient.SendTextMessageAsync(chatId, myMessage, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
    }
}