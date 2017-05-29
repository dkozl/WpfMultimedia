using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MVVM
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool UpdateProperty<T>(string propertyName, T newValue, ref T currentValue, IEqualityComparer<T> equalityComparer)
        {
            if (!equalityComparer.Equals(newValue, currentValue))
            {
                currentValue = newValue;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        protected bool UpdateProperty<T>(string propertyName, T newValue, ref T currentValue)
        {
            return UpdateProperty(propertyName, newValue, ref currentValue, EqualityComparer<T>.Default);
        }
    }
}
