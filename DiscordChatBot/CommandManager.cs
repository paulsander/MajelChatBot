using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordChatBot
{
    class CommandManager
    {
        private Dictionary<string, IBotCommand> _commandDictionary;
        private char _commandPrefix;

        public CommandManager()
        {
            _commandDictionary = new Dictionary<string, IBotCommand>();
            _commandPrefix = '!';
        }

        public void AddModule(ICommandModule addModule)
        {
            List<IBotCommand> newCommands = addModule.CommandList;

            //Next we loop over all of the new commands and register the associated bot command
            foreach ( IBotCommand newCommand in newCommands)
            {
                if (!_commandDictionary.ContainsKey(newCommand.Name))
                {
                    // Command with same name is not already registered.
                    _commandDictionary.Add(newCommand.Name, newCommand);
                }
                else
                {
                    //In case of name conflicts, add a number to the command and keep going.
                    int suffix = 2;
                    while (true)
                    {
                        if (!_commandDictionary.ContainsKey(newCommand.Name + suffix))
                        {
                            _commandDictionary.Add(newCommand.Name + suffix, newCommand);
                            break;
                        }
                        else
                        {
                            suffix += 1;
                        }
                    }
                }
            }
        }

        public void ParseMessage(DiscordBot discordBot, Discord.MessageEventArgs e, string message, bool parsePrefix = true)
        {
            string messagePrefix = "";
            string messageCommand = "";
            string messageText = "";

            Console.WriteLine("Raw message: {0}", e.Message.Text);

            //Check for prefix?
            if (parsePrefix)
            {
                if (message[0] == _commandPrefix)
                {
                    //strip off prefix and continue
                    message = message.Substring(1);
                }
                else
                {
                    //If we're checking for a prefix and don't find it, then skip.
                    return;
                }
            }

            //Now we need to split off the first word.

            Utility.SplitCommandWord(message, out messageCommand, out messageText);

            //Now that we have the messageCommand and messageText, check for the indicated 
            // command.
            if (_commandDictionary.ContainsKey(messageCommand))
            {
                //We have a valid command.
                _commandDictionary[messageCommand].ExecuteAction(discordBot, e, messageCommand, messageText);
            }
            else
            {
                e.Channel.SendMessage("I'm not familiar with that command. Try !help for more assistance.");
            }
        }

        public char Prefix
        {
            get { return _commandPrefix; }
        }

        public Dictionary<string, IBotCommand> Dictionary
        {
            get { return _commandDictionary; }
        }
    }
}
