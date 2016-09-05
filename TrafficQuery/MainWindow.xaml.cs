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
using System.Windows.Shapes;
using TrafficQuery.Models;
using System.Xml.Linq;

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

            uint counter = 0;
            var nodes = new List<Node>();
            XElement root = XElement.Load("RouteData.xml");
            var lines =
                from el in root.Elements("Line") select el;
            foreach(XElement line in lines)
            {
                Node prev = null;
                var stations =
                    from el in line.Elements("Station") select el;

                var lid = Convert.ToUInt32(line.Attribute("ID").Value);

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
                    nodes.Add(node);
                    prev = node;
                }
            }

            return nodes;
       }

        private void Query_Click(object sender, RoutedEventArgs e)
        {
            string origin = OriginTextBox.Text;
            string destination = DestinationTextBox.Text;
        }

        private uint FindClosestPath(uint originID, uint destinationID)
        {
            uint[] dis = new uint[Length];
            for (int i = 0; i < Length; ++i) dis[i] = INF;
            return 1;
        }
    }
}
