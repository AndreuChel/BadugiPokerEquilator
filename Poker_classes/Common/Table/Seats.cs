using System;
using System.Collections; //.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cards.Poker_classes.Common.Player;

namespace Cards.Poker_classes.Common.Table
{
    class seats : IEnumerable
    {
        public pokerPlayer[] players { get; private set; }
        public int cursor { get; private set; }

        //public readonly pokerTableType tableType;
        private pokerTable parent;
        public int Capacity { get { return (int)this.parent.tableType; } }

        #region Конструкторы...
        private void init() { this.players = new pokerPlayer[(int)this.parent.tableType]; this.cursor = -1; }
        public seats(pokerTable _pt) { this.parent = _pt; this.init(); }
        #endregion

        #region Add & Remove functions...
        public resultType Add(int seatNum, pokerPlayer _pp)
        {
            if (_pp == null || !(_pp is pokerPlayer) ||
                seatNum < 0 || seatNum >= this.Capacity  ||
                this.players[seatNum] != pokerPlayer.Empty || this.players.Contains(_pp)) return resultType.error;
            
            this.players[seatNum] = _pp;
            this.cursor = seatNum;
            this.Count++;
            
            this.parent.sendMessage(new AddPlayerMessageArgs() { addedPlayer = _pp, seatNum = seatNum });

            return resultType.ok;
        }
        public resultType Add(pokerPlayer _pp)
        {
            for (int i = 0; i < this.players.Length; i++) 
                if (this.Add(i,_pp) == resultType.ok) return resultType.ok;
            return resultType.error;
            
        }
        public void Remove(pokerPlayer _pp)
        {
            //тут еще надо будет сделать свиг курсора (если удаляемый игрок на курсоре)
            int _index = this.indexOf(_pp);
            if (_index == -1) return;
            this.parent.sendMessage(new exitPlayerMessageArgs() { Player = _pp, seatNum = _index });
            this.players[_index] = pokerPlayer.Empty;
            this.Count--;
        }
        #endregion
        
        /// <summary>
        /// Индекс игрового места, где сидит игрок
        /// </summary>
        public int indexOf(pokerPlayer _pp)
        {
            for (int i = 0; i < this.Capacity; i++) if (this.players[i] == _pp) return i;
            return -1;
        }
        
        /// <summary>
        /// Нахождение текущей позиции игрока
        /// </summary>
        public int positionOf(pokerPlayer _pp) { return this.activePlayers.IndexOf(_pp); }
        /// <summary>
        /// Количество занятых мест
        /// </summary>
        public int Count { get; private set; }
        /// <summary>
        /// Получение игрока в текущей позиции
        /// </summary>
        public pokerPlayer this[int pos] { get { return this.activePlayers[pos % this.Count]; } }
        
        /// <summary>
        /// Сдиг сдающего на 1 по часовой стрелке
        /// </summary>
        public void RotateNext()
        {
            _cacheActivePlayers.Clear();
            for (int i = this.cursor + 1; i < this.Capacity + this.cursor + 1; i++)
            {
                int _index = i % this.Capacity;
                if (this.players[_index] != pokerPlayer.Empty) { this.cursor = _index; return; }
            }
        }

        #region реализация IEnumerable...
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public SeatsEnum GetEnumerator()
        {
            //if (this.cursor == -1) return ;
            return new SeatsEnum(this);
        }
        #endregion

        private List<pokerPlayer> _cacheActivePlayers = new List<pokerPlayer>();
        /// <summary>
        /// Массив не пустых мест, с учетом сдвига сдающего
        /// </summary>
        public List<pokerPlayer> activePlayers
        {
            get
            {
                if (_cacheActivePlayers.Count > 0) return _cacheActivePlayers;
                
                List<pokerPlayer> _res = new List<pokerPlayer>();
                foreach (pokerPlayer _el in this) _res.Add(_el);
                this._cacheActivePlayers = _res;
                return  _res;
            }
        }
    }

    class SeatsEnum : IEnumerator
    {
        private seats parent;
        public SeatsEnum(seats _parent) { this.parent = _parent; }

        private int position = -1;
        public bool MoveNext()
        {
            if (parent.cursor == -1) return false;

            while (++position < parent.Capacity)
                if (parent.players[(parent.cursor + this.position) % parent.Capacity] != pokerPlayer.Empty) return true;
            return false;
        }

        public void Reset()
        {
            this.position = -1;
        }
        public object Current
        {
            get
            {
                if (position == -1) return pokerPlayer.Empty;
                return parent.players[(parent.cursor + this.position) % parent.Capacity];
            }
        }
    }
}
