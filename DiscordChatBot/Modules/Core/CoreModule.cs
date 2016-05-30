using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordChatBot.Modules.Core
{
    class CoreModule : CommandModule
    {
        public CoreModule() : base("Core")
        {
            AddCommand(new Command.Help());
        }
    }
}
