using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cards.Poker_classes.Common.DeckAndCards
{
    class suit : IComparable
    {
        public static suit Empty { get { return null; } }
        public int id;
        public string letter;
        public string nameEng;
        public string nameRu;
        public override string ToString()
        {
            return this.letter;
        }
        public int CompareTo(object obj)
        {
            if ((obj as suit) != null) return this.id.CompareTo((obj as suit).id);
            else throw new ArgumentException("Object is not a Suit");
        }
    }
}
