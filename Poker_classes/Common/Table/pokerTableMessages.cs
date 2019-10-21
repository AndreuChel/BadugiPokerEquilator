using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cards.Poker_classes.Common.Game;
using Cards.Poker_classes.Common.Player;

using Cards.Poker_classes.Common.DeckAndCards;

namespace Cards.Poker_classes.Common.Table
{
    enum pokerTableEventType { playerSay, addPlayer, exitPlayer, startGame, endGame, newIteration,
                               setPos, dealCards, pickCards, setHandStatus, startRound, endRound, PlayerTurn, anteGet, sBlindGet, blindGet, bringGet
    }

    delegate void pokerTableEventHandler(object sender, pokerTableMessageArgs e);

    abstract class pokerTableMessageArgs : EventArgs
    {
        public pokerTableEventType eventType { get; private set; }
        public pokerPlayer recipient { get; set; }
        public pokerTableMessageArgs(pokerTableEventType _eType) 
        { 
            this.eventType = _eType;
            this.recipient = pokerPlayer.Empty;
        }
    }

    class dealCardsMessageArgs : pokerTableMessageArgs
    {
        public dealCardsMessageArgs()
            : base(pokerTableEventType.dealCards) { }
        public List<card> cards { get; set; }
        public int count { get; set; }

        public override string ToString()
        {
            string _cardsString = String.Empty;
            cards.ForEach(_el => _cardsString += _el.ToString());

            return String.Format("Игроку {0} было выдано карт:{1} [{2}]", this.recipient.ToString(), count.ToString(), _cardsString);
        }
    }
    class pickCardsMessageArgs : pokerTableMessageArgs
    {
        public pickCardsMessageArgs()
            : base(pokerTableEventType.pickCards) { }
        public List<card> cards { get; set; }

        public override string ToString()
        {
            string _cardsString = String.Empty;
            cards.ForEach(_el => _cardsString += _el.ToString());

            return String.Format("Игрок {0} сбросил карты:{1} [{2}]", this.recipient.ToString(), cards.Count.ToString(), _cardsString);
        }
    }
    class setHandStatusMessageArgs : pokerTableMessageArgs
    {
        public setHandStatusMessageArgs()
            : base(pokerTableEventType.setHandStatus) { }
        public handStatus hand_status;
        public bool isShowdown;

        public override string ToString()
        {
            string statusString = this.hand_status == handStatus.loss ? "проиграл" : (this.hand_status == handStatus.win ? "выйграл" : "сыграл в ничью");
            return String.Format("Игрок {0} {1}!", this.recipient.ToString(), statusString);
        }
    }

    class PlayerSayMessageArgs : pokerTableMessageArgs
    {
        public PlayerSayMessageArgs() 
            : base(pokerTableEventType.playerSay) { }
        public pokerPlayer sender;
        public pokerPlayerMessageArgs message;
        public override string ToString()
        {
            return "Игрок " + (this.sender != pokerPlayer.Empty ? this.sender.ToString() : "#") +
                   " сообщает: "+this.message.ToString();
        }
    }

    class AddPlayerMessageArgs : pokerTableMessageArgs
    {
        public AddPlayerMessageArgs()
            : base(pokerTableEventType.addPlayer) { }
        public pokerPlayer addedPlayer;
        public int seatNum;
        public override string ToString()
        {
            return "Игрок " + (this.addedPlayer != pokerPlayer.Empty ? this.addedPlayer.ToString() : "#") +
                   " посажен за стол на место " + this.seatNum;
        }
    }
    class exitPlayerMessageArgs : pokerTableMessageArgs
    {
        public exitPlayerMessageArgs()
            : base(pokerTableEventType.exitPlayer) { }
        public pokerPlayer Player;
        public int seatNum;

