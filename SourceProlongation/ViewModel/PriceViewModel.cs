using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SourceProlongation.Base;

namespace SourceProlongation.ViewModel
{
    public class PriceViewModel: ViewModelBase
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
            }
        }

        private int _from;
        public int From
        {
            get
            {
                return _from;
            }
            set
            {
                if (_from == value) return;
                _from = value;
                OnPropertyChanged("From");
                UpdateProperty("fromCount", value);
            }
        }

        private int _to;
        public int To
        {
            get
            {
                return _to;
            }
            set
            {
                if (_to == value) return;
                _to = value;
                OnPropertyChanged("To");
                UpdateProperty("toCount", value);
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
                UpdateProperty("pricePerItem", value);
            }
        }

        #endregion

        public PriceViewModel(Price p)
        {
            Id = p.id;

            _from = p.fromCount;
            _to = p.toCount;
            _price = p.pricePerItem;


            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                if (p.workTypeId > 0) _work = cntx.GetTable<WorkType>().SingleOrDefault(x => x.id == p.workTypeId);
                if (p.deviceId > 0) _dev = cntx.GetTable<Device>().SingleOrDefault(x => x.id == p.deviceId);
            }
        }

        private void UpdateProperty(string propertyName, object value)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var table = cntx.GetTable<Price>();
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
