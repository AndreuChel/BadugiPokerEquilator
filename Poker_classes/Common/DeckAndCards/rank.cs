using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cards.Poker_classes.Common.DeckAndCards
{
    class rank : IComparable
    {
        public static rank Empty { get { return null; } }
        public int id;
        public int PrimeValue;
        public string letter;
        public override string ToString()
        {
            return this.letter.ToUpper();
        }
        public int CompareTo(object obj)
        {
            rank otherRank = obj as rank;
            if (otherRank != null) return this.id.CompareTo(otherRank.id);
            else throw new ArgumentException("Object is not a Rank");
        }
    }
}
