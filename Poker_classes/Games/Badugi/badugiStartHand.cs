using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cards.Poker_classes.Common;
using Cards.Poker_classes.utils;
using Cards.Poker_classes.Common.HandAndRange;
using Cards.Poker_classes.Common.DeckAndCards;


namespace Cards.Poker_classes.Games.Badugi
{
    class badugiStartHandCards : startHand
    {
        private IEnumerable<card> mainSet;
        private badugiHand Hand;
        public badugiStartHandCards(IEnumerable<card> cards)
        {
            this.Priority = Priority.normal;
            int hash = cardSet.getHash(cards);
            this.Hand = badugiHandsHash.Items[hash].Hand as badugiHand; //hash = this.Hand.BadugiCards.GetHashCode();

            this.mainSet = badugiHandsHash.Items.Where(_el => _el.Value.BadugiCards.GetHashCode() == hash)
            .SelectMany(_el => _el.Value.BadugiPickCards).Distinct()
            .Where(_el => !cards.Contains(_el))
            .ToList();

            /*
            this.mainSet = badugiHandsHash.Items.Where(_el => _el.Value.Value == this.Hand.value && _el.Key % hash == 0)
                .SelectMany(_el => _el.Value.BadugiPickCards).Distinct()
                .Where(_el => !cards.Contains(_el))
                .ToList();
             */ 
            
        }
        public override pokerHand generateHand(Deck deck, IEnumerable<card> reservedCards)
        {
            cardSet cs = new cardSet(4, deck.getCard(this.Hand.Cards));
            var _set = this.mainSet.Where(_el => !reservedCards.Contains(_el)).ToList();

            while (cs.Count() != 4)
            {
                var tCs = new cardSet(4, cs); tCs.Add(deck.getCardRandom(_set));
                if (badugiHandsHash.Items[tCs.GetHashCode()].Value == this.Hand.value) cs = tCs;
            }
            return badugiHandsHash.Items[cs.GetHashCode()].Hand;
            /*
            for (int i = 4 - cs.Count; i > 0; i--) cs.Add(deck.getCardRandom(_set));
            return badugiHandsHash.Items[cs.GetHashCode()].Hand;
            */
        }
        public override IEnumerable<card> ReservedCards
        {
            get { return this.Hand.Cards; }
        }
    }
    class badugiStartHandRange : startHand
    {
        public IEnumerable<pokerHand> rangeHands { get; private set; }
        public MersenneTwister _randomGenerator = new MersenneTwister();
        public badugiRange Range { get; private set; }
        
        public badugiStartHandRange(badugiRange range)
        {
            this.Priority = Priority.high;
            this.Range = range;
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
                int rnd = this._randomGenerator.Next(this.rangeHands.Count());
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
            return badugiHandsHash.Items[cardSet.getHash(deck.getCards(4, reservedCards))].Hand;
        }
    }

}
