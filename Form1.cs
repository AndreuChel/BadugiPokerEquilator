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
using System.Threading.Tasks;

using Cards.Poker_classes;
using Cards.Poker_classes.Common;
using Cards.Poker_classes.Common.Game;
using Cards.Poker_classes.utils.Iterator;
using Cards.Poker_classes.Common.Player;
using Cards.Poker_classes.Common.Table;
using Cards.Poker_classes.Common.DeckAndCards;
using Cards.Poker_classes.Games.Badugi;

namespace Cards
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            testRange();
            //newTableTest();
            //testNewBadugiEvaluator2();
            //testSession2();
            //testNewBadugiEvaluator();
        }
        public void testRange()
        {
            /*
            badugiHand test_bh = badugiHand.get("As2c3dKh");

            listBox1.Items.Add(badugiHand.getLowerBadugi(test_bh).ToString());
            return;
            */
            /*
            //badugiRange bh = new badugiRange("725+");
            badugiRange bR = new badugiRange(textBox1.Text);

            int res = 0, iCount = 1000000;
            session _s = new session(iCount);
            
            _s.sessionFinish += (sender, e) =>
            {
                double procents = Math.Round((double)res * 100 / (double)iCount , 2);
                listBox1.Items.Add("процент попадания в диапазон: "+procents.ToString()+"%");
                listBox1.Items.Add(String.Format("Затраченное время: {0}", e.ElapsedMilliseconds));
            };
            _s.Run((sender, e) =>
            {
                badugi.Deck.Shuffle();
                badugiHand bh = badugiHand.get(badugi.Deck.getCards(4));
                
                while (bh.Count > 0)
                {
                    if (bR.inRange(bh)) { res++; break; }
                    bh = badugiHand.getLowerBadugi(bh);
                }
            });
            */
        }


        public void newTableTest()
        {
            /*
            pokerGame _game = new badugi(
                new gameInfo
                {
                    LimitType = LimitType.fixedLimit,
                    sBlind = 0.15,
                    Bet = 0.25,
                    bigBet = 0.5,
                    ante = 0.01
                });

            pokerTable pT = new pokerTable(_game, pokerTableType.max8, TableOption.Ante | TableOption.Blinds);
            
            pT.dealerMessages += (sende, e) => { listBox1.Items.Add(e.ToString()); };
            
            pokerPlayer pp = new pokerPlayer(double.MaxValue);
            pokerPlayer pp2 = new pokerPlayer(double.MaxValue);
            pokerPlayer pp3 = new pokerPlayer(double.MaxValue);
            pokerPlayer pp4 = new pokerPlayer(double.MaxValue);
            pokerPlayer pp5 = new pokerPlayer(double.MaxValue);
            pokerPlayer pp6 = new pokerPlayer(double.MaxValue);
            pokerPlayer pp7 = new pokerPlayer(double.MaxValue);
            pokerPlayer pp8 = new pokerPlayer(double.MaxValue);

            pp.sitDown(pT, double.MaxValue);
            pp2.sitDown(pT, double.MaxValue);
            pp3.sitDown(pT, double.MaxValue);
            pp4.sitDown(pT, double.MaxValue);
            pp5.sitDown(pT, double.MaxValue);
            pp6.sitDown(pT, double.MaxValue);
            pp7.sitDown(pT, double.MaxValue);
            pp8.sitDown(pT, double.MaxValue);

            //session _s = new session(1000000);
            session _s = new session(1);
            _s.sessionFinish +=(sender,e)=>{
                listBox1.Items.Add(e.ElapsedMilliseconds.ToString());
            };
            _s += pT; _s.Run(); _s -= pT;;

            pp.Exit(pT);
            pp2.Exit(pT);
            pp3.Exit(pT);
            pp4.Exit(pT);
            pp5.Exit(pT);
            pp6.Exit(pT);
            pp7.Exit(pT);
            pp8.Exit(pT);
            */
        }

        public void testSession2()
        {
            /*
            playerStatus ps = new playerStatus();
            ps.roundBets += 10;
            ps.roundBets += 10;
            ps.roundBets = 0;
            ps.roundBets += 10;
            listBox1.Items.Add(ps.investment.ToString());
            return;
            */
            /*
            session _s = new session(1000000);

            _s.sessionFinish += (sender, e) =>
            {
                listBox1.Items.Add(String.Format("Затраченное время: {0}", e.ElapsedMilliseconds));
            };

            pokerGame _game = new badugi(
                new gameInfo
                {
                    LimitType = LimitType.fixedLimit,
                    sBlind = 0.15,
                    Bet = 0.25,
                    bigBet = 0.5
                });
            
            pokerTable pT = new pokerTable(_game, pokerTableType.max8, TableOption.noRotate | TableOption.Empty);

            pokerPlayer pp = new pokerPlayer(100);
            double test = pp.getChips(137);
            listBox1.Items.Add(test.ToString());

            pT += pp;
            
            pT += new pokerPlayer();
            pT += new pokerPlayer();
            pT += new pokerPlayer();
            
            pT += new pokerPlayer();
            pT += new pokerPlayer();
            pT += new pokerPlayer();
            pT += new pokerPlayer();

            pokerDealer _pM = new pokerDealer(pT);
            //_pM.nextRound();
            //_pM.fold(pp);

            //pT += new pokerPlayer();
            
            _s.Run((sender, e) => {
                pT.onNextIteration(sender, e);
                
                //string s = string.Empty;
                foreach (pokerPlayer el in pT.seats) ;
                    //s += (s == string.Empty ? "" : ", ") + el.id.ToString();
                //listBox1.Items.Add(s);
                  
            }); 
            */
            
            
            
        }


        public void testNewBadugiEvaluator2()
        {
            session _s = new session(1000000);
            //session _s = new session(1);
            _s.sessionFinish += (sender, e) =>
            {
                listBox1.Items.Add(e.ElapsedMilliseconds.ToString());
            };
            _s.Run((sender, e) => {
                badugi.Deck.Shuffle();
                badugiHand bh = badugiHand.get(badugi.Deck.getCards(4));
                badugiHand bh2 = badugiHand.get(badugi.Deck.getCards(4));
                /*
                badugiHand bh3 = badugiHand.get(badugi.Deck.getCards(4));
                badugiHand bh4 = badugiHand.get(badugi.Deck.getCards(4));

                badugiHand bh5 = badugiHand.get(badugi.Deck.getCards(4));
                badugiHand bh6 = badugiHand.get(badugi.Deck.getCards(4));

                badugiHand bh7 = badugiHand.get(badugi.Deck.getCards(4));
                badugiHand bh8 = badugiHand.get(badugi.Deck.getCards(4));
                */
                //if (bh >= bh2 && bh >= bh3 && bh >= bh4 && bh >= bh5 && bh >= bh6 && bh >= bh7 && bh >= bh8) res++;
            });
        }
        public void testNewBadugiEvaluator()
        {
            
            listBox1.Items.Clear();
            Stopwatch t = new Stopwatch();
            t.Start();
            int res = 0;
            int countIterations = 1000000;
            for (int cx = 0; cx < countIterations; cx++)
            {
                badugi.Deck.Shuffle();
                //badugiHand bh = badugiHand.get(badugi.Deck.getCard("Ts9c8d"));

                badugiHand bh = badugiHand.get(badugi.Deck.getCards(4));
                badugiHand bh2 = badugiHand.get(badugi.Deck.getCards(4));
                badugiHand bh3 = badugiHand.get(badugi.Deck.getCards(4));
                badugiHand bh4 = badugiHand.get(badugi.Deck.getCards(4));

                badugiHand bh5 = badugiHand.get(badugi.Deck.getCards(4));
                badugiHand bh6 = badugiHand.get(badugi.Deck.getCards(4));

                badugiHand bh7 = badugiHand.get(badugi.Deck.getCards(4));
                badugiHand bh8 = badugiHand.get(badugi.Deck.getCards(4));


                if (bh >= bh2 && bh >= bh3 && bh >= bh4 && bh >= bh5 && bh >= bh6 && bh >= bh7 && bh >= bh8) res++;
            }
            t.Stop();
              
            listBox1.Items.Add(t.ElapsedMilliseconds.ToString());
            listBox1.Items.Add(((double)res * 100 / countIterations).ToString() + "%");
            
        }
        
        public void testCardSet()
        {
            /*
            card c1 = new card() { ID = 1 };
            card c2 = new card() { ID = 2 };
            card c3 = new card() { ID = 3 };
            card c4 = new card() { ID = 4 };
            card c5 = new card() { ID = 5 };
            
            cardSet cs = new cardSet(4, c1);
            cardSet cs1 = cs + c2 + c3 + c3 + c5 - c1;
            listBox1.Items.Add(cs1.Count.ToString());
            */
        }
        public void testPokerHand()
        {
            /*
            listBox1.Items.Add(badugiHand.get("As2c3d7s").value.ToString());
            listBox1.Items.Add(badugiHand.get("As2c3d7s").value.ToString());
            
            listBox1.Items.Clear();
            for (int cx = 0; cx < 1000000; cx++)
            {
                badugi.Deck.Shuffle();
                badugiHand bh = badugiHand.get(badugi.Deck.getCards(4));
            }
            //var _values = badugiHandAnalyzer._cache.GroupBy(_el => _el.Value);
            //listBox1.Items.Add(_values.Count().ToString());
            listBox1.Items.Add(badugiHandAnalyzer._cache.Count().ToString());
            */ 
        }
        public void РасчетНаменаБадуги2()
        {
            /*
            progressBar1.Visible = true;
            progressBar1.Value = 0;
            int iterationCount = 1000000;
            int progressStep = (int)Math.Ceiling((double)iterationCount / 100);
            
            listBox1.Items.Clear();
            DeckOfCards dc = new deck52CardsAceLow();
            int resCount = 0;
            List<card> _handCards = dc.parseCardString("As2c");
            List<int>  _handBase = _handCards.Select(_el => _el.ID).ToList<int>();

            List<card> _handCards_opp2 = dc.parseCardString("7c6d5s");
            List<int> _handBase_opp2 = _handCards_opp2.Select(_el => _el.ID).ToList<int>();

            List<card> _handCards_opp3 = dc.parseCardString("JsTc9d8h");
            List<int> _handBase_opp3 = _handCards_opp2.Select(_el => _el.ID).ToList<int>();
            int badugi_opp3 = badugiHand.code(_handCards_opp3);
            


            //int goodDraw = badugiHand.code(_handCards_opp2);
            int goodDraw = badugiHand.code(dc.parseCardString(textBox1.Text));
            int drawCounts = Int16.Parse(textBox2.Text);
            for (int cx = 0; cx < iterationCount; cx++)
            {
                dc.Shuffle();
                
                List<card> _h = dc.getCard(_handBase);
                //List<card> _h_opp2 = dc.getCard(_handBase_opp2);
                //List<card> _h_opp3 = dc.getCard(_handBase_opp3);

                //опп1
                int badugi_opp1 = 0;
                for (int i = 1; i <= drawCounts; i++)
                {
                    card c1, c2;

                    c1 = dc.getCard();
                    c2 = _h.Count==2 ? dc.getCard() : card.Empty;
                    
                    List<card> _arr1 = new List<card>(); _arr1.AddRange(_h); _arr1.Add(c1);
                    List<card> _arr2 = new List<card>(); if (c2 != card.Empty) { _arr2.AddRange(_h); _arr2.Add(c2); }
                    List<card> _arr3 = new List<card>(); _arr3.AddRange(_arr1); if (c2 != card.Empty) _arr3.Add(c2);
                    
                    int val3 = badugiHand.code(_arr3);
                    badugi_opp1 = val3;
                    if (val3 > 0x8000) break;

                    if (i < drawCounts && _h.Count == 2)
                    {
                        if (badugiHand.code(_arr1) >= goodDraw) { _h.Add(c1); continue; }
                        if (badugiHand.code(_arr2) >= goodDraw) { _h.Add(c2); continue; }
                    }
                    if (i == drawCounts && val3 >= goodDraw) break; 
                }
                
                //опп2
                int badugi_opp2 = badugiHand.code(_h_opp2);
                for (int i = 1; i <= drawCounts; i++)
                {
                    card c1;
                    c1 = dc.getCard();
                    List<card> _arr1 = new List<card>(); _arr1.AddRange(_h_opp2); _arr1.Add(c1);
                    if ((badugi_opp2 = badugiHand.code(_arr1)) > 0x8000) break;
                }
                  
                if (badugi_opp1 >= goodDraw) resCount++;
                //if (badugi_opp1 >= badugi_opp2 ) resCount++;
                //if (badugi_opp1 >= badugi_opp2 ) resCount++;

                if (cx % progressStep == 0) progressBar1.Value++;
            }
            
            double procents = Math.Round((double)resCount * 100 / (double)iterationCount, 2);
            listBox1.Items.Add("Результат: " + procents.ToString() + "% случаев. "+drawCounts.ToString()+" обмена");
            progressBar1.Visible = false;
            */
        }
        public void ШансыВыпаденияКомбинаций() {
            /*
            progressBar1.Visible = true;
            progressBar1.Value = 0;
            
            pokerGame game = new badugi();
            listBox1.Items.Clear();

            DeckOfCards dc = new deck52CardsAceLow();
            
            int resCount = 0;
            int iterationCount = 1000000;
            int progressStep = (int)Math.Ceiling((double)iterationCount / 100);

            //int maxValue = badugiHand.code(dc.parseCardString(textBox1.Text));
            badugiInfo maxValue = badugiInfo.get(badugiHand.code(dc.parseCardString(textBox1.Text)));

            badugiInfo minValue = badugiInfo.get(badugiHand.code(dc.parseCardString(textBox2.Text)));
            progressBar1.Maximum = 100;
            // KsQcJdTh QsJcTd9h JsTc9d8h Ts9c8d7h 9s8c7d6h 8s7c6d5h 7s6c5d4h 6s5c4d3h 5s4c3d2h 4s3c2dAh
            //int maxDraw = badugiHand.code(dc.parseCardString("QsJcTd9h"));

            for (int cx = 0; cx < iterationCount; cx++)
            {
                dc.Shuffle();
                card c; int cCount = 0;
                List<card> _hand = new List<card>();
                while ((c = dc.getCard()) != card.Empty && cCount<4)
                {
                    _hand.Add(c); cCount++;
                }
                badugiInfo bValue = badugiInfo.get(badugiHand.code(_hand));
                int bVal = bValue.suitCount <= maxValue.suitCount ? bValue.value : bValue.badugi[maxValue.suitCount];
                if ((bVal >= minValue.value) && (bVal <= maxValue.value)) resCount++;
                
                //if (bValue >= minValue.value && bValue <= maxValue.value) resCount++;
                 
                if (cx % progressStep == 0) progressBar1.Value++;
            }

            double procents = Math.Round((double)resCount * 100 / (double)iterationCount, 2);
            listBox1.Items.Add("Бадуги в диапозоне " + textBox2.Text + ".." + textBox1.Text + " приходят в " + procents.ToString() + "%");
            
            //uint _val = badugiHandAnalyzer.bitCount(Convert.ToUInt32(0x4fffffff));
            //listBox1.Items.Add(_val.ToString());
             
            progressBar1.Visible = false;
             */ 
        }
    }
          
}
