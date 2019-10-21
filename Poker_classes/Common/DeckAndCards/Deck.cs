using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Cards.Poker_classes.utils;
using System.Text.RegularExpressions;

namespace Cards.Poker_classes.Common.DeckAndCards
{

    abstract class Deck
    {
        #region Init RegistredRanks & RegistredSuits
        public Dictionary<int, rank> RegistredRanks = new Dictionary<int, rank>() {
                {0,  new rank {id=0, letter="2", PrimeValue=2}}, 
                {1,  new rank {id=1, letter="3", PrimeValue=3}}, 
                {2,  new rank {id=2, letter="4", PrimeValue=5}},
                {3,  new rank {id=3, letter="5", PrimeValue=7}}, 
                {4,  new rank {id=4, letter="6", PrimeValue=11}}, 
                {5,  new rank {id=5, letter="7", PrimeValue=13}},
                {6,  new rank {id=6, letter="8", PrimeValue=17}}, 
                {7,  new rank {id=7, letter="9", PrimeValue=19}}, 
                {8,  new rank {id=8, letter="t", PrimeValue=23}},
                {9,  new rank {id=9, letter="j", PrimeValue=29}}, 
                {10, new rank {id=10, letter="q", PrimeValue=31}},
                {11, new rank {id=11, letter="k", PrimeValue=37}},
                {12, new rank {id=12, letter="a", PrimeValue=41}}
        };
        public Dictionary<int,suit> RegistredSuits = new Dictionary<int,suit> {
                {0, new suit {id=0, letter="s",nameEng = "spades",nameRu="пики"}},
                {1, new suit {id=1, letter="c",nameEng = "clubs",nameRu="трефы"}},
                {2, new suit {id=2, letter="d",nameEng = "diamonds",nameRu="буби"}},
                {3, new suit {id=3, letter="h",nameEng = "hearts",nameRu="черви"}}
        };
        //private string rankLetters = "23456789tjqka";
        //private string suitLetters = "scdh";
        #endregion

        public readonly MersenneTwister _randomGenerator = new MersenneTwister();

        #region abstract propertys and methods...
        abstract public ACE_RANK AceRank { get; }
        abstract public JOKER Jokers { get; }
        abstract public card createCard(int _rank, int _suit);
        #endregion

        #region class property...
        public int Capacity;
        public Dictionary<int, card> Cards = new Dictionary<int,card>();
        public List<int> ShuffledCards = new List<int>();
        public List<int> FoldedCards = new List<int>();
        private int ShuffleCursor;
        

        public string name { get { return this.GetType().Name; } }
        #endregion

        public Deck()
        {
            if (this.AceRank == ACE_RANK.low)
            {
                Dictionary<int, rank> tmpRanks = new Dictionary<int, rank>();
                tmpRanks.Add(0, this.RegistredRanks[12]); tmpRanks[0].id = 0; tmpRanks[0].PrimeValue = this.RegistredRanks[12].PrimeValue;
                for (int i = 0; i< this.RegistredRanks.Count-1; i++) {
                    tmpRanks.Add(i + 1, this.RegistredRanks[i]); tmpRanks[i + 1].id = i + 1;
                    tmpRanks[i + 1].PrimeValue = this.RegistredRanks[i + 1].PrimeValue;
                }
                this.RegistredRanks = tmpRanks;
                
            }
            for (int cy = 0; cy < this.RegistredSuits.Count; cy++)
            {
                for (int cx = 0; cx < this.RegistredRanks.Count; cx++)
                {
                    //int _id = cx + cy * this.RegistredRanks.Count;
                    card _card = this.createCard(cx, cy);
                    if (_card == card.Empty) continue;

                    this.Cards.Add(_card.ID, _card);
                    this.ShuffledCards.Add(_card.ID);
                    this.Cards.Last().Value.posInDesc = this.ShuffledCards.Count - 1;
                }
            }
            this.Capacity = this.ShuffledCards.Count;
            this.ShuffleCursor = this.ShuffledCards.Count - 1;
        }

        //вспомогательная функция для обмена элементов массива местами.
        private void SwapCards(int index1, int index2)
        {
            int tmpValue = this.ShuffledCards[index1]; 
            this.ShuffledCards[index1] = this.ShuffledCards[index2]; 
            this.ShuffledCards[index2] = tmpValue;
            this.Cards[this.ShuffledCards[index1]].posInDesc = index1;
            this.Cards[this.ShuffledCards[index2]].posInDesc = index2;
        }
        
