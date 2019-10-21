using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cards.Poker_classes.Common;
using Cards.Poker_classes.Common.Table;
using Cards.Poker_classes.Common.Player;
using Cards.Poker_classes.Common.DeckAndCards;

namespace Cards.Poker_classes.Common.Game
{
    abstract class pokerGame
    {
        abstract public int Rounds { get; }
        abstract public game name { get; }
        abstract public Deck deck { get; }
        public gameInfo info { get; private set; }

        public pokerGame(gameInfo _gInfo) 
        {
            if (_gInfo == null) throw new pokerGameException("Объект gameInfo не может быть пустым!");
            this.info = _gInfo; 
        }

        abstract public pokerDealer getDealer(pokerTable _pt);
        abstract public pokerPlayerHelper getHelper(pokerTable _pt, pokerPlayer _pp);
        
        public override string ToString()
        {
            return string.Format("game = {0}", this.name.ToString("f"));
        }
    }
}
