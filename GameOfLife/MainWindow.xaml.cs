using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TestWpfApp
{

    public partial class MainWindow : Window
    {
        bool[,] matrixMap = new bool[12, 12];
        readonly Button[,] buttons = new Button[12, 12];
        int delay = 1000;
    

        public MainWindow()
        {
            InitializeComponent();

            for (int i = 0; i < PlayGround.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < PlayGround.ColumnDefinitions.Count; j++)
                {
                    matrixMap[i, j] = false;
                }
            }

            for (int row = 0; row < PlayGround.RowDefinitions.Count; row++)
            {
                for (int col = 0; col < PlayGround.ColumnDefinitions.Count; col++)
                {
                    Button button = new()
                    {
                        Name = "Button" + row + "_" + col,
                        Background = Brushes.Silver,
                    };
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                    buttons[row, col] = button;
                    button.Click += Button_Paint;

                    PlayGround.Children.Add(button);

                }
            }

        }
        private void Button_Paint(object sender, RoutedEventArgs e)
        {
            Button? clickedButton = sender as Button;
            int row = Grid.GetRow(clickedButton);
            int column = Grid.GetColumn(clickedButton);

            if (matrixMap[row, column])
            {
                matrixMap[row, column] = false;
            }
            else
            {
                matrixMap[row, column] = true;
            }


            Display_Map();

        }


        private void Display_Map()
        {
            for (int row = 0; row < 12; row++)
            {
                for (int col = 0; col < 12; col++)
                {
                    if (matrixMap[row, col] == false)
                    {
                        buttons[row, col].Background = Brushes.Silver;
                    }
                    else
                    {
                        buttons[row, col].Background = Brushes.Black;
                    }
                }
            }
        }

        private int CountAliveNeighbors(int x, int y)
        {
            int count = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    if (GetCellState(x + i, y + j))
                        count++;
                }
            }
            return count;
        }
        public bool GetCellState(int x, int y)
        {
            if (x < 0 || x >= PlayGround.RowDefinitions.Count || y < 0 || y >= PlayGround.ColumnDefinitions.Count)
                return false;
            return matrixMap[x, y];
        }
        public void Generation()
        {
            bool[,] newCells = new bool[PlayGround.RowDefinitions.Count, PlayGround.ColumnDefinitions.Count];

            for (int x = 0; x < PlayGround.RowDefinitions.Count; x++)
            {
                for (int y = 0; y < PlayGround.ColumnDefinitions.Count; y++)
                {
                    int neighbors = CountAliveNeighbors(x, y);
                    if (matrixMap[x, y])
                    {
                        newCells[x, y] = (neighbors == 2 || neighbors == 3);
                    }
                    else
                    {
                        newCells[x, y] = neighbors == 3;
                    }
                }
            }

            matrixMap = newCells;
        }
        private bool isRunning = false;
        private async void Run_Game()
        {
            isRunning = true;
            await Task.Run(() =>
            {
                while (isRunning)
                {
                    Dispatcher.Invoke(() =>
                    {

                        Generation();
                        Display_Map();

                    });
                    Thread.Sleep(delay);

                }
            });
        }
        #region Buttons_Click
        private void Button_Click_Stop(object sender, RoutedEventArgs e)
        {
            isRunning = false;
        }
        private void Button_Click_Reset(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    matrixMap[i, j] = false;
                }
            }
            isRunning = false;
            Display_Map();
        }

        private void Button_Click_Run(object sender, RoutedEventArgs e)
        {
            Run_Game();
        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            delay = (int)slider.Value;
        }
        private void Button_Click_OneStep(object sender, RoutedEventArgs e)
        {
            Generation();
            Display_Map();
        }
        #endregion
    }
}

