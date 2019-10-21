using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cards.Poker_classes.Common;
using Cards.Poker_classes.utils;
using Cards.Poker_classes.Common.Table;
using Cards.Poker_classes.Common.HandAndRange;

namespace Cards.Poker_classes.Common.Player
{
    class pokerIterationInfo
    {
        public pokerIterationInfo(pokerPlayerHelper _helper) 
        { 
            this.owner = _helper;
            this.handHistory = new List<pokerHand>(this.owner.table.ActiveRoundCount+1);
        }
        private pokerPlayerHelper owner;
        public int playerPosition = 0;
        public int Round = 0;
        public List<pokerHand> handHistory;
    }

    abstract class pokerPlayerHelper
    {
        private bool initialization = false;
        public double Chips;
        public pokerTable table;
        protected pokerPlayer owner;

        public String startHand;
        public String startRange;

        protected startHand _startHand = null;
        abstract public startHand startHandObject { get; }

        protected pokerIterationInfo iterationInfo;

        public pokerPlayerHelper(pokerTable _pt, pokerPlayer _pp)
        {
            if (_pt == null) throw new pokerPlayerException("tableWindow. Игрок не может сидеть за пустым столом!");
            this.table = _pt; this.owner = _pp;
        }

        virtual public void initHelper(double _chips, String _hand = "", String _range = "")
        {
            this.initialization = true;
            if (_chips > 0) this.Chips = _chips;
            this.startHand = _hand;
            this.startRange = _range;
        }
        
        virtual public void onAnteGet(object sender, pokerTableMessageArgs e)
        {
            this.SendToTable(new antePutMessageArgs()
            {
                ChipCount = this.payChips((e as anteGetMessageArgs).chipsCount),
                itsAllIn = this.Chips <= 0
            });
        }
        virtual public void onSBlindGet(object sender, pokerTableMessageArgs e)
        {
            this.SendToTable(new sBlindPutMessageArgs()
            {
                ChipCount = this.payChips((e as sBlindGetMessageArgs).chipsCount),
                itsAllIn = this.Chips <= 0
            });
        }
        virtual public void onBlindGet(object sender, pokerTableMessageArgs e)
        {
            this.SendToTable(new bBlindPutMessageArgs()
            {
                ChipCount = this.payChips((e as blindGetMessageArgs).chipsCount),
                itsAllIn = this.Chips <= 0
            });
        }
        virtual public void onBringGet(object sender, pokerTableMessageArgs e)
        {
            this.SendToTable(new bringPutMessageArgs()
            {
                ChipCount = this.payChips((e as bringGetMessageArgs).chipsCount),
                itsAllIn = this.Chips <= 0
            });
        }

        virtual public void onStartGame(object sender, pokerTableMessageArgs e) { }
        virtual public void onNewIteration(object sender, pokerTableMessageArgs e) 
        {
            if (this.Chips <= 0)
            {
                owner.Exit(this.table);
                return;
            }
        }
        virtual public void onSetPos(object sender, pokerTableMessageArgs e) 
        {
            this.iterationInfo.playerPosition = (e as setPlayerPosMessageArgs).pos;
        }
        virtual public void onStartRound(object sender, pokerTableMessageArgs e) 
        {
            this.iterationInfo.Round = (e as startRoundMessageArgs).round;
        }
        virtual public void onEndRound(object sender, pokerTableMessageArgs e)  { throw new NotImplementedException(); }
        virtual public void onDealCards(object sender, pokerTableMessageArgs e) { throw new NotImplementedException(); }
        virtual public void onSetHandStatus(object sender, pokerTableMessageArgs e) { }
        
        public void onTableMessage(object sender, pokerTableMessageArgs e)
        {
            if (!this.initialization)
                throw new Exception("для использования объекта \"pokerPlayerHelper\" необходим вызов метода \"initHelper\"");

            if (e.recipient != pokerPlayer.Empty && !ReferenceEquals(e.recipient, this.owner)) return;

            switch (e.eventType)
            {
                case pokerTableEventType.startGame: { this.onStartGame(sender,e); break; }
                case pokerTableEventType.newIteration: 
                    {
                        this.iterationInfo = new pokerIterationInfo(this);
                        this.onNewIteration(sender, e); 
                        break; 
                    }
                case pokerTableEventType.setPos: { this.onSetPos(sender, e); break; }
                case pokerTableEventType.startRound: { this.onStartRound(sender, e); break; }
                case pokerTableEventType.endRound: { this.onEndRound(sender, e); break; }
                case pokerTableEventType.dealCards: { this.onDealCards(sender, e); break; }
                case pokerTableEventType.setHandStatus: { this.onSetHandStatus(sender, e); break; }

                case pokerTableEventType.anteGet: { this.onAnteGet(sender, e); break; }
                case pokerTableEventType.sBlindGet: { this.onSBlindGet(sender, e); break; }
                case pokerTableEventType.blindGet: { this.onBlindGet(sender, e); break; }
                case pokerTableEventType.bringGet: { this.onBringGet(sender, e); break; }
                case pokerTableEventType.PlayerTurn:  throw new NotImplementedException(); 
                default: break;
            }
            owner.toStream(this, streamEventType.incoming, e);
            
        }
        public void SendToTable(pokerPlayerMessageArgs _args)
        {
            owner.SendToTable(this.table, _args);
            owner.toStream(this, streamEventType.outgoing, _args);
        }
        
        public double payChips(double howMany)
        {
            double res = this.Chips > howMany ? howMany : this.Chips;
            this.Chips -= res;
            
            return res;
        }
    
    }
}
