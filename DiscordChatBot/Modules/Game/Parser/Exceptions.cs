using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordChatBot.Modules.Game.Parser
{
    class InconsistentStateException : Exception
    {
        private DieRollingToken _token;

        public InconsistentStateException ( DieRollingToken token = null )
        {
            ConstructorHelper(token);
        }

        public InconsistentStateException ( string message, DieRollingToken token = null )
            : base(message)
        {
            ConstructorHelper(token);
        }

        public InconsistentStateException(string message, Exception inner, DieRollingToken token = null )
            : base(message, inner)
        {
            ConstructorHelper(token);
        }

        private void ConstructorHelper( DieRollingToken token)
        {
            _token = token;
        }

        public DieRollingToken Token
        {
            get { return _token; }
        }

    }
}
