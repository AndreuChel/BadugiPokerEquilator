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
    class _rHandStatistic : report<pokerPlayer>
    {
        private int gamesCount = 0;
        private Dictionary<int, int> handInRangeStat = new Dictionary<int, int>();
        private Dictionary<int, Dictionary<string, int>> roundStat = new Dictionary<int, Dictionary<string, int>>();
        public String StartHand = "";
        public String StartRange = "";
        public badugiPlayerHelper helperPlayer;

        public _rHandStatistic(pokerPlayer _pp) : base(_pp) { }

        public override void onStreamMessage(object sender, streamEventType _evType, EventArgs e)
        {
            badugiPlayerHelper _bh = sender as badugiPlayerHelper;
            if (sender is badugiPlayerHelper && this.gamesCount == 0)
            {
                this.helperPlayer = _bh; 
                this.StartHand = (sender as badugiPlayerHelper).startHand;
                if (this.StartHand == String.Empty) this.StartHand = "random";

                this.StartRange = (sender as badugiPlayerHelper).startRange;
                if (this.StartRange == String.Empty) this.StartRange = " --- ";
            }

            if (_evType == streamEventType.incoming && e is newIterationMessageArgs) { this.gamesCount++; }
            if (_evType == streamEventType.outgoing && e is showHandMessageArgs)
            {
                showHandMessageArgs _args = e as showHandMessageArgs;
                if (_bh.startRange != String.Empty && _args.round > 0)
                {
                    if (!this.handInRangeStat.ContainsKey(_args.round)) this.handInRangeStat.Add(_args.round, 0);
                    if (_args.inRange) this.handInRangeStat[_args.round]++;
                }
                if (_args.inRange && (_args.round > 0 
                    || this.StartHand == "random" 
                    || (sender as badugiPlayerHelper).startHandObject is badugiStartHandRange ))
                {
                    if (!this.roundStat.ContainsKey(_args.round)) this.roundStat.Add(_args.round, new Dictionary<string, int>());
                    if (!this.roundStat[_args.round].ContainsKey(_args.rangeString)) this.roundStat[_args.round].Add(_args.rangeString, 0);
                    this.roundStat[_args.round][_args.rangeString]++;
                }
               
            }
        }
        
        public override string ToString()
        {
            String inRangeString = this.handInRangeStat.Aggregate(String.Empty, (__result, next) =>
            {
                return __result + (__result != String.Empty ? ", " : "") +
                       ((int)Math.Floor(100 * ((double)next.Value / (double)this.gamesCount))).ToString() + "%";
            });
            inRangeString = inRangeString == String.Empty ? "" : 
                String.Format("\r\n\tПопадание в диапазон: < {0} >", inRangeString);

            
            String roundStatString = this.roundStat
                .OrderBy(_el=>_el.Key)
                .Aggregate(String.Empty, (__result,next) => {
                    String _rStat = String.Concat(next.Value.OrderBy(_el=>_el.Key.Length).Aggregate(String.Empty, (__r, _n) =>
                    {
                        return __r + "\t"+ 
                               String.Format("[{0}]: {1}", _n.Key, 
                               ((int)Math.Floor(100 * ((double)_n.Value / (double)this.gamesCount))).ToString() + "%")
                               .PadRight(12);
                    }));
                    return __result + (next.Key != 0 ? String.Format("\r\n\tраунд {0}: {1}", next.Key, _rStat) :
                                      String.Format("\r\n\tраздача: {0}",_rStat));
                });

            String vpip = String.Empty;
            if (this.helperPlayer.startHandObject is badugiStartHandRange)
            {
                double procents = (double) (this.helperPlayer.startHandObject as badugiStartHandRange).rangeHands.Count() * 100 / (double)270725;
                vpip = String.Format("\r\n\tvpip : {0}%", procents.ToString("0.00"));


            }

            String PlayerInfo = String.Format("Игрок {0}:\r\n\t[{1}]: {2}", Owner.ToString(),this.StartHand, this.StartRange);

            return String.Format("{0}{1}{2}{3}", PlayerInfo, vpip, inRangeString, roundStatString);

        }
        
        public override string Text { get { return this.ToString() + "\r\n"; } }
    }
}
