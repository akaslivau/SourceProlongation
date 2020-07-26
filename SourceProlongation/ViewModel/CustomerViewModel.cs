using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SourceProlongation.Base;

namespace SourceProlongation.ViewModel
{
    public class CustomerViewModel:ViewModelBase
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

        private string _nameKogo;
        public string NameKogo
        {
            get
            {
                return _nameKogo;
            }
            set
            {
                if (_nameKogo == value) return;
                _nameKogo = value;
                OnPropertyChanged("NameKogo");
                UpdateProperty("name_kogo", value);
            }
        }

        private string _nameKomu;
        public string NameKomu
        {
            get
            {
                return _nameKomu;
            }
            set
            {
                if (_nameKomu == value) return;
                _nameKomu = value;
                OnPropertyChanged("NameKomu");
                UpdateProperty("name_komu", value);
            }
        }

        private string _nameKem;
        public string NameKem
        {
            get
            {
                return _nameKem;
            }
            set
            {
                if (_nameKem == value) return;
                _nameKem = value;
                OnPropertyChanged("NameKem");
                UpdateProperty("name_kem", value);
            }
        }

        private string _fio;
        public string Fio
        {
            get
            {
                return _fio;
            }
            set
            {
                if (_fio == value) return;
                _fio = value;
                OnPropertyChanged("Fio");
                UpdateProperty("fio", value);
            }
        }

        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                if (_email == value) return;
                _email = value;
                OnPropertyChanged("Email");
                UpdateProperty("email", value);
            }
        }
        private string _phone;
        public string Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                if (_phone == value) return;
                _phone = value;
                OnPropertyChanged("Phone");
                UpdateProperty("phone", value);
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

        public CustomerViewModel(Customer c)
        {
            Id = c.id;

            _name = c.name;
            _nameKem = c.name_kem;
            _nameKogo = c.name_kogo;
            _nameKomu = c.name_komu;
            _fio = c.fio;
            _phone = c.phone;
            _email = c.email;
            _other = c.other;
        }

        private void UpdateProperty(string propertyName, object value)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var table = cntx.GetTable<Customer>();
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
