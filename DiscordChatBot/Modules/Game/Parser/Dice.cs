using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordChatBot.Modules.Game.Parser
{
    internal static class RNG
    {
        static private System.Random rng;

        static RNG()
        {
            rng = new System.Random();
        }

        public static int Roll(int numSides)
        {
            return rng.Next(0, numSides) + 1; //Should produce a value from 1 to numsides, inclusive
        }
    }

    /// <summary>
    /// This class represents a single die roll.
    /// </summary>
    class Die
    {
        private int _numSides;
        private int _value;
        private bool _discarded;
            
        public Die( int numSides )
        {
            _numSides = numSides;
            _value = RNG.Roll(numSides);
            _discarded = false;
        }

        #region Properties
        public int Sides
        {
            get { return _numSides; }
        }

        public int Value
        {
            get { return _value; }
        }

        public bool Discarded
        {
            get { return _discarded; }
            set { _discarded = value; }
        }
        #endregion
    }

    class DiceGroup
    {
        private List<Die> _dice;

        public DiceGroup()
        {
            _dice = new List<Die>();
        }

        public int AddDie(int numSides)
        {
            Die die = new Die(numSides);
            _dice.Add(die);
            return die.Value;
        }

        #region Drop Operations
        public int DropLow(int num)
        {
            IEnumerable<Die> queryDropLow =
                (from die in _dice
                 where die.Discarded != true
                 orderby die.Value ascending
                 select die).Take(num);
            
            return DropHelper(num, queryDropLow);
        }

        public int DropHigh(int num)
        {
            IEnumerable<Die> queryDropHigh =
                (from die in _dice
                 where die.Discarded != true
                 orderby die.Value descending
                 select die).Take(num);

            return DropHelper(num, queryDropHigh);
        }

        private int DropHelper(int num, IEnumerable<Die> queryDrop )
        {
            int sum = 0;

            foreach(Die die in queryDrop )
            {
                die.Discarded = true;
                sum += die.Value;
            }

            return sum;
        }
        #endregion

        #region Properties
        public List<Die> Dice
        {
            get { return _dice; }
        }
        #endregion
    }
}