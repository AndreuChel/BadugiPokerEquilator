using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cards.Poker_classes.Common;
using Cards.Poker_classes.Common.Player;
using Cards.Poker_classes.Common.DeckAndCards;
using Cards.Poker_classes.Common.Table;

namespace Cards.Poker_classes.Games.Badugi
{
    class badugiDealer : pokerDealer
    {
        public badugiDealer(pokerTable _pt)
            : base(_pt) { }
    }
}
