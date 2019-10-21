using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Cards.Poker_classes.Common;
using Cards.Poker_classes.Common.Game;
using Cards.Poker_classes.Common.HandAndRange;
using Cards.Poker_classes.Common.Player;
using Cards.Poker_classes.Common.Table;
using Cards.Poker_classes.Common.DeckAndCards;
using Cards.Poker_classes.Games.Badugi;
using Cards.Poker_classes.utils.Iterator;

namespace Cards.Poker_classes.Games.Badugi_new
{
    /*
    abstract class rangeRule
    {
        public static rangeRule Empty { get { return null; } }
        public readonly int LowValue, HighValue;
        abstract public String Name { get; }

        public rangeRule(int _from, int _to)
        {
            this.LowValue = _from <= _to ? _from : _to;
            this.HighValue = _from > _to ? _from : _to;
        }
        public bool check(int _value) { return _value >= this.LowValue && _value <= this.HighValue; }
        public override string ToString() { return String.Format("[{0}]", this.Name); }

    }
    abstract class range<T> : List<T> where T: rangeRule
    {
        public bool inRange(int _value, out T _rule) 
        {
            _rule = this.Where(_r => _r.check(_value)).FirstOrDefault();
            return _rule != rangeRule.Empty;
        }
        public bool inRange(pokerHand _hand, out String _ruleName)
        {
            T _rule; _ruleName = String.Empty;
            if (this.inRange(_hand.value, out _rule)) { _ruleName = _rule.Name; return true; }
            return false;
        }
        public bool inRange(pokerHand _hand)
        {
            return this.Any(_r => _r.check(_hand.value));
        }
        public override string ToString() 
        {
            return String.Format("[{0}]",
                this.Aggregate(String.Empty, (__result, __next) => 
                    __result += (__result != String.Empty ? ", " : "") + __next.Name));
        }
    }

    class badugiRangeRule : rangeRule
    {
        public badugiRangeRule(int _from, int _to) : base(_from, _to) { }
        private String _name = String.Empty;
        public override string Name
        {
            get 
            {
                if (this._name == String.Empty) 
                    this._name = badugiInfo.RuleString(this.HighValue) + ".." + badugiInfo.RuleString(this.LowValue);
                return this._name;
            }
        }
        public int CardCount { get { return this.LowValue >> 13; } }
    }
    class badugiRange : range<badugiRangeRule>
    {
        public int MinLengthRule, MaxLengthRule;
        
        protected badugiRange(IEnumerable<badugiRangeRule> _rules) 
        {
            this.AddRange(_rules.OrderBy(_el=>_el.CardCount).ToList());
            this.MinLengthRule = this.Min(_el => _el.CardCount);
            this.MaxLengthRule = this.Max(_el => _el.CardCount);
        }
        static public badugiRange get(String _rangeString) 
        {
            _rangeString = _rangeString.ToLower().Replace(" ", "");
            if (Regex.IsMatch(_rangeString, @"[^\s\-\+\.\,akqjt2-9]"))
                throw new rangeException(String.Format("Диапазон [{0}] содержит недопустимые символы!", _rangeString));
            IEnumerable<String> _rules = _rangeString.Split(',');
            
            Regex _ruleType2 = new Regex(@"([akqjt2-9]+)(\+|\-|\.\.)?([akqjt2-9]+)?", RegexOptions.Compiled);

            //_from, _to, cardCount, SignRange
            var _tmpRules = new List<Tuple<int, int, int, SignRange>>();

            foreach (String _rule in _rules)
            {
                MatchCollection _mm = _ruleType2.Matches(_rule);
                if (_mm.Count != 1) throw new rangeException(String.Format("Диапазон [{0}] содержит ошибки!", _rangeString));
                Match _m = _mm[0];

                String _fromString = _m.Groups[1].Value;
                String _op = _m.Groups.Count > 2 ? _m.Groups[2].Value : String.Empty;
                String _toString = _m.Groups.Count > 3 ? _m.Groups[3].Value : String.Empty;

                if ((_fromString.Length > 4 || _toString.Length > 4) ||
                    (_toString != String.Empty && (_op == "+" || _op == "-")) ||
                    (_toString == String.Empty && (_op == "..")) ||
                    (_ruleType2.Replace(_rule, "").Trim().Length != 0))
                    throw new rangeException(String.Format("Диапазон [{0}] содержит ошибки!", _rangeString));

                int _from = badugiInfo.badugiVal(_fromString);
                int _to = _toString != String.Empty ? badugiInfo.badugiVal(_toString) : _from;

                int _cardCount = (int)((uint)_from >> 13);
                SignRange _sign = SignRange.None;

                if (_op == "+") { _to = badugiInfo.badugiMaxVal(_cardCount); _sign = SignRange.allTheWorst; }
                if (_op == "-") { _to = badugiInfo.badugiMinVal(_cardCount); _sign = SignRange.allTheBest; }

                if (_from > _to) { int t = _to; _to = _from; _from = t; }

                if (_op == "..")
                {
                    _sign = SignRange.allBetween;
                    int count1 = (int)((uint)_from >> 13); int count2 = (int)((uint)_to >> 13);
                    for (int i = count1; i <= count2; i++)
                    {
                        int fTmp = i == count1 ? _from : badugiInfo.badugiMaxVal(i);
                        int tTmp = i == count2 ? _to : badugiInfo.badugiMinVal(i);

                        if (count1 == count2) { fTmp = _from; tTmp = _to; }
                        if (fTmp > tTmp) { int t = fTmp; fTmp = tTmp; tTmp = t; }
                        _tmpRules.Add(new Tuple<int, int, int, SignRange>(fTmp, tTmp, i, _sign));
                    }
                }
                else
                    _tmpRules.Add(new Tuple<int, int, int, SignRange>(_from, _to, _cardCount, _sign));

            }

            HashSet<int> cardCounts = new HashSet<int>(_tmpRules.Select(_el => _el.Item3).Distinct());
            IEnumerable<Tuple<int, int, int, SignRange>> _lRules = _tmpRules.Where(_el => _el.Item4 == SignRange.allTheBest).ToList();

            var _tmpRules2 = new List<Tuple<int, int, int, SignRange>>();
            foreach (var _r in _lRules)
                for (int i = _r.Item3 + 1; i <= 4; i++)
                {
                    if (cardCounts.Contains(i)) break;
                    _tmpRules2.Add(new Tuple<int, int, int, SignRange>(badugiInfo.badugiMaxVal(i), 
                        badugiInfo.badugiMinVal(i), i, SignRange.allTheBest));
                }
            _lRules = _tmpRules.Where(_el => _el.Item4 == SignRange.allTheWorst).ToList();
            foreach (var _r in _lRules)
                for (int i = _r.Item3 - 1; i >= 1; i--)
                {
                    if (cardCounts.Contains(i)) break;
                    _tmpRules2.Add(new Tuple<int, int, int, SignRange>(badugiInfo.badugiMaxVal(i),
                        badugiInfo.badugiMinVal(i), i, SignRange.allTheWorst));
                }
            _tmpRules.AddRange(_tmpRules2);

            return new badugiRange(_tmpRules.Select(_el => new badugiRangeRule (_el.Item1, _el.Item2)));
        }
    }
    */

