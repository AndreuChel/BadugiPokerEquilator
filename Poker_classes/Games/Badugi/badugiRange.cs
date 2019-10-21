using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Cards.Poker_classes.Common;
using Cards.Poker_classes.Common.DeckAndCards;
using Cards.Poker_classes.utils;
using Cards.Poker_classes.Common.HandAndRange;

namespace Cards.Poker_classes.Games.Badugi
{
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
            this.AddRange(_rules.OrderBy(_el => _el.CardCount).ToList());
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

            return new badugiRange(_tmpRules.Select(_el => new badugiRangeRule(_el.Item1, _el.Item2)));
        }
        static public bool tryGet(string _rangeString, out badugiRange bR)
        {
            bR = null;
            try { bR = badugiRange.get(_rangeString); return true; }
            catch (Exception) { return false; }
            
        }
        public badugiHand getHandInRange(badugiHand _ph) 
        {
            int _hash = _ph.Cards.GetHashCode();
            while (!this.inRange(_ph)
                    && _ph.Count >= this.MinLengthRule
                    && (_hash = badugiHandsHash.Items[_hash].lowBadugi) != -1)
                _ph = badugiHandsHash.Items[_hash].Hand as badugiHand;

            return _ph;
        }
    }
}








