using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordChatBot.Modules.Help
{
    class Help : CommandModule
    {
        public Help() : base("Help")
        {
            AddCommand(new Command.Help());
            AddCommand(new Command.Command());
            AddCommand(new Command.Commands());
        }
    }
}
