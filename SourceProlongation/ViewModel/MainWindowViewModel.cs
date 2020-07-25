using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SourceProlongation.Base;
using SourceProlongation.Model;
using SourceProlongation.Properties;

namespace SourceProlongation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        private bool _hideFinished = Settings.Default.hideFinished;
        public bool HideFinished
        {
            get => _hideFinished;
            set
            {
                if (_hideFinished == value) return;
                _hideFinished = value;
                OnPropertyChanged("HideFinished");
                Settings.Default.hideFinished = value;
                Settings.Default.Save();
                if (value)
                {
                    TableView.Filter = Filter;
                }
                else
                {
                    TableView.Filter = null;
                }
                TableView.Refresh();
            }
        }

        private ListCollectionView _tableView;
        public ListCollectionView TableView
        {
            get
            {
                return _tableView;
            }
            set
            {
                if (_tableView == value) return;
                _tableView = value;
                OnPropertyChanged("TableView");
            }
        }

        private ObservableCollection<OrderViewModel> _items;
        public ObservableCollection<OrderViewModel> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        private OrderViewModel _selected = null;
        public OrderViewModel Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged("Selected");
            }
        }


        #endregion 

        public ICommand WindowClosingCommand { get; private set; }

        public MainWindowViewModel()
        {
            WindowClosingCommand = new RelayCommand(a =>
            {
              
            });
            _hideFinished = Settings.Default.hideFinished;

            Items = new ObservableCollection<OrderViewModel>();
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var orders = cntx.GetTable<Order>();
                foreach (var order in orders)
                {
                    Items.Add(new OrderViewModel(order));
                }
                TableView = new ListCollectionView(Items);
                TableView.Filter = Filter;
                TableView.Refresh();

                if (TableView.Count > 0) Selected = (OrderViewModel) TableView.GetItemAt(0);
            }
        }

        private bool Filter(object obj)
        {
            var ord = (OrderViewModel) obj;
            return ord.Status != Status.Finished;
        }
    }
}
