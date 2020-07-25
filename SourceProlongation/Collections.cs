using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceProlongation.ViewModel;

namespace SourceProlongation
{
    public static class Collections
    {
        private static ObservableCollection<ExecutorViewModel> _executors = null;
        public static ObservableCollection<ExecutorViewModel> Executors
        {
            get
            {
                if (_executors == null)
                {
                    //Executors
                    var lines =
                        File.ReadAllLines(Strings.EXEC_PATH, Encoding.GetEncoding(1251)).Where(x => x.Length > 1).ToList();

                    _executors = new ObservableCollection<ExecutorViewModel>();
                    foreach (var newEx in
                        from line in lines
                        select line.Split(new[] { "\t" }, StringSplitOptions.None)
                            into exData
                            where exData.Count() == 2
                            select new ExecutorViewModel(exData[0], exData[1]))
                    {
                       _executors.Add(newEx);
                    }
                }
                return _executors;
            }
        }

        private static ObservableCollection<NucleideViewModel> _nucleides = null;

        public static ObservableCollection<NucleideViewModel> Nucleides
        {
            get
            {
                if (_nucleides == null)
                {
                    _nucleides = new ObservableCollection<NucleideViewModel>();
                    var data = File.ReadAllLines(Strings.NUCLEIDES_PATH, Encoding.GetEncoding(1251)).Where(x => x.Length > 0).ToList();
                    foreach (var item in data)
                    {
                        var dt = item.Split(new[] { "\t" }, StringSplitOptions.None);
                        if (dt.Count() != 2) continue;
                        _nucleides.Add(new NucleideViewModel(dt[0], double.Parse(dt[1])));
                    }
                }
                return _nucleides;
            }
        }

        private static ObservableCollection<string> _ranks = null;
        public static ObservableCollection<string> Ranks
        {
            get
            {
                if (_ranks == null)
                {
                    _ranks = new ObservableCollection<string>(File.ReadAllLines(Strings.RANK_PATH, Encoding.GetEncoding(1251)).Where(x => x.Length > 1).ToList());
                }
                return _ranks;
            }
        }

        private static ObservableCollection<string> _statuses = null;

        public static ObservableCollection<string> Statuses
        {
            get
            {
                if (_statuses == null)
                {
                    _statuses = new ObservableCollection<string>();
                    _statuses.Add("Не задан");
                    _statuses.Add("Создание заявки");
                    _statuses.Add("Заявка обработана");
                    _statuses.Add("Заключен договор");
                    _statuses.Add("Выполнение работ");
                    _statuses.Add("Оформить комплект");
                    _statuses.Add("Направлен в Росстандарт");
                    _statuses.Add("Утвержден Росстандартом");
                    _statuses.Add("Работа завершена");
                }
                return _statuses;
            }
        }

        public static readonly Dictionary<string, string> FileNames = new Dictionary<string, string>
        {
           {"Act","1 Акт"},
           {"ExpConcl","2 Экспертное заключение"},
           {"ExpConclApplication","3 Приложение к ЭЗ"},
           {"Protokol","4 Протокол ИС"},
           {"ProtokolApplication","5 Приложение к ИС"},
        };
    }



}
