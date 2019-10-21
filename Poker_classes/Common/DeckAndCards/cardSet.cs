using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cards.Poker_classes.utils;

namespace Cards.Poker_classes.Common.DeckAndCards
{
    class cardSet : List<card>
    {
        public cardSet(int _capacity) : base(_capacity) { this.Capacity = _capacity; }
        public cardSet(int _capacity, card _c) : this(_capacity) { this.Add(_c); }
        
        public cardSet(int _capacity, IEnumerable<card> _s)
            : this(_capacity)
        {
            _s.Take(_capacity).ToList().ForEach(_card => this.Add(_card));
        }

        
        private cardSet addition(card _c)
        {
            cardSet res = new cardSet(this.Capacity, this);
            //if (res.Count >= this.Capacity || res.Contains(_c)) return res;
            res.Add(_c); return res;
        }
        private cardSet addition(List<card> _c)
        {
            cardSet res = new cardSet(this.Capacity, this);
            res.AddRange(_c.Take(this.Capacity - this.Count).ToList());
            return res;
        }
        private cardSet subtraction(card _c)
        {
            cardSet res = new cardSet(this.Capacity, this);
            res.Remove(_c);
            return res;
        }
        private cardSet subtraction(List<card> _c)
        {
            cardSet res = new cardSet(this.Capacity, this);
            res.RemoveAll(_card => _c.Contains(_card));
            return res;
        }

        public static cardSet operator +(cardSet _s, card _c2) { return _s.addition(_c2); }
        public static cardSet operator +(cardSet _s, List<card> _s2)
        {
            //_s2.ForEach(_card => _s += _card); return _s;
            return _s.addition(_s2);
        }

        public static cardSet operator -(cardSet _s, card _c) { return _s.subtraction(_c); }
        public static cardSet operator -(cardSet _s, List<card> _s2)
        {
            //_s2.ForEach(_card => _s -= _card); return _s;
            return _s.subtraction(_s2);
        }
        
        public static cardSet operator ^(cardSet _s, List<card> _s2)
        {
            return new cardSet(_s.Capacity, _s.Intersect(_s2).ToList()); 
        }
        
        public override string ToString()
        {
            return "["+
                    this.OrderBy(_el => _el.Rank)
                        .Aggregate(String.Empty, 
                        (__result, next) => __result += (__result != String.Empty?",":"")+next.ToString()) +
                    "]";
        }

        public override int GetHashCode()
        {
            return this.Aggregate(1, (__result, next) => __result *= Arrays.primeNumbers[next.ID]);
        }
        static public int getHash(IEnumerable<card> _cards)
        {
            return _cards.Aggregate(1, (__result, next) => __result *= Arrays.primeNumbers[next.ID]);
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is cardSet)) return false;
            return this.GetHashCode() == (obj as cardSet).GetHashCode();
        }
    }
}
