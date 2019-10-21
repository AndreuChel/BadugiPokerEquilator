using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cards.Poker_classes.Common
{
    enum resultType : int { ok = 1, error = -1, empty = 0 }
    enum Priority { high = 0, normal = 1, low = 2 }
    enum SignRange { None, allTheBest, allTheWorst, allBetween, withCards}
    enum handStatus : int { win = 1, tie = 0, loss = -1 }
    
    enum game
    {
        TexasHoldem, Omaha, OmahaHiLow, Omaha5Card, Omaha5CardHiLow,
        Courchevel, CourchevelHiLow, Stud, StudHiLow, Razz, FiveCardDraw,
        Draw27Triple, Draw27Single, Badugi
    }
    enum LimitType { noLimit, potLimit, fixedLimit };
    enum BetType { ante, sBlind, bBlind, bet, call, raise }
    
    //Common.Table
    enum pokerTableType : int { max2 = 2, max4 = 4, max6 = 6, max8 = 8, max9 = 9, max10 = 10 }
    enum TableOption : int { Empty = 0, noRotate = 0x1, noBets = 0x10, Blinds = 0x100, Bring = 0x1000, Ante = 0x10000 }

    //Common.DeckAndCards
    enum CardType { Hidden, FaceUp, Community }
    enum ACE_RANK { low = 0, high = 12 };
    enum JOKER { no = 0, two = 2 };

}
