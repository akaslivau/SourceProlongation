using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
using SourceProlongation.Base;

namespace SourceProlongation.ViewModel
{
    public class OrderListViewModel: ViewModelBase
    {
        #region Fields
        private readonly MainWindowViewModel _parent;

        private ObservableCollection<OrderViewModel> _items = new ObservableCollection<OrderViewModel>();
        public ObservableCollection<OrderViewModel> Items { get { return _items;} set{ _items = value; OnPropertyChanged("Items");}}


        private CollectionView _tableView = null;
        public CollectionView TableView { get { return _tableView; } set { _tableView = value; OnPropertyChanged("TableView"); } }

        private OrderViewModel _selected;

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

        #region StatusFilter
        private string _filterStatus = Collections.Statuses[0];

        public string FilterStatus
        {
            get { return _filterStatus; }
            set
            {
                _filterStatus = value;
                OnPropertyChanged("FilterStatus");
                RefreshView();
            }
        }

        private void RefreshView()
        {
            TableView.Filter = Filter;
            TableView.Refresh();

            if (!TableView.IsEmpty)
            {
                Selected = (OrderViewModel)TableView.GetItemAt(0);
            }
        }

        private bool Filter(object item)
        {
            if (FilterStatus == Collections.Statuses[0]) return true;

            var row = (OrderViewModel)item;
            return (FilterStatus == row.Status);
        }
        #endregion

      
        public OrderListViewModel(MainWindowViewModel parent)
        {
            _parent = parent;
            DisplayName = "Все заявки";

            TableView = new ListCollectionView(Items);
/*            var tmp = Directory.GetFiles(Strings.ORDER_FOLDER).
                Select(json => JsonConvert.DeserializeObject<OrderViewModel>
                    (File.ReadAllText(json))).
                    Select(order => new OrderViewModel(order)).
                    OrderBy(x => x.Year).ThenBy(x=>x.Number).
                    ToList();

            foreach (var orderViewModel in tmp)
            {
                Items.Add(orderViewModel);
            }*/

            
            CanBeClosed = false;
        }
    }
}
