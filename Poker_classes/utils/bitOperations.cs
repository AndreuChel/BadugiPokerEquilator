using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cards.Poker_classes.utils
{

    class bitOperations
    {
        public static uint bitCount(uint _arg)
        {
            _arg = (_arg & 0x55555555) + ((_arg >> 1) & 0x55555555);
            _arg = (_arg & 0x33333333) + ((_arg >> 2) & 0x33333333);
            _arg = (_arg + (_arg >> 4)) & 0x0F0F0F0F;
            _arg += _arg >> 8;
            return (_arg + (_arg >> 16)) & 0x3F;
        }
        public static uint highBit(uint _arg)
        {
            //оставляем только старший бит в маске
            _arg |= _arg >> 1; _arg |= _arg >> 2; _arg |= _arg >> 4; _arg |= _arg >> 8; _arg |= _arg >> 16;
            return _arg - (_arg >> 1);
        }
    }
}
