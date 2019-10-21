using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cards.Poker_classes.utils
{
    enum streamEventType : int { incoming = 1, outgoing = -1 }
    delegate void StreamEventHandler (object sender, streamEventType _evType, EventArgs e);

    interface IStream
    {
        //Подписка, отписка
        void StreamSubscribe(IStreamViewer _viewer);
        void StreamUnSubscribe(IStreamViewer _viewer);
        
        //функция, которая публикует сообщения в стрим
        void toStream(object sender, streamEventType _evType, EventArgs e);
    }
    interface IStreamViewer
    {
        StreamEventHandler StreamEventHandler { get; }
    }
}
