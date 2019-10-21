using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cards.Poker_classes.Common;
using Cards.Poker_classes.Common.Table;
using Cards.Poker_classes.Common.Player;
using Cards.Poker_classes.utils;
using Cards.Poker_classes.Games.Badugi;

namespace Cards.Poker_classes.Reports
{
    class _rStartRangeStatistic : report<pokerPlayer>
    {
        public int gamesCount = 0;
        public int inRangeCount = 0;
        public int currentRound = 0;
        public String StartRange = String.Empty;

        public _rStartRangeStatistic(pokerPlayer _pp) : base(_pp) { }
        public override void onStreamMessage(object sender, streamEventType _evType, EventArgs e)
        {
            if (sender is badugiPlayerHelper && this.gamesCount == 0) this.StartRange = (sender as badugiPlayerHelper).startRange;
            if (_evType == streamEventType.incoming && e is newIterationMessageArgs) { this.gamesCount++; }
            if (_evType == streamEventType.incoming && e is startRoundMessageArgs) { this.currentRound = (e as startRoundMessageArgs).round; }
            if (_evType == streamEventType.outgoing && this.currentRound ==0 && e is showHandMessageArgs && (e as showHandMessageArgs).inRange)
            {
                inRangeCount++;
            }
        }
        public override string ToString()
        {
            //double _procent = Math.Floor(100 * ((double)this.inRangeCount / (double)this.gamesCount));
            return String.Format("[{0}]: {1}",
                                 this.StartRange, 
                                 (100 * ((double)this.inRangeCount / (double)this.gamesCount)).ToString() + "%");
        }
        public override string Text { get { return this.ToString() + "\r\n"; } }

    }
}
