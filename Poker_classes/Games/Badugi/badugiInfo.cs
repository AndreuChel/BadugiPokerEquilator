using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Cards.Poker_classes.Common.DeckAndCards;
using Cards.Poker_classes.utils;

namespace Cards.Poker_classes.Games.Badugi
{
    class badugiInfo// : IComparable
    {
        
        public cardSet Cards;
        public int val = 0;

        private badugiInfo(cardSet _cSet) { this.eval(_cSet); }
        public static badugiInfo get(cardSet _cSet) 
        {
            badugiInfo bi;
            int _hSet = _cSet.GetHashCode();
            if (!badugiInfo._hash.TryGetValue(_hSet, out bi))
            {
                bi = new badugiInfo(_cSet);
                badugiInfo._hash.Add(_hSet, bi);
            }
            return bi;
        }
        public static badugiInfo Empty { get { return null; } }
        public static Dictionary<int, badugiInfo> _hash = new Dictionary<int, badugiInfo>();

        private static int[] _badugiMaxVal = { 12287, 18431, 25599, 33279 };
        private static int[] _badugiMinVal = { 16382, 24572, 32760, 40944 };
        private static String[] _DefaultRuleString = { "a..k", "2a..kq", "32a..kqj", "432a..kqjt" };

        public static int badugiMinVal(int _cardCount) { return _badugiMinVal[_cardCount - 1];  }
        public static int badugiMaxVal(int _cardCount) { return _badugiMaxVal[_cardCount - 1]; }
        public static String DefaultRuleString(int _cardCount) { return _DefaultRuleString[_cardCount - 1]; }
        
        /// <summary>
        /// Возвращает значение бадуги
        /// </summary>
        /// <param name="ruleString">строка состоящая из рангов карт без мастей(например, "765" или "kqjt")</param>
        /// <returns></returns>
        public static int badugiVal(String ruleString)
        {
            int _val = ruleString.Aggregate(0, (__result, next) => __result |= (1 << badugi.Deck.RegistredRanks.First(_el => _el.Value.letter[0] == next).Value.id));
            return (int)(((bitOperations.bitCount((uint)_val) << 13) | 0x1fff) & (~_val));
        }

        /// <summary>
        /// Возращает строкое представление бадуги (без мастей)
        /// </summary>
        /// <param name="_badugiVal"></param>
        /// <returns></returns>
        public static String RuleString(int _badugiVal)
        {
            String _valStr = Convert.ToString(_badugiVal, 2);

            return
                _valStr.Skip(_valStr.Length-13).Reverse().Select((_el, _index) => new { letter = _el, index = _index })
                .Aggregate(String.Empty, (__result, next) => __result = (next.letter != '0' ? "" :
                           badugi.Deck.RegistredRanks.First(_el => _el.Value.id == next.index).Value.letter) + __result);

        }
        
        private static int ToBadugi(List<card> _cSet) {
            uint _val = 0;
            foreach (var _card in _cSet) _val |= _card.bitMask >> 16;
            return (int)(((bitOperations.bitCount(_val) << 13) | 0x1fff) & (~_val));
        }
        
        protected void eval(cardSet _cSet)
        {
            Dictionary<int, cardSet> _sList = new Dictionary<int, cardSet>();
            foreach (var _card in _cSet)
            {
                int _suit = _card.Suit.id;
                if (!_sList.ContainsKey(_suit)) _sList.Add(_suit, new cardSet(4));
                _sList[_suit].Add(_card);
            }
            List<cardSet> combinations = this.findCombination(_sList);
            int max = int.MinValue;
            foreach (var _badugiSet in combinations)
            {
                int _badugiValue = badugiInfo.ToBadugi(_badugiSet);
                if (_badugiValue >= max) { max = this.val = (int)_badugiValue; this.Cards = _badugiSet; }
            }
            this.Cards = new cardSet(4, this.Cards.OrderBy(_el => _el.Rank.id).ToList());

        }
        protected List<cardSet> findCombination(Dictionary<int, cardSet> _sList, int level = 0)
        {
            List<cardSet> _rlist = new List<cardSet>();
            if (level >= _sList.Count) return _rlist;

            cardSet _cards = _sList.ElementAt(level).Value;
            List<cardSet> lowerCombinations = this.findCombination(_sList, level + 1);

            foreach (var _card in _cards)
                if (lowerCombinations.Count > 0)
                    foreach (var _combination in lowerCombinations)
                    {
                        cardSet _tmpList = new cardSet(4, _combination);
                        if (_combination.All(_el => _el.Rank.id != _card.Rank.id)) _tmpList +=_card;
                        _rlist.Add(_tmpList);
                    }
                else _rlist.Add(new cardSet(4, _card));
            return _rlist;
        }

    }
}
