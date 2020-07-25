using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
using Novacode;
using SourceProlongation.Base;

namespace SourceProlongation.ViewModel
{
    public class OrderViewModel: ViewModelBase
    {
        #region Fields
        private string _customer = "";
        public string Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                OnPropertyChanged("Customer");
                OnPropertyChanged("DisplayName");
            }
        }

        //В родительном падеже
        private string _customer_KOGO = "";
        public string Customer_KOGO { get { return _customer_KOGO; } set { _customer_KOGO = value; OnPropertyChanged("Customer_KOGO"); } }

        //В дательном падеже
        private string _customer_KOMU = "";
        public string Customer_KOMU { get { return _customer_KOMU; } set { _customer_KOMU = value; OnPropertyChanged("Customer_KOMU"); } }

        //В творительном падеже
        private string _customer_KEM = "";
        public string Customer_KEM { get { return _customer_KEM; } set { _customer_KEM = value; OnPropertyChanged("Customer_KEM"); } }


        private int _number;
        public int Number
        {
            get { return _number; }
            set
            {
                _number = value;
                OnPropertyChanged("Number");
                OnPropertyChanged("ActNumber");
                OnPropertyChanged("DisplayName");
            }
        }

        private int _year = Int32.Parse(DateTime.Now.ToString("yy"));
        public int Year
        {
            get { return _year; }
            set
            {
                _year = value;
                OnPropertyChanged("Year");
                OnPropertyChanged("ActNumber");
                OnPropertyChanged("DisplayName");
            }
        }


        public string ActNumber { get { return Number + "/" + Year; } }

        private DateTime _docDate = DateTime.Now;
        public DateTime DocDate { get { return _docDate;} set{ _docDate = value; OnPropertyChanged("DocDate");}}

        private DateTime _beginWorkDate = DateTime.Now.AddDays(-14);

        public DateTime BeginWorkDate
        {
            get { return _beginWorkDate; }
            set
            {
                if (value > EndWorkDate) return;
                if (value > DocDate) return;

                _beginWorkDate = value;
                OnPropertyChanged("BeginWorkDate");
            }
        }

        private DateTime _endWorkDate = DateTime.Now;

        public DateTime EndWorkDate
        {
            get { return _endWorkDate; }
            set
            {
                if (value < BeginWorkDate) return;
                if (value > DocDate) return;

                _endWorkDate = value;
                OnPropertyChanged("EndWorkDate");
            }
        }


        private string _comment = "";
        public string Comment { get { return _comment;} set{ _comment = value; OnPropertyChanged("Comment");}}


        private string _status = Collections.Statuses[1];
        public string Status { get { return _status;} set{ _status = value; OnPropertyChanged("Status");}}


        public int SourceCount
        {
            get { return Sources.Count; }
        }


        #endregion

        #region Executors
        public ExecutorViewModel SelectedComboboxExecutor { get; set; }


        private ObservableCollection<ExecutorViewModel> _executors = new ObservableCollection<ExecutorViewModel>();
        public ObservableCollection<ExecutorViewModel> Executors { get { return _executors;} set{ _executors = value; OnPropertyChanged("Executors");}}


        private ExecutorViewModel _selectedExecutor;
        public ExecutorViewModel SelectedExecutor { get { return _selectedExecutor;} set{ _selectedExecutor = value; OnPropertyChanged("SelectedExecutor");}}

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

        #region Sources
        private ObservableCollection<SourceViewModel> _sources = new ObservableCollection<SourceViewModel>();
        public ObservableCollection<SourceViewModel> Sources { get { return _sources;} set{ _sources = value; OnPropertyChanged("Sources");}}

        private SourceViewModel _selectedSource;
        public SourceViewModel SelectedSource { get { return _selectedSource;} set{ _selectedSource = value; OnPropertyChanged("SelectedSource");}}

        public ICommand AddSourceCommand { get; private set; }
        public ICommand CopySourceCommand { get; private set; }

        public ICommand SerializeCommand { get; private set; }
        public ICommand DeserializeCommand { get; private set; }
        #endregion

        #region Checkboxes for final set
        private bool _hasLicense;
        public bool HasLicense { get { return _hasLicense; } set { _hasLicense = value; OnPropertyChanged("HasLicense"); OnPropertyChanged("IsDocumentSetOk"); } }

        private bool _hasSpravka;
        public bool HasSpravka { get { return _hasSpravka; } set { _hasSpravka = value; OnPropertyChanged("HasSpravka"); OnPropertyChanged("IsDocumentSetOk"); } }

        private bool _hasPassports;
        public bool HasPassports { get { return _hasPassports; } set { _hasPassports = value; OnPropertyChanged("HasPassports"); OnPropertyChanged("IsDocumentSetOk"); } }

        private bool _hasRadProtokol;
        public bool HasRadProtokol { get { return _hasRadProtokol; } set { _hasRadProtokol = value; OnPropertyChanged("HasRadProtokol"); OnPropertyChanged("IsDocumentSetOk"); } }

        public bool IsDocumentSetOk
        {
            get { return HasLicense && HasSpravka && HasPassports && HasRadProtokol; }
        }

        public ICommand SaveDocumentSetCommand { get; private set; }

        private void SaveDocumentSet()
        {
            var dirPath = "[Пр-" + Number + "-" + DocDate.ToString("yy") + " " + Customer + "]";
            dirPath = MyStatic.CleanPath(dirPath);
            Directory.CreateDirectory(dirPath);

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

            doc.ReplaceText("@Cust_KOGO", Customer_KOGO);
            doc.ReplaceText("@Cust_KOMU", Customer_KOMU);
            doc.ReplaceText("@Cust_KEM", Customer_KEM);

            doc.ReplaceText("@DateFrom", BeginWorkDate.ToShortDateString());
            doc.ReplaceText("@DateTo", EndWorkDate.ToShortDateString());

            doc.ReplaceText("@SVID_AND_SERT_TAG",
                Sources.All(x => x.IsSvid) ?
                "свидетельств о поверке" :
                (Sources.All(x => !x.IsSvid) ? "сертификатов калибровки" : "свидетельств о поверке, сертификатов калибровки")
                );

        }



        public void MakeAct(string dir)
        {
            string actFile = @"Data\" + Collections.FileNames["Act"] + EXT;
            if (!File.Exists(actFile)) return;

            var doc = DocX.Load(actFile);
            ReplaceBaseTags(doc);
            doc.SaveAs(dir + "\\" + Collections.FileNames["Act"] + EXT);
        }

        public void MakeExpConcl(string dir)
        {
            string file = @"Data\" + Collections.FileNames["ExpConcl"] + EXT;
            if (!File.Exists(file)) return;

            var doc = DocX.Load(file);
            ReplaceBaseTags(doc);
            doc.SaveAs(dir + "\\" + Collections.FileNames["ExpConcl"] + EXT);
        }

        private const int TableFontsize = 12;

        public void MakeExpConclApplication(string dir)
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

        #region Table move
        public ICommand MoveUpCommand { get; private set; }
        public ICommand MoveDownCommand { get; private set; }
        
        private void MoveUp()
        {
            var indexes = new List<int>();
            for (int i = 0; i < Sources.Count; i++)
            {
                if(Sources[i].IsChecked) indexes.Add(i);
            }

            indexes = indexes.Where(x => x > 0).ToList();
            if (indexes.Count == 0) return;

            for (int i = 0; i < indexes.Count; i++)
            {
                var index = indexes[i];
                var itemToMove = Sources[index];
                Sources.RemoveAt(index);
                Sources.Insert(index - 1, itemToMove);
            }
            SelectedSource = Sources[indexes[0] - 1];
        }

        private void MoveDown()
        {
            var indexes = new List<int>();
            for (int i = SourceCount-1; i >= 0; i--)
            {
                if (Sources[i].IsChecked) indexes.Add(i);
            }
            
            indexes = indexes.Where(x => x < Sources.Count - 1).ToList();
            if (indexes.Count == 0) return;

            for (int i = 0; i < indexes.Count; i++)
            {
                var index = indexes[i];
                var itemToMove = Sources[index];
                Sources.RemoveAt(index);
                Sources.Insert(index + 1, itemToMove);
            }
            SelectedSource = Sources[indexes[0] + 1];
        }
        #endregion

        public ICommand SaveJsonCommand { get; private set; }

        public OrderViewModel(Order order)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var cust = order.customerId < 0 ? "[!] " + order.oldCustomerName
                    : cntx.GetTable<Customer>().Single(x => x.id == order.customerId).name;
                base.DisplayName = cust + " " + order.actNumber + "/" + order.year;
            }

