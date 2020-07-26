using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SourceProlongation.Base;

namespace SourceProlongation.ViewModel
{
    public class RequestItemViewModel: ViewModelBase
    {
        #region Fields
        public int Id { get; }
        
        private WorkType _work;
        public WorkType Work
        {
            get
            {
                return _work;
            }
            set
            {
                if (_work == value) return;
                _work = value;
                OnPropertyChanged("Work");
                if(value!=null) UpdateProperty("workTypeId", value.id);
                UpdatePrice();
            }
        }

        private Device _dev;
        public Device Dev
        {
            get
            {
                return _dev;
            }
            set
            {
                if (_dev == value) return;
                _dev = value;
                OnPropertyChanged("Dev");
                if (value != null) UpdateProperty("deviceId", value.id);
                UpdatePrice();
            }
        }

        private int _count;
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                if (_count == value) return;
                _count = value;
                OnPropertyChanged("Count");
                UpdateProperty("count", value);
                UpdatePrice();
            }
        }

        private double _price;
        public double Price
        {
            get
            {
                return _price;
            }
            set
            {
                if (_price == value) return;
                _price = value;
                OnPropertyChanged("Price");
                UpdateProperty("price", value);
            }
        }

        private string _pricePerItem;
        public string PricePerItem
        {
            get
            {
                return _pricePerItem;
            }
            set
            {
                if (_pricePerItem == value) return;
                _pricePerItem = value;
                OnPropertyChanged("PricePerItem");
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

        private void UpdatePrice()
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var price = cntx.GetTable<Price>().
                    Where(x => x.workTypeId == Work.id && x.deviceId == Dev.id).
                    Where(x=> Count > x.fromCount && Count <= x.toCount).
                    ToList();

                if (!price.Any())
                {
                    Price = 0;
                    PricePerItem = "Не найдено";
                }
                if (price.Count == 1)
                {
                    Price = price[0].pricePerItem * Count;
                    PricePerItem = price[0].pricePerItem.ToString();
                }
                if (price.Count > 1)
                {
                    Price = 0;
                    PricePerItem = "БАРДАК!";
                }

            }
        }

        public RequestItemViewModel(RequestItem item)
        {
            Id = item.id;
            _count = item.count;
            _price = item.price;
            _other = item.other;

            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                if (item.workTypeId > 0) _work = cntx.GetTable<WorkType>().SingleOrDefault(x => x.id == item.workTypeId);
                if (item.deviceId > 0) _dev = cntx.GetTable<Device>().SingleOrDefault(x => x.id == item.deviceId);
            }
        }

        private void UpdateProperty(string propertyName, object value)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var table = cntx.GetTable<RequestItem>();
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