        /// <summary>тасовка карт</summary>
        public void Shuffle()
        {
            this.FoldedCards.Clear();
            this.ShuffleCursor = this.ShuffledCards.Count - 1;
        }
        
        #region remove & getCard function...

        public card getCard(int _id)
        {
            if (this.Cards[_id].posInDesc > this.ShuffleCursor) throw new deckException("Запрошенная карта уже выдана!"); //return card.Empty;
            this.SwapCards(this.Cards[_id].posInDesc, this.ShuffleCursor);
            return this.Cards[this.ShuffledCards[this.ShuffleCursor--]];
        }
        public void setCard(int _id)
        {
            if (!this.Cards.ContainsKey(_id)) return;
            int _sid = this.Cards[_id].posInDesc;
            if (_sid <= this.ShuffleCursor) return;
            this.SwapCards(_sid, ++this.ShuffleCursor);
        }
        private void foldedToDesc()
        {
            if (this.FoldedCards.Count == 0) throw new deckException("В колоде кончились все карты!");
            foreach (var f_id in this.FoldedCards) this.setCard(f_id);
            this.FoldedCards.Clear();
        }
        
        public List<card> getCard(List<int> _ids)
        {
            List<card> _list = new List<card>();
            foreach (int _id in _ids) {
                card _card = this.getCard(_id);
                if (_card != card.Empty) _list.Add(_card);
            }
            return _list;
        }
        public List<card> getCard(IEnumerable<card> cards)
        {
            List<card> _list = new List<card>();
            cards.ToList().ForEach(_el => _list.Add(this.getCard(_el.ID)));

            return _list;
        }
        public List<card> getCard(string cardsString)
        {
            List<card> _list = new List<card>();
            List<card> _fcards = this.parseCardString(cardsString);
            
            foreach (card _c in _fcards)
            {
                card _card = this.getCard(_c.ID);
                if (_card != card.Empty) _list.Add(_card);
            }
            if (_list.Count == 0) return _list;
            return _list;
        }
        public card getCardRandom(IEnumerable<card> _cards)
        {
            var cards = _cards.Where(_el => this.Cards[_el.ID].posInDesc <= this.ShuffleCursor).ToList();
            if (_cards.Count() <= 0) throw new deckException("getCardRandom: ошибка генерации!");
            
            int _id = this._randomGenerator.Next(cards.Count());
            return this.getCard(cards[_id].ID);
        }
        public IEnumerable<card> getCards(int cardsCount)
        {
            return this.getCards(cardsCount, Enumerable.Empty<card>());
        }
        public IEnumerable<card> getCards(int cardsCount, IEnumerable<card> reservedCards)
        {
            List<card> _list = new List<card>();
            while (_list.Count < cardsCount)
            {
                if (this.ShuffleCursor < 0) this.foldedToDesc();
                int _id = this._randomGenerator.Next(this.ShuffleCursor + 1);
                if (reservedCards.Contains(this.Cards[this.ShuffledCards[_id]])) continue;
                
                _list.Add(this.getCard(this.ShuffledCards[_id]));
            }
            return _list;
        }

        /// <summary> выданные карты </summary>
        public IEnumerable<card> PickedCards 
        {
            get
            {
                return this.Cards.Select(_el => _el.Value).Where(_el => _el.posInDesc > this.ShuffleCursor).ToList();
            }
        }
        /// <summary>оставшиеся в колоде карты</summary>
        public IEnumerable<card> notPickedCards
        {
            get
            {
                return this.Cards.Select(_el => _el.Value).Where(_el => _el.posInDesc <= this.ShuffleCursor).ToList();
            }
        }

        #endregion

        #region parse vars & functions...
        private Dictionary<string, List<card>> _cacheParsingString = new Dictionary<string, List<card>>();

