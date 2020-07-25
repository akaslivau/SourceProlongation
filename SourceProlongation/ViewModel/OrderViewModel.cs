using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
using Novacode;
using SourceProlongation.Base;
using SourceProlongation.Model;
using SourceProlongation.Properties;

namespace SourceProlongation.ViewModel
{
    public class OrderViewModel: ViewModelBase
    {
        #region Fields
        public int OrderId { get; }
        public int SourceCount => Sources.Count;
        private bool _hideFinished = Settings.Default.hideFinished;
        public bool HideFinished
        {
            get => _hideFinished;
            set
            {
                if (_hideFinished == value) return;
                _hideFinished = value;
                OnPropertyChanged("HideFinished");
                Settings.Default.hideFinished = value;
                Settings.Default.Save();
            }
        }

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

                if(value!=null) UpdateProperty("customerId", value.id);
            }
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
            set{ _other = value; OnPropertyChanged("Other");}}

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

        #region Executors
        public ExecutorViewModel SelectedComboboxExecutor { get; set; }


        private ObservableCollection<ExecutorViewModel> _executors = new ObservableCollection<ExecutorViewModel>();
        public ObservableCollection<ExecutorViewModel> Executors { get => _executors;
            set{ _executors = value; OnPropertyChanged("Executors");}}


        private ExecutorViewModel _selectedExecutor;
        public ExecutorViewModel SelectedExecutor { get => _selectedExecutor;
            set{ _selectedExecutor = value; OnPropertyChanged("SelectedExecutor");}}

        public ICommand AddExecutorCommand { get; private set; }

        public ICommand RemoveExecutorCommand { get; private set; }

        private void AddExecutor()
        {
            if (Executors.Any(x => x.Name == SelectedComboboxExecutor.Name)) return;
            Executors.Add(new ExecutorViewModel(SelectedComboboxExecutor.Position, SelectedComboboxExecutor.Name));
        }

        private void RemoveExecutor()
        {
            if (SelectedExecutor == null) return;
            Executors.Remove(SelectedExecutor);
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

        private void AddSource(object obj)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
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
                SelectedSource = Sources.Last();
            }
        }

        private void RemoveSource(object obj)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var sourcesTable = cntx.GetTable<Source>();
                var toDelete = sourcesTable.Single(x => x.id == SelectedSource.Id);
                sourcesTable.DeleteOnSubmit(toDelete);
                cntx.SubmitChanges();

                Sources.Remove(SelectedSource);
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
                    extensionPeriod = SelectedSource.AdditionalPeriod,
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
                        single.extensionPeriod = first.AdditionalPeriod;
                        cur.AdditionalPeriod = first.AdditionalPeriod;
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
            for (int k = 0; k < Executors.Count; k++)
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

            for (int k = 0; k < Executors.Count; k++)
            {
                var executor = Executors[k];
                var p1 = executorsTable.Rows[1 + k * 3].Cells[0].Paragraphs[0].Append(executor.Position);
                p1.FontSize(14);

                var p2 = executorsTable.Rows[2 + k * 3].Cells[2].Paragraphs[0].Append("(подпись)");
                p2.FontSize(14);
                p2.Alignment = Alignment.center;

                var p3 = executorsTable.Rows[1 + k * 3].Cells[4].Paragraphs[0].Append(executor.Name);
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
            for (int k = 0; k < Executors.Count; k++)
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

            for (int k = 0; k < Executors.Count; k++)
            {
                var executor = Executors[k];
                var p1 = executorsTable.Rows[1 + k * 2].Cells[0].Paragraphs[0].Append(executor.Position);
                p1.FontSize(12);

                var p3 = executorsTable.Rows[1 + k * 2].Cells[3].Paragraphs[0].Append(executor.Name);
                p3.FontSize(12);
            }

            executorsTable.SetColumnWidth(0, 4500);

            doc.SaveAs(dir + "\\" + Collections.FileNames["ProtokolApplication"] + EXT);
        }
        #endregion
        
        public OrderViewModel(Order order)
        {
            //TODO: скрывать завершенные по-человечески
            //TODO: список исполнителей
            //TODO: генератор расчета стоимости
            //TODO: расчет значений
            //TODO: ранки и поверочные схемы

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

                var cust = order.customerId < 0 ? "[!] " + order.oldCustomerName
                    : cntx.GetTable<Customer>().Single(x => x.id == order.customerId).name;
                base.DisplayName = cust + " " + order.actNumber + "/" + order.year;

                var sources = cntx.GetTable<Source>().Where(x => x.orderId == order.id).ToList();
                foreach (var source in sources)
                {
                    Sources.Add(new SourceViewModel(source, _docDate));
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
