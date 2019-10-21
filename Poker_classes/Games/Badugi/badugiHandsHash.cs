using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using Cards.Poker_classes.Common.HandAndRange;
using Cards.Poker_classes.Common.DeckAndCards;
using Cards.Poker_classes.utils;

namespace Cards.Poker_classes.Games.Badugi_old
{
    using Cards.Poker_classes.Games.Badugi;

    class hashRecord 
    { 
        public int Value = 0; 
        public List<int> Cards = new List<int>(); 
        public List<int> pickCards = new List<int>(); 
    }

    static class badugiHandsHash
    {
        static Object locker = new Object();
        
        private static Dictionary<int, hashRecord> _hands;
        public static Dictionary<int, hashRecord> Items {
            get  {
                lock (badugiHandsHash.locker)
                {
                    if (badugiHandsHash._hands == null) badugiHandsHash.fillHashDictionary();
                    return badugiHandsHash._hands;
                }
            }
        }
        
        private static Dictionary<int, HashSet<int>> _cardInHands = new Dictionary<int, HashSet<int>>();
        public static Dictionary<int, HashSet<int>> CardInHandsDictionary
        {
            get
            {
                lock (badugiHandsHash.locker)
                {
                    if (badugiHandsHash._hands == null) badugiHandsHash.fillHashDictionary();
                    return badugiHandsHash._cardInHands;
                }
            }
        }

        /// <summary>
        /// Формирует в фоновом потоке массив хешей.
        /// можно запускать в onLoad
        /// </summary>
        public static void prepareHashAsync()
        {
            (new Thread(badugiHandsHash.fillHashDictionary)).Start();
        }
        
        #region заполнение хеша...
        private static void fillHashDictionary()
        {
            lock (badugiHandsHash.locker)
            {
                foreach (var card in badugi.Deck.Cards) badugiHandsHash._cardInHands.Add(card.Key, new HashSet<int>());
                if (badugiHandsHash._hands == null) badugiHandsHash._hands = new Dictionary<int, hashRecord>();
                else badugiHandsHash._hands.Clear();

                foreach (var _combination in badugiHandsHash.getCombinations(0))
                {
                    cardSet cs = new cardSet(4, _combination);
                    int _hash = cs.GetHashCode();
                    foreach (var card in cs) badugiHandsHash._cardInHands[card.ID].Add(_hash);
                    badugiInfo bi = badugiInfo.get(cs);
                    badugiHandsHash._hands.Add(_hash, new hashRecord()
                    {
                        Value = bi.val,
                        Cards = bi.Cards.OrderBy(_el => _el.Rank.id).Select(_el => _el.ID).ToList(),
                        pickCards = (cs - bi.Cards).Select(_el => _el.ID).ToList()
                    });
                }
            }
        }
        private static IEnumerable<cardSet> getCombinations(int startCardID, int level = 3)
        {
            List<cardSet> result = new List<cardSet>();
            for (int i = startCardID; i < badugi.Deck.Cards.Count - level; i++)
                if (level > 0)
                    foreach (var _combination in badugiHandsHash.getCombinations(i + 1, level - 1))
                        result.Add(new cardSet(4, _combination) + badugi.Deck.Cards[i]);
                else
                    result.Add(new cardSet(4, badugi.Deck.Cards[i]));
            
            return result;
        }
        #endregion
    }

}

namespace Cards.Poker_classes.Games.Badugi
{
    class hashRecord
    {
        public hashRecord(pokerHand ph) { this.Hand = ph; }
        public readonly pokerHand Hand;
        public int Value { get { return this.Hand.value; } }
        public int Count { get { return this.Hand.Cards.Count; } }
        public cardSet Cards { get { return this.Hand.Cards; } }
    }
    class badugiHashRecord : hashRecord
    {
        public readonly int lowBadugi;
        
        public cardSet BadugiCards { get { return (this.Hand as badugiHand).info.Cards; } }
        public cardSet BadugiPickCards { get { return this.Cards - this.BadugiCards; } }
        public int BadugiCardsCount { get { return (this.Hand as badugiHand).info.Cards.Count; } }

        public badugiHashRecord(badugiHand bh) : base(bh)  
        { 
            this.lowBadugi = this.BadugiCards.Count == 1 ? -1 : cardSet.getHash((this.BadugiCards.Take(this.BadugiCards.Count - 1).ToList()));
        }
    }


    static class badugiHandsHash
    {
        static Object locker = new Object();

        private static Dictionary<int, badugiHashRecord> _hands;
        public static Dictionary<int, badugiHashRecord> Items
        {
            get
            {
                lock (badugiHandsHash.locker)
                {
                    if (badugiHandsHash._hands == null) badugiHandsHash.fillHashDictionary();
                    return badugiHandsHash._hands;
                }
            }
        }

        private static Dictionary<int, HashSet<int>> _cardInHands = new Dictionary<int, HashSet<int>>();
        public static Dictionary<int, HashSet<int>> CardInHandsDictionary
        {
            get
            {
                lock (badugiHandsHash.locker)
                {
                    if (badugiHandsHash._hands == null) badugiHandsHash.fillHashDictionary();
                    return badugiHandsHash._cardInHands;
                }
            }
        }

        public static badugiHand get(List<card> _cs)
        {
            return badugiHandsHash.Items[cardSet.getHash(_cs)].Hand as badugiHand;
        }
        /// <summary>
        /// Формирует в фоновом потоке массив хешей.
        /// можно запускать в onLoad
        /// </summary>
        public static void prepareHashAsync()
        {
            (new Thread(badugiHandsHash.fillHashDictionary)).Start();
        }

        #region заполнение хеша...
        public static void fillHashDictionary()
        {
            lock (badugiHandsHash.locker)
            {
                foreach (var card in badugi.Deck.Cards) badugiHandsHash._cardInHands.Add(card.Key, new HashSet<int>());
                if (badugiHandsHash._hands == null) badugiHandsHash._hands = new Dictionary<int, badugiHashRecord>();
                else badugiHandsHash._hands.Clear();

                for (int cardCount = 4; cardCount>=1; cardCount--)
                    foreach (var _combination in badugiHandsHash.getCombinations(0, cardCount - 1))
                    {
                        cardSet cs = new cardSet(4, _combination);
                        int _hash = cs.GetHashCode();

                        foreach (var card in cs) badugiHandsHash._cardInHands[card.ID].Add(_hash);
                        badugiHandsHash._hands.Add(_hash, new badugiHashRecord(badugiHand.get(cs)));
                    }
            }
        }
        private static IEnumerable<cardSet> getCombinations(int startCardID, int level = 3)
        {
            List<cardSet> result = new List<cardSet>();
            for (int i = startCardID; i < badugi.Deck.Cards.Count - level; i++)
                if (level > 0)
                    foreach (var _combination in badugiHandsHash.getCombinations(i + 1, level - 1))
                        result.Add(new cardSet(4, _combination) + badugi.Deck.Cards[i]);
                else
                    result.Add(new cardSet(4, badugi.Deck.Cards[i]));

            return result;
        }
        #endregion
    }

}
