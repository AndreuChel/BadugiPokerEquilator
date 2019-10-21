using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cards.Poker_classes.Common.Player;
using Cards.Poker_classes.Common.DeckAndCards;
using Cards.Poker_classes.Games.Badugi;
using Cards.Poker_classes.Common.HandAndRange;

namespace Cards.Poker_classes.Common.Table
{

    //должно формировать в отдельном потоке руки для игроков
    //доступ с списку рук из другого потока
    /*
    class DealCardFactory : IEnumerable<Dictionary<pokerPlayer, cardSet>>
    {
        protected IEnumerable<pokerPlayer> PlayerList;
        protected pokerTable Owner;
        public int IterationsCount { get; private set; }

        protected Task dealTask;
        protected CancellationTokenSource cancelToken;
        public ConcurrentDictionary<int, Dictionary<pokerPlayer, cardSet>> dealCardsDictionary =
            new ConcurrentDictionary<int, Dictionary<pokerPlayer, cardSet>>();

        public DealCardFactory(pokerTable _owner, IEnumerable<pokerPlayer> _players, int _iterationsCount = 0) 
        {
            this.Owner = _owner; this.PlayerList = _players; this.IterationsCount = _iterationsCount;
            this.RunAsync();
        }

        protected void RunAsync()
        {
            if (this.dealTask != null && this.dealTask.Status == TaskStatus.Running) return;
            this.cancelToken = new CancellationTokenSource();

            IEnumerable<pokerPlayer> _pp = this.PlayerList.OrderBy(_el => _el.getHelper(this.Owner).startHandRange.Priority).ToList();
            this.dealTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    
                    for (int i = 0; i < this.IterationsCount; i++)
                    {
                        this.cancelToken.Token.ThrowIfCancellationRequested();

                        HashSet<card> dealCards = new HashSet<card>();
                        Dictionary<pokerPlayer, cardSet> _h = new Dictionary<pokerPlayer, cardSet>();
                        foreach (var _p in _pp)
                        {
                            range t = _p.getHelper(this.Owner).startHandRange;
                            if (t == null) throw new Exception();
                            cardSet _cs; int cx = 0;
                            while ((_cs = _p.getHelper(this.Owner).startHandRange.getRandomHand()).Any(_c => dealCards.Contains(_c)))
                                if (++cx > 100) throw new Exception();
                            _cs.ForEach(_c => dealCards.Add(_c));
                            
                            _h.Add(_p, _cs);
                        }
                        this.dealCardsDictionary.TryAdd(i, _h);
                    }
                }
                catch (OperationCanceledException) {  }
                catch (Exception) { }
            }, this.cancelToken.Token);
        }
        public void Stop()
        {
            if (this.dealTask != null && this.dealTask.Status == TaskStatus.Running)  this.cancelToken.Cancel();  
        }

        public Dictionary<pokerPlayer, cardSet> this[int index]
        {
            get
            {
                Dictionary<pokerPlayer, cardSet> _result = new Dictionary<pokerPlayer, cardSet>();
                if (index < 0 || index >= this.IterationsCount) throw new Exception(String.Format("Элемента с индексом {0} не существует!", index));
                
                while ((this.dealTask != null && this.dealTask.Status == TaskStatus.Running && !this.cancelToken.IsCancellationRequested)
                    && !this.dealCardsDictionary.ContainsKey(index)) ;

                if (this.dealCardsDictionary.ContainsKey(index)) _result = this.dealCardsDictionary[index];
                return _result;
            }
        }
        public int Count { get { return this.dealCardsDictionary.Count; } }

        public IEnumerator<Dictionary<pokerPlayer, cardSet>> GetEnumerator()
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
    }
    */
}
