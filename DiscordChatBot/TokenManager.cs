using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordChatBot
{
    //Gets the token from a text file in the executing directory.
    static class TokenManager
    {
        //This is horrible and needs to be redone at some point.
        static public string GetToken()
        {
            return System.IO.File.ReadAllText("token.txt");
        }
    }
}
