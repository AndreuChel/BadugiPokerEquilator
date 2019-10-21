using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cards.Poker_classes.Common;
using Cards.Poker_classes.Common.Player;
using Cards.Poker_classes.Common.DeckAndCards;


namespace Cards.Poker_classes.Common.Table
{
    class pokerReferee : Dictionary<pokerPlayer, int>
    {
        public IEnumerable<pokerPlayer> getWinner()
        {
            IEnumerable<pokerPlayer> _res = this.Where(_el => _el.Value == this.Values.Max()).Select(_el => _el.Key);
            return _res;
        }

    }
}
