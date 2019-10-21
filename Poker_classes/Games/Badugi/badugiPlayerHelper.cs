using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cards.Poker_classes.Common;
using Cards.Poker_classes.Common.Table;
using Cards.Poker_classes.Common.Player;
using Cards.Poker_classes.Common.DeckAndCards;
using Cards.Poker_classes.Common.HandAndRange;

namespace Cards.Poker_classes.Games.Badugi
{
    class badugiPlayerHelper : pokerPlayerHelper
    {
        private badugiRange handRange;
        public badugiPlayerHelper(pokerTable _pt, pokerPlayer _pp) : base(_pt, _pp) { }
        override public startHand startHandObject
        {
            get
            {
                if (this._startHand != null) return this._startHand;

                badugiRange bR; badugiHand bH; 
                if (badugiRange.tryGet(this.startHand, out bR)) this._startHand = new badugiStartHandRange(bR);
                else if (badugiHand.tryGet(this.startHand, out bH)) this._startHand = new badugiStartHandCards(bH.Cards);
                else this._startHand = new badugiStartHandRandom();
                return this._startHand;
            }
        }
        override public void initHelper(double _chips, String _hand = "", String _range = "")
        {
            base.initHelper(_chips, _hand, _range);

            this.handRange = badugiRange.get(this.startRange != String.Empty ? this.startRange : "k-");
        }

        //public override void onStartGame(object sender, pokerTableMessageArgs e) { }
        
        override public void onStartRound(object sender, pokerTableMessageArgs e)
        {
            base.onStartRound(sender, e);
            int round = (e as startRoundMessageArgs).round;
            this.SendToTable(new getCardsMessageArgs()
            {
                cardsCount = round == 0 ? 4 : 4 - this.iterationInfo.handHistory[round - 1].Count,
            });

        }
        override public void onEndRound(object sender, pokerTableMessageArgs e)
        {
            badugiHand _bh =  this.iterationInfo.handHistory[(e as endRoundMessageArgs).round] as badugiHand;
            List<card> _foldedCards = _bh.Cards - _bh.BadugiCards;
            _bh = badugiHandsHash.get(_bh.BadugiCards);
            this.iterationInfo.handHistory[this.iterationInfo.Round] = _bh;

            Cards.Poker_classes.Common.HandAndRange.startHand sH = this.startHandObject;

            bool _inRangeFlag = true;
            String _ruleString = badugiInfo.DefaultRuleString(_bh.Count);
            if (this.iterationInfo.Round == 0)
                if (this.startHandObject is badugiStartHandRange)
                    _inRangeFlag = (this.startHandObject as badugiStartHandRange).Range
                        .inRange(this.iterationInfo.handHistory[this.iterationInfo.Round], out _ruleString);
            
            if (this.startRange != string.Empty) 
                if (this.iterationInfo.Round < this.table.ActiveRoundCount)
                {
                    pokerHand _bhLow = this.handRange.getHandInRange(_bh);
                    _foldedCards.AddRange(_bh.BadugiCards - _bhLow.Cards);
                    this.iterationInfo.handHistory[this.iterationInfo.Round] = _bhLow;
                }
            if (this.iterationInfo.Round > 0)
                _inRangeFlag = this.handRange.inRange(this.iterationInfo.handHistory[this.iterationInfo.Round], out _ruleString);
            
            this.SendToTable(new foldCardsMessageArgs() { cards = _foldedCards });
            
            this.SendToTable(new showHandMessageArgs()
            {
                round = (e as endRoundMessageArgs).round,
                hand = this.iterationInfo.handHistory[this.iterationInfo.Round],
                inRange = _inRangeFlag,
                rangeString = _ruleString,
                value = this.iterationInfo.handHistory[this.iterationInfo.Round].value,
                _cardsString = _bh.ToString()
            });
        }
        override public void onDealCards(object sender, pokerTableMessageArgs e)
        {
            dealCardsMessageArgs _args = e as dealCardsMessageArgs;
            List<card> _c = this.iterationInfo.Round == 0 ? 
                _args.cards : 
                this.iterationInfo.handHistory[this.iterationInfo.Round - 1].Cards + _args.cards;

            this.iterationInfo.handHistory.Add(badugiHandsHash.get(_c));
        }

    }
}
