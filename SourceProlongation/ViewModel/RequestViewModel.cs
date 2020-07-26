using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SourceProlongation.Base;

namespace SourceProlongation.ViewModel
{
    public class RequestViewModel:ViewModelBase
    {
        #region Fields
        public int Id { get; }

        private ObservableCollection<RequestItemViewModel> _items;
        public ObservableCollection<RequestItemViewModel> Items
        {
            get
            {
                return _items;
            }
            set
            {
                if (_items == value) return;
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        private RequestItemViewModel _selectedWork;
        public RequestItemViewModel SelectedWork
        {
            get
            {
                return _selectedWork;
            }
            set
            {
                if (_selectedWork == value) return;
                _selectedWork = value;
                OnPropertyChanged("SelectedWork");
            }
        }


        public string CustomerName => SelectedCustomer.name;

        private Customer _selectedCustomer = null;
        public Customer SelectedCustomer
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
                OnPropertyChanged("CustomerName");

                if (value != null) UpdateProperty("customerId", value.id);
            }
        }

        private string _incomingNumber;
        public string IncomingNumber
        {
            get
            {
                return _incomingNumber;
            }
            set
            {
                if (_incomingNumber == value) return;
                _incomingNumber = value;
                OnPropertyChanged("IncomingNumber");
                UpdateProperty("incomingNum", value);
            }
        }

        private DateTime _incomingDate;
        public DateTime IncomingDate
        {
            get
            {
                return _incomingDate;
            }
            set
            {
                if (_incomingDate == value) return;
                _incomingDate = value;
                OnPropertyChanged("IncomingDate");
                UpdateProperty("incomingDate", value);
            }
        }

        private string _other;
        public string Other
        {
            get
            {
                return _other;
            }
            set
            {
                if (_other == value) return;
                _other = value;
                OnPropertyChanged("Other");
                UpdateProperty("other", value);
            }
        }

        #endregion

        #region Commands

        public ICommand AddRequestItemCommand { get; }
        public ICommand RemoveRequestItemCommand { get; }

        private void AddRequestItem(object obj)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var model = new RequestItem()
                {
                   requestId = Id,
                   workTypeId = -1,
                   deviceId = -1,
                   count = 0,
                   price = 0,
                   other = ""
                };

                var table = cntx.GetTable<RequestItem>();
                table.InsertOnSubmit(model);
                cntx.SubmitChanges();

                Items.Add(new RequestItemViewModel(model));
                SelectedWork = Items.Last();
            }
        }

        private void RemoveRequestItem(object obj)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var table = cntx.GetTable<RequestItem>();
                var toDelete = table.Single(x => x.id == SelectedWork.Id);
                table.DeleteOnSubmit(toDelete);
                cntx.SubmitChanges();

                var req = Items.Single(x => x.Id == toDelete.id);
                Items.Remove(req);
            }
        }
        #endregion

        public RequestViewModel(Request request)
        {
            Id = request.id;
            Items = new ObservableCollection<RequestItemViewModel>();

            _other = request.other;
            _incomingNumber = request.incomingNum;
            _incomingDate = request.incomingDate;
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var customersTable = cntx.GetTable<Customer>();
                _selectedCustomer = customersTable.SingleOrDefault(x => x.id == request.customerId);

                var items = cntx.GetTable<RequestItem>().Where(x => x.requestId == Id).ToList();
                foreach (var requestItem in items)
                {
                    Items.Add(new RequestItemViewModel(requestItem));
                }

                if (Items.Any()) SelectedWork = Items[0];
            }

            
            //Commands
            AddRequestItemCommand = new RelayCommand(AddRequestItem);
            RemoveRequestItemCommand = new RelayCommand(RemoveRequestItem, a => SelectedWork != null);
        }

        private void UpdateProperty(string propertyName, object value)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var table = cntx.GetTable<Request>();
                var item = table.Single(x => x.id == Id);

                PropertyInfo prop = item.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanWrite)
                {
                    prop.SetValue(item, value, null);
                }
                cntx.SubmitChanges();
            }
        }
    }
}
