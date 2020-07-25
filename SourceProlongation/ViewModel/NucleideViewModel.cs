using System.Linq;
using System.Reflection;
using SourceProlongation.Base;

namespace SourceProlongation.ViewModel
{
    public class NucleideViewModel: ViewModelBase
    {
        #region Fields
        public int Id { get; }

        private string _name = "";
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
                UpdateProperty("name", value);
            }
        }

        private double _halfPeriod;
        public double HalfPeriod
        {
            get => _halfPeriod;
            set
            {
                _halfPeriod = value;
                OnPropertyChanged("HalfPeriod");
                UpdateProperty("halfPeriod", value);
            }
        }

        private void UpdateProperty(string propertyName, object value)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var table = cntx.GetTable<Nucleide>();
                var item = table.Single(x => x.id == Id);

                PropertyInfo prop = item.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanWrite)
                {
                    prop.SetValue(item, value, null);
                }
                cntx.SubmitChanges();
            }
        }
        #endregion

        public NucleideViewModel(Nucleide nucleide)
        {
            if (nucleide == null) return;
            Id = nucleide.id;
            _name = nucleide.name;
            _halfPeriod = nucleide.halfPeriod;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
