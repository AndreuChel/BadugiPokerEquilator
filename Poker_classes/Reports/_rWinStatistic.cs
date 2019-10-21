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
    class _rWinStatistic : report<pokerPlayer>
    {
        //private int winCount = ;
        public List<int> winStatistic = new List<int>();
        public String StartHand = "";
        public String StartRange = "";

        public _rWinStatistic(pokerPlayer _pp) : base(_pp) { this.gamesCount = 0; }
        public int gamesCount { get; private set; }
        private int currentRound = 0;

        public override void onStreamMessage(object sender, streamEventType _evType, EventArgs e)
        {
            if (sender is badugiPlayerHelper && this.winStatistic.Count == 0)
            {
                this.StartHand = (sender as badugiPlayerHelper).startHand;
                if (this.StartHand == String.Empty) this.StartHand = "random";
                
                this.StartRange = (sender as badugiPlayerHelper).startRange;
                if (this.StartRange == String.Empty) this.StartRange = " --- ";
                for (int i = 0; i <= (sender as badugiPlayerHelper).table.ActiveRoundCount; i++) this.winStatistic.Add(0);
            }
            if (_evType == streamEventType.incoming && e is newIterationMessageArgs) { this.gamesCount++; }
            if (_evType == streamEventType.incoming && e is startRoundMessageArgs) { this.currentRound = (e as startRoundMessageArgs).round; }

            if (_evType == streamEventType.incoming && e is setHandStatusMessageArgs)// && this.currentRound != 0)
            {
                setHandStatusMessageArgs _args = e as setHandStatusMessageArgs;
                if (!_args.isShowdown && _args.hand_status == handStatus.win) this.winStatistic[this.currentRound]++;
            }
        }
        public override string ToString()
        {
            String winsString = String.Empty;
            if (this.gamesCount > 0)
                foreach (int _wc in this.winStatistic)
                    winsString += (winsString != String.Empty ? ", " : "") +
                                  String.Format("{0}%", ((int)Math.Floor(100 * ((double)_wc / (double)this.gamesCount))).ToString());
            else winsString = "----";
            return String.Format("Игрок {0}:   Win: < {1} >\t[{2}]: {3}", Owner.ToString(), winsString, this.StartHand, this.StartRange);
        }
        public override string Text { get { return this.ToString()+"\r\n"; } }
    }
}