        public rank parseRank(char _rankChar) //символ уже должен быть в lowcase
        {
            var q = this.RegistredRanks.Where(_rank => _rank.Value.letter == _rankChar.ToString());
            return q.Count() == 1 ? q.First().Value : rank.Empty;
        }
        public suit parseSuit(char _suitChar) //символ уже должен быть в lowcase
        {
            var q = this.RegistredSuits.Where(_suit => _suit.Value.letter == _suitChar.ToString());
            return q.Count() == 1 ? q.First().Value : suit.Empty;
        }
        public card parseCard(string _card) //строка уже должна быть в lowcase
        {
            if (_card.Length != 2) return card.Empty;
            
            rank _r = this.parseRank(_card[0]);
            suit _s = this.parseSuit(_card[1]);
            
            return this.Cards.Where(_el => _el.Value.Rank == _r && _el.Value.Suit == _s)
                             .Select(_el=>_el.Value).FirstOrDefault();
        }
        public List<card> parseCardString(String cardString)
        {
            cardString = cardString.ToLower().Replace(" ", "");
            if (cardString == string.Empty) return new List<card>();
            if (cardString != string.Empty && this._cacheParsingString.ContainsKey(cardString)) 
                return this._cacheParsingString[cardString];

            List<card> _list = new List<card>();
            RegexOptions option = RegexOptions.IgnoreCase | RegexOptions.Compiled;
            
            if (Regex.IsMatch(cardString, @"[^\sakqjt2-9scdh]", option)) 
                throw new deckException(String.Format("\"{0}\" содержит недопустимые символы!", cardString));


            foreach (Match _el in Regex.Matches(cardString, @"([akqjt2-9][scdh])", option))
                _list.Add(this.parseCard(_el.Groups[0].Value));

            if (Regex.Replace(cardString, @"([akqjt2-9][scdh])", "").Count() > 0)
                throw new deckException(String.Format("\"{0}\" содержит ошибки!", cardString));
            
            this._cacheParsingString.Add(cardString, _list);
            return _list;
        }

        #endregion
    
        #region fold functions...
        public void fold(int _cId)
        {
            if (!this.Cards.ContainsKey(_cId)) throw new deckException("Карты с таким ID не существует!");
            if (this.Cards[_cId].posInDesc <= this.ShuffleCursor) throw new deckException("Неудачная попытка сфолдить карту!");
            this.FoldedCards.Add(_cId);
        }
        public void fold(card _c)
        {
            this.fold(_c.ID);
        }
        public void fold(List<int> _cList)
        {
            foreach (var _cId in _cList) this.fold(_cId);
        }
        public void fold(List<card> _cList)
        {
            foreach (var _cId in _cList) this.fold(_cId);
        }
        public void fold(string cardsString)
        {
            foreach (var _c in this.parseCardString(cardsString)) this.fold(_c);
        }
        #endregion
    }



    class deck52CardsAceLow : Deck
    {
        public override ACE_RANK AceRank { get { return ACE_RANK.low; }}
        public override JOKER Jokers { get { return JOKER.no; } }

        public override card createCard(int _rank, int _suit)
        {
            int _id = _rank + _suit * this.RegistredRanks.Count;


            /// <summary>
            ///   This routine initializes the deck.  A deck of cards is
            ///   simply an integer array of length 52 (no jokers).  This
            ///   array is populated with each card, using the following
            ///   scheme:
            ///
            ///   An integer is made up of four bytes.  The high-order
            ///   bytes are used to hold the rank bit pattern, whereas
            ///   the low-order bytes hold the suit/rank/prime value
            ///   of the card.
            ///
            ///   +--------+--------+--------+--------+
            ///   |xxxbbbbb|bbbbbbbb|cdhsrrrr|xxpppppp|
            ///   +--------+--------+--------+--------+
            ///
            ///   p = prime number of rank (deuce=2,trey=3,four=5,five=7,...,ace=41)
            ///   r = rank of card (deuce=0,trey=1,four=2,five=3,...,ace=12)
            ///   cdhs = suit of card
            ///   b = bit turned on depending on rank of card
            ///   
            ///  if Ace if low : p = prime number of rank (ace=2,two=3,trey=5,...,king=41)
            ///                  r = rank of card (acw=0,deuce=1,trey=2,four=3,...,king=12)
            /// </summary>
            int _maskCard = this.RegistredRanks[_rank].PrimeValue | (_rank << 8) | 0x1000 << _suit | (1 << (16 + _rank));

            //if (_rank >= 0 && _rank <= 3) return card.Empty;
            return new card { ID = _id, Rank = this.RegistredRanks[_rank], Suit = this.RegistredSuits[_suit], bitMask = (uint)_maskCard };
        }
    }

    class deckException : Exception
    {
        public deckException(String message) : base(message) { }
    }
}
