using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cards.Poker_classes.Common
{
    class pokerGameException : Exception
    {
        public pokerGameException(string message) : base("Object pokerGame: " + message) { }
    }

    class pokerTableException : Exception
    {
        public pokerTableException(string message) : base("Object pokerTable: " + message) { }
    }
    class potManagerException : Exception
    {
        public potManagerException(string message) : base("Object potManager: " + message) { }
    }

    class pokerPlayerException : Exception
    {
        public pokerPlayerException(string message) : base("Object pokerPlayer: " + message) { }
    }



}
