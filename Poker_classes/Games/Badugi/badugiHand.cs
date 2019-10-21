using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cards.Poker_classes.Common.DeckAndCards;
using Cards.Poker_classes.Common;
using Cards.Poker_classes.Common.HandAndRange;
using Cards.Poker_classes.Common.Game;
//using System.Text.RegularExpressions;

namespace Cards.Poker_classes.Games.Badugi
{
    class badugiHand : pokerHand
    {
        public static int _capacity = 4;
        public override game gameName { get { return game.Badugi; } }
        
        public override int value { get { return _value; } }
        public override int Capacity { get { return _capacity; } }

        public cardSet BadugiCards { get { return this.info.Cards; } }

        private int _value = 0;
        public badugiInfo info = badugiInfo.Empty;

        private badugiHand(List<card> _cs) {
            if (_cs.Count > 4) throw new HandException("Много карт!");
            this.Cards = new cardSet(this.Capacity, _cs);
            this.info = badugiInfo.get(this.Cards);
            this._value = this.info.val;
        }

        #region static functions: code & get ...
        public static badugiHand get(List<card> _cList) { return new badugiHand(_cList);  }
        public static badugiHand get(string _hand) { return badugiHand.get(badugi.Deck.parseCardString(_hand)); }
        public static bool tryGet(string _hand, out badugiHand bH)
        {
            bH = null;
            try { bH = badugiHand.get(_hand); return true; }
            catch (Exception) { return false; }
        }
        
        #endregion

        #region Operations...
        
        public override int GetHashCode()
        {
            return this.Cards.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType()) return false;
            return this.GetHashCode() == (obj as badugiHand).GetHashCode();
        }
        public override int CompareTo(object obj)
        {
            badugiHand otherBH = obj as badugiHand;
            if (otherBH != null) return this.info.val.CompareTo(otherBH.info.val);
            else throw new ArgumentException("Object is not a badugiHand");
        }
         
        #endregion

        public override string ToString() { return this.Cards.ToString(); }
    
    }
}
