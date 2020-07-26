using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceProlongation.ViewModel;

namespace SourceProlongation.Model
{
    public static class Collections
    {
        private static ObservableCollection<NucleideViewModel> _nucleidesCollection = null;

        public static ObservableCollection<NucleideViewModel> NucleidesCollection
        {
            get
            {
                if (_nucleidesCollection == null)
                {
                    _nucleidesCollection = new ObservableCollection<NucleideViewModel>();

                    using (var cntx = new SqlDataContext(Connection.Con))
                    {
                        var table = cntx.GetTable<Nucleide>();
                        foreach (var nucleide in table)
                        {
                            _nucleidesCollection.Add(new NucleideViewModel(nucleide));
                        }
                    }
                }

                return _nucleidesCollection;
            }
        }

        public static readonly Dictionary<string, string> FileNames = new Dictionary<string, string>
        {
            {"Act", "1 Акт"},
            {"ExpConcl", "2 Экспертное заключение"},
            {"ExpConclApplication", "3 Приложение к ЭЗ"},
            {"Protokol", "4 Протокол ИС"},
            {"ProtokolApplication", "5 Приложение к ИС"},
        };

        private static ObservableCollection<Customer> _customers = null;

        public static ObservableCollection<Customer> Customers
        {
            get
            {
                if (_customers == null)
                {
                    _customers = new ObservableCollection<Customer>();

                    using (var cntx = new SqlDataContext(Connection.ConnectionString))
                    {
                        foreach (var customer in cntx.GetTable<Customer>().ToList())
                        {
                            _customers.Add(customer);
                        }
                    }
                }
                return _customers;
            }
        }

        private static ObservableCollection<WorkType> _workTypes = null;
        public static ObservableCollection<WorkType> WorkTypes
        {
            get
            {
                if (_workTypes == null)
                {
                    _workTypes = new ObservableCollection<WorkType>();

                    using (var cntx = new SqlDataContext(Connection.ConnectionString))
                    {
                        foreach (var w in cntx.GetTable<WorkType>().ToList())
                        {
                            _workTypes.Add(w);
                        }
                    }
                }
                return _workTypes;
            }
        }

        private static ObservableCollection<Device> _devices = null;
        public static ObservableCollection<Device> Devices
        {
            get
            {
                if (_devices == null)
                {
                    _devices = new ObservableCollection<Device>();

                    using (var cntx = new SqlDataContext(Connection.ConnectionString))
                    {
                        foreach (var w in cntx.GetTable<Device>().ToList())
                        {
                            _devices.Add(w);
                        }
                    }
                }
                return _devices;
            }
        }

        private static ObservableCollection<Price> _prices = null;
        public static ObservableCollection<Price> Prices
        {
            get
            {
                if (_prices == null)
                {
                    _prices = new ObservableCollection<Price>();

                    using (var cntx = new SqlDataContext(Connection.ConnectionString))
                    {
                        foreach (var w in cntx.GetTable<Price>().ToList())
                        {
                            _prices.Add(w);
                        }
                    }
                }
                return _prices;
            }
        }

        private static ObservableCollection<Rank> _ranks = null;
        public static ObservableCollection<Rank> Ranks
        {
            get
            {
                if (_ranks == null)
                {
                    _ranks = new ObservableCollection<Rank>();

                    using (var cntx = new SqlDataContext(Connection.ConnectionString))
                    {
                        foreach (var w in cntx.GetTable<Rank>().ToList())
                        {
                            _ranks.Add(w);
                        }
                    }
                }
                return _ranks;
            }
        }

        public static ObservableCollection<string> Statuses { get; } = new ObservableCollection<string>()
        {
            Status.Created.ToString(),
            Status.InProgress.ToString(),
            Status.NeedSigning.ToString(),
            Status.SentToRst.ToString(),
            Status.CameFromRst.ToString(),
            Status.Finished
        };
    }

    public static class Todelete
    {

    }

}


