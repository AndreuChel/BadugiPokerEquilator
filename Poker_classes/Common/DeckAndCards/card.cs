using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cards.Poker_classes.utils;

namespace Cards.Poker_classes.Common.DeckAndCards
{
    
    class card
    {
        public static card Empty { get { return null; } }

        public int ID;
        public int posInDesc = 0;
        public uint bitMask;
        public rank Rank;
        public suit Suit;
        public CardType type = CardType.Hidden;
        
        public override string ToString()
        {
            return this.Rank.letter.ToUpper() + this.Suit.letter;
        }
        public override int GetHashCode() { return Arrays.primeNumbers[this.ID]; }
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is card)) return false;
            return this.GetHashCode() == (obj as card).GetHashCode();
        }
    }
}
