﻿using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Novacode;
using SourceProlongation.Base;
using SourceProlongation.Model;
using SourceProlongation.Properties;

namespace SourceProlongation.ViewModel
{
    public class OrderViewModel: ViewModelBase
    {
        private MainWindowViewModel _parent = null;

        #region Fields

        public string Name => (SelectedCustomer == null ? oldCustomerName : SelectedCustomer.name) + " [" + Number +
                              "-" + Year + "]"; 

        public int OrderId { get; }
        public int SourceCount => Sources.Count;

        private ObservableCollection<SourceViewModel> _sources = new ObservableCollection<SourceViewModel>();
        public ObservableCollection<SourceViewModel> Sources { get => _sources;
            set { _sources = value; OnPropertyChanged("Sources"); } }

        private SourceViewModel _selectedSource;
        public SourceViewModel SelectedSource { get => _selectedSource;
            set { _selectedSource = value; OnPropertyChanged("SelectedSource"); } }

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
                OnPropertyChanged("Name");

                if(value!=null) UpdateProperty("customerId", value.id);

            }
        }

        private CheckableObservableCollection<Executor> _executors = null;
        public CheckableObservableCollection<Executor> Executors
        {
            get => _executors;
            set => _executors = value;
        }

        private int _number;
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                OnPropertyChanged("Number");
                UpdateProperty("actNumber", value);
                OnPropertyChanged("IsActNumberValid");
                OnPropertyChanged("Name");
            }
        }

        private int _year = Int32.Parse(DateTime.Now.ToString("yy"));
        public int Year
        {
            get => _year;
            set
            {
                _year = value;
                OnPropertyChanged("Year");
                UpdateProperty("year", value);
                OnPropertyChanged("IsActNumberValid");
                OnPropertyChanged("Name");
            }
        }

        public bool IsActNumberValid
        {
            get
            {
                using (var cntx = new SqlDataContext(Connection.ConnectionString))
                {
                    return cntx.GetTable<Order>().Count(x => x.actNumber == Number && x.year == Year) == 1;
                }
            }
        }
        
        private DateTime _docDate = DateTime.Now;
        public DateTime DocDate { get => _docDate;
            set
            {
                _docDate = value; OnPropertyChanged("DocDate");
               // UpdateProperty("docDate", value);
            }}

        private DateTime _beginWorkDate = DateTime.Now.AddDays(-28);

        public DateTime BeginWorkDate
        {
            get => _beginWorkDate;
            set
            {
                if (value > DocDate) return;

                _beginWorkDate = value;
                OnPropertyChanged("BeginWorkDate");
                UpdateProperty("beginDate", value);
            }
        }
        
        private string _other;
        public string Other { get => _other;
            set{ _other = value; OnPropertyChanged("Other"); UpdateProperty("other", value); }
        }

        private string _status;
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (_status == value) return;
                _status = value;
                OnPropertyChanged("Status");
                UpdateProperty("status", value);
            }
        }
        #endregion

        #region LikeFirstBoolFlags
        private bool _lfNucleide = Settings.Default.lfNucleide;
        public bool LfNucleide
        {
            get => _lfNucleide;
            set
            {
                if (_lfNucleide == value) return;
                _lfNucleide = value;
                OnPropertyChanged("LfNucleide");
                Settings.Default.lfNucleide = value;
                Settings.Default.Save();
            }
        }

        private bool _lfType = Settings.Default.lfType;
        public bool LfType
        {
            get => _lfType;
            set
            {
                if (_lfType == value) return;
                _lfType = value;
                OnPropertyChanged("LfType");
                Settings.Default.lfType = value;
                Settings.Default.Save();
            }
        }

        private bool _lfYear = Settings.Default.lfYear;
        public bool LfYear
        {
            get => _lfYear;
            set
            {
                if (_lfYear == value) return;
                _lfYear = value;
                OnPropertyChanged("LfYear");
                Settings.Default.lfYear = value;
                Settings.Default.Save();
            }
        }

        private bool _lfUnit = Settings.Default.lfUnit;
        public bool LfUnit
        {
            get => _lfUnit;
            set
            {
                if (_lfUnit == value) return;
                _lfUnit = value;
                OnPropertyChanged("LfUnit");
                Settings.Default.lfUnit = value;
                Settings.Default.Save();
            }
        }

        private bool _lfPaspDate = Settings.Default.lfPaspDate;
        public bool LfPaspDate
        {
            get => _lfPaspDate;
            set
            {
                if (_lfPaspDate == value) return;
                _lfPaspDate = value;
                OnPropertyChanged("LfPaspDate");
                Settings.Default.lfPaspDate
 = value;
                Settings.Default.Save();
            }
        }

        private bool _lfMeasDate = Settings.Default.lfMeasDate;
        public bool LfMeasDate
        {
            get => _lfMeasDate;
            set
            {
                if (_lfMeasDate == value) return;
                _lfMeasDate = value;
                OnPropertyChanged("LfMeasDate");
                Settings.Default.lfMeasDate = value;
                Settings.Default.Save();
            }
        }

        private bool _lfIsvid = Settings.Default.lfIsSvid;
        public bool LfIsSvid
        {
            get => _lfIsvid;
            set
            {
                if (_lfIsvid == value) return;
                _lfIsvid = value;
                OnPropertyChanged("LfIsSvid");
                Settings.Default.lfIsSvid = value;
                Settings.Default.Save();
            }
        }

        private bool _lfExtPeriod = Settings.Default.lfExtPeriod;
        public bool LfExtPeriod
        {
            get => _lfExtPeriod;
            set
            {
                if (_lfExtPeriod == value) return;
                _lfExtPeriod = value;
                OnPropertyChanged("LfExtPeriod");
                Settings.Default.lfExtPeriod = value;
                Settings.Default.Save();
            }
        }
        #endregion 

        #region Commands
        public ICommand AddSourceCommand { get; private set; }
        public ICommand RemoveSourceCommand { get; private set; }
        public ICommand CopySourceCommand { get; private set; }
        public ICommand LikeFirstCommand { get; private set; }

        public ICommand MakeAktCommand { get; private set; }
        public ICommand MakeDocsCommand { get; private set; }
        public ICommand MakeMazkiCommand { get; private set; }

        public ICommand SetRankCommand { get; }

        private void AddSource(object obj)
        {
            var cnt = (int) obj;
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                for (int i = 0; i < cnt; i++)
                {
                    var model = new Source()
                    {
                        orderId = OrderId,
                        nucleideId = 0,
                        type = "",
                        number = "",
                        passport = "",
                        rank = "",
                        madeYear = -1,
                        extensionPeriod = -1,
                        baseValueDate = DateTime.MinValue,
                        baseValue = 0,
                        unit = "",
                        measValue = 0,
                        measDate = DateTime.MinValue,
                        docNumber = "",
                        isSvid = true
                    };

                    var sourcesTable = cntx.GetTable<Source>();
                    sourcesTable.InsertOnSubmit(model);
                    cntx.SubmitChanges();

                    Sources.Add(new SourceViewModel(model, _docDate));
                }
                SelectedSource = Sources.Last();
            }
        }

        private void RemoveSource(object obj)
        {
            IList items = (IList)obj;
            var sources = items.Cast<SourceViewModel>().ToList();
            if (!sources.Any()) return;

            var ids = sources.Select(x => x.Id).ToList();

            foreach (var id in ids)
            {
                using (var cntx = new SqlDataContext(Connection.ConnectionString))
                {
                    var sourcesTable = cntx.GetTable<Source>();
                    var toDelete = sourcesTable.Single(x => x.id == id);
                    sourcesTable.DeleteOnSubmit(toDelete);
                    cntx.SubmitChanges();

                    var src = Sources.Single(x => x.Id == id);
                    Sources.Remove(src);
                }
            }
        }

        private void CopySource(object obj)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var model = new Source()
                {
                    orderId = OrderId,
                    nucleideId = SelectedSource.Nucleide.Id,
                    type = SelectedSource.Type,
                    number = SelectedSource.Number,
                    passport = SelectedSource.PassportNum,
                    rank = SelectedSource.Rank,
                    madeYear = SelectedSource.MadeYear,
                    extensionPeriod = SelectedSource.ExtensionPeriod,
                    baseValueDate = SelectedSource.BaseValueDate,
                    baseValue = SelectedSource.BaseValue,
                    unit = SelectedSource.Unit,
                    measValue = SelectedSource.MeasValue,
                    measDate = SelectedSource.MeasDate,
                    docNumber = SelectedSource.Number,
                    isSvid = SelectedSource.IsSvid
                };

                var sourcesTable = cntx.GetTable<Source>();
                sourcesTable.InsertOnSubmit(model);
                cntx.SubmitChanges();

                Sources.Add(new SourceViewModel(model, _docDate));
                SelectedSource = Sources.Last();
            }
        }

        private void LikeFirst(object a)
        {
            IList items = (IList)a;
            var sources = items.Cast<SourceViewModel>().ToList();
            if (!sources.Any()) return;

            var first = sources.First();
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var table = cntx.GetTable<Source>();
                for (int i = 1; i < sources.Count; i++)
                {
                    var single = table.Single(x => x.id == sources[i].Id);
                    var cur = Sources.Single(x => x.Id == sources[i].Id);

                    if (LfYear)
                    {
                        single.madeYear = first.MadeYear;
                        cur.MadeYear = first.MadeYear;
                    }

                    if (LfExtPeriod)
                    {
                        single.extensionPeriod = first.ExtensionPeriod;
                        cur.ExtensionPeriod = first.ExtensionPeriod;
                    }

                    if (LfType)
                    {
                        single.type = first.Type;
                        cur.Type = first.Type;
                    }

                    if (LfUnit)
                    {
                        single.unit = first.Unit;
                        cur.Unit = first.Unit;
                    }

                    if (LfPaspDate)
                    {
                        single.baseValueDate = first.BaseValueDate;
                        cur.BaseValueDate = first.BaseValueDate;
                    }

                    if (LfMeasDate)
                    {
                        single.measDate = first.MeasDate;
                        cur.MeasDate = first.MeasDate;
                    }

                    if (LfIsSvid)
                    {
                        single.isSvid = first.IsSvid;
                        cur.IsSvid = first.IsSvid;
                    }

                    if (LfNucleide)
                    {
                        single.nucleideId = first.Nucleide.Id;
                        
                    }
                    cntx.SubmitChanges();

                    if (LfNucleide)
                    {
                        cur.Nucleide = first.Nucleide;
                    }
                }
            }
        }

        private void SetRank(object obj)
        {
            IList items = (IList)obj;
            var sources = items.Cast<SourceViewModel>().ToList();
            if (!sources.Any()) return;

            var rank = _parent.ComboboxRank;
            if (rank == null) return;

            sources.ForEach(x =>
                x.Rank = rank.rankv + 
                         (
                             !string.IsNullOrEmpty(rank.scheme) ? 
                                 ("(" + rank.scheme + ")") 
                                 : "")
                         );
        }

        private void MakeAct(object obj)
        {
            var dirPath = "[Пр-" + Number + "-" + DocDate.ToString("yy") + " " + SelectedCustomer.name + "]";
            dirPath = MyStatic.CleanPath(dirPath);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            MakeAct(dirPath);
        }

        private void MakeDocs(object obj)
        {
            var dirPath = "[Пр-" + Number + "-" + DocDate.ToString("yy") + " " + SelectedCustomer.name + "]";
            dirPath = MyStatic.CleanPath(dirPath);
            if(!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            
            MakeAct(dirPath);
            MakeExpConcl(dirPath);
            MakeExpConclApplication(dirPath);
            MakeProtokol(dirPath);
            MakeProtokolApplication(dirPath);
        }

        private void UpdateExecutors()
        {
            var executors = Executors.CheckedItems.Cast<CheckWrapper<Executor>>().ToList().Select(x => x.Value)
                .Select(x => x.id).ToList();
            var newList = string.Join(";", executors);
            UpdateProperty("executors", newList);
        }

        private void ReplaceBaseTags(DocX doc)
        {
            doc.ReplaceText("@Num", Number.ToString());
            doc.ReplaceText("@Year", DocDate.ToString("yy"));
            doc.ReplaceText("@DateAsWords", DocDate.ToString("dd MMMM yyyy") + " г.");

            doc.ReplaceText("@Cust_KOGO", SelectedCustomer.name_kogo);
            doc.ReplaceText("@Cust_KOMU", SelectedCustomer.name_komu);
            doc.ReplaceText("@Cust_KEM", SelectedCustomer.name_kem);

            doc.ReplaceText("@DateFrom", BeginWorkDate.ToShortDateString());
            doc.ReplaceText("@DateTo", DocDate.ToShortDateString());

            doc.ReplaceText("@SVID_AND_SERT_TAG",
                Sources.All(x => x.IsSvid) ?
                "свидетельств о поверке" :
                (Sources.All(x => !x.IsSvid) ? "сертификатов калибровки" : "свидетельств о поверке, сертификатов калибровки")
                );

        }
        private void MakeAct(string dir)
        {
            string actFile = @"Data\" + Collections.FileNames["Act"] + EXT;
            if (!File.Exists(actFile)) return;

            var doc = DocX.Load(actFile);
            ReplaceBaseTags(doc);
            doc.SaveAs(dir + "\\" + Collections.FileNames["Act"] + EXT);
        }
        private void MakeExpConcl(string dir)
        {
            string file = @"Data\" + Collections.FileNames["ExpConcl"] + EXT;
            if (!File.Exists(file)) return;

            var doc = DocX.Load(file);
            ReplaceBaseTags(doc);
            doc.SaveAs(dir + "\\" + Collections.FileNames["ExpConcl"] + EXT);
        }
        private void MakeExpConclApplication(string dir)
        {
            string file = @"Data\" + Collections.FileNames["ExpConclApplication"] + EXT;
            if (!File.Exists(file)) return;

            var doc = DocX.Load(file);
            ReplaceBaseTags(doc);

            var table = doc.Tables[0];
            int rowIndex = 3;
            foreach (var source in Sources)
            {
                table.InsertRow(rowIndex);

                var row = table.Rows[rowIndex];

                for (int i = 0; i < 9; i++)
                {
                    var num = row.Cells[i].Paragraphs[0].Append(source.GetExpApplText(i, rowIndex));
                    num.FontSize(TableFontsize);
                    num.Alignment = Alignment.center;
                }

                rowIndex++;
            }

            doc.SaveAs(dir + "\\" + Collections.FileNames["ExpConclApplication"] + EXT);
        }
        private void MakeProtokol(string dir)
        {
            string file = @"Data\" + Collections.FileNames["Protokol"] + EXT;
            if (!File.Exists(file)) return;

            var doc = DocX.Load(file);
            ReplaceBaseTags(doc);

            var executorsTable = doc.Tables[0];
            var jobExecutors = Executors.CheckedItems.Cast<CheckWrapper<Executor>>().ToList();
            for (int k = 0; k < jobExecutors.Count; k++)
            {
                for (int i = 0; i < 3; i++)
                {
                    executorsTable.InsertRow(1 + i);
                }
            }

            var transp = new Border(BorderStyle.Tcbs_single, BorderSize.one, 1, Color.Transparent);

            for (int i = 1; i < executorsTable.RowCount; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    executorsTable.Rows[i].Cells[j].SetBorder(TableCellBorderType.Left, transp);
                    executorsTable.Rows[i].Cells[j].SetBorder(TableCellBorderType.Right, transp);
                    executorsTable.Rows[i].Cells[j].SetBorder(TableCellBorderType.Top, transp);
                    executorsTable.Rows[i].Cells[j].SetBorder(TableCellBorderType.Bottom, transp);
                }
            }

            var podpBorder = new Border(BorderStyle.Tcbs_single, BorderSize.one, 1, Color.Black);
            for (int i = 1; i < executorsTable.RowCount; i++)
            {
                if ((i + 2) % 3 == 0)
                    executorsTable.Rows[i].Cells[2].SetBorder(TableCellBorderType.Bottom, podpBorder);
            }

            
            for (int k = 0; k < jobExecutors.Count; k++)
            {
                var executor = jobExecutors[k].Value;
                var p1 = executorsTable.Rows[1 + k * 3].Cells[0].Paragraphs[0].Append(executor.post);
                p1.FontSize(14);

                var p2 = executorsTable.Rows[2 + k * 3].Cells[2].Paragraphs[0].Append("(подпись)");
                p2.FontSize(14);
                p2.Alignment = Alignment.center;

                var p3 = executorsTable.Rows[1 + k * 3].Cells[4].Paragraphs[0].Append(executor.fio);
                p3.FontSize(14);
            }

            executorsTable.SetColumnWidth(0, 4500);


            doc.SaveAs(dir + "\\" + Collections.FileNames["Protokol"] + EXT);
        }
        private void MakeProtokolApplication(string dir)
        {
            string file = @"Data\" + Collections.FileNames["ProtokolApplication"] + EXT;
            if (!File.Exists(file)) return;

            var doc = DocX.Load(file);
            ReplaceBaseTags(doc);
            
            var table = doc.Tables[0];
            int rowIndex = 3;
            foreach (var source in Sources)
            {
                table.InsertRow(rowIndex);
                var row = table.Rows[rowIndex];

                for (int i = 0; i < 13; i++)
                {
                    var num = row.Cells[i].Paragraphs[0].Append(source.GetProtApplText(i, rowIndex));
                    num.FontSize(10);
                    num.Alignment = Alignment.center;
                }
                rowIndex++;
            }


            //Исполнители
            var executorsTable = doc.Tables[1];
            var jobExecutors = Executors.CheckedItems.Cast<CheckWrapper<Executor>>().ToList();
            for (int k = 0; k < jobExecutors.Count; k++)
            {
                for (int i = 0; i < 2; i++)
                {
                    executorsTable.InsertRow(1 + i);
                }
            }

            var transp = new Border(BorderStyle.Tcbs_single, BorderSize.one, 1, Color.Transparent);

            for (int i = 1; i < executorsTable.RowCount; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    executorsTable.Rows[i].Cells[j].SetBorder(TableCellBorderType.Left, transp);
                    executorsTable.Rows[i].Cells[j].SetBorder(TableCellBorderType.Right, transp);
                    executorsTable.Rows[i].Cells[j].SetBorder(TableCellBorderType.Top, transp);
                    executorsTable.Rows[i].Cells[j].SetBorder(TableCellBorderType.Bottom, transp);
                }
            }

            var podpBorder = new Border(BorderStyle.Tcbs_single, BorderSize.one, 1, Color.Black);
            for (int i = 1; i < executorsTable.RowCount; i++)
            {
                if ((i + 1) % 2 == 0)
                    executorsTable.Rows[i].Cells[1].SetBorder(TableCellBorderType.Bottom, podpBorder);
            }

            
            for (int k = 0; k < jobExecutors.Count; k++)
            {
                var executor = jobExecutors[k].Value;
                var p1 = executorsTable.Rows[1 + k * 2].Cells[0].Paragraphs[0].Append(executor.post);
                p1.FontSize(12);

                var p3 = executorsTable.Rows[1 + k * 2].Cells[3].Paragraphs[0].Append(executor.fio);
                p3.FontSize(12);
            }

            executorsTable.SetColumnWidth(0, 4500);

            doc.SaveAs(dir + "\\" + Collections.FileNames["ProtokolApplication"] + EXT);
        }
        #endregion

        private string oldCustomerName = "";

        public OrderViewModel(Order order, MainWindowViewModel parent)
        {
            _parent = parent;
            //TODO: Где делать override, если что
            //Customer
            //Nucleide
            //WorkType
            //Device
            //Executor
            //Ranks

            OrderId = order.id;
            Sources.CollectionChanged += (a, b) => { OnPropertyChanged("SourceCount"); };

            _number = order.actNumber;
            _year = order.year;
            _other = order.other;
            _docDate = order.docDate;
            _beginWorkDate = order.beginDate;
            _status = order.status;
            
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var customersTable = cntx.GetTable<Customer>();
                _selectedCustomer = customersTable.SingleOrDefault(x => x.id == order.customerId);

                _executors = new CheckableObservableCollection<Executor>();
                foreach (var executor in cntx.GetTable<Executor>())
                {
                    _executors.Add(executor);
                }

                if (order.executors.Length > 0)
                {
                    var ids = order.executors.Split(';').Select(int.Parse).ToList();
                    foreach (var checkWrapper in _executors)
                    {
                        if (ids.Contains(checkWrapper.Value.id))
                        {
                            checkWrapper.IsChecked = true;
                        }
                    }
                }
                _executors.CheckedChangedEvent += (a, b) => UpdateExecutors();

                oldCustomerName = order.customerId < 0 ? "[!] " + order.oldCustomerName : "";

                var sources = cntx.GetTable<Source>().Where(x => x.orderId == order.id).ToList();
                foreach (var source in sources)
                {
                    var added = new SourceViewModel(source, _docDate);
                    added.RecalculateParameters();
                    Sources.Add(added);
                }
                if (Sources.Any()) SelectedSource = Sources[0];
            }

            AddSourceCommand = new RelayCommand(AddSource);
            RemoveSourceCommand = new RelayCommand(RemoveSource, x=>SelectedSource!=null);
            CopySourceCommand = new RelayCommand(CopySource, x => SelectedSource != null);
            LikeFirstCommand = new RelayCommand(LikeFirst, x=>SelectedSource != null);
            MakeAktCommand = new RelayCommand(MakeAct, a=> SelectedCustomer !=null && IsActNumberValid);
            MakeDocsCommand = new RelayCommand(MakeDocs, a => SelectedCustomer != null && IsActNumberValid);
            MakeMazkiCommand = new RelayCommand(a => { MessageBox.Show("Not implemented!");});
            SetRankCommand = new RelayCommand(SetRank, x => SelectedSource != null);
        }
        
        public const string EXT = ".docx";
        private const int TableFontsize = 12;

        private void UpdateProperty(string propertyName, object value)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var table = cntx.GetTable<Order>();
                var item = table.Single(x => x.id == OrderId);

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
