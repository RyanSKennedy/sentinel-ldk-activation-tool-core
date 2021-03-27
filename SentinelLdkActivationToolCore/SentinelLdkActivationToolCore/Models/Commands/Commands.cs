using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SentinelLdkActivationToolCore.Models.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract Task Execute(Message message, TelegramBotClient client);

        public abstract bool Contains(Message message);
    }
}
