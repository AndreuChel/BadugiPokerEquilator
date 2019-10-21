using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace Cards.Poker_classes.utils.Iterator
{
    class sessionAsync : session
    {
        protected Task thisTask;
        protected CancellationTokenSource cancelToken;

        public sessionAsync(int _count)
            : base(_count) { }
        public override int Run()
        {
            if (this.thisTask != null && this.thisTask.Status == TaskStatus.Running) return 0;
            this.cancelToken = new CancellationTokenSource();
            this.thisTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    this.sessionIteration += (_sender, _e) =>
                    {
                        this.cancelToken.Token.ThrowIfCancellationRequested();
                    };
                    base.Run();
                }
                catch (OperationCanceledException) { }
                catch (Exception ex) 
                {
                    this.Stop(ex.Message);
                }
                        
            }, this.cancelToken.Token);


            return 0;
        }
        public override int Run(sessionIterationEventHandler _func)
        {
            this.sessionIteration += _func;
            return this.Run();
        }
        public override void Stop(String message = "")
        {
            if (this.thisTask != null && this.thisTask.Status == TaskStatus.Running)
            {
                this.cancelToken.Cancel();
                base.Stop(message);
            }
        }

        public TaskStatus Status { get { return this.thisTask.Status; } }
    }
}
