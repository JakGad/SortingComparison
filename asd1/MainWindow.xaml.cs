using System;
using System.Threading;
using System.Windows;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace asd1
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread _calculationThread;
        private int _maxSize;
        private int _minSize;
        private calculation _problemSolver = new calculation();
        private int _step;

        public MainWindow()
        {
            InitializeComponent();
            _problemSolver.UpdateUI += UpdateValue;

            succeded.Visibility = Visibility.Hidden;
            _problemSolver.End += End;
            //_calculationThread.SetApartmentState(ApartmentState.STA);


            plotReset(plot);
            plotReset(plot2);
        }

        private void UpdateValue(object sender, calculation.DataConnection data)
        {
            Dispatcher.Invoke(() =>
                {
                    foreach (var val in data.Bubble) plot.Series[0].Values.Add(val);

                    foreach (var val in data.Insertion) plot.Series[1].Values.Add(val);

                    foreach (var val in data.Quick) plot.Series[2].Values.Add(val);

                    foreach (var val in data.BubbleElem) plot2.Series[0].Values.Add(val);

                    foreach (var val in data.InsertionElem) plot2.Series[1].Values.Add(val);

                    foreach (var val in data.QuickElem) plot2.Series[2].Values.Add(val);

                    calculationProgress.Value = data.ProcessedQuantity;
                }
            );
        }

        private void End(object sender, EventArgs arg)
        {
            Dispatcher.Invoke(() =>
                succeded.Text = "Obliczenia zakończone sukcesem!"
            );
        }


        private void plotReset(CartesianChart chart)
        {
            chart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "BubbleSort",
                    Values = new ChartValues<ObservablePoint>()
                },
                new LineSeries
                {
                    Title = "InsertionSort",
                    Values = new ChartValues<ObservablePoint>()
                },
                new LineSeries
                {
                    Title = "QuickSort",
                    Values = new ChartValues<ObservablePoint>()
                }
            };
        }


        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(MinBox.Text, out _minSize) || !int.TryParse(MaxBox.Text, out _maxSize) ||
                !int.TryParse(StepBox.Text, out _step))
            {
                MessageBox.Show("Wprowadź liczbę!");
                return;
            }

            if (_minSize < 0)
            {
                MessageBox.Show("Wprowadź liczbę nieujemną!");
                return;
            }

            if (_maxSize < 1 || _step < 1)
            {
                MessageBox.Show("Wprowadź liczbę dodatnią!");
                return;
            }


            _problemSolver.Stop = true;
            _problemSolver.UpdateUI -= UpdateValue;
            _problemSolver.End -= End;
            _problemSolver = new calculation();
            _problemSolver.UpdateUI += UpdateValue;
            _problemSolver.End += End;

            plotReset(plot);
            plotReset(plot2);

            succeded.Visibility = Visibility.Visible;
            succeded.Text = "Obliczenia rozpoczęte";
            calculationProgress.Value = 0;
            _calculationThread = new Thread(_problemSolver.Calculate);
            _calculationThread.IsBackground = true;

            calculationProgress.Maximum = _maxSize - _minSize + 1;
            _calculationThread.Start(new Tuple<int, int, int>(_minSize, _maxSize, _step));
        }
    }
}