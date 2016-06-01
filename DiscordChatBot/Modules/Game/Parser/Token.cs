using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordChatBot.Modules.Game.Parser
{
    enum TokenType
    {
        DieRoll, DropLowerst, DropHighest,
        Multiply, Add, Subtract, Repeat,
        LeftParen, RightParen, Number
    };

    class DieRollingToken
    {
        private TokenType _tokenType;
        private string _text;

        public DieRollingToken(TokenType tokenType, string textValue )
        {
            _tokenType = tokenType;
            Text = textValue;
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
    }
}