using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SourceProlongation.Base;

namespace SourceProlongation.ViewModel
{
    public class TripViewModel:ViewModelBase
    {
        #region Fields
        public int Id { get; }

        private string _place = "";

        public string Place
        {
            get { return _place; }
            set
            {
                _place = value;
                OnPropertyChanged("Place");
                UpdateProperty("destination", value);
            }
        }

        private int _days = 0;

        public int Days
        {
            get { return _days; }
            set
            {
                _days = value;
                OnPropertyChanged("Days");
                UpdateProperty("days", value);
            }
        }

        private double _sum = 0;

        public double Sum
        {
            get { return _sum; }
            set
            {
                _sum = value;
                OnPropertyChanged("Sum");
                UpdateProperty("sum", value);
            }
        }


        private string _other = "";

        public string Other
        {
            get { return _other; }
            set
            {
                _other = value;
                OnPropertyChanged("Other");
                UpdateProperty("other", value);
            }
        }


        #endregion


        public TripViewModel(RequestTrip t)
        {
            Id = t.id;
            _days = t.days;
            _other = t.other;
            _place = t.destination;
            _sum = t.sum;
        }


        private void UpdateProperty(string propertyName, object value)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var table = cntx.GetTable<RequestTrip>();
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
