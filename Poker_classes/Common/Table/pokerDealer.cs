using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cards.Poker_classes.Common;
using Cards.Poker_classes.Common.HandAndRange;
using Cards.Poker_classes.Common.DeckAndCards;
using Cards.Poker_classes.Common.Player;

namespace Cards.Poker_classes.Common.Table
{
    abstract class pokerDealer
    {
        public double Pot {get; private set;}
        public double roundtPot { get; private set; }
        public pokerTable table { get; private set; }

        private int _cursor;
        public int round { get; private set; }
        public int iteration { get; private set; }
        public void reset() { this.round = -1; this.Pot = 0; this.roundtPot = 0; }

        public pokerDealer(pokerTable _pT)  { this.table = _pT; this.reset(); }

        virtual public void nextIteration(int _id)
        {
            this.iteration = _id;
            this.table.sendMessage(new newIterationMessageArgs() { iterationID = _id });
            this.reset();
            for (int i = 0; i < this.table.Seats.Count; i++)
            {
                this.table.sendMessage(new setPlayerPosMessageArgs()
                {
                    recipient = this.table.Seats[i],
                    pos = i
                });
            }
            
        }
        virtual public void nextRound() 
        {
            this._cursor = 0; this.round++;  this.roundtPot = 0;
            this.table.sendMessage(new startRoundMessageArgs() { round = this.round });
            this.table.referee = new pokerReferee();
        }
        virtual public void endRound()
        {
            this.table.sendMessage(new endRoundMessageArgs () { round = this.round });
            this.ShowDown(false);
        }
        virtual public void getAnteBlindsBring()
        {
            int _first = this.getFirst();
            if (this.table.isSet(TableOption.noBets)) return;
            
            //сбор анте            
            if (this.table.isSet(TableOption.Ante))
                for (int i = 0; i < this.table.Seats.Count; i++)
                    this.table.sendMessage(new anteGetMessageArgs()
                    {
                        recipient = this.table.Seats[i],
                        chipsCount = this.table.game.info.ante
                    });

            if (this.table.game.info.sBlind > 0 && this.table.game.info.Bet > 0)
                {
                    this.table.sendMessage(new sBlindGetMessageArgs()
                    {
                        recipient = this.table.Seats[_first],
                        chipsCount = this.table.game.info.sBlind
                    });
                    this.table.sendMessage(new blindGetMessageArgs()
                    {
                        recipient = this.table.Seats[_first+1],
                        chipsCount = this.table.game.info.Bet
                    });
                    return;
                }
            if (this.table.game.info.Bring > 0)
            {
                this.table.sendMessage(new bringGetMessageArgs()
                {
                    recipient = this.table.Seats[_first],
                    chipsCount = this.table.game.info.Bring
                });
                return;
            }
        }
        public void betting()
        {
            if (this.table.isSet(TableOption.noBets)) return;

            int pos = this._cursor;
            while (true)
            {
                int index = pos++ % this.table.Seats.Count;
                
                this.table.sendMessage(new PlayerTurnMessageArgs()
                {
                    recipient = this.table.Seats[index],
                    canCheck = this.canCheck, canBet = this.canBet, canCall = this.canCall, canRaise = this.canRaise,
                    callSize = 0, minBetSize = 0, maxBetSize = 0
                });
                
                if ((pos) % this.table.Seats.Count == this._cursor) break;
            }
            this.Pot += this.roundtPot;
        }

        #region capabilities...
        public bool canCheck { get { return this.roundtPot == 0; } }
        public bool canBet { get { return this.canCheck; } }
        public bool canCall { get { return !this.canCheck; } }
        public bool canRaise
        {
            get
            {
                return (this.canCall) && !(this.table.game.info.LimitType == LimitType.fixedLimit && this.roundtPot >= this.table.game.info.bigBet * 4);
            }
        }
        #endregion
        #region operations...
        public pokerDealer shiftCursor(int offset)
        {
            this._cursor = (this._cursor + offset) % this.table.Seats.Count;
            this._cursor = this._cursor < 0 ? this.table.Seats.Count + this._cursor : this._cursor;
            return this;
        }
        public static pokerDealer operator >> (pokerDealer _pm, int offset)
        {
            return _pm.shiftCursor(offset);
        }
        public static pokerDealer operator << (pokerDealer _pm, int offset)
        {
            return _pm.shiftCursor(-offset);
        }
        #endregion

        //определение первого опрашиваемого        
        virtual public int getFirst()
        {
            //можно переоприделить, напирмер для STUD
            return 0;
        }

        virtual public void dealCards(pokerPlayer _pp, int cardsCount, string cardsString)
        {
            List<card> _c;
            if (this.round == 0)
                _c = this.table.deck.getCard(this.table.StartHandGenerator[this.iteration][_pp].Cards);
            else
                _c = this.table.deck.getCards(cardsCount).ToList();
            
            this.table.sendMessage(new dealCardsMessageArgs()
            {
                recipient = _pp,
                count = _c.Count,
                cards = _c
            });

            /*
            if (cardsString != String.Empty)
            {
                List<card> _c = this.table.deck.getCard(cardsString);
                //if (_c.Count == 0) return;
                this.table.sendMessage(new dealCardsMessageArgs()
                {
                    recipient = _pp,
                    count = _c.Count,
                    cards = _c
                });
            }
            else
                this.table.sendMessage(new dealCardsMessageArgs()
                {
                    recipient = _pp,
                    count = cardsCount,
                    cards = this.table.deck.getCards(cardsCount).ToList()
                });
            */

        }
        virtual public void pickCards(pokerPlayer _pp, List<card> _cards)
        {
            this.table.deck.fold(_cards);
            this.table.sendMessage(new pickCardsMessageArgs()
                {
                    recipient = _pp,
                    cards = _cards
                });

        }
        virtual public void playerShowHand(pokerPlayer _pp, int _handValue) { this.table.referee.Add(_pp, _handValue); }
        virtual public void ShowDown(bool _isShowdown = true)
        {
            IEnumerable<pokerPlayer> _winners = this.table.referee.getWinner();
            foreach (pokerPlayer _pp in this.table.Seats)
            {
                this.table.sendMessage(new setHandStatusMessageArgs()
                {
                    isShowdown = _isShowdown,
                    recipient = _pp,
                    hand_status = !_winners.Contains(_pp) ? handStatus.loss : (_winners.Count() > 1 ? handStatus.tie : handStatus.win)
                });
            }
        }

    }
}
