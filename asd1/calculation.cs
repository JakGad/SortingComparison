using System;
using System.Collections.Generic;
using System.Diagnostics;
using LiveCharts.Defaults;

namespace asd1
{
    internal class calculation
    {
        private readonly List<ObservablePoint> _bubble = new List<ObservablePoint>();
        private readonly List<ObservablePoint> _bubbleElem = new List<ObservablePoint>();
        private readonly List<ObservablePoint> _insertion = new List<ObservablePoint>();
        private readonly List<ObservablePoint> _insertionElem = new List<ObservablePoint>();
        private int _processedQuantity;
        private readonly List<ObservablePoint> _quick = new List<ObservablePoint>();
        private readonly List<ObservablePoint> _quickElem = new List<ObservablePoint>();
        private readonly Random _rMachine = new Random();
        private long Elementary;
        public EventHandler<DataConnection> End;
        public bool Stop = false;
        public EventHandler<DataConnection> UpdateUI;

        private long GetExecutionTime(Action<int[]> sortingProcedure, int[] toSort)
        {
            var watch = Stopwatch.StartNew();
            sortingProcedure(toSort);
            watch.Stop();
            return watch.ElapsedMilliseconds;
        }

        private bool check(int[] tab, int offset, int val)
        {
            for (var i = 0; i < offset; i++)
                if (tab[i] == val)
                    return false;

            return true;
        }

        public void Calculate(object receiver)
        {
            var temp = 0;
            _bubble.Clear();
            _insertion.Clear();
            _quick.Clear();
            _bubbleElem.Clear();
            _insertionElem.Clear();
            _quickElem.Clear();
            UpdateUI?.Invoke(this,
                new DataConnection(0, _bubble, _insertion, _quick, _bubbleElem, _insertionElem, _quickElem));
            var receivedData = (Tuple<int, int, int>) receiver;
            for (var i = receivedData.Item1; i <= receivedData.Item2 && !Stop; i += receivedData.Item3)
            {
                var sort = new int[i];
                for (var j = 0; j < sort.Length; j++)
                {
                    var val = _rMachine.Next();
                    while (!check(sort, j, val))
                        val = _rMachine.Next();
                    sort[j] = val;
                }

                Elementary = 0;
                _bubble.Add(new ObservablePoint(i, GetExecutionTime(BubbleSort, (int[]) sort.Clone())));
                _bubbleElem.Add(new ObservablePoint(i, Elementary));
                Elementary = 0;
                _insertion.Add(new ObservablePoint(i, GetExecutionTime(InsertionSort, (int[]) sort.Clone())));
                _insertionElem.Add(new ObservablePoint(i, Elementary));
                Elementary = 0;
                _quick.Add(new ObservablePoint(i,
                    GetExecutionTime(tab => qSort(tab, 0, sort.Length - 1), (int[]) sort.Clone())));
                _quickElem.Add(new ObservablePoint(i, Elementary));


                if (100 * (i - receivedData.Item1) / (decimal) (receivedData.Item2 - receivedData.Item1) >
                    temp + 1 && !Stop)
                {
                    UpdateUI?.Invoke(this,
                        new DataConnection(i - receivedData.Item1, _bubble, _insertion, _quick, _bubbleElem,
                            _insertionElem, _quickElem));
                    _bubble.Clear();
                    _insertion.Clear();
                    _quick.Clear();
                    _bubbleElem.Clear();
                    _insertionElem.Clear();
                    _quickElem.Clear();
                    //Thread.Sleep(20);
                    temp++;
                }
            }

            if (!Stop)
                UpdateUI?.Invoke(this,
                    new DataConnection(receivedData.Item2 - receivedData.Item1 + 1, _bubble, _insertion, _quick,
                        _bubbleElem, _insertionElem, _quickElem));
            if (!Stop) End?.Invoke(this, null);
        }

        private void BubbleSort(int[] toSort)
        {
            var changed = true;
            for (var i = 1; i < toSort.Length && changed; i++)
            {
                changed = false;
                for (var j = 0; j < toSort.Length - i; j++)
                {
                    Elementary++;
                    if (toSort[j] > toSort[j + 1])
                    {
                        var temp = toSort[j];
                        toSort[j] = toSort[j + 1];
                        toSort[j + 1] = temp;
                        changed = true;
                    }
                }
            }
        }

        private void InsertionSort(int[] toSort)
        {
            for (var i = 1; i < toSort.Length; i++)
            {
                var j = i - 1;
                var Temp = toSort[i];
                while (j >= 0 && toSort[j] > Temp)
                {
                    Elementary++;
                    toSort[j + 1] = toSort[j];
                    j--;
                }

                toSort[j + 1] = Temp;
            }
        }

        private void qSort(int[] Tab, int p, int r)
        {
            if (p < r)
            {
                var q = Partition(Tab, p, r);
                qSort(Tab, p, q - 1);
                qSort(Tab, q + 1, r);
            }
        }

        private void swap(ref int a, ref int b)
        {
            var _temp = a;
            a = b;
            b = _temp;
        }

        private int Partition(int[] Tab, int p, int r)
        {
            var x = Tab[r];
            var i = p - 1;
            for (var j = p; j < r; j++)
            {
                Elementary++;
                if (Tab[j] <= x)
                {
                    i++;
                    swap(ref Tab[i], ref Tab[j]);
                }
            }

            swap(ref Tab[i + 1], ref Tab[r]);
            return i + 1;
        }

        public class DataConnection : EventArgs
        {
            public List<ObservablePoint> Bubble;
            public List<ObservablePoint> BubbleElem;
            public List<ObservablePoint> Insertion;
            public List<ObservablePoint> InsertionElem;
            public int ProcessedQuantity;
            public List<ObservablePoint> Quick;
            public List<ObservablePoint> QuickElem;


            public DataConnection(int processedQuantity, List<ObservablePoint> bubble, List<ObservablePoint> insertion,
                List<ObservablePoint> quick, List<ObservablePoint> bubbleElem, List<ObservablePoint> insertionElem,
                List<ObservablePoint> quickElem)
            {
                ProcessedQuantity = processedQuantity;
                Bubble = bubble;
                Insertion = insertion;
                Quick = quick;
                BubbleElem = bubbleElem;
                InsertionElem = insertionElem;
                QuickElem = quickElem;
            }
        }
    }
}