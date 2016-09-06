using System;
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

        private void NotifyPropertyChanged([CallerMemberName]string propName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
