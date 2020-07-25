using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Newtonsoft.Json;
using SourceProlongation.Base;
using SourceProlongation.Model;
using SourceProlongation.Properties;

namespace SourceProlongation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields
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
               //TODO: Save All
            });
            Items = new ObservableCollection<OrderViewModel>();
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var orders = cntx.GetTable<Order>();
                foreach (var order in orders)
                {
                    //if(Settings.Default.hideFinished && order.status == Status.Finished) continue;
                    Items.Add(new OrderViewModel(order));
                }

                if (Items.Any()) Selected = Items[0];
            }
        }
    }
}