/*

            CanBeClosed = true;
            //Исполнители
            AddExecutorCommand = new RelayCommand(a => AddExecutor());
            RemoveExecutorCommand = new RelayCommand(a => RemoveExecutor());

            //Источники
            Sources.CollectionChanged += (a, b) => OnPropertyChanged("SourceCount");
            AddSourceCommand = new RelayCommand(a => Sources.Add(new SourceViewModel(this)));
            CopySourceCommand = new RelayCommand(a => Sources.Add(new SourceViewModel(SelectedSource, this)),
                a => SelectedSource != null);

            SerializeCommand = new RelayCommand(a =>
            {
                var sDlg = new SaveFileDialog();
                sDlg.Filter = "Источник (.source)|*.source";
                sDlg.FileName = SelectedSource.Type + " №" + SelectedSource.Number;
                if (sDlg.ShowDialog() == true)
                {
                    var res = JsonConvert.SerializeObject(SelectedSource);
                    File.WriteAllText(sDlg.FileName, res);
                }
            },
                a => SelectedSource != null);

            DeserializeCommand = new RelayCommand(a =>
            {
                var oDlg = new OpenFileDialog();
                oDlg.Filter = "Источник (.source)|*.source";
                if (oDlg.ShowDialog() == true)
                {
                    var source = JsonConvert.DeserializeObject<SourceViewModel>(File.ReadAllText(oDlg.FileName));
                    Sources.Add(new SourceViewModel(source, this));
                }
            });

            //Создание комплекта
            SaveDocumentSetCommand = new RelayCommand(a => SaveDocumentSet(), a => IsDocumentSetOk);

            //Перемещение источников по таблице
            MoveUpCommand = new RelayCommand(a=>MoveUp());
            MoveDownCommand = new RelayCommand(a=>MoveDown());


            //Закрытие окна
            SaveJsonCommand = new RelayCommand(a =>
            {
                var res = JsonConvert.SerializeObject(this);

                var fileName = Number + "_" + DocDate.ToString("yy") + " " + Customer;
                fileName = MyStatic.CleanPath(fileName);
                File.WriteAllText(Strings.ORDER_FOLDER + "//" + fileName, res);
            });
            RequestClose += (sender, args) => SaveJsonCommand.Execute(null);*/
        }

        public const string EXT = ".docx";
    }
}