    //===============================================
    /*
    class startHandFactory : IEnumerable<Dictionary<pokerPlayer, pokerHand>>
    {
        public sessionAsync _generatingTask { get; private set; }
        
        protected pokerTable Owner;
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
        public void Stop()
        {
            if (this._generatingTask != null && this._generatingTask.Status == TaskStatus.Running) 
                this._generatingTask.Stop();
        }
        private void Run() 
        {
            this._generatingTask = new sessionAsync(this.IterationsCount);
            Deck deck = this.Owner.game.deck;
            //IEnumerable<pokerPlayer> PlayerList = this.Owner.Seats.Cast<pokerPlayer>();
            List<startHand> shList = new List<startHand>();
            
            shList.Add(new badugiStartHandCards(deck.parseCardString("As2c")));
            shList.Add(new badugiStartHandRange(badugiRange.get("432-,7654-")));
            shList.Add(new badugiStartHandRange(badugiRange.get("432-,7654-")));
            shList.Add(new badugiStartHandRange(badugiRange.get("432-,7654-")));
            shList.Add(new badugiStartHandRandom());
            
            var reserv = shList.SelectMany(_el => _el.ReservedCards).Distinct();
            shList = shList.OrderBy(_el => _el.Priority).ToList();

            this._generatingTask.Run((sender, e) =>
            {
                deck.Shuffle();
                int pIndex = 0;
                var tDic = new Dictionary<pokerPlayer, pokerHand>();
                shList.ForEach(sh => tDic.Add(new pokerPlayer("p"+(pIndex++).ToString(), double.MaxValue), sh.generateHand(deck, reserv)));
                dealCardsDictionary.TryAdd((e as sessionIterationEventArgs).Iteration, tDic);
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
    
    abstract class startHand
    {
        public Priority Priority = Priority.low;
        abstract public pokerHand generateHand(Deck deck, IEnumerable<card> reservedCards);
        virtual public IEnumerable<card> ReservedCards { get { return Enumerable.Empty<card>(); } }
    }
    
    class badugiStartHandCards : startHand 
    {
        private IEnumerable<card> mainSet;
        private badugiHand Hand;
        public badugiStartHandCards(IEnumerable<card> cards)  
        {
            this.Priority = Priority.normal;
            int hash = cardSet.getHash(cards);
            this.Hand = badugiHandsHash.Items[hash].Hand as badugiHand; hash = this.Hand.BadugiCards.GetHashCode();
            this.mainSet = badugiHandsHash.Items.Where(_el => _el.Value.Value == this.Hand.value && _el.Key % hash == 0)
                .SelectMany(_el => _el.Value.BadugiPickCards).Distinct()
                .Where(_el=>!cards.Contains(_el))
                .ToList();
        }
        public override pokerHand generateHand(Deck deck, IEnumerable<card> reservedCards)
        {
            cardSet cs = new cardSet(4, deck.getCard(this.Hand.BadugiCards));
            var _set = this.mainSet.Where(_el => !reservedCards.Contains(_el)).ToList();
            for (int i = 4 - cs.Count; i > 0; i--) cs.Add(deck.getCardRandom(_set));
            return badugiHandsHash.Items[cs.GetHashCode()].Hand;//badugiHand.get(cs);
        }
        public override IEnumerable<card> ReservedCards
        {
            get { return this.Hand.BadugiCards; }
        }
    }
    class badugiStartHandRange : startHand 
    {
        private IEnumerable<pokerHand> rangeHands;
        public badugiStartHandRange(badugiRange range) 
        { 
            this.Priority = Priority.high;
            this.rangeHands = range.SelectMany(_r =>
                badugiHandsHash.Items
                .Where(_el => _el.Value.Count == 4 && _el.Value.Value >= _r.LowValue && _el.Value.Value <= _r.HighValue)
                .Select(_el => _el.Value.Hand)).ToList();
        }
        public override pokerHand generateHand(Deck deck, IEnumerable<card> reservedCards)
        {
            badugiHand bH;
            do
            {
                int rnd = Deck._randomGenerator.Next(this.rangeHands.Count());
                bH = this.rangeHands.ElementAt(rnd) as badugiHand;
            }
            while (bH.Cards.Any(_c => deck.PickedCards.Contains(_c) || reservedCards.Contains(_c)));
            deck.getCard(bH.Cards);
            return bH;
        }
    }
    class badugiStartHandRandom : startHand
    {
        public override pokerHand generateHand(Deck deck, IEnumerable<card> reservedCards)
        {
           return badugiHandsHash.Items[cardSet.getHash(deck.getCards(4,reservedCards))].Hand;
        }
    }
    */
}
