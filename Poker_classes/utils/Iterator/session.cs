using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Cards.Poker_classes.Reports;

namespace Cards.Poker_classes.utils.Iterator
{
    delegate void sessionStartEventHandler(object sender, sessionEventArgs e);
    delegate void sessionIterationEventHandler(object sender, sessionIterationEventArgs e);
    delegate void sessionErrorEventHandler(object sender, sessionErrorEventArgs e);
    delegate void sessionFinishEventHandler(object sender, sessionEventArgs e);

    interface ISessionSubscriber
    {
        void SessionSubscribe(session sesion);
        void SessionUnSubscribe(session sesion);
    }

    class session
    {
        private static int sessionID = 0;
        protected int _cursor = 0;
        protected Stopwatch _watch;
        
        public int Count { get; private set; }
        public int ID { get; private set; }

        public session(int _count) 
        { 
            this.Count = _count; 
            this.ID = session.sessionID++;
        }

        protected int iteration = 0;
        public bool nextIteration() 
        {
            if (this._cursor >= this.Count) return false;

            if (this._cursor == 0) this.onStart();

            sessionIterationEventArgs _arg = new sessionIterationEventArgs
            {
                sID = this.ID,
                iID = this.iteration++,
                Count = this.Count,
                Iteration = this._cursor++,
            };
            if (this.sessionIteration != null) this.sessionIteration(this, _arg);

            if (this._cursor >= this.Count)  { this.onFinish(); return false;  }
            return true;
        }
        virtual public void Stop(String message = "")
        {
            this.onFinish(true, message);
            this._cursor = this.Count;
        }

        private void onStart()
        {
            this._watch = new Stopwatch();
            sessionEventArgs _arg = new sessionEventArgs
            {
                sID = this.ID,
                Count = this.Count
            };
            if (this.sessionStart !=null) this.sessionStart(this, _arg);
            this._watch.Start();
        }
        private void onFinish(bool isStopped = false, string message = "")
        {
            this._watch.Stop();
            sessionEventArgs _arg = new sessionEventArgs
            {
                sID = this.ID,
                Count = this._cursor+1,
                ElapsedMilliseconds = (int)this._watch.ElapsedMilliseconds, 
                Message=message,
                Stopped = isStopped
            };
            if (this.sessionFinish!=null) this.sessionFinish(this, _arg);
        }

        virtual public int Run() { this._cursor = 0; while (this.nextIteration()); return (int)this._watch.ElapsedMilliseconds; }
        virtual public int Run(sessionIterationEventHandler _func)
        {
            this.sessionIteration += _func;
            this.Run();
            this.sessionIteration -= _func;
            return (int)this._watch.ElapsedMilliseconds; 
        }

        public event sessionIterationEventHandler sessionIteration;
        public event sessionStartEventHandler sessionStart;
        public event sessionFinishEventHandler sessionFinish;

        public void Subscribe(ISessionSubscriber obj)
        {
            obj.SessionSubscribe(this);
        }
        public void UnSubscribe(ISessionSubscriber obj)
        {
            obj.SessionUnSubscribe(this);
        }
        public void UnSubscribeAll()
        {
            this.sessionIteration = null;
            this.sessionStart = null;
            this.sessionFinish = null;
        }

        private List<report> _reports = new List<report>();
        public void AddReport(report _rep) { _reports.Add(_rep); }
        public String ReportsText {
            get { return String.Concat(_reports.Select(_el => _el.Text)); } 
        }

    }
    class sessionErrorEventArgs : EventArgs
    {
        public int sID { get; set; }
        public int Count { get; set; }
        public String Message { get; set; }
    }

    class sessionEventArgs : EventArgs
    {
        public int sID { get; set; }
        public int Count { get; set; }
        public bool Stopped {get; set; }
        public String Message { get; set; }
        public int ElapsedMilliseconds { get; set; }
    }
    class sessionIterationEventArgs : EventArgs
    {
        public int sID { get; set; }
        public int iID { get; set; }
        public int Count { get; set; }
        public int Iteration { get; set; }
    }
}
