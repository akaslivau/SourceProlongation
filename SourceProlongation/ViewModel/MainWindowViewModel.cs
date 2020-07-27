using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Novacode;
using SourceProlongation.Base;
using SourceProlongation.Model;
using SourceProlongation.Properties;

namespace SourceProlongation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        private Rank _comboboxRank;
        public Rank ComboboxRank
        {
            get
            {
                return _comboboxRank;
            }
            set
            {
                if (_comboboxRank == value) return;
                _comboboxRank = value;
                OnPropertyChanged("ComboboxRank");
            }
        }

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

        private ObservableCollection<CustomerViewModel> _customers;
        public ObservableCollection<CustomerViewModel> Customers
        {
            get
            {
                return _customers;
            }
            set
            {
                if (_customers == value) return;
                _customers = value;
                OnPropertyChanged("Customers");
            }
        }

        private CustomerViewModel _selectedCustomer;
        public CustomerViewModel SelectedCustomer
        {
            get
            {
                return _selectedCustomer;
            }
            set
            {
                if (_selectedCustomer == value) return;
                _selectedCustomer = value;
                OnPropertyChanged("SelectedCustomer");
            }
        }

        private ObservableCollection<RankViewModel> _ranks;
        public ObservableCollection<RankViewModel> Ranks
        {
            get
            {
                return _ranks;
            }
            set
            {
                if (_ranks == value) return;
                _ranks = value;
                OnPropertyChanged("Ranks");
            }
        }

        private RankViewModel _selectedRank;
        public RankViewModel SelectedRank
        {
            get
            {
                return _selectedRank;
            }
            set
            {
                if (_selectedRank == value) return;
                _selectedRank = value;
                OnPropertyChanged("SelectedRank");
            }
        }


        #endregion

        #region Commands
        public ICommand AddOrderCommand { get; }
        public ICommand RemoveOrderCommand { get; }


        private void AddOrder(object obj)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var model = new Order()
                {
                    customerId = -1,
                    oldCustomerName = "",
                    actNumber = 0,
                    year = 0,
                    beginDate = DateTime.Now.AddDays(-28),
                    docDate = DateTime.Now,
                    executors = "",
                    status = Status.Created,
                    other = ""
                };

                var table = cntx.GetTable<Order>();
                table.InsertOnSubmit(model);
                cntx.SubmitChanges();

                Items.Add(new OrderViewModel(model, this));
                Selected = Items.Last();
            }
        }

        private void RemoveOrder(object obj)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var table = cntx.GetTable<Order>();
                var toDelete = table.Single(x => x.id == Selected.OrderId);
                table.DeleteOnSubmit(toDelete);
                cntx.SubmitChanges();

                var o = Items.Single(x => x.OrderId == toDelete.id);
                Items.Remove(o);
            }
        }


        public ICommand AddRequestCommand { get; }
        public ICommand RemoveRequestCommand { get; }
        public ICommand ExportRequestCommand { get; }

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

        private const string REQUEST_FILE = "Request.docx";

        private bool IsMultDoc(string name)
        {
            return name.Contains("оверк") || name.Contains("алибровк") || name.Contains("ттестац");
        }
        private void ExportRequest(object obj)
        {
            var r = SelectedRequest;

            var doc = DocX.Load(REQUEST_FILE);

            doc.ReplaceText("#Customer#", r.CustomerName);
            doc.ReplaceText("#OrderNum#", r.IncomingNumber);
            doc.ReplaceText("#OrderDate#", r.IncomingDate.ToShortDateString());
            //Given docs
            var weGive = r.Items.Select(x => x.Work).ToList();
            var lst = new List<string>();
            foreach (var w in weGive)
            {
                lst.AddRange(w.docsToGive.Split(';'));
            }

            lst = lst.Distinct().ToList();
            var dict = lst.ToDictionary(x => x, x => new int());
            foreach (var it in r.Items)
            {
                var splt = it.Work.docsToGive.Split(';');
                foreach (var s in splt)
                {
                    if (IsMultDoc(s)) dict[s] += it.Count;
                }
            }

            var toWrite = string.Join("\n",dict.Select(x => x.Value > 0 ? 
                x.Key + " - " + x.Value + " шт." :
                x.Key
            ).ToList());
            
            doc.ReplaceText("#GivenDocs#", toWrite);
            //CustomerDocs
            var op = (from a in r.Items.Select(x => x.Work.custDocs)
                from b in a.Split(';')
                select b).Distinct().Where(x=>x.Length>1).ToList();

            doc.ReplaceText("#CustomerDocs#", string.Join("\n", op));


            var table = doc.Tables[1];
            int rowIndex = 1;
            foreach (var item in r.Items)
            {
                table.InsertRow(rowIndex);
                var row = table.Rows[rowIndex];

                var txt = new string[]
                {
                    item.Work.name,
                    item.Dev.ToString(),
                    item.Count.ToString(),
                    item.Count.ToString() + " x " + (int)(item.Price/item.Count) + " = " + item.Price,
                    item.Other
                };

                for (int i = 0; i < txt.Length; i++)
                {
                    var num = row.Cells[i].Paragraphs[0].Append(txt[i]);
                    num.FontSize(12);
                    num.Alignment = Alignment.center;
                    num.Font(new FontFamily("Times New Roman"));
                }
                rowIndex++;
            }

            var tripTable = doc.Tables[2];
            if (!r.Trips.Any())
            {
                doc.ReplaceText("#TRIPS#", "");
                tripTable.Remove();
                doc.SaveAs("test.docx");
                return;
            }
            doc.ReplaceText("#TRIPS#", "КОМАНДИРОВКИ");
            rowIndex = 1;
            foreach (var item in r.Trips)
            {
                tripTable.InsertRow(rowIndex);
                var row = tripTable.Rows[rowIndex];

                var txt = new string[]
                {
                    item.Place,
                    "1 чел. на " + item.Days + " к.д.",
                    item.Sum.ToString(),
                    item.Other
                };

                for (int i = 0; i < txt.Length; i++)
                {
                    var num = row.Cells[i].Paragraphs[0].Append(txt[i]);
                    num.FontSize(12);
                    num.Alignment = Alignment.center;
                    num.Font(new FontFamily("Times New Roman"));
                }
                rowIndex++;
            }

            var path = string.Join(" ",
                MyStatic.CleanName(SelectedRequest.CustomerName),
                MyStatic.CleanName(SelectedRequest.IncomingNumber));
            doc.SaveAs(path + ".docx");
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

        public ICommand AddCustomerCommand { get; }
        public ICommand RemoveCustomerCommand { get; }

        private void AddCustomer(object obj)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var model = new Customer()
                {
                    name = "Customer",
                    name_kogo = "",
                    name_kem = "",
                    name_komu = "",
                    fio = "",
                    email = "",
                    phone = "",
                    other = ""
                };

                var table = cntx.GetTable<Customer>();
                table.InsertOnSubmit(model);
                cntx.SubmitChanges();

                Customers.Add(new CustomerViewModel(model));
                SelectedCustomer = Customers.Last();
            }
        }

        private void RemoveCustomer(object obj)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var table = cntx.GetTable<Customer>();
                var toDelete = table.Single(x => x.id == SelectedCustomer.Id);
                table.DeleteOnSubmit(toDelete);
                cntx.SubmitChanges();

                var d = Customers.Single(x => x.Id == toDelete.id);
                Customers.Remove(d);
            }
        }

        public ICommand AddRankCommand { get; }
        public ICommand RemoveRankCommand { get; }

        private void AddRank(object obj)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var model = new Rank()
                {
                    scheme = "",
                    rankv = "Рабочий эталон 1-го разряда"
                };

                var table = cntx.GetTable<Rank>();
                table.InsertOnSubmit(model);
                cntx.SubmitChanges();

                Ranks.Add(new RankViewModel(model));
                SelectedRank = Ranks.Last();
            }
        }

        private void RemoveRank(object obj)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var table = cntx.GetTable<Rank>();
                var toDelete = table.Single(x => x.id == SelectedRank.Id);
                table.DeleteOnSubmit(toDelete);
                cntx.SubmitChanges();

                var d = Ranks.Single(x => x.Id == toDelete.id);
                Ranks.Remove(d);
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
                    Items.Add(new OrderViewModel(order, this));
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

                //Customers
                Customers = new ObservableCollection<CustomerViewModel>();
                foreach (var p in cntx.GetTable<Customer>().ToList())
                {
                    Customers.Add(new CustomerViewModel(p));
                }
                if (Customers.Any()) SelectedCustomer = Customers[0];

                //Ranks
                Ranks = new ObservableCollection<RankViewModel>();
                foreach (var p in cntx.GetTable<Rank>().ToList())
                {
                    Ranks.Add(new RankViewModel(p));
                }
                if (Ranks.Any()) SelectedRank = Ranks[0];
            }

            //Commands
            AddOrderCommand = new RelayCommand(AddOrder);
            RemoveOrderCommand = new RelayCommand(RemoveOrder, a => Selected != null);
            AddRequestCommand = new RelayCommand(AddRequest);
            RemoveRequestCommand = new RelayCommand(RemoveRequest, a => SelectedRequest != null);
            ExportRequestCommand = new RelayCommand(ExportRequest, a => SelectedRequest != null);
            AddDeviceCommand = new RelayCommand(AddDevice);
            RemoveDeviceCommand = new RelayCommand(RemoveDevice, a=> SelectedDevice!=null);
            AddPriceCommand = new RelayCommand(AddPrice);
            RemovePriceCommand = new RelayCommand(RemovePrice, a => SelectedPrice != null);
            AddCustomerCommand = new RelayCommand(AddCustomer);
            RemoveCustomerCommand = new RelayCommand(RemoveCustomer, a => SelectedCustomer != null);
            AddRankCommand = new RelayCommand(AddRank);
            RemoveRankCommand = new RelayCommand(RemoveRank, a => SelectedRank != null);
        }

        private bool Filter(object obj)
        {
            var ord = (OrderViewModel) obj;
            return ord.Status != Status.Finished;
        }
    }
}
