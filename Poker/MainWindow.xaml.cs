using System;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.Integration;
using System.Threading.Tasks;

/*
 File->new->Project->C#->WPF App "Poker"
 download cards.dll from https://onedrive.live.com/redir?resid=D69F3552CEFC21!74629&authkey=!AGaX84aRcmB1fB4&ithint=file%2cDll
 Solution->Add Existing Item Cards.dll (Properties: Copy to Output Directory=Copy If Newer)
 Add Project->Add Reference to System.Drawing, WindowsFormsIntegration, System.Windows.Forms, System.Windows.Forms.DataVisualizetion
     * 
     * * */
namespace Poker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int NumDealsPerGroup { get; set; } = 1;
        int hghtCard = 100;
        int wdthCard = 80;
        public MainWindow()
        {
            InitializeComponent();
            Width = 1100;
            Height = 800;
            WindowState = WindowState.Maximized;
            Title = "CardDist";
            this.Loaded += MainWindow_Loaded;
        }
        void AddStatusMsg(string msg, params object[] args)
        {
            if (_txtStatus != null)
            {
                // we want to read the threadid 
                //and time immediately on current thread
                var dt = string.Format("[{0}],{1,2},",
                    DateTime.Now.ToString("hh:mm:ss:fff"),
                    Thread.CurrentThread.ManagedThreadId);
                _txtStatus.Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        // this action executes on main thread
                        var str = string.Format(dt + msg + "\r\n", args);
                        _txtStatus.AppendText(str);
                        _txtStatus.ScrollToEnd();
                    }));
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.DataContext = this;
                var sp = new StackPanel() { Orientation = Orientation.Vertical };
                sp.Children.Add(new Label() { Content = "Card Dealing Program." });
                var spControls = new StackPanel() { Orientation = Orientation.Horizontal };
                sp.Children.Add(spControls);
                spControls.Children.Add(new Label() { Content = "nDeals" });
                var txtnDeals = new TextBox()
                {
                    Width = 100,
                    ToolTip=""
                };
                txtnDeals.SetBinding(TextBox.TextProperty, nameof(NumDealsPerGroup));
                spControls.Children.Add(txtnDeals);

                _txtStatus = new TextBox()
                {
                    IsReadOnly = true,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    IsUndoEnabled = false,
                    MaxHeight = 60,
                    HorizontalContentAlignment = HorizontalAlignment.Left
                };

                var dictHandValues = new Dictionary<string, int>(); // Hand value, cnt. Like "Pair" = 4
                foreach (var k in Enum.GetValues(typeof(PokerHand.HandValues)))
                {
                    dictHandValues[k.ToString()] = 0;
                }
                var chart = new Chart();
                //                chart.Height = 500;
                chart.Width = 200;
                chart.Dock = System.Windows.Forms.DockStyle.Fill;
                var chartArea = new ChartArea("ChartArea");
                chart.ChartAreas.Add(chartArea);
                chartArea.AxisX.Interval = 1;
                var wfh = new WindowsFormsHost();
                wfh.Height = 600;
                wfh.Child = chart;
                sp.Children.Add(wfh);

                chart.DataSource = dictHandValues;
                var series1 = new Series();
                series1.ChartType = SeriesChartType.Bar;
                chart.Series.Add(series1);
                series1.XValueMember = "Key";
                series1.YValueMembers = "Value";
                var canvas = new Canvas() { Height = hghtCard + 3 };
                sp.Children.Add(canvas);
                sp.Children.Add(_txtStatus);

                this.Content = sp;

                var deck = new Card[52];
                for (var suit = 0; suit < 4; suit++)
                {
                    for (var denom = 0; denom < 13; denom++)
                    {
                        var card = CardDeck.GetCard((CardDeck.Suit)suit, denom);
                        deck[suit * 13 + denom] = card;
                    }
                }
                for (int i = 0; i < PokerHand.HandSize; i++)
                {
                    var img = new Image()
                    {
                        Height = hghtCard
                    };
                    // add it to the canvas
                    canvas.Children.Add(img);
                    // set it's position on the canvas
                    Canvas.SetLeft(img, i * wdthCard);
                    //                    Canvas.SetTop(img, suit * (1 + hghtCard));
                }
                var numHands = 0;
                var rand = new Random(1);
                PokerHand hand = null;
                Action UpdateUI = () =>
                {
                    series1.ToolTip = $"Num deals = {numHands:n0}";
                    // draw the cards
                    for (int i = 0; i < PokerHand.HandSize; i++)
                    {
                        var img = (Image)canvas.Children[i];
                        img.Source = hand.Cards[i].bmpSource;
                    }
                    chart.DataBind();
                    _txtStatus.Text = hand.PokerValue().ToString();
                };
                Action<int> ShuffleAndDeal = (nDeals) =>
                 {
                     for (int i = 0; i < nDeals; i++)
                     {
                         numHands++;
                         // shuffle
                         for (int n = 0; n < 52; n++)
                         {
                             //get a random number 0-51
                             var tempNdx = rand.Next(52);
                             var tmp = deck[tempNdx];
                             deck[tempNdx] = deck[n];
                             deck[n] = tmp;
                         }
                         // deal
                         hand = new PokerHand(deck.Take(5).OrderBy(c => c).ToList());
                         var val = hand.PokerValue().ToString();
                         dictHandValues[val]++;
                     }
                 };

                var btnGo = new Button()
                {
                    Content="_Go",
                    Width = 20
                };
                spControls.Children.Add(btnGo);
                bool fIsGoing = false;
                btnGo.Click +=async (ob, eb) =>
                  {
                      fIsGoing = !fIsGoing;
                      if (fIsGoing)
                      {
                          while (fIsGoing)
                          {
                              await Task.Run(() =>
                              {
                                  ShuffleAndDeal(NumDealsPerGroup);
                              });
                              UpdateUI();
                          }
                      }
                  };
                this.MouseUp += (om, em) =>
                {
                    ShuffleAndDeal(1);
                    UpdateUI();
                };
                btnGo.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, null));
            }
            catch (Exception ex)
            {
                this.Content = ex.ToString();
            }
        }

        // http://www.math.hawaii.edu/~ramsey/Probability/PokerHands.html
        internal class PokerHand
        {
            public static int HandSize = 5;
            public enum HandValues
            {
                Nothing,
                Pair, // .047539
                TwoPair, // .021128
                ThreeOfAKind,
                Straight,
                Flush,
                FullHouse,
                FourOfAKind,
                StraightFlush,
                RoyalFlush
            }
            public List<Card> Cards;

            public PokerHand(List<Card> cards)
            {
                this.Cards = cards;
            }
            public HandValues PokerValue()
            {
                // using Linq is probably not the most efficient
                var value = HandValues.Nothing;
                var groupsBySuits = Cards.GroupBy(c => c.suit);
                var groupsByRank = Cards.OrderBy(c => c.denom).GroupBy(c => c.denom);
                if (groupsBySuits.Count() == 1) // only one suit. Flush See if it's a straight
                {
                    if (IsStraight(groupsByRank))
                    {
                        if (groupsByRank.First().Min().denom == 8) //it's a ten
                        {
                            value = HandValues.RoyalFlush;
                        }
                        else
                        {
                            value = HandValues.StraightFlush;
                        }
                    }
                    else
                    {
                        value = HandValues.Flush;
                    }
                }
                else
                {
                    // if the hand has a 3,5,6,9,J, then there are 5 groups by denom
                    // if there's a pair, then there will be 4 groups
                    // 2 pair: there will be 3
                    // if there's a triple, then there will be 2 (full house) or 3 groups
                    // 4 of a kind: 2 groups
                    switch (groupsByRank.Count())
                    {
                        case 5: // there are 5 groups of denoms, so can only be straigh
                            if (IsStraight(groupsByRank))
                            {
                                value = HandValues.Straight;
                            }
                            break;
                        case 4:
                            value = HandValues.Pair;
                            break;
                        case 3: // 2 pair or triple
                            {
                                var nn = groupsByRank.OrderByDescending(g => g.Count());
                                int nMaxCount = nn.First().Count();
                                if (nMaxCount == 3)
                                {
                                    value = HandValues.ThreeOfAKind;
                                }
                                else
                                {
                                    value = HandValues.TwoPair;
                                }
                            }
                            break;
                        case 2: // full house or 4 of a kind
                            {
                                var nn = groupsByRank.OrderByDescending(g => g.Count());
                                int nMaxCount = nn.First().Count();
                                if (nMaxCount == 4)
                                {
                                    value = HandValues.FourOfAKind;
                                }
                                else
                                {
                                    value = HandValues.FullHouse;
                                }
                            }
                            break;
                    }
                }
                return value;
            }

            private bool IsStraight(IEnumerable<IGrouping<int, Card>> groupsByRank)
            {
                bool isStraight = false;
                if (groupsByRank.Count() == HandSize)
                {
                    var first = groupsByRank.First().Min().denom;
                    var last = groupsByRank.Last().Max().denom;
                    if (first + HandSize - 1 == last)
                    {
                        isStraight = true;
                    }
                    else
                    { // special case: Ace low
                        if (last == 12) // Ace
                        {
                            int ndx = 0;
                            foreach (var g in groupsByRank)
                            {
                                var c = g.First().denom;
                                if (ndx++ != c)
                                {
                                    break;
                                }
                            }
                            if (ndx == HandSize)
                            {
                                isStraight = true;
                            }
                        }
                    }
                }

                return isStraight;
            }
        }

        public class Card : Image, IComparable
        {
            public CardDeck.Suit suit;
            public int denom; // 0-12
            public int Value => (int)suit * 13 + denom;// 0-51
            public BitmapSource bmpSource { get; private set; }
            /// <summary>
            /// Create a new card
            /// </summary>
            /// <param name="suit"></param>
            /// <param name="denom"> 12=A, 11=K, Q=10, J=9,... 0=2</param>
            /// <param name="bmpSource">can be null</param>
            public Card(CardDeck.Suit suit, int denom, BitmapSource bmpSource = null)
            {
                this.suit = suit;
                this.denom = denom;
                this.bmpSource = bmpSource;
            }
            public string GetDenomString()
            {
                var result = (denom + 2).ToString();
                switch (denom)
                {
                    case 9:
                        result = "J";
                        break;
                    case 10:
                        result = "Q";
                        break;
                    case 11:
                        result = "K";
                        break;
                    case 12:
                        result = "A";
                        break;

                }
                return result;
            }
            public override string ToString()
            {
                return $"{GetDenomString()} of {suit}";
            }

            public int CompareTo(object obj)
            {
                if (obj is Card)
                {
                    var c = (Card)obj;
                    if (c.suit == this.suit)
                    {
                        return c.denom.CompareTo(this.denom);
                    }
                    return ((int)c.suit).CompareTo((int)(this.suit));
                }
                throw new InvalidOperationException();
            }
        }
        public class CardDeck
        {
            public enum Suit
            {
                Clubs = 0,
                Diamonds = 1,
                Hearts = 2,
                Spades = 3
            }
            // put cards in 2 d array, suit, rank (0-12 => 2-A)
            public Card[,] _Cards;
            public BitmapSource[] _bitmapCardBacks;
            private static CardDeck _instance;

            public static int NumCardBacks => _instance._bitmapCardBacks.Length;

            public CardDeck()
            {
                _Cards = new Card[4, 13];
                var hmodCards = LoadLibraryEx("cards.dll", IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
                if (hmodCards == IntPtr.Zero)
                {
                    throw new FileNotFoundException("Couldn't find cards.dll");
                }
                // the cards are resources from 1 - 52.
                // here is a func to load an int rsrc and return it as a BitmapSource
                Func<int, BitmapSource> GetBmpSrc = (rsrc) =>
                {
                    // we first load the bitmap as a native resource, and get a ptr to it
                    var bmRsrc = LoadBitmap(hmodCards, rsrc);
                    // now we create a System.Drawing.Bitmap from the native bitmap
                    var bmp = System.Drawing.Bitmap.FromHbitmap(bmRsrc);
                    // we can now delete the LoadBitmap
                    DeleteObject(bmRsrc);
                    // now we get a handle to a GDI System.Drawing.Bitmap
                    var hbmp = bmp.GetHbitmap();
                    // we can create a WPF Bitmap source now
                    var bmpSrc = Imaging.CreateBitmapSourceFromHBitmap(
                        hbmp,
                        palette: IntPtr.Zero,
                        sourceRect: Int32Rect.Empty,
                        sizeOptions: BitmapSizeOptions.FromEmptyOptions());

                    // we're done with the GDI bmp
                    DeleteObject(hbmp);
                    return bmpSrc;
                };
                // now we call our function for the cards and the backs
                for (Suit suit = Suit.Clubs; suit <= Suit.Spades; suit++)
                {
                    for (int denom = 0; denom < 13; denom++)
                    {
                        // 0 -12 => 2,3,...j,q,k,a
                        int ndx = 1 + 13 * (int)suit + (denom == 12 ? 0 : denom + 1);
                        _Cards[(int)suit, denom] = new Card(suit, denom, GetBmpSrc(ndx));
                    }
                }
                //The card backs are from 53 - 65
                _bitmapCardBacks = new BitmapSource[65 - 53 + 1];
                for (int i = 53; i <= 65; i++)
                {
                    _bitmapCardBacks[i - 53] = GetBmpSrc(i);
                }
            }
            public static double MeasureRandomness(Card[] _cards)
            {
                var dist = 0.0;
                for (int suit = 0; suit < 4; suit++)
                {
                    for (int denom = 0; denom < 13; denom++)
                    {
                        var ndx = suit * 13 + denom;
                        int curval = _cards[ndx].Value;
                        dist += Math.Pow((ndx - curval), 2);
                    }
                }
                return dist;
            }
            /// <summary>
            /// Return a card
            /// </summary>
            /// <param name="nSuit"></param>
            /// <param name="nDenom">1-13 = A, 2,3,4,J,Q,K</param>
            /// <returns></returns>
            public static Card GetCard(Suit nSuit, int nDenom)
            {
                if (_instance == null)
                {
                    _instance = new CardDeck();
                }
                if (nDenom < 0 || nDenom > 12)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return _instance._Cards[(int)nSuit, nDenom];
            }

            internal static ImageSource GetCardBack(int i)
            {
                return _instance._bitmapCardBacks[i];
            }
        }

        public const int LOAD_LIBRARY_AS_DATAFILE = 2;
        private TextBox _txtStatus;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFileReserved, uint dwFlags);

        [DllImport("User32.dll")]
        public static extern IntPtr LoadBitmap(IntPtr hInstance, int uID);

        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);
    }

}
