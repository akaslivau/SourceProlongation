using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SourceProlongation.Model
{
    public class CheckWrapper<T> : INotifyPropertyChanged
    {
        private readonly CheckableObservableCollection<T> _parent;

        public CheckWrapper(CheckableObservableCollection<T> parent)
        {
            _parent = parent;
        }

        private T _value;

        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                CheckChanged();
                OnPropertyChanged("IsChecked");
            }
        }

        private void CheckChanged()
        {
            _parent.Refresh();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler pceh = PropertyChanged;
            if (pceh != null)
            {
                pceh(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class CheckableObservableCollection<T> : ObservableCollection<CheckWrapper<T>>
    {
        public event EventHandler CheckedChangedEvent;

        private ListCollectionView _selected;

        public CheckableObservableCollection()
        {
            _selected = new ListCollectionView(this);
            _selected.Filter = delegate (object checkObject) {
                return ((CheckWrapper<T>)checkObject).IsChecked;
            };
        }

        public void Add(T item)
        {
            this.Add(new CheckWrapper<T>(this) { Value = item });
        }

        public ICollectionView CheckedItems
        {
            get { return _selected; }
        }

        internal void Refresh()
        {
            _selected.Refresh();
            CheckedChangedEvent?.Invoke(this, new EventArgs());
        }
    }
}
