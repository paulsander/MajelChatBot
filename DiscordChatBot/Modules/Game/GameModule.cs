using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordChatBot.Modules.Game
{
    class Game : CommandModule
    {
        public Game() : base("Game")
        {
            AddCommand(new Command.Roll());
        }
    }
}
