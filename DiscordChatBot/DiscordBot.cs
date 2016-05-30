using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordChatBot
{
    class DiscordBot
    {
        private DiscordClient _bot;
        private CommandManager _commandManager;
        private CommandManager _helpCommandManager;

        public DiscordBot()
        {
            _bot = new DiscordClient();
            _commandManager = new CommandManager();
            _helpCommandManager = new CommandManager();

            _commandManager.AddModule(new Modules.Core.CoreModule());
            _commandManager.AddModule(new Modules.Game.Game());
            _helpCommandManager.AddModule(new Modules.Help.Help());

            _bot.MessageReceived += Bot_MessageReceived;

            _bot.Connect(TokenManager.GetToken());

            _bot.Wait();
        }

        private void Bot_MessageReceived(object sender, MessageEventArgs e)
        {
            /*
               Console.WriteLine("User name:{0}, User nickname: {1}, User private channel: {2}, User statis: {3}, Channel name: {4}, Channel type: {5}, Channel isprivate: {6}, Message channel: {7}, Message client: {8}, Message state: {9}, Message text: {10}",
                e.User.Name, e.User.Nickname, e.User.PrivateChannel, e.User.Status,
                e.Channel.Name, e.Channel.Type, e.Channel.IsPrivate, e.Message.Channel,
                e.Message.Client, e.Message.State, e.Message.Text);
            */

            //Bot should not reply to its own messages.
            //Avoids the possibility of a feedback loop.
            if (e.Message.IsAuthor) return;

            //TODO: Come up with a better way to make the bot gracefully quit.
            if ( (e.Message.Text == "quit") && (e.Channel.Name == "@Alphacat#2653") )
            {
                Console.WriteLine("Quitting...");
                _bot.Disconnect();
            }

            _commandManager.ParseMessage(this, e, e.Message.Text);
        }

        public CommandManager CommandManager
        {
            get { return _commandManager; }
        }

        public CommandManager HelpCommandManager
        {
            get { return _helpCommandManager; }
        }
    }
}
