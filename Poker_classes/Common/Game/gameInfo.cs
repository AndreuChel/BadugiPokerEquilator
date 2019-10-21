using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cards.Poker_classes.Common.Game
{

    class gameInfo
    {
        public LimitType LimitType { get; set; }
        
        /// <summary>
        /// малый блаинд
        /// </summary>
        public double sBlind { get; set; }

        /// <summary>
        /// стартовая ставка на первых улицах
        /// это может быть или большим блаиндом или бринг-ин
        /// Bet - минимальный шаг для рейза
        /// </summary>
        public double Bet { get; set; }

        public double Bring { get; set; }

        /// <summary>
        /// стартовая ставка на поздних улицах
        /// обычно это удвоение
        /// bigBet - минимальный шаг для рейза
        /// </summary>
        public double bigBet { get; set; }

        /// <summary>
        /// анте
        /// </summary>
        public double ante { get; set; }

        /// <summary>
        /// Номер первого раунда с увеличенными ставками (bigBet)
        /// </summary>
        public int bigBetRound { get; set; }

        public double getMinBet(int roundNum)
        {
            return (this.bigBetRound > 0 && roundNum >= this.bigBetRound) ? this.bigBet : this.Bet;
        }
        public double getMinBring(int roundNum)
        {
            return (this.bigBetRound > 0 && roundNum >= this.bigBetRound) ? this.bigBet : this.Bring;
        }

        public override string ToString()
        {
            string res = "limit: ";
            res += (Bet>0 && bigBet>0) ? Bet.ToString()+"/"+bigBet.ToString()+(ante>0? " анте "+ante.ToString():"") : "";
            res += (Bring > 0 && bigBet > 0) ? Bring.ToString() + "/" + bigBet.ToString() + (ante > 0 ? " анте " + ante.ToString() : "") : "";
            return res;
        }
    }
}
