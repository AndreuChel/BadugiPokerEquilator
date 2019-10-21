using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cards.Poker_classes.Common.DeckAndCards;
using Cards.Poker_classes.Common.Player;
using Cards.Poker_classes.Common.HandAndRange;

namespace Cards.Poker_classes.Common.Player
{
    enum pokerPlayerEventType { trySitDown, exitTable, getCards, foldCards, showHand, sayFold, sayCheck, sayBet, sayCall, sayRaise, 
                                antePut, sBlindPut, bBlindPut, bringPut, betPut, raisePut, callPut  }

    abstract class pokerPlayerMessageArgs : EventArgs
    {
        public pokerPlayer sender;
        public pokerPlayerEventType eventType { get; private set; }
        public pokerPlayerMessageArgs(pokerPlayerEventType _eType)
        {
            this.eventType = _eType;
        }
    }
    class trySitMessageArgs : pokerPlayerMessageArgs
    {
        public trySitMessageArgs()
            : base(pokerPlayerEventType.trySitDown) { }
        public int seatNum = -1;
        public override string ToString()
        {
            return "Запрос на посадку за стол" + (seatNum != -1 ? ". На место" + seatNum.ToString() : "");
        }
    }
    class exitTableMessageArgs : pokerPlayerMessageArgs
    {
        public exitTableMessageArgs()
            : base(pokerPlayerEventType.exitTable) { }
        public override string ToString()
        {
            return "Запрос на выход со стола";
        }
    }
    class getCardsMessageArgs : pokerPlayerMessageArgs
    {
        public getCardsMessageArgs()
            : base(pokerPlayerEventType.getCards) { }
        public int cardsCount = 0;
        public String cardsString = "";
        public override string ToString()
        {
            if (this.cardsString=="") return String.Format("Запрос на получение карт :{0}", cardsCount);
            return String.Format("Запрос на получение карт :{0}", this.cardsString);
        }
    }
    class foldCardsMessageArgs : pokerPlayerMessageArgs
    {
        public foldCardsMessageArgs()
            : base(pokerPlayerEventType.foldCards) { }
        public List<card> cards;
        public override string ToString()
        {
            String cardsStr = String.Empty; cards.ForEach(_el => cardsStr+=_el.ToString());
            return String.Format("Сброс карт:{0} ({1})", cards.Count, cardsStr);
        }
    }
    class showHandMessageArgs : pokerPlayerMessageArgs
    {
        public showHandMessageArgs()
            : base(pokerPlayerEventType.showHand) { }
        public int round = 0;
        public pokerHand hand;
        public bool inRange = false;
        public String rangeString = String.Empty;
        public int value;
        public String _cardsString;
        
        public override string ToString()
        {
            return String.Format("Демонстрация руки:[{0}] ({1})", _cardsString, value);
        }
    }
        
    class sayFoldMessageArgs : pokerPlayerMessageArgs
    {
        public sayFoldMessageArgs()
            : base(pokerPlayerEventType.sayFold) { }
        public override string ToString()
        {
            return "Fold";
        }
    }
    class sayCheckMessageArgs : pokerPlayerMessageArgs
    {
        public sayCheckMessageArgs()
            : base(pokerPlayerEventType.sayCheck) { }
        public override string ToString()
        {
            return "Check";
        }
    }
    class sayBetMessageArgs : pokerPlayerMessageArgs
    {
        public sayBetMessageArgs()
            : base(pokerPlayerEventType.sayBet) { }
        public override string ToString()
        {
            return "Bet";
        }
    }

    class sayCallMessageArgs : pokerPlayerMessageArgs
    {
        public sayCallMessageArgs()
            : base(pokerPlayerEventType.sayCall) { }
        public override string ToString()
        {
            return "Call";
        }
    }
    class sayRaiseMessageArgs : pokerPlayerMessageArgs
    {
        public sayRaiseMessageArgs()
            : base(pokerPlayerEventType.sayRaise) { }
        public override string ToString()
        {
            return "Raise";
        }
    }

    abstract class putChipsMessageArgs : pokerPlayerMessageArgs
    {
        public putChipsMessageArgs(pokerPlayerEventType _et)
            : base(_et) { }
        public virtual double ChipCount { get; set; }
        public virtual bool itsAllIn { get; set; }
    }
    class antePutMessageArgs : putChipsMessageArgs
    {
        public antePutMessageArgs()
            : base(pokerPlayerEventType.antePut) { }
        public override string ToString()
        {
            return "Поставил ante="+ChipCount.ToString()+(itsAllIn? ", All-in":"");
        }
    }
    class sBlindPutMessageArgs : putChipsMessageArgs
    {
        public sBlindPutMessageArgs()
            : base(pokerPlayerEventType.sBlindPut) { }
        public override string ToString()
        {
            return "Поставил small Blind=" + ChipCount.ToString() + (itsAllIn ? ", All-in" : "");
        }
    }
    class bBlindPutMessageArgs : putChipsMessageArgs
    {
        public bBlindPutMessageArgs()
            : base(pokerPlayerEventType.bBlindPut) { }
        public override string ToString()
        {
            return "Поставил big Blind=" + ChipCount.ToString() + (itsAllIn ? ", All-in" : "");
        }
    }
    class bringPutMessageArgs : putChipsMessageArgs
    {
        public bringPutMessageArgs()
            : base(pokerPlayerEventType.bringPut) { }
        public override string ToString()
        {
            return "Поставил bring-in=" + ChipCount.ToString() + (itsAllIn ? ", All-in" : "");
        }
    }
}
