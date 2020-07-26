using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SourceProlongation.Base;

namespace SourceProlongation.ViewModel
{
    public class RankViewModel:ViewModelBase
    {
        #region Fields
        public int Id { get; }

        private string _scheme;
        public string Scheme
        {
            get
            {
                return _scheme;
            }
            set
            {
                if (_scheme == value) return;
                _scheme = value;
                OnPropertyChanged("Scheme");
                UpdateProperty("scheme", value);
            }
        }

        private string _rank;
        public string Rank
        {
            get
            {
                return _rank;
            }
            set
            {
                if (_rank == value) return;
                _rank = value;
                OnPropertyChanged("Rank");
                UpdateProperty("rankv", value);
            }
        }
        #endregion

        public RankViewModel(Rank r)
        {
            Id = r.id;
            _rank = r.rankv;
            _scheme = r.scheme;
        }

        private void UpdateProperty(string propertyName, object value)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var table = cntx.GetTable<Rank>();
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
