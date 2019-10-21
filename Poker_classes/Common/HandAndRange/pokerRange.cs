using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Cards.Poker_classes.Common.DeckAndCards;
using Cards.Poker_classes.utils;

namespace Cards.Poker_classes.Common.HandAndRange
{
    class rangeException : Exception { public rangeException(String message) : base(message) { } }

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
    abstract class range<T> : List<T> where T : rangeRule
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
    abstract class range : range<rangeRule> { }
}
