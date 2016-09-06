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
                var stations =
                    from el in lineElm.Elements("Station") select el;

                // var lid = Convert.ToUInt32(line.Attribute("ID").Value);
                uint lid = lineCounter++;
                line.UID = lid;

                foreach(XElement station in stations)
                {
                    Node node = null;
                    var name = station.Attribute("Name").Value;

                    var linkAttr = station.Attribute("LinkLine");
                    if (linkAttr != null)
                    {
                        var linkLineID = Convert.ToUInt32(linkAttr.Value);
                        node = nodes.Single(n => n.LineIDs.Contains(linkLineID) && n.Name == name);
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

        private void Query_Click(object sender, RoutedEventArgs e)
        {
            string originName = OriginTextBox.Text;
            string destinationName = DestinationTextBox.Text;

            Node origin = nodes.Single(n => n.Name == originName);
            Node destination = nodes.Single(n => n.Name == destinationName);

            var stationList = FindClosestPath(origin.UID, destination.UID);
            var line = nodes[(int)stationList.First.Value].LineIDs;

            var jounaryList = new ObservableCollection<Jounary>();
            var list = stationList.Reverse().ToList();
            var j = new Jounary();
            // j.Start = nodes[(int)stationList.First.Value];
            j.Start = nodes[(int)list[0]];
            j.LineNumber = j.Start.LineIDs[0];


            for (int i = 0; i < list.Count; ++i)
            {
                var node = nodes[(int)list[i]];

                if (i < list.Count - 1 && !nodes[(int)list[i+1]].LineIDs.Contains(j.LineNumber))
                {
                    // j.End = nodes[(int)list[i-1]];
                    j.End = node;

                    jounaryList.Add(j);

                    j = new Jounary();
                    // j.Start = nodes[(int)list[i-1]];
                    j.Start = node;
                    j.LineNumber = nodes[(int)list[i + 1]].LineIDs[0];
                    // j.LineNumber = nodes[(int)list[i-1]].LineIDs[0];
                }
                else if (i == list.Count - 1)
                {
                    j.End = node;
                    jounaryList.Add(j);
                }
            }

            /*
            for (var i = stationList.First; (i = i.Next) != null;)
            {
                if (!nodes[(int)i.Value].LineIDs.Contains(j.LineNumber))
                {
                    j.End = nodes[(int)i.Previous.Value];

                    jounaryList.Add(j);

                    j = new Jounary();
                    j.Start = nodes[(int)i.Previous.Value];
                    j.LineNumber = nodes[(int)i.Previous.Value].LineIDs[0];
                }
                else if (i.Next == null)
                {
                    j.End = nodes[(int)i.Previous.Value];
                    jounaryList.Add(j);
                }
            }
            */

            ResultListView.ItemsSource = jounaryList;
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

    }
}
