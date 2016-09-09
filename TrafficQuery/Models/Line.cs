using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Media;
using System.Windows.Media;

namespace TrafficQuery.Models
{
    class Line : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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

        private Color color;
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                NotifyPropertyChanged();
            }
        }

        private void NotifyPropertyChanged([CallerMemberName]string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public override bool Equals(object obj)
        {
            var it = obj as Line;
            if (it != null && this.uid == it.uid)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return (int)uid;
        }

    }
}
