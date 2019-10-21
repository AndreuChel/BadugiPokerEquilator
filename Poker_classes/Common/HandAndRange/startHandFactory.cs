using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cards.Poker_classes.utils.Iterator;
using Cards.Poker_classes.Common.DeckAndCards;
using Cards.Poker_classes.Common.Player;
using Cards.Poker_classes.Common.Table;
using Cards.Poker_classes.Games.Badugi;
using Cards.Poker_classes.utils;

namespace Cards.Poker_classes.Common.HandAndRange
{
    abstract class startHand
    {
        public Priority Priority = Priority.low;
        static public object locker = new object();
        abstract public pokerHand generateHand(Deck deck, IEnumerable<card> reservedCards);
        virtual public IEnumerable<card> ReservedCards { get { return Enumerable.Empty<card>(); } }
    }
    
    class startHandFactory : IEnumerable<Dictionary<pokerPlayer, pokerHand>>
    {
        public sessionAsync _generatingTask { get; private set; }

        protected pokerTable Owner;
        public resultType Result = resultType.empty;
        public String errorMessage = "";
        public int IterationsCount { get; private set; }
        private ConcurrentDictionary<int, Dictionary<pokerPlayer, pokerHand>> dealCardsDictionary =
            new ConcurrentDictionary<int, Dictionary<pokerPlayer, pokerHand>>();

        public startHandFactory(pokerTable _owner, int _iterationsCount = 0)
        {
            this.Owner = _owner; this.IterationsCount = _iterationsCount;
            if (this._generatingTask != null && this._generatingTask.Status == TaskStatus.Running)
                throw new Exception();

            this.Run();

        }
        public void Stop(String message = "")
        {
            if (this._generatingTask != null && this._generatingTask.Status == TaskStatus.Running)
                this._generatingTask.Stop(message);
        }
        private void Run()
        {
            this._generatingTask = new sessionAsync(this.IterationsCount);
            Deck deck = this.Owner.game.deck;
            
            var _pl = this.Owner.Seats.Cast<pokerPlayer>()
                .ToDictionary(_k => _k, _v => _v.getHelper(this.Owner).startHandObject)
                .OrderBy(_el => _el.Value.Priority)
                .AsEnumerable();
            var reserv = _pl.SelectMany(_el => _el.Value.ReservedCards).Distinct().AsEnumerable(); 

            this._generatingTask.Run((sender, e) =>
            {
                try
                {
                    deck.Shuffle();
                    var tDic = new Dictionary<pokerPlayer, pokerHand>();
                    foreach (var pp in _pl) tDic.Add(pp.Key, pp.Value.generateHand(deck, reserv));
                    dealCardsDictionary.TryAdd((e as sessionIterationEventArgs).Iteration, tDic);
                }
                catch (Exception ex)
                {
                    this.Stop(ex.Message);
                }
            });
        }

        public Dictionary<pokerPlayer, pokerHand> this[int index]
        {
            get
            {
                Dictionary<pokerPlayer, pokerHand> _result = new Dictionary<pokerPlayer, pokerHand>();
                if (index < 0 || index >= this.IterationsCount) throw new Exception(String.Format("Элемента с индексом {0} не существует!", index));

                while ((this._generatingTask != null && this._generatingTask.Status == TaskStatus.Running)
                    && !this.dealCardsDictionary.ContainsKey(index)) ;

                if (this.dealCardsDictionary.ContainsKey(index)) _result = this.dealCardsDictionary[index];
                return _result;
            }
        }
        public int Count { get { return this.dealCardsDictionary.Count; } }
        #region IEnumerator<Dictionary<pokerPlayer, pokerHand>>...
        public IEnumerator<Dictionary<pokerPlayer, pokerHand>> GetEnumerator()
        {
            for (int i = 0; i < this.IterationsCount; i++)
            {
                var _dic = this[i]; if (_dic.Count == 0) break;
                yield return _dic;
            }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
