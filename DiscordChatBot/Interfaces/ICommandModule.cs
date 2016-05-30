using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordChatBot
{
    interface ICommandModule
    {
        string Name { get; }

        List<IBotCommand> CommandList { get; }

        void AddCommand(IBotCommand botCommand);
    }
}
