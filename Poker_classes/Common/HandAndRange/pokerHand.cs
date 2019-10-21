using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cards.Poker_classes.Common.DeckAndCards;
using Cards.Poker_classes.Common.Game;


namespace Cards.Poker_classes.Common.HandAndRange
{
    abstract class pokerHand : IComparable
    {
        abstract public game gameName { get; }
        abstract public int Capacity { get; }

        public cardSet Cards;
        public int Count { get { return this.Cards.Count; } }

        abstract public int value { get; }
        
        #region Operations...
        abstract public int CompareTo(object obj);
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();
        public static bool operator <(pokerHand b1, pokerHand b2)
        {
            IComparable tmp = (IComparable)b1;
            return (tmp.CompareTo(b2) < 0);
        }
        public static bool operator <=(pokerHand b1, pokerHand b2)
        {
            IComparable tmp = (IComparable)b1;
            return (tmp.CompareTo(b2) <= 0);
        }
        public static bool operator >(pokerHand b1, pokerHand b2)
        {
            IComparable tmp = (IComparable)b1;
            return (tmp.CompareTo(b2) > 0);
        }
        public static bool operator >=(pokerHand b1, pokerHand b2)
        {
            IComparable tmp = (IComparable)b1;
            return (tmp.CompareTo(b2) >= 0);
        }
        #endregion
    }
    class HandException : Exception { public HandException(String message) : base(message) { } }
}
