using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace TrafficQuery.Models
{
    class Node : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private uint[] distance;
        public uint[] Distance
        {
            get { return distance; }
            set
            {
                distance = value;
                NotifyPropertyChanged();
            }
        } 

        private uint uid;
        public uint UID
        {
            get { return uid; }
            set
            {
                uid = value;
                NotifyPropertyChanged();
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged();
            }
        }

        private List<uint> lineIDs = new List<uint>();
        public List<uint> LineIDs
        {
            get { return lineIDs; }
            set
            {
                lineIDs = value;
                NotifyPropertyChanged();
            }
        }

        private LinkedList<Arc> arcs = new LinkedList<Arc>();
        public LinkedList<Arc> Arcs
        {
            get { return arcs; }
            set
            {
                arcs = value;
                NotifyPropertyChanged();
            }
        }

        private void NotifyPropertyChanged([CallerMemberName]string propName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

}
