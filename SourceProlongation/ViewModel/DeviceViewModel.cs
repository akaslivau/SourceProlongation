using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SourceProlongation.Base;

namespace SourceProlongation.ViewModel
{
    public class DeviceViewModel:ViewModelBase
    {
        #region Fields
        public int Id { get; }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged("Name");
                UpdateProperty("name", value);
            }
        }
        
        private string _type;
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (_type == value) return;
                _type = value;
                OnPropertyChanged("Type");
                UpdateProperty("type", value);
            }
        }

        private string _reestr;
        public string Reestr
        {
            get
            {
                return _reestr;
            }
            set
            {
                if (_reestr == value) return;
                _reestr = value;
                OnPropertyChanged("Reestr");
                UpdateProperty("reestr", value);
            }
        }

        #endregion


        public DeviceViewModel(Device d)
        {
            Id = d.id;
            _name = d.name;
            _type = d.type;
            _reestr = d.reestr;
        }


        private void UpdateProperty(string propertyName, object value)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var table = cntx.GetTable<Device>();
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
