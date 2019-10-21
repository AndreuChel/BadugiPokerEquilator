using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cards.Poker_classes.Common;
using Cards.Poker_classes.Common.Game;
using Cards.Poker_classes.Common.Player;
using Cards.Poker_classes.Common.Table;

using Cards.Poker_classes.Common.DeckAndCards;

namespace Cards.Poker_classes.Games.Badugi
{
    class badugi: pokerGame
    {
        public override game name { get { return game.Badugi; } }
        public override int Rounds { get { return 3; } }
        public badugi(gameInfo _gInfo) : base(_gInfo) { }
               
        public static readonly Deck Deck = new deck52CardsAceLow();
        public override Deck deck
        {
            get { return new deck52CardsAceLow(); }
        }
        public override pokerDealer getDealer(pokerTable _pt) { return new badugiDealer(_pt); }
        public override pokerPlayerHelper getHelper(pokerTable _pt, Common.Player.pokerPlayer _pp)
        {
            return new badugiPlayerHelper(_pt, _pp);
        }
    }
}
