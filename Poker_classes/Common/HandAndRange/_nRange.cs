using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Cards.Poker_classes.utils;
using Cards.Poker_classes.Games.Badugi;
using Cards.Poker_classes.Common.DeckAndCards;

namespace Cards.Poker_classes.Common.HandAndRange
{
    /*
    abstract class rangeRule : IEnumerable<int>
    {
        public static int counter = 0;
        public static rangeRule Empty { get { return null; } }
       
        public static Dictionary<String, rangeRule> AllRules = new Dictionary<String, rangeRule>();
        public static bool TryGetRule(int _lowValue, int _highValue, out rangeRule _rule)
        {
            int _low  = (_lowValue  <= _highValue ? _lowValue  : _highValue);
            int _high = (_highValue >= _lowValue  ? _highValue : _lowValue);

            var _r = rangeRule.AllRules.Where(_el =>
                            _el.Value.LowValue == _low
                            && _el.Value.HighValue == _high).Select(_el => _el.Value);
            _rule = _r.FirstOrDefault();

            return _rule != null;
        }
        
        protected String _name = String.Empty;
        public int LowValue, HighValue;
        public SignRange SignRange; 
        public readonly int ID;
        
        private rangeRule() { this.ID = ++rangeRule.counter; }
        protected rangeRule(SignRange _sign) : this()
        {
            this.SignRange = _sign; 
        }
        protected rangeRule(int _from, int _to, SignRange _sign)
            : this(_sign)
        {
            this.LowValue = _from <= _to ? _from : _to;
            this.HighValue = _from > _to ? _from : _to;
        }
        
        abstract public int this[int index] { get; }
        abstract public int Count { get; }
        abstract public bool inRange(int Hash);
        abstract public String Name { get; }
        
        abstract public IEnumerator<int> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public override string ToString()
        {
            return String.Format("[{0}]",this.Name);
        }
    }

    abstract class range : IEnumerable<rangeRule>
    {
        public Priority Priority = Priority.low;
        private List<String> _ruleIDsList = new List<String>();
        public static readonly MersenneTwister _randomGenerator = new MersenneTwister();
        public int Count { get { return this.Aggregate(0, (__result, next) => __result += next.Count); } }
        
        public range(String rangeString)  {  this.parseRangeString(rangeString); }

        protected void Add(String ruleString)
        {
            if (!this._ruleIDsList.Contains(ruleString)) this._ruleIDsList.Add(ruleString);
        }

        abstract protected void parseRangeString(String _rangeString);
        public bool inRange(int Hash, out rangeRule _rule)
        {
            _rule = rangeRule.Empty;
            foreach (var _r in this) if (_r.inRange(Hash)) { _rule = _r; return true; }
            return false;
        }
        public bool inRange(pokerHand _hand, out String _ruleName)
        {
            rangeRule _rule; _ruleName = String.Empty;
            if (this.inRange(_hand.Cards.GetHashCode(), out _rule))
            {
                _ruleName = _rule.Name; return true;
            }
            return false;
        }
        public bool inRange(pokerHand _hand)
        {
            String _ruleString = String.Empty;
            return this.inRange(_hand, out _ruleString);
        }

        protected int _getRandomHand()
        {
            int elemNumber = range._randomGenerator.Next(this.Count);

            foreach (var _rule in this)
            {
                if (elemNumber < _rule.Count) return _rule[elemNumber];
                elemNumber -= _rule.Count;
            }
            throw new rangeException("getRandomHand: Ошибка формирования рандомной руки!");
        }
        abstract public cardSet getRandomHand(); 
        #region IEnumerator<_nRangeRule>...
        public IEnumerator<rangeRule> GetEnumerator() {
            foreach (var _ruleID in this._ruleIDsList) yield return rangeRule.AllRules[_ruleID];
        }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        #endregion
    }
    */
    //*********************************************************************************************************
    /*
    class babugiRangeRule : rangeRule
    {
        private HashSet<int> _handsSet; //массив хэшей всех рук (1-4 карты)  для inRange
        private int[]  _handsArray; //массив хэшей, только для рук с 4мя картами (для рандома)
        public readonly int CardCount = 0;

        public babugiRangeRule(int _from, int _to, SignRange _sign)
            : base(_from, _to, _sign)
        {
            this.CardCount = (int)((uint)this.LowValue >> 13);
            var _hashDic = badugiHandsHash.Items.Where(_el => _el.Value.Value >= this.LowValue && _el.Value.Value <= this.HighValue)
                .ToDictionary(_k => _k.Key, _v => _v.Value.Count);
            this._handsSet = new HashSet<int>(_hashDic.Select(_el=>_el.Key));
            this._handsArray = _hashDic.Where(_el => _el.Value == 4).Select(_el=>_el.Key).ToArray();
            
        }
        public babugiRangeRule(List<card> _cards)
            : base(SignRange.withCards)
        {
            cardSet _cs = new cardSet(4, _cards);
            int _hash = _cs.GetHashCode();
            badugiInfo _bi = badugiInfo.get(new cardSet(4, _cards));

            this.LowValue = this.HighValue = _bi.val;
            this.CardCount = _bi.Cards.Count;
            this._name = _bi.Cards.Aggregate("", (__r, next) => __r += next.ToString());

            this._handsSet = new HashSet<int>(badugiHandsHash.Items.Where(_el => _el.Value.Count==4 && _el.Key % _hash == 0 && _el.Value.Value == _bi.val)
                                                       .Select(_el => _el.Key));
            this._handsArray = this._handsSet.ToArray<int>();
        }

        public override int this[int index] { get { return this._handsArray[index]; } }
        public override int Count { get { return this._handsArray.Length; } }
        public override string Name
        {
            get {
                if (this._name == String.Empty) this._name = badugiInfo.RuleString(this.HighValue) + ".." + badugiInfo.RuleString(this.LowValue);
                return this._name;
            }
        }
        public override bool inRange(int Hash) { return this._handsSet.Contains(Hash); }

        public override IEnumerator<int> GetEnumerator() { return (IEnumerator<int>)this._handsArray.GetEnumerator(); }
    }

    class badugiRange : range
    {
        //public Priority Priority { get; private set; }
        public badugiRange(String rangeString) : base(rangeString) { }

        private void AddRule(int lowValue, int highValue, SignRange _s)
        {
            int _low = (lowValue <= highValue ? lowValue : highValue);
            int _high = (highValue >= lowValue ? highValue : lowValue);

            var _br = rangeRule.AllRules.Where(_el =>
                            _el.Value.LowValue == _low
                            && _el.Value.HighValue == _high
                            && _el.Value.SignRange != SignRange.withCards).Select(_el => _el.Value).FirstOrDefault();

            if (_br == null)
            {
                _br = new babugiRangeRule(lowValue, highValue, _s);
                rangeRule.AllRules.Add(_br.Name, _br);
            }
            rangeRule.AllRules[_br.Name].SignRange = _s;
            this.Add(_br.Name);
        }
        private void AddRule(String _rule)
        {
            this.Priority = Priority.high;

            List<card> _cards = badugi.Deck.parseCardString(_rule);
            if (!rangeRule.AllRules.ContainsKey(_rule))
                rangeRule.AllRules.Add(_rule, new babugiRangeRule(_cards));
            this.Add(_rule);
        }

        protected override void parseRangeString(string _rangeString)
        {
            if (_rangeString == String.Empty) _rangeString = "k-";
            _rangeString = _rangeString.ToLower().Replace(" ", "");
            if (Regex.IsMatch(_rangeString, @"[^\s\-\+\.\,akqjt2-9schd]"))
                throw new rangeException(String.Format("Диапазон [{0}] содержит недопустимые символы!", _rangeString));
            IEnumerable<String> _rules = _rangeString.Replace(" ", "").Split(',');

            Regex _ruleType1 = new Regex(@"(([akqjt2-9][scdh]){1,4})", RegexOptions.Compiled);
            Regex _ruleType2 = new Regex(@"([akqjt2-9]+)(\+|\-|\.\.)?([akqjt2-9]+)?", RegexOptions.Compiled);

            var _tmpRules = new List<Tuple<int, int, SignRange>>();

            foreach (String _rule in _rules)
            {
                if (_ruleType1.IsMatch(_rule))
                {
                    this.AddRule(_rule); continue;
                }

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
                        this.AddRule(fTmp, tTmp, _sign);
                    }
                }
                else
                    this.AddRule(_from, _to, _sign);

                this.MinLengthRule = this.Min(_el => (_el as babugiRangeRule).CardCount);
                this.MaxLengthRule = this.Max(_el => (_el as babugiRangeRule).CardCount); ;
            }

            HashSet<int> cardCounts = new HashSet<int>(this.Select(_el => (_el as babugiRangeRule).CardCount).Distinct());
            IEnumerable<rangeRule> _lRules = this.Where(_el => _el.SignRange == SignRange.allTheBest).ToList();
            foreach (var _r in _lRules)
                for (int i = (_r as babugiRangeRule).CardCount + 1; i <= 4; i++)
                {
                    if (cardCounts.Contains(i)) break;
                    this.AddRule(badugiInfo.badugiMaxVal(i), badugiInfo.badugiMinVal(i), SignRange.allTheBest);
                }
            _lRules = this.Where(_el => _el.SignRange == SignRange.allTheWorst).ToList();
            foreach (var _r in _lRules)
                for (int i = (_r as babugiRangeRule).CardCount - 1; i >= 1; i--)
                {
                    if (cardCounts.Contains(i)) break;
                    this.AddRule(badugiInfo.badugiMaxVal(i), badugiInfo.badugiMinVal(i), SignRange.allTheWorst);
                }
        }

        public pokerHand getHandInRange(pokerHand _ph)
        {
            int _hash = _ph.Cards.GetHashCode();
            while (!this.inRange(_ph)
                    && _ph.Count >= this.MinLengthRule
                    && (_hash = badugiHandsHash.Items[_hash].lowBadugi) != -1)
                _ph = badugiHandsHash.Items[_hash].Hand;
                    
            return _ph;
            //метод будет удален. оставлен для компиляции
            //throw new NotImplementedException();
        }
        public override cardSet getRandomHand()
        {
            return badugiHandsHash.Items[this._getRandomHand()].Hand.Cards;
        }


        public int MinLengthRule { get; private set; }
        public int MaxLengthRule { get; private set; }
    }
    */
}
