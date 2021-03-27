using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using SentinelLdkActivationToolCore.Models.Commands;
using MyLogClass;
using System;
using SentinelLdkActivationToolCore.Controllers;

namespace SentinelLdkActivationToolCore.Models
{
    public class Bot
    {
        private static TelegramBotClient botClient;
        public static List<Command> commandsList;

        public static IReadOnlyList<Command> Commands => commandsList.AsReadOnly();

        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (botClient != null)
            { 
                return botClient;
            }

            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Set Commands list");
            commandsList = new List<Command>();
            commandsList.Add(new HelpCommand());
            commandsList.Add(new HelloCommand());
            commandsList.Add(new GetPkInfoCommand());
            commandsList.Add(new GetActivationByPkCommand());
            commandsList.Add(new DebugCommand());
            commandsList.Add(new GetLicenseByAidCommand());
            //TODO: Add more commands

            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Create new Bot Client with Key: " + AppSettings.Key);
            botClient = new TelegramBotClient(AppSettings.Key);
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to Delete WebHooks");
            await botClient.DeleteWebhookAsync();
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Prepare link for Set WebHooks: " + Startup.myAppSettings.WebHook);
            await botClient.SetWebhookAsync(Startup.myAppSettings.WebHook);
            var webhookStatus = botClient.GetWebhookInfoAsync();
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Try to Set WebHooks, status: " + webhookStatus.Status.ToString());
            return botClient;
        }
    }
}