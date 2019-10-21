using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Cards.Poker_classes;
using Cards.Poker_classes.Reports;
using Cards.Poker_classes.Common;
using Cards.Poker_classes.Common.Game;
using Cards.Poker_classes.utils.Iterator;
using Cards.Poker_classes.Common.Player;
using Cards.Poker_classes.Common.Table;
using Cards.Poker_classes.Common.DeckAndCards;
using Cards.Poker_classes.Games.Badugi;
using Cards.Poker_classes.Common.HandAndRange;
using Cards.Poker_classes.utils;

namespace Cards
{
    public partial class Form2 : Form
    {
        public int Counter = 0;
        sessionAsync startButtonSession;
        
        public Form2() { InitializeComponent(); }
        private void StartButton_Click(object sender, EventArgs e)
        {
            if (this.startButtonSession != null 
                && this.startButtonSession.Status == TaskStatus.Running) { this.startButtonSession.Stop(); return; }

            ReportList _reports = new ReportList<_rWinStatistic>(WinLossRadioButton.Text);
            if (PlayerActionsRadioButton.Checked) _reports = new ReportList<reportPlayerAction>(PlayerActionsRadioButton.Text);
            if (HandStatisticRadioButton.Checked) _reports = new ReportList<_rHandStatistic>(HandStatisticRadioButton.Text);

            try
            {
                IEnumerable<int> chekedPlayersIndex = PlayersPanel.Controls.OfType<CheckBox>()
                                .Where(_el => _el.Checked && _el.Name.Substring(2) == "Check")
                                .Select(_el => int.Parse(_el.Name[1].ToString()));
                int drawsCount = int.Parse(drawCountPanel.Controls.OfType<RadioButton>()
                                .First(_el => _el.Checked && _el.Name.Substring(0,4) == "draw").Text);
                int iterations = PlayerActionsRadioButton.Checked ? 50 : int.Parse(iterationsCount.Text);

                _reports.Add(String.Format("-- Запуск №{0}  ( количество обменов:{1} ) --\r\n", (this.Counter++).ToString(), drawsCount.ToString()));

                pokerTable pT = new pokerTable(new badugi(new gameInfo()), pokerTableType.max8, TableOption.noBets | TableOption.noRotate);
                pT.ActiveRoundCount = drawsCount;
                _reports.Add(new reportPlayerAction(pT));

                
                foreach (int _index in chekedPlayersIndex)
                {
                    TextBox _handControl = PlayersPanel.Controls.OfType<TextBox>().First(_el => _el.Name == "p" + _index.ToString() + "Hand");
                    TextBox _rangeControl = PlayersPanel.Controls.OfType<TextBox>().First(_el => _el.Name == "p" + _index.ToString() + "Range");
                    _handControl.ForeColor = Color.Black; _rangeControl.ForeColor = Color.Black;
                    
                    String _hand = _handControl.Text; String _range = _rangeControl.Text;

                    badugiHand bh; badugiRange br;
                    if (_hand!= String.Empty && !badugiHand.tryGet(_hand, out bh) && !badugiRange.tryGet(_hand, out  br))
                    {
                        _handControl.ForeColor = Color.Red; throw new myExeption("Стартовая рука содержит ошибки!");
                    }
                    if (_range != String.Empty && !badugiRange.tryGet(_range, out  br))
                    {
                        _rangeControl.ForeColor = Color.Red; throw new myExeption("Диапазон содержит ошибки!");
                    }

                    pokerPlayer pp = new pokerPlayer("№" + _index.ToString(), double.MaxValue);
                    pp.sitDown(pT, double.MaxValue, _hand, _range);

                    _reports.Add(new _rWinStatistic(pp));
                    _reports.Add(new _rHandStatistic(pp));
                }
                if (pT.Seats.Cast<pokerPlayer>()
                    .SelectMany(_pp => _pp.getHelper(pT).startHandObject.ReservedCards)
                    .GroupBy(v => v).Where(g => g.Count() > 1).Count() > 0)
                    throw new myExeption("Одна и таже карта содержится в разных стартовых руках!");
                

                this.startButtonSession = new sessionAsync(iterations);
                this.startButtonSession.Subscribe(pT);

                this.startButtonSession.sessionStart += (_sender, _e) =>
                {
                    Invoke(new Action(() =>
                    {
                        toolStripProgressBar1.Step = 1; toolStripProgressBar1.Value = 0;
                        toolStripProgressBar1.Visible = true;
                        (sender as Button).Text = "Cancel";
                        StatusBarLabel.Text = "Выполнение...";
                        statusStrip1.Update();
                    }));
                };
                
                int counter = 0; double delta = (double)iterations / 100; int cx = 1;
                int pStep = delta > 1 ? 1 : (int)((double)1 / delta);
                String statusString = String.Empty;
                
                this.startButtonSession.sessionIteration += (_sender, _e) =>
                {
                    Invoke(new Action(() => { if (++counter > cx * delta) { toolStripProgressBar1.Value += pStep;  cx++; } }));
                };

                this.startButtonSession.sessionFinish += (_sender, _e) =>
                {
                    Invoke(new Action(() =>
                    {
                        StatusBarLabel.Text = 
                            _e.Stopped ? 
                            (_e.Message!=String.Empty? "Ошибка! "+_e.Message: "Остановлено пользователем") 
                            : "Готово";
                        if (!(_e as sessionEventArgs).Stopped)
                            ResultWindow.Text = _reports.ToString() + "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\r\n\r\n" + ResultWindow.Text;

                        toolStripProgressBar1.Visible = false;
                        
                        (sender as Button).Text = "Start";
                        statusStrip1.Update();
                    }));

                };

                this.startButtonSession.Run();
            }
            catch (Exception ex)
            {
                StatusBarLabel.Text = "Ошибка! " + ex.Message;
            }
            
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            //подготовка массива хэшей рук
            sessionAsync preloader = new sessionAsync(1);
            preloader.sessionFinish += (_sender, _e) =>
            {
                Invoke(new Action(()=> {
                    tabControl1.Enabled = true;
                    StatusBarLabel.Text = ""; statusStrip1.Update();
                }));
                
            };
            preloader.Run((_sender, _e) => { badugiHandsHash.fillHashDictionary(); });

            StartRangeTextBox.Enter += (object _sender, EventArgs _e) => { (_sender as TextBox).ForeColor = Color.Black; };
            
            foreach (TextBox tb in PlayersPanel.Controls.OfType<TextBox>().Where(_el => Regex.IsMatch(_el.Name, @"p\dRange")))
            {
                tb.Enter += (object _sender, EventArgs _e) => {(_sender as TextBox).ForeColor = Color.Black; };
                tb.Validating += (object _sender, CancelEventArgs _e) =>
                {
                    badugiRange br;
                    if (!badugiRange.tryGet((_sender as TextBox).Text, out  br))
                        (_sender as TextBox).ForeColor = Color.Red;
                };
            }
            foreach (TextBox tb in PlayersPanel.Controls.OfType<TextBox>().Where(_el => Regex.IsMatch(_el.Name, @"p\dHand")))
            {
                tb.Enter += (object _sender, EventArgs _e) => { (_sender as TextBox).ForeColor = Color.Black; };
                tb.Validating += (object _sender, CancelEventArgs _e) =>
                {
                    badugiHand bh; badugiRange br;
                    if (!badugiHand.tryGet((_sender as TextBox).Text, out bh)
                        && !badugiRange.tryGet((_sender as TextBox).Text, out  br))
                        (_sender as TextBox).ForeColor = Color.Red;
                };
            }
        }
        private void CalcRangeButton_Click(object sender, EventArgs e)
        {
            try
            {
                badugiRange _rangeObj = badugiRange.get(StartRangeTextBox.Text);
                badugiStartHandRange sh = new badugiStartHandRange(_rangeObj);
                double procents = (double) sh.rangeHands.Count() * 100 / (double)270725;

                resultWindowTab2.Text += (resultWindowTab2.Text != String.Empty ? "\r\n" : "") +
                    String.Format("Попадание в диапазон [{0}]: {1}%", StartRangeTextBox.Text, procents.ToString("0.00"));

            }
            catch (Exception ex)
            {
                StartRangeTextBox.ForeColor = Color.Red;
                StatusBarLabel.Text = "Ошибка! " + ex.Message;
                return;
            }
        }
        private void clearButton_Click(object sender, EventArgs e)
        {
            ResultWindow.Text = "";
            ResultWindow.Update();
        }
        