        public override string ToString()
        {
            return "Игрок " + (this.Player != pokerPlayer.Empty ? this.Player.ToString() : "#") +
                   " вышел из-за стола с места " + this.seatNum;
        }
    }
    class startGameMessageArgs : pokerTableMessageArgs
    {
        public startGameMessageArgs()
            : base(pokerTableEventType.startGame) { }
        public pokerGame game { get; set; }
        public gameInfo info { get; set; }
        public override string ToString()
        {
            return "Начало игры за столом. " + (game != null ? game.ToString() : "") + ". " + (info != null ? info.ToString() : "");
        }
    }
    class endGameMessageArgs : pokerTableMessageArgs
    {
        public endGameMessageArgs()
            : base(pokerTableEventType.endGame) { }
        public override string ToString()
        {
            return "Конец игры за столом";
        }

    }
    class newIterationMessageArgs : pokerTableMessageArgs
    {
        public newIterationMessageArgs()
            : base(pokerTableEventType.newIteration) { }
        public int iterationID { get; set; }

        public override string ToString()
        {
            return "Новая итерация №"+this.iterationID.ToString();
        }
    }
    
    class setPlayerPosMessageArgs : pokerTableMessageArgs
    {
        public setPlayerPosMessageArgs()
            : base(pokerTableEventType.setPos) { }
        public int pos { get; set; }
        //public int pCount { get; set; }
        //public pokerHand hand;
        public override string ToString()
        {
            return "Игрок " + (this.recipient != pokerPlayer.Empty ? this.recipient.ToString() : "#") +
                   ". Позиция:" + this.pos;// + ", Карты:" + this.hand.ToString(); 
        }

    }

    class PlayerTurnMessageArgs : pokerTableMessageArgs
    {
        public PlayerTurnMessageArgs()
            : base(pokerTableEventType.PlayerTurn) { }
        public bool canCheck;
        public bool canBet;
        public bool canCall;
        public bool canRaise;
        public double callSize;
        public double minBetSize;
        public double maxBetSize;

        public override string ToString()
        {
            return "Отправка запроса на решение игрока " + (this.recipient != pokerPlayer.Empty ? this.recipient.ToString() : "#") +
                   this.canCheck.ToString() + "," + this.canBet.ToString() + "," +
                   this.canCall.ToString() + "," + this.canRaise.ToString() + "; " +
                   this.callSize.ToString() + ", " + this.minBetSize.ToString() + ", " + this.maxBetSize.ToString();
        }

    }
    class startRoundMessageArgs : pokerTableMessageArgs
    {
        public startRoundMessageArgs()
            : base(pokerTableEventType.startRound) { }
        public int round;
        public override string ToString()
        {
            return "Начало раунда №" + this.round.ToString();
        }
    }
    class endRoundMessageArgs : pokerTableMessageArgs
    {
        public endRoundMessageArgs()
            : base(pokerTableEventType.endRound) { }
        public int round;
        public override string ToString()
        {
            return "Конец раунда №" + this.round.ToString();
        }
    }

    abstract class getChipsMessageArgs : pokerTableMessageArgs
    {
        public getChipsMessageArgs(pokerTableEventType _et)
            : base(_et) { }
        public virtual double chipsCount { get; set; }
        public override string ToString()
        {
            return (this.recipient !=pokerPlayer.Empty ? "Запрос фишек от игрока "+this.recipient.ToString()+
                ". Основание - " + this.eventType.ToString("f") : "");
        }
    }
    class anteGetMessageArgs : getChipsMessageArgs
    {
        public anteGetMessageArgs()
            : base(pokerTableEventType.anteGet) { }
    }
    class sBlindGetMessageArgs : getChipsMessageArgs
    {
        public sBlindGetMessageArgs()
            : base(pokerTableEventType.sBlindGet) { }
    }
    class blindGetMessageArgs : getChipsMessageArgs
    {
        public blindGetMessageArgs()
            : base(pokerTableEventType.blindGet) { }
    }
    class bringGetMessageArgs : getChipsMessageArgs
    {
        public bringGetMessageArgs()
            : base(pokerTableEventType.bringGet) { }
    }

}
