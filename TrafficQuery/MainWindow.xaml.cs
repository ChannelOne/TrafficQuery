using System;
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
using TrafficQuery.Models;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Globalization;

namespace TrafficQuery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string DataFileName = "RouteData.xml";
        public const uint INF = uint.MaxValue;
        private List<Node> nodes;
        private List<Line> lines;

        public int Length
        {
            get { return nodes.Count; }
        }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
             nodes = LoadXmlData();
        }

        private List<Node> LoadXmlData()
        {
            uint counter = 0, lineCounter = 0;
            var nodes = new List<Node>();
            this.lines = new List<Line>();
            XElement root = XElement.Load("RouteData.xml");
            var lines =
                from el in root.Elements("Line") select el;
            foreach(XElement lineElm in lines)
            {
                Node prev = null;
                Line line = new Line();
                line.Name = lineElm.Attribute("Name").Value;
                var lineColor = lineElm.Attribute("Color");
                if (lineColor != null)
                {
                    string str = lineColor.Value;
                    line.Color = (Color)ColorConverter.ConvertFromString(str);
                }
                else
                    line.Color = Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
                var stations =
                    from el in lineElm.Elements("Station") select el;

                uint lid = lineCounter++;
                line.UID = lid;

                foreach(XElement station in stations)
                {
                    Node node = null;
                    var name = station.Attribute("Name").Value;

                    var linkStationName = station.Attribute("LinkLineName");
                    if (linkStationName != null)
                    {
                        Line li = this.lines.Single(it => it.Name == linkStationName.Value);
                        node = nodes.Single(n => n.LineIDs.Contains(li.UID) && n.Name == name);
                    }
                    else
                    {
                        node = new Node();
                        node.UID = counter++;
                        node.Name = name;
                        nodes.Add(node);
                    }
                    node.LineIDs.Add(lid);

                    if (prev != null)
                    {
                        var arc = new Arc();
                        arc.NodeID = prev.UID;
                        node.Arcs.AddLast(arc);

                        arc = new Arc();
                        arc.NodeID = node.UID;
                        prev.Arcs.AddLast(arc);
                    }
                    prev = node;
                }

                this.lines.Add(line);
            }

            return nodes;
       }

        private void Query()
        {
            string originName = OriginTextBox.Text;
            string destinationName = DestinationTextBox.Text;

            Node origin = nodes.Find(n => n.Name == originName);
            Node destination = nodes.Find(n => n.Name == destinationName);
            
            if (origin == null || destination == null)
            {
                MessageBox.Show("亲！没有这个站点哦！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var stationList = FindClosestPath(origin.UID, destination.UID);
            var line = nodes[(int)stationList.First.Value].LineIDs;

            int linesLen = lines.Count;

            var jounaryList = new ObservableCollection<Jounary>();
            List<Node> list = new List<Node>();
            var reverse = stationList.Reverse().ToList();

            foreach (var i in reverse)
            {
                list.Add(nodes[(int)i]);
            }

            var j = new Jounary();
            j.Start = list[0];
            j.Line = lines[(int)GuessLineNumber(list, 0)];

            int beginI = 0;
            for (int i = 0; i < list.Count; ++i)
            {
                var node = list[i];

                if (i < list.Count - 1)
                {
                    var nextNode = list[i + 1];

                    if (!nextNode.LineIDs.Contains(j.Line.UID))
                    {
                        j.End = node;
                        j.NodesCount = Math.Abs(i - beginI);
                        jounaryList.Add(j);
                        beginI = i;

                        j = new Jounary();
                        j.Start = node;
                        j.Line = lines[(int)GuessLineNumber(list, i)];
                    }

                }
                else if (i == list.Count - 1)
                {
                    j.End = node;
                    j.NodesCount = Math.Abs(i - beginI);
                    jounaryList.Add(j);
                }
            }

            ResultListView.ItemsSource = jounaryList;
            int totalTime = 0;
            foreach(Jounary it in jounaryList)
            {
                totalTime += it.Time;
            }
            totalTime += (jounaryList.Count - 1) * Jounary.JounaryChangeTime;

            TotalNameTextBlock.Text = (string)new TimeFormator().Convert(totalTime, typeof(string), null, CultureInfo.CurrentCulture);

        }

        private LinkedList<uint> FindClosestPath(uint originID, uint destinationID)
        {
            var len = nodes.Count;
            var distance = new uint[len];
            var path = new uint[len];
            var book = new bool[len];
            for (int i = 0; i < len; ++i)
            {
                distance[i] = INF;
                book[i] = false;
            }

            distance[originID] = 0;
            book[originID] = true;

            int pointer = (int)originID;

            for (int i=0; i < len - 1; ++i)
            {
                uint min = INF;

                for (int j = 0; j < len; ++j)
                {
                    if (book[j] == false && distance[j] < min)
                    {
                        min = distance[j];
                        pointer = j;
                    }
                }
                book[pointer] = true;

                foreach(Arc arc in nodes[pointer].Arcs)
                {
                    if (distance[pointer] + arc.Weight < distance[arc.NodeID])
                    {
                        distance[arc.NodeID] = distance[pointer] + arc.Weight;
                        path[arc.NodeID] = (uint) pointer;
                    }
                }

            }

            var result = new LinkedList<uint>();
            uint p = destinationID;

            while (p != originID)
            {
                result.AddLast(p);
                p = path[p];
            }
            result.AddLast(originID);

            return result;
        }

        private uint GuessLineNumber(List<Node> target, int i)
        {
            Node currentNode = target[i], nextNode = null;
            switch(currentNode.LineIDs.Count)
            {
                case 0:
                    throw new IndexOutOfRangeException();
                case 1:
                    return currentNode.LineIDs.First();
                default:
                    nextNode = target[++i];
                    var filled = GetFilledList(currentNode.LineIDs, nextNode.LineIDs);
                    while (filled.Count > 1)
                    {
                        currentNode = nextNode;
                        nextNode = target[i++];
                        filled = GetFilledList(currentNode.LineIDs, nextNode.LineIDs);
                    }
                    return filled[0];
            }
        }

        private static List<T> GetFilledList<T>(List<T> a, List<T> b)
        {
            var result = new List<T>();
            foreach (T i in a)
            {
                foreach (T j in b)
                {
                    if (i.Equals(j))
                        result.Add(i);
                }
            }
            return result;
        }

        private void Query_Click(object sender, RoutedEventArgs e)
        {
            Query();
        }

        private void DestinationTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Query();
            }
        }

        private void Exchange_Click(object sender, RoutedEventArgs e)
        {
            var dest = DestinationTextBox.Text;
            DestinationTextBox.Text = OriginTextBox.Text;
            OriginTextBox.Text = dest;
        }
    }
}
