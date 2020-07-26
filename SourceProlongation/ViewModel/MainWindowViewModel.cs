using System;
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


        private ListCollectionView _reqView;
        public ListCollectionView ReqView
        {
            get
            {
                return _reqView;
            }
            set
            {
                if (_reqView == value) return;
                _reqView = value;
                OnPropertyChanged("ReqView");
            }
        }

        private ObservableCollection<RequestViewModel> _requests;
        public ObservableCollection<RequestViewModel> Requests
        {
            get
            {
                return _requests;
            }
            set
            {
                if (_requests == value) return;
                _requests = value;
                OnPropertyChanged("Requests");
            }
        }

        private RequestViewModel _selectedRequest;
        public RequestViewModel SelectedRequest
        {
            get
            {
                return _selectedRequest;
            }
            set
            {
                if (_selectedRequest == value) return;
                _selectedRequest = value;
                OnPropertyChanged("SelectedRequest");
            }
        }

        private ObservableCollection<DeviceViewModel> _devices;
        public ObservableCollection<DeviceViewModel> Devices
        {
            get
            {
                return _devices;
            }
            set
            {
                if (_devices == value) return;
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        private DeviceViewModel _selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get
            {
                return _selectedDevice;
            }
            set
            {
                if (_selectedDevice == value) return;
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        private ObservableCollection<PriceViewModel> _prices;
        public ObservableCollection<PriceViewModel> Prices
        {
            get
            {
                return _prices;
            }
            set
            {
                if (_prices == value) return;
                _prices = value;
                OnPropertyChanged("Prices");
            }
        }

        private PriceViewModel _selectedPrice;
        public PriceViewModel SelectedPrice
        {
            get
            {
                return _selectedPrice;
            }
            set
            {
                if (_selectedPrice == value) return;
                _selectedPrice = value;
                OnPropertyChanged("SelectedPrice");
            }
        }

        #endregion

        #region Commands
        public ICommand AddRequestCommand { get; }
        public ICommand RemoveRequestCommand { get; }

        private void AddRequest(object obj)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var model = new Request()
                {
                    customerId = -1,
                    incomingNum = "",
                    incomingDate = DateTime.Now,
                    other = ""
                };

                var table = cntx.GetTable<Request>();
                table.InsertOnSubmit(model);
                cntx.SubmitChanges();

                Requests.Add(new RequestViewModel(model));
                SelectedRequest = Requests.Last();
            }
        }

        private void RemoveRequest(object obj)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var table = cntx.GetTable<Request>();
                var toDelete = table.Single(x => x.id == SelectedRequest.Id);
                table.DeleteOnSubmit(toDelete);
                cntx.SubmitChanges();

                var req = Requests.Single(x => x.Id == toDelete.id);
                Requests.Remove(req);
            }
        }

        public ICommand AddDeviceCommand { get; }
        public ICommand RemoveDeviceCommand { get; }

        private void AddDevice(object obj)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var model = new Device()
                {
                    name = "New item",
                    type = "",
                    reestr = ""
                };

                var table = cntx.GetTable<Device>();
                table.InsertOnSubmit(model);
                cntx.SubmitChanges();

                Devices.Add(new DeviceViewModel(model));
                SelectedDevice = Devices.Last();
            }
        }

        private void RemoveDevice(object obj)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var table = cntx.GetTable<Device>();
                var toDelete = table.Single(x => x.id == SelectedDevice.Id);
                table.DeleteOnSubmit(toDelete);
                cntx.SubmitChanges();

                var d = Devices.Single(x => x.Id == toDelete.id);
                Devices.Remove(d);
            }
        }

        public ICommand AddPriceCommand { get; }
        public ICommand RemovePriceCommand { get; }

        private void AddPrice(object obj)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var model = new Price()
                {
                    workTypeId = -1,
                    deviceId = -1,
                    pricePerItem = 0,
                    fromCount = 0,
                    toCount = 1
                };

                var table = cntx.GetTable<Price>();
                table.InsertOnSubmit(model);
                cntx.SubmitChanges();

                Prices.Add(new PriceViewModel(model));
                SelectedPrice = Prices.Last();
            }
        }

        private void RemovePrice(object obj)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var table = cntx.GetTable<Price>();
                var toDelete = table.Single(x => x.id == SelectedPrice.Id);
                table.DeleteOnSubmit(toDelete);
                cntx.SubmitChanges();

                var d = Prices.Single(x => x.Id == toDelete.id);
                Prices.Remove(d);
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

                //Request
                Requests = new ObservableCollection<RequestViewModel>();
                foreach (var request in cntx.GetTable<Request>())
                {
                    Requests.Add(new RequestViewModel(request));
                }
                ReqView = new ListCollectionView(Requests);
                ReqView.Refresh();

                if(ReqView.Count>0) SelectedRequest = (RequestViewModel) ReqView.GetItemAt(0);

                //Devices
                Devices = new ObservableCollection<DeviceViewModel>();
                foreach (var device in cntx.GetTable<Device>().ToList())
                {
                    Devices.Add(new DeviceViewModel(device));
                }

                if (Devices.Any()) SelectedDevice = Devices[0];

                //Prices
                Prices = new ObservableCollection<PriceViewModel>();
                foreach (var p in cntx.GetTable<Price>().ToList())
                {
                    Prices.Add(new PriceViewModel(p));
                }
                if (Prices.Any()) SelectedPrice = Prices[0];
            }

            //Commands
            AddRequestCommand = new RelayCommand(AddRequest);
            RemoveRequestCommand = new RelayCommand(RemoveRequest, a => SelectedRequest != null);
            AddDeviceCommand = new RelayCommand(AddDevice);
            RemoveDeviceCommand = new RelayCommand(RemoveDevice, a=> SelectedDevice!=null);
            AddPriceCommand = new RelayCommand(AddPrice);
            RemovePriceCommand = new RelayCommand(RemovePrice, a => SelectedPrice != null);
        }

        private bool Filter(object obj)
        {
            var ord = (OrderViewModel) obj;
            return ord.Status != Status.Finished;
        }
    }
}