        /*
        private sessionAsync _sess;
        private void button2_Click(object sender, EventArgs e)
        {
            //var bR = Cards.Poker_classes.Games.Badugi_new.badugiRange.get(testTextBox.Text);
            //ResultWindowTab3.Text += bR.ToString() + "\r\n";

            pokerTable pT = new pokerTable(new badugi(new gameInfo()), pokerTableType.max8, TableOption.noBets | TableOption.noRotate);
            
            pokerPlayer pp = new pokerPlayer("№1", double.MaxValue);
            pp.sitDown(pT, double.MaxValue, "As2c", "543-");
            
            pokerPlayer pp2 = new pokerPlayer("№2", double.MaxValue);
            pp2.sitDown(pT, double.MaxValue, "As3h", "k-");
            
            pokerPlayer pp3 = new pokerPlayer("№3", double.MaxValue);
            pp3.sitDown(pT, double.MaxValue, "432-,7654-", "k-");
            
            pokerPlayer pp4 = new pokerPlayer("№4", double.MaxValue);
            pp4.sitDown(pT, double.MaxValue, "", "k-");

            pokerPlayer pp5 = new pokerPlayer("№5", double.MaxValue);
            pp5.sitDown(pT, double.MaxValue, "", "k-");

            var bR = new startHandFactory(pT, 100);
            bR._generatingTask.sessionFinish += (_sender, _e) =>
            {
                Invoke(new Action(() =>
                {
                    //ResultWindowTab3.Text += String.Format("\r\nВремя: {0}мс\r\n", (_e as sessionEventArgs).ElapsedMilliseconds);
                }));
            };

            //return;
            
            foreach (var el in bR)
                ResultWindowTab3.Text += String.Format("\r\n{0}",
                    el.Aggregate(String.Empty, (__result, next) =>
                        __result += (__result != String.Empty ? "\t" : "") 
                        + String.Format("{0}: {1}", next.Key.Name, next.Value.ToString())));
                

            //ResultWindowTab3.Text += "\r\n" + bR[999999].Count;
            
            //bR.Stop();
            return;

            if (_sess != null && _sess.Status == TaskStatus.Running) { _sess.Stop(); return; }
            
            _sess = new sessionAsync(1);
            
            _sess.sessionStart += (_sender, _e) =>
            {
                Invoke(new Action(() =>
                {
                    toolStripProgressBar1.Step = 1; toolStripProgressBar1.Value = 0;
                    toolStripProgressBar1.Visible = true;
                }));

            };
            _sess.sessionIteration += (_sender, _e) =>
            {
                Invoke(new Action(() => 
                { 
                    ResultWindowTab3.Text += "."; ResultWindowTab3.Update();
                    toolStripProgressBar1.Value++; 
                }));
            };
             
            _sess.sessionFinish += (_sender, _e) =>
            {
                Invoke(new Action(() => 
                {
                    ResultWindowTab3.Text += " Time: " + (_e as sessionEventArgs).ElapsedMilliseconds.ToString()+"\r\n";
                }));
            };
            _sess.Run((_sender, _e) =>
            {
                badugiRange br = badugiRange.get("k-");
            });
        }
        */
    }

    class myExeption : Exception
    {
        public myExeption(String message) : base(message) { }
    }
}

