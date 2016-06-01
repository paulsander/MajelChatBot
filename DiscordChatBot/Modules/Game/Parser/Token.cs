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
        private int _position;

        public DieRollingToken(TokenType tokenType, string textValue, int position )
        {
            _tokenType = tokenType;
            Text = textValue;
            Position = position;
        }

        #region Properties
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public int Position
        {
            get { return _position; }
            set { _position = value; }
        }
        #endregion

        #region ToString Override
        public override string ToString()
        {
            return Text;
        }
        #endregion
    }
}