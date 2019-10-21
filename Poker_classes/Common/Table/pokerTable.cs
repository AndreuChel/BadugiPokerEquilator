using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cards.Poker_classes.Common.Game;
using Cards.Poker_classes.utils;
using Cards.Poker_classes.Common.DeckAndCards;
using Cards.Poker_classes.Common.Player;
using Cards.Poker_classes.utils.Iterator;
using Cards.Poker_classes.Common.HandAndRange;


namespace Cards.Poker_classes.Common.Table
{
    class pokerTable : ISessionSubscriber, IStream
    {
        #region class propertys...
        private static int counter = 0;
        public int id = pokerTable.counter++;

        public pokerGame game { get; private set; }
        public Deck deck { get; private set; }
        public readonly pokerTableType tableType;
        public TableOption options { get; private set; }

        public seats Seats;
        public pokerDealer dealer;
        public pokerReferee referee;
        public startHandFactory StartHandGenerator;

        public bool isSet(TableOption _op) { return (this.options & _op) != 0; }

        #endregion

        public pokerTable(pokerGame _pokerGame, pokerTableType tabType, TableOption _options = 0)
        {
            this.game = _pokerGame;
            this.deck = this.game.deck;
            this.tableType = tabType;
            this.options = _options;
            this.Seats = new seats(this);
            this.dealer = this.game.getDealer(this);
            this.ActiveRoundCount = this.game.Rounds;
        }
        public override int GetHashCode()
        {
            return this.id;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj != null && (obj is pokerTable) && this.id == (obj as pokerTable).id) return true;
            return false;
        }
        private int _roundCount;
        public int ActiveRoundCount
        {
            get { return this._roundCount; }
            set { _roundCount = (value >= 0 && value <= this.game.Rounds) ? value : this.game.Rounds; }
        }
        
        /// <summary> Интерфейс сообщений от дилера к игрокам
        /// </summary>
        public pokerTableEventHandler dealerMessages;
        
        public void sendMessage(pokerTableMessageArgs e)
        {
            if (this.dealerMessages == null) return;
            this.dealerMessages(this, e);
            this.toStream(this, streamEventType.outgoing, e);
        }
        public void SubscribeToTableMessages(pokerTableEventHandler f) { this.dealerMessages += f; }
        public void unSubscribeFromTableMessages(pokerTableEventHandler f) { this.dealerMessages -= f; }
        
        void ISessionSubscriber.SessionSubscribe(session sesion)
        {
            sesion.sessionStart += this.onSessionStart;
            sesion.sessionFinish += this.onSessionFinish;
            sesion.sessionIteration += this.onNextIteration;
        }
        void ISessionSubscriber.SessionUnSubscribe(session sesion)
        {
            sesion.sessionStart -= this.onSessionStart;
            sesion.sessionFinish -= this.onSessionFinish;
            sesion.sessionIteration -= this.onNextIteration;
        }
        private void onSessionStart(object obj, sessionEventArgs e) 
        {
            this.StartHandGenerator = new startHandFactory(this, e.Count);
            this.sendMessage(new startGameMessageArgs() { game = this.game, info=this.game.info});
        }
        private void onSessionFinish(object obj, sessionEventArgs e) 
        {
            this.StartHandGenerator.Stop();
            this.sendMessage(new endGameMessageArgs());
        }
        public void onNextIteration(object obj, sessionIterationEventArgs e)
        {
            if (!this.isSet(TableOption.noRotate))  this.Seats.RotateNext();
            
            this.deck.Shuffle();
            this.dealer.nextIteration(e.Iteration);
            this.dealer.getAnteBlindsBring();

            while (true)
            {
                dealer.nextRound();
                dealer.betting();
                dealer.endRound();
                if (dealer.round >= this.ActiveRoundCount) break;
            }
            dealer.ShowDown();
        }

        /// <summary>
        /// Обработка сообщений от игроков
        /// </summary>
        /// <param name="sender">pokerPlayer</param>
        /// <param name="e">должен быть производным от pokerPlayerMessageArgs</param>
        public resultType onPlayerMessage(object sender, pokerPlayerMessageArgs e)
        {
            pokerPlayer _player = sender as pokerPlayer;
            e.sender = _player;
            switch (e.eventType)
            {
                case pokerPlayerEventType.trySitDown:
                    {
                        if ((e as trySitMessageArgs).seatNum != -1)
                            return this.Seats.Add((e as trySitMessageArgs).seatNum, _player);
                        return this.Seats.Add(_player); 
                    }
                case pokerPlayerEventType.exitTable: { this.Seats.Remove(_player); break; }
                
                case pokerPlayerEventType.getCards:
                    {
                        this.dealer.dealCards(_player, (e as getCardsMessageArgs).cardsCount, (e as getCardsMessageArgs).cardsString);
                        break;
                    }
                case pokerPlayerEventType.foldCards:
                    {
                        this.dealer.pickCards(_player, (e as foldCardsMessageArgs).cards);
                        break;
                    }
                case pokerPlayerEventType.showHand:
                    {
                        showHandMessageArgs _args = e as showHandMessageArgs;
                        //if (_args.round == this.ActiveRoundCount)
                            this.dealer.playerShowHand(_player, _args.value);
                        break;
                    }

                default:
                    {
                        this.sendMessage(new PlayerSayMessageArgs() { sender = _player, message = e });
                        break;
                    }
            }
            this.toStream(this, streamEventType.incoming, e);
            return resultType.ok;
        }

        #region реализация IStream...
        private StreamEventHandler StreamPublic;
        public void StreamSubscribe(IStreamViewer _viewer)   { this.StreamPublic += _viewer.StreamEventHandler; }
        public void StreamUnSubscribe(IStreamViewer _viewer) { this.StreamPublic -= _viewer.StreamEventHandler; }
        public void toStream(object sender, streamEventType _evType, EventArgs e) 
        {
            if (this.StreamPublic != null) this.StreamPublic(sender, _evType, e);
        }
        #endregion
    }
}
