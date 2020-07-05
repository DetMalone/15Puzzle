using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace _15
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        int[,] tiles = new int[6,6];

        public MainWindow()
        {
            InitializeComponent();

            winText.Visibility = Visibility.Hidden; 

            dispatcherTimer.Tick += new EventHandler(TimerTick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            var mixedArray = GetMixedArray(16);
            while (!Solvable(mixedArray))
                mixedArray = GetMixedArray(16);

            for (int i = 0; i < tiles.GetLength(0); i++)
                for (int j = 0; j < tiles.GetLength(1); j++)
                    if (i > 0 && i < 5 && j > 0 && j < 5)
                        tiles[i, j] = mixedArray[4 * (i - 1) + j - 1];
                    else tiles[i, j] = -1;

            foreach (Button tile in LayoutField.Children)
            {
                tile.Click += ButtonClick;

                tile.Uid = (string)tile.Content;
                var id = int.Parse(tile.Uid);
                var x = (id - 1) / 4 + 1;
                var y = (id - 1) % 4 + 1;
                tile.Content = tiles[x, y];

                if (tiles[x, y] == 0) tile.Visibility = Visibility.Hidden;
            }
        }
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (!Win())
            {
                var id = int.Parse(((Button)sender).Uid);
                var x = (id - 1) / 4 + 1;
                var y = (id - 1) % 4 + 1;
                for (int i = -1; i < 2; i += 2)
                {
                    if (tiles[x + i, y] == 0)
                    {
                        MoveTile(x, y, i, 0);
                        dispatcherTimer.Start();
                        countBlock.Text = UpdateCounterText(countBlock.Text);
                    }
                    if (tiles[x, y + i] == 0)
                    {
                        MoveTile(x, y, 0, i);
                        dispatcherTimer.Start();
                        countBlock.Text = UpdateCounterText(countBlock.Text);
                    }

                }
                if (Win())
                {
                    winText.Visibility = Visibility.Visible;
                    dispatcherTimer.Stop();
                }
            }
        }

        private void MoveTile(int x, int y, int dx, int dy)
        {
            tiles[x + dx, y + dy] = tiles[x, y];
            tiles[x, y] = 0;
            ((Button)LayoutField.Children[y + dy - 1 + 4 * (x + dx - 1)]).Content = tiles[x + dx, y + dy];
            LayoutField.Children[y + dy - 1 + 4 * (x + dx - 1)].Visibility = Visibility.Visible;
            LayoutField.Children[y - 1 + 4 * (x - 1)].Visibility = Visibility.Hidden;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            timeBlock.Text = UpdateCounterText(timeBlock.Text);
        }

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private string UpdateCounterText(string currentText)
        {
            return currentText.Split(' ')[0] + " " + 
                (int.Parse(currentText.Split(' ')[1]) + 1).ToString();
        }

        private int[] GetMixedArray(int length)
        {
            var random = new Random();
            var mixedArray = new int[length];
            for (int i = 0; i < mixedArray.Length; i++)
            {
                int j = random.Next(i + 1);
                if (j != i)
                    mixedArray[i] = mixedArray[j];
                mixedArray[j] = i;
            }
            return mixedArray;
        }

        private bool Solvable(int[] checkArray)
        {
            int parity = 0;
            for (int i = 0; i < checkArray.Length; i++)
            {
                if (checkArray[i] == 0) parity += (i - 1) / 4 + 1;
                else for (int j = i + 1; j < checkArray.Length; j++)
                        if (checkArray[i] > checkArray[j] && checkArray[j] != 0) parity++;
            }
            return parity % 2 == 0;
        }

        private bool Win()
        {
            var result = true;

            for (int i = 0; i < tiles.GetLength(0); i++)
                for (int j = 0; j < tiles.GetLength(1); j++)
                    if (i > 0 && i < 5 && j > 0 && j < 5 && tiles[i, j] !=0)
                        result &= 4 * (i - 1) + j == tiles[i, j];

            return result;
        }
    }
}
