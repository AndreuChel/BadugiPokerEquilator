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
    abstract class report : IStreamViewer
    {
        protected String outString = "";
        virtual public void toOut(String _str)
        {
            this.outString += (this.outString != String.Empty ? "\r\n" : "") + _str;
        }
        virtual public String Text { get { return this.outString; } }

        abstract public void SubscribeToStream();
        #region IStreamViewer...
        public StreamEventHandler StreamEventHandler { get { return onStreamMessage; } }
        abstract public void onStreamMessage(object sender, streamEventType _evType, EventArgs e);
        #endregion

    }
    abstract class report<T> : report where T : IStream
    {
        public T Owner;
        public report(T _owner) { this.Owner = _owner; }
        public override void SubscribeToStream() { this.Owner.StreamSubscribe(this); }
        
    }
    abstract class ReportList : List<report>
    {
        protected List<String> Strings = new List<string>();
        public String Name {get; private set;}
        public ReportList(String _nameReport) { this.Name = _nameReport; }
        
        new public void Add(report _r)  { if (this.isValidObject(_r)) { _r.SubscribeToStream(); base.Add(_r); } }
        public void Add(String _str) { this.Strings.Add(_str); }

        abstract public bool isValidObject(report _r);
        public override string ToString()
        {
            return String.Concat(this.Strings) +
                   String.Format("{0}\r\n{1}", this.Name, String.Concat(this.Select(_el => _el.Text)));
        }
    }
    class ReportList<T> : ReportList where T : report
    {
        public ReportList(String _name) : base(_name) { }
        public override bool isValidObject(report _r) { return _r is T; }

    }

    class ReportPlayerHandsStatistic : report<pokerPlayer>
    {
        private List<badugiHand> handHistory = new List<badugiHand>();
        private List<Dictionary<int, int>> statsArray = new List<Dictionary<int, int>>();
        public ReportPlayerHandsStatistic(pokerPlayer _pp) : base(_pp) 
        {
            this.statsArray.Add(new Dictionary<int, int>()); 
            this.statsArray.Add(new Dictionary<int, int>()); 
            this.statsArray.Add(new Dictionary<int, int>());
        }

        public override void onStreamMessage(object sender, streamEventType _evType, EventArgs e)
        {
            if (_evType == streamEventType.outgoing && e is showHandMessageArgs)
            {
                showHandMessageArgs _args = e as showHandMessageArgs;
                this.handHistory.Add(_args.hand as badugiHand);
                if (_args.round > 0)
                {
                    badugiHand _bh0 = this.handHistory.First();
                    badugiHand _bh = _args.hand as badugiHand;
                    if (_bh.value > _bh0.value)
                    {
                        //if (this.statsArray.Count < _args.round) this.statsArray.Add(new Dictionary<int,int>());
                        Dictionary<int, int> statElement = this.statsArray.ElementAt(_args.round - 1);
                        if (!statElement.ContainsKey(_bh.Cards.Count)) statElement.Add(_bh.Cards.Count, 0);
                        statElement[_bh.Cards.Count]++;
                    }
                }
                if (_args.round == 3) this.handHistory.Clear();
                
            }
        }
        public override string Text
        {
            get
            {
                String _res = String.Format("Игрок {0}: \r\n",Owner.ToString());
                
                foreach (var _round in this.statsArray)
                {
                    _res += String.Concat(_round.OrderBy(_el => _el.Key).Select(_el =>
                                         {
                                            return String.Format("\t{0}x: {1}\t", _el.Key.ToString(), _el.Value.ToString());
                                         })) + "\r\n";
                }

                return _res;
            }
        }
    }
    class reportPlayerAction : report<pokerTable>
    {
        private Dictionary<pokerPlayer, String> playerIterationStrings;
        public reportPlayerAction(pokerTable pT)
            : base(pT)
        {
            this.playerIterationStrings = new Dictionary<pokerPlayer, string>();
        }

        public override void onStreamMessage(object sender, streamEventType _evType, EventArgs e)
        {
            if (_evType == streamEventType.outgoing && e is newIterationMessageArgs)
            {
                if (this.playerIterationStrings.Count >0)
                    this.playerIterationStrings.ToList()
                        .ForEach(_pair => this.toOut(String.Format("Игрок {0}: {1}", _pair.Key.ToString(), _pair.Value)));

                this.playerIterationStrings.Clear();
                Owner.Seats.players.Where(_el => _el != null).ToList().ForEach(_el => this.playerIterationStrings.Add(_el, ""));

                this.toOut("~~~~~~~~~~~~~~~~~~~~~~");
            }
            if (this.playerIterationStrings.Count == 0) return;
            if (_evType == streamEventType.outgoing && e is dealCardsMessageArgs)
            {
                dealCardsMessageArgs _args = e as dealCardsMessageArgs;
                this.playerIterationStrings[_args.recipient] += "[+" + String.Concat(_args.cards.Select(_c => _c.ToString()));
            }
            if (_evType == streamEventType.outgoing && e is pickCardsMessageArgs)
            {
                pickCardsMessageArgs _args = e as pickCardsMessageArgs;
                this.playerIterationStrings[_args.recipient] += ",-" + String.Concat(_args.cards.Select(_c => _c.ToString()))+"] ";
            }
            if (_evType == streamEventType.incoming && e is showHandMessageArgs)
            {
                showHandMessageArgs _args = e as showHandMessageArgs;
                if (_args.round == (this.Owner as pokerTable).ActiveRoundCount) this.playerIterationStrings[_args.sender] += " => " + _args._cardsString;
            }
            if (_evType == streamEventType.outgoing && e is setHandStatusMessageArgs && (e as setHandStatusMessageArgs).isShowdown)
            {
                setHandStatusMessageArgs _args = e as setHandStatusMessageArgs;
                string statusString = _args.hand_status == handStatus.win ? "WIN" : (_args.hand_status == handStatus.loss ? "LOSS" : "TIE");
                this.playerIterationStrings[_args.recipient] += String.Format(" ({0}) ",statusString);
            }
        }
    }
}
