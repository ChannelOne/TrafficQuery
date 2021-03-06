﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace TrafficQuery.Models
{
    class Jounary : INotifyPropertyChanged
    {
        public static int MinutesPreNode = 3;
        public static int JounaryChangeTime = 3;
        public event PropertyChangedEventHandler PropertyChanged;

        private Node start;
        public Node Start
        {
            get { return start; }
            set
            {
                start = value;
                NotifyPropertyChanged();
            }
        }

        private Node end;
        public Node End
        {
            get { return end; }
            set
            {
                end = value;
                NotifyPropertyChanged();
            }
        }

        private int nodesCount = 0;
        public int NodesCount
        {
            get { return nodesCount; }
            set
            {
                nodesCount = value;
                NotifyPropertyChanged("NodesCount");
                NotifyPropertyChanged("Time");
            }
        }

        public int Time
        {
            get { return nodesCount * MinutesPreNode; }
        }

        private Line line;
        public Line Line
        {
            get { return line; }
            set
            {
                line = value;
                NotifyPropertyChanged();
            }
        }

        /*
        private uint lineNumber;
        public uint LineNumber
        {
            get { return lineNumber; }
            set
            {
                lineNumber = value;
                NotifyPropertyChanged();
            }
        }
        */

        private void NotifyPropertyChanged([CallerMemberName]string propName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
