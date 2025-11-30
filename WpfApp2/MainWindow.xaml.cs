using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int[,] matrix;
        bool isRunning = true;

        public MainWindow()
        {
            InitializeComponent();
            matrix = new int[20, 20];
            // BuildGrid();  //בנייה לפי אחוזים - אפשר לעשות מסך קלט
            PrintGrid();
            isRunning = false;
            //יש שני כפתורים או להתחיל לפי רצף או כפתור נקסט
        }
        public async void Start1()
        {
            for (int i = 0; i < 100; i++) //אפשר לעשות עד שנלחץ על עצור 
            {
                UpdateNextGeneration();
                PrintGrid();
                await Task.Delay(500); // מחכה שנייה בלי לחסום את ה־UI            
            }
        }
        public async void Start()
        {
            isRunning = !isRunning;
            if (isRunning) btnPlay.Content = "Stop"; else btnPlay.Content = "Start Run";
            while (isRunning) // לולאה אינסופית עד שיגיע Stop
            {
                UpdateNextGeneration();
                PrintGrid();
                await Task.Delay(500);
            }
        }

        public void BuildGrid()
        {
            Random random = new Random();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    int num = random.Next(1, 101);
                    if (num < 20)
                    {
                        matrix[i, j] = 1;
                    }
                    else
                    {
                        matrix[i, j] = 0;
                    }
                }
            }
        }
        public void PrintGrid()
        {
            grid1.Children.Clear();

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    Button btn = new Button
                    {
                        Background = (matrix[i, j] == 1 ? Brushes.Black : Brushes.White),
                        Tag = (i, j)  // שמירת המיקום בטאג
                    };

                    btn.Click += Cell_Click;  // כל הכפתורים מגיעים לאותו אירוע

                    //Grid.SetRow(btn, i);
                    //Grid.SetColumn(btn, j);

                    grid1.Children.Add(btn);
                }
            }
        }
        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            var (i, j) = ((int, int))btn.Tag; // שליפה מהמיקום ששמרנו

            // הופכים ערך במטריצה
            matrix[i, j] = (matrix[i, j] == 1 ? 0 : 1);

            // עדכון הצבע
            btn.Background = (matrix[i, j] == 1 ? Brushes.Black : Brushes.White);
        }
        //public void PrintGrid()
        //{
        //    grid1.Children.Clear();
        //    for (int i = 0; i < 20; i++)
        //    {
        //        for (int j = 0; j < 20; j++)
        //        {
        //            if (matrix[i, j] == 1)
        //            {
        //                grid1.Children.Add(new Button() { Background = Brushes.Black });  
        //            }
        //            else
        //            {
        //                grid1.Children.Add(new Button() { Background = Brushes.White });
        //            }
        //        }
        //    }

        //}

        private int CountNeighbors(int row, int col)
        {
            int counter = 0;

            //top and buttom row
            int tempRow = row - 1;
            for (int i = 0; i < 2; i++)
            {
                if (tempRow >= 0 && tempRow < matrix.GetLength(0))
                {
                    counter += matrix[tempRow, col];
                    if (col - 1 >= 0)
                        counter += matrix[tempRow, col - 1];
                    if (col + 1 < matrix.GetLength(1))
                        counter += matrix[tempRow, col + 1];
                }
                tempRow = row + 1;
            }
            //sides
            if (col - 1 >= 0)
                counter += matrix[row, col - 1];
            if (col + 1 < matrix.GetLength(1))
                counter += matrix[row, col + 1];
            return counter;
        }

        private int NextLife(bool isAlive, int numNeighbors)
        {
            if (!isAlive && numNeighbors == 3)
                return 1;
            if (isAlive && numNeighbors == 2 || numNeighbors == 3)
                return 1;

            return 0;
        }
        private bool IsAlive(int row, int col)
        {
            if (matrix[row, col] == 1)
                return true;
            return false;
        }
        public void UpdateNextGeneration()
        {
            int[,] tempMatrix = new int[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    tempMatrix[i, j] = NextLife(IsAlive(i, j), CountNeighbors(i, j));
                }
            }
            matrix = tempMatrix;
        }

        private void Next(object sender, RoutedEventArgs e)
        {
            UpdateNextGeneration();
            PrintGrid();
        }

        private void StartPlay(object sender, RoutedEventArgs e)
        {
            Start();
        }
    }
}
