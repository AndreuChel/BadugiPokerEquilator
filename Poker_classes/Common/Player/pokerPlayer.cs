using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Cards.Poker_classes.Common.Table;
using Cards.Poker_classes.utils;

namespace Cards.Poker_classes.Common.Player
{
    class pokerPlayer : IStream
    {
        public static pokerPlayer Empty { get { return null; } }
        
        private static int _counter = 0;
        public readonly int id = pokerPlayer._counter++;
        public readonly String Name;
        public double Bankroll { get; private set; }
        
        public Dictionary<int, pokerPlayerHelper> playedTables = new Dictionary<int, pokerPlayerHelper>();

        public pokerPlayer(double chips) { this.Bankroll = chips; }
        public pokerPlayer(String _name, double chips) : this(chips) { this.Name = _name; }
        
        public resultType SendToTable(pokerTable pt, pokerPlayerMessageArgs _args)
        {
            return pt.onPlayerMessage(this, _args);
        }
        public resultType sitDown(pokerTable _pt, double _chips, String _hand= "", String _range = "")
        {
            if (_chips > this.Bankroll) _chips = this.Bankroll;
            if (_chips <= 0) return resultType.error;

            if (this.SendToTable(_pt, new trySitMessageArgs()) != resultType.ok) return resultType.error;

            pokerPlayerHelper _helper = _pt.game.getHelper(_pt, this);
            _helper.initHelper(_chips, _hand, _range);

            this.playedTables.Add(_pt.id, _helper);
            
            //подписка на сообщения от стола
            _pt.SubscribeToTableMessages(_helper.onTableMessage);
            
            return resultType.ok;
        }
        public void Exit(pokerTable _pt)
        {
            if (_pt == null || !this.playedTables.ContainsKey(_pt.id)) return;
            _pt.unSubscribeFromTableMessages(this.playedTables[_pt.id].onTableMessage);
            this.SendToTable(_pt, new exitTableMessageArgs());
        }

        public pokerPlayerHelper getHelper(pokerTable pT)
        {
            try { return this.playedTables[pT.id]; }
            catch (Exception)
            {
                throw new pokerPlayerException(String.Format("Игрок \"{0}\"не сидит за столом {1}", this.Name, pT.id));
            }
        }

        #region operations...
        public override int GetHashCode()
        {
            return this.id;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is pokerPlayer)) return false;
            return this.GetHashCode() == (obj as pokerPlayer).GetHashCode();
        }
        public static bool operator ==(pokerPlayer _p1, pokerPlayer _p2)
        {
            if (_p1 as object == null && _p2 as object == null) return true;
            if (_p1 as object != null) return _p1.Equals(_p2);
            return false;
        }
        public static bool operator !=(pokerPlayer _p1, pokerPlayer _p2)
        {
            return !(_p1 == _p2);
        }
        #endregion

        public override string ToString()
        {
            return this.Name != String.Empty ? this.Name : "id" + this.id.ToString();
        }

        #region IStream...
        private StreamEventHandler StreamPublic;
        public void StreamSubscribe(IStreamViewer _viewer)     { this.StreamPublic += _viewer.StreamEventHandler; }
        public void StreamUnSubscribe(IStreamViewer _viewer) { this.StreamPublic -= _viewer.StreamEventHandler; }
        public void toStream(object sender, streamEventType _evType, EventArgs e) 
        {
            if (this.StreamPublic != null) this.StreamPublic(sender, _evType, e);
        }
        #endregion
    }
}
