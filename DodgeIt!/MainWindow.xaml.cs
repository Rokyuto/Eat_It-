using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;

namespace EatIt_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DispatcherTimer GameTimer = new DispatcherTimer();
        Random rand = new Random();

        //Variables
        bool Is_GameStarted = false;

        int playerScore;

        bool pCircle_MoveLeft, pCircle_MoveRight;
        int pCircle_SideMove = 5;

        bool pCircle_Jump;
        int pCircle_Gravity = 5;
        int pCircle_Force;

        int Feed_xLoc, Feed_yLoc;
        int Enemy_xLoc, Enemy_yLoc;

        int pCircle_Radius = 25;
        int FeedCircle_Radius;
        int EnemyCircle_Radius;

        int pCircle_xCenter, pCircle_yCenter;
        int FeedCircle_xCenter, FeedCircle_yCenter;
        int EnemyCircle_xCenter, EnemyCircle_yCenter;

        Image FeedCircle;
        Image EnemyCircle;

        int SpaceBox_Size;
        int SpaceBoxes_xQuantity, SpaceBoxes_yQuantity;
        int[] SpaceBoxes_Arr;
        List<int> SpaceBoxes_ArrList;

        int GeneratedSpaceBox;

        float BoxToColumnRatio_float;
        int BoxToColumnRatio_int;
        float SpaceBox_Column_ID; //SpaceBox Column ID

        double BoxToRowIndex_int; //SpaceBox Row ID

        int SpaceBox_ID_Max_xCoord, SpaceBox_ID_Min_xCoord;
        int SpaceBox_ID_Max_yCoord, SpaceBox_ID_Min_yCoord;

        int FeedCircle_Get_xLoc, FeedCircle_Get_yLoc;

        float FeedCircle_ColumnRatio_float, FeedCircle_RowRatio_float;
        int FeedCircle_ColumnRatio_int, FeedCircle_RowRatio_int;

        int Blacklisted_x_SpaceBoxes = 1, Blacklisted_y_SpaceBoxes = 1;

        int Total_Blacklist_SpaceBoxes;

        int[] SpaceBox_Blacklist_Arr;
        List<int> SpaceBox_Blacklist_ArrList;

        int Busy_x_SpaceBox, Busy_y_SpaceBox;

        public MainWindow()
        {
            InitializeComponent();

            GameArea.Focus();

            GameTimer.Tick += GameLoop;
            GameTimer.Interval = TimeSpan.FromMilliseconds(20);
            GameTimer.Start();

            SpaceBox_Size = (pCircle_Radius * 2) + 10;

            int Application_Width = 1920;
            int Application_Height = 1080;

            SpaceBoxes_xQuantity = Application_Width / SpaceBox_Size; //32
            SpaceBoxes_yQuantity = Application_Height / SpaceBox_Size; //18

            int SpaceBoxes_TotalQuantity = SpaceBoxes_xQuantity * SpaceBoxes_yQuantity; //Calculate Total SpaceBox Quantity :: 576

            SpaceBoxes_Arr = new int[SpaceBoxes_TotalQuantity];
            
            TestLabel.Content = SpaceBoxes_TotalQuantity.ToString();

            SpaceBoxes_ArrList = new List<int>(); //SpaceBox array list declaration

            for (int i = 1; i <= SpaceBoxes_TotalQuantity; i++)
            {
                SpaceBoxes_ArrList.Add(SpaceBoxes_Arr[i-1] = i); //Fill SpaceBox ArrayList with index
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (Is_GameStarted == false)
            {
                if (e.Key == Key.Space || e.Key == Key.W || e.Key == Key.Up)
                {
                    pCircle_Jump = true;
                    pCircle_Force = pCircle_Gravity;

                    Is_GameStarted = true;
                    SpawnFeedCircle();
                }
            }
            if (e.Key == Key.Space || e.Key == Key.W || e.Key == Key.Up)
            {
                pCircle_Jump = true;
                pCircle_Force = pCircle_Gravity;
            }
            if(e.Key == Key.A || e.Key == Key.Left)
            {
                pCircle_MoveLeft = true;
            }
            if(e.Key == Key.D || e.Key == Key.Right)
            {
                pCircle_MoveRight = true;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A || e.Key == Key.Left)
            {
                pCircle_MoveLeft = false;
            }
            if (e.Key == Key.D || e.Key == Key.Right)
            {
                pCircle_MoveRight = false;
            }
        }

        private void GameLoop(object sender, EventArgs e)
        {
            pCircle_xCenter = (int)(Canvas.GetLeft(PlayerCircle) + pCircle_Radius);
            pCircle_yCenter = (int)(Canvas.GetTop(PlayerCircle) + pCircle_Radius);

            EatFeedCircle();

            if (pCircle_Jump == true)
            {
                Canvas.SetTop(PlayerCircle, Canvas.GetTop(PlayerCircle) - pCircle_Force);
                pCircle_Force -= 1;
            }

            if (pCircle_MoveLeft == true)
            {
                Canvas.SetLeft(PlayerCircle, Canvas.GetLeft(PlayerCircle) - pCircle_SideMove);
            }
            if (pCircle_MoveRight == true)
            {
                Canvas.SetLeft(PlayerCircle, Canvas.GetLeft(PlayerCircle) + pCircle_SideMove);
            }
        }

        private void SpawnFeedCircle()
        {
            FeedCircle = new Image();
            FeedCircle.Source = new BitmapImage(new Uri("Assets/FeedCircle.png", UriKind.Relative));

            FeedCircle.Stretch = Stretch.Fill;
            FeedCircle.Width = 30;
            FeedCircle.Height = 30;
            FeedCircle_Radius = (int)FeedCircle.Width / 2;

            Feed_xLoc = rand.Next((int)(Application.Current.MainWindow.Width * 0.1), (int)(Application.Current.MainWindow.Width * 0.9)); //Generate Random x Location
            Feed_yLoc = rand.Next((int)(Application.Current.MainWindow.Height * 0.1), (int)(Application.Current.MainWindow.Height * 0.85)); //Generate Random y Location

            Canvas.SetTop(FeedCircle, Feed_yLoc);
            Canvas.SetLeft(FeedCircle, Feed_xLoc);

            FeedCircle_Get_xLoc = (int)Canvas.GetLeft(FeedCircle);
            FeedCircle_ColumnRatio_float = (float)FeedCircle_Get_xLoc / 1920; //Find Busy xSpaceBoxes Quantity

            FeedCircle_Get_yLoc = (int)Canvas.GetTop(FeedCircle);
            FeedCircle_RowRatio_float = (float)FeedCircle_Get_yLoc / 1080; //Find Busy ySpaceBoxes Quantity

            FeedCircle_ColumnRatio_int = (int)FeedCircle_ColumnRatio_float;
            FeedCircle_RowRatio_int = (int)FeedCircle_RowRatio_float;

            if (FeedCircle_ColumnRatio_float > FeedCircle_ColumnRatio_int) // If FeedCircle is on 2 xSpaceBoxes
            {
                Blacklisted_x_SpaceBoxes++; //Blacklisted xSpaceBoxes are 2
                Busy_x_SpaceBox = ((FeedCircle_RowRatio_int - 1) * SpaceBoxes_xQuantity) + FeedCircle_ColumnRatio_int; // Final ID first xSpaceBox
            }
            else //Feed Circle is on 1 xSpaceBox
            {
                Blacklisted_x_SpaceBoxes = 1;
            }
            if (FeedCircle_RowRatio_float > FeedCircle_RowRatio_int) // If Feed Circle is on 2 ySpaceBoxes
            {
                Blacklisted_y_SpaceBoxes++; //Blacklisted ySpaceBoxes are 2
                Busy_y_SpaceBox = (Busy_x_SpaceBox - 1) + SpaceBoxes_xQuantity; // Final ID first ySpaceBox ; Busy_x_SpaceBox = Second xSpaceBox
            }
            else //Feed Circle is on 1 ySpaceBoxes
            {
                Blacklisted_y_SpaceBoxes = 1;
            }

            Total_Blacklist_SpaceBoxes = Blacklisted_x_SpaceBoxes * Blacklisted_y_SpaceBoxes;
            SpaceBox_Blacklist_Arr = new int[Total_Blacklist_SpaceBoxes];
            SpaceBox_Blacklist_ArrList = new List<int>();

            for (int Busy_SpaceBox = 0; Busy_SpaceBox < Total_Blacklist_SpaceBoxes; Busy_SpaceBox++)
            {
                if (Busy_SpaceBox < Blacklisted_x_SpaceBoxes)
                {
                    SpaceBox_Blacklist_ArrList.Add(SpaceBox_Blacklist_Arr[Busy_SpaceBox] = Busy_x_SpaceBox); //Fill Blacklist with Busy xSpaceBoxes
                    SpaceBoxes_ArrList.RemoveAt(Busy_x_SpaceBox);
                    Busy_x_SpaceBox++; // Second x SpaceBox
                }
                if (Busy_SpaceBox >= Blacklisted_x_SpaceBoxes)
                {
                    SpaceBox_Blacklist_ArrList.Add(SpaceBox_Blacklist_Arr[Busy_SpaceBox] = Busy_y_SpaceBox); //Fill Blacklist with Busy ySpaceBoxes
                    SpaceBoxes_ArrList.RemoveAt(Busy_y_SpaceBox);
                    Busy_y_SpaceBox++; // Second y SpaceBox
                }
            }

            FeedCircle_xCenter = (int)(Canvas.GetLeft(FeedCircle) + FeedCircle_Radius);
            FeedCircle_yCenter = (int)(Canvas.GetTop(FeedCircle) + FeedCircle_Radius);

            GameArea.Children.Add(FeedCircle);
        }

        private void EatFeedCircle()
        {
            int Circle_xDis = (int)Math.Pow(pCircle_xCenter - FeedCircle_xCenter, 2);
            int Circle_yDis = (int)Math.Pow(pCircle_yCenter - FeedCircle_yCenter, 2);
            int Circle_Dis = (int)Math.Sqrt(Circle_xDis + Circle_yDis); //Calculate Distance Between Circles

            int Circles_Radius_Total = (int)(pCircle_Radius + FeedCircle_Radius);

            if (Circle_Dis <= Circles_Radius_Total)
            {
                playerScore += 1;
                GameScore.Content = playerScore;

                GameArea.Children.Remove(FeedCircle);

                SpawnFeedCircle();
                SpawnEnemyCircle();
            }
        }

        private void SpawnEnemyCircle()
        {
            EnemyCircle = new Image();
            EnemyCircle.Source = new BitmapImage(new Uri("Assets/EnemyCircle.png", UriKind.Relative));

            EnemyCircle.Stretch = Stretch.Fill;
            EnemyCircle.Width = 30;
            EnemyCircle.Height = 30;
            EnemyCircle_Radius = (int)EnemyCircle.Width / 2;

            GeneratedSpaceBox = rand.Next(0, SpaceBoxes_Arr.Length); //Generate Random SpaceBox

            BoxToColumnRatio_float = (float)GeneratedSpaceBox / (float)SpaceBoxes_xQuantity;
            BoxToColumnRatio_int = (int)BoxToColumnRatio_float;
            SpaceBox_Column_ID = (BoxToColumnRatio_float - BoxToColumnRatio_int) * SpaceBoxes_xQuantity; //SpaceBox Final Column ID

            TestLabel.Content = SpaceBox_Column_ID.ToString();

            SpaceBox_ID_Max_xCoord = (int)SpaceBox_Column_ID * SpaceBox_Size; //Final Max x
            SpaceBox_ID_Min_xCoord = SpaceBox_ID_Max_xCoord - SpaceBox_Size; //Final Min x

            BoxToRowIndex_int = Math.Ceiling((double)BoxToColumnRatio_float); // SpaceBox Final Row ID

            SpaceBox_ID_Max_yCoord = (int)BoxToRowIndex_int * SpaceBox_Size; //Final Max y
            SpaceBox_ID_Min_yCoord = SpaceBox_ID_Max_yCoord - SpaceBox_Size; //Final Min y

            Enemy_xLoc = rand.Next(SpaceBox_ID_Min_xCoord, (int)(SpaceBox_ID_Max_xCoord - EnemyCircle.Width)); // Enemy Random x Location
            Enemy_yLoc = rand.Next(SpaceBox_ID_Min_yCoord, (int)(SpaceBox_ID_Max_yCoord - EnemyCircle.Width)); // Enemy Random y Location

            EnemyCircle_xCenter = (int)(Canvas.GetLeft(EnemyCircle) + EnemyCircle_Radius);
            EnemyCircle_yCenter = (int)(Canvas.GetTop(EnemyCircle) + EnemyCircle_Radius);
                                    
            Canvas.SetTop(EnemyCircle, Enemy_yLoc);
            Canvas.SetLeft(EnemyCircle, Enemy_xLoc);

            //------------------------------------------------ Debug
            Canvas.SetLeft(SpaceBox, SpaceBox_ID_Min_xCoord);
            Canvas.SetTop(SpaceBox, SpaceBox_ID_Min_yCoord);

            Canvas.SetTop(DebugDummyX, Enemy_yLoc);
            Canvas.SetLeft(DebugDummyX, Enemy_xLoc);

            Canvas.SetTop(DebugDummyY, Enemy_yLoc);
            Canvas.SetLeft(DebugDummyY, Enemy_xLoc);
            //------------------------------------------------ Debug

            GameArea.Children.Add(EnemyCircle);

            SpaceBoxes_ArrList.RemoveAt(GeneratedSpaceBox); //Remove Generated SpaceBox

        }
    }
}