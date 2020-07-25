using System;
using System.Linq;
using System.Reflection;
using SourceProlongation.Base;
using SourceProlongation.Model;

namespace SourceProlongation.ViewModel
{
    public class SourceViewModel:ViewModelBase
    {
        #region Other
        private void UpdateProperty(string propertyName, object value)
        {
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var table = cntx.GetTable<Source>();
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

        #region Fields
        public int Id { get; }
        public DateTime DocDate { get; }

        private NucleideViewModel _nucleide = null;
        public NucleideViewModel Nucleide
        {
            get => _nucleide;
            set
            {
                _nucleide = value;
                OnPropertyChanged("Nucleide");
                UpdateProperty("nucleideId", value.Id);
                RecalculateParameters();
            }
        }
        
        private string _type = "";
        public string Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged("Type");
                UpdateProperty("type", value);
            }
        }

        private string _number = "";
        public string Number
        {
            get => _number;
            set
            {
                _number = value;
                OnPropertyChanged("Number");
                UpdateProperty("number", value);
            }
        }

        private string _passportNum = "";
        public string PassportNum
        {
            get => _passportNum;
            set
            {
                _passportNum = value;
                OnPropertyChanged("PassportNum");
                UpdateProperty("passport", value);
            }
        }

        private string _rank = "";
        public string Rank
        {
            get => _rank;
            set
            {
                _rank = value;
                OnPropertyChanged("Rank");
                UpdateProperty("rank", value);
            }
        }

        private int _madeYear = -1;
        public int MadeYear
        {
            get => _madeYear;
            set
            {
                _madeYear = value;
                OnPropertyChanged("MadeYear");
                UpdateProperty("madeYear", value);
            }
        }

        private int _extensionPeriod = 5;

        public int ExtensionPeriod
        {
            get => _extensionPeriod;
            set
            {
                _extensionPeriod = value;
                OnPropertyChanged("ExtensionPeriod");
                UpdateProperty("extensionPeriod", value);
            }
        }

        private DateTime _baseValueDate = DateTime.Now.AddYears(-5);
        public DateTime BaseValueDate
        {
            get => _baseValueDate;
            set
            {
                _baseValueDate = value;
                OnPropertyChanged("BaseValueDate");
                RecalculateParameters();
                UpdateProperty("baseValueDate", value);
            }
        }

        private double _baseValue = 0;
        public double BaseValue
        {
            get => _baseValue;
            set
            {
                _baseValue = value;
                OnPropertyChanged("BaseValue");
                RecalculateParameters();
                UpdateProperty("baseValue", value);
            }
        }

        private string _unit = "";
        public string Unit
        {
            get => _unit;
            set
            {
                _unit = value;
                OnPropertyChanged("Unit");
                UpdateProperty("unit", value);
            }
        }


        private double _measValue = 0;
        public double MeasValue
        {
            get => _measValue;
            set
            {
                _measValue = value;
                OnPropertyChanged("MeasValue");
                RecalculateParameters();
                UpdateProperty("measValue", value);
            }
        }

        private DateTime _measDate = DateTime.Now;
        public DateTime MeasDate
        {
            get => _measDate;
            set
            {
                _measDate = value;
                OnPropertyChanged("MeasDate");
                RecalculateParameters();
                UpdateProperty("measDate", value);
            }
        }

        private string _docNumber = "";
        public string DocNumber
        {
            get => _docNumber;
            set
            {
                _docNumber = value;
                OnPropertyChanged("DocNumber");
                UpdateProperty("docNumber", value);
            }
        }

        private bool _isSvid = true;
        public bool IsSvid
        {
            get => _isSvid;
            set
            {
                _isSvid = value;
                OnPropertyChanged("IsSvid");
                UpdateProperty("isSvid", value);
            }
        }
        #endregion

        #region Отклонение
        private double _calcedVal = 0;
        public double CalcedVal { get => _calcedVal;
            set { _calcedVal = value; OnPropertyChanged("CalcedVal"); } }

        public void RecalculateParameters()
        {
            if (Nucleide == null) return;

            var old = BaseValue;
            var days = (BaseValueDate - MeasDate).Days;
            CalcedVal = old * Math.Pow(2, days / Nucleide.HalfPeriod);

            Difference = Math.Round(100 * (MeasValue / CalcedVal - 1), 1) + " %";
        }
        private string _difference = "";
        public string Difference { get => _difference;
            set{ _difference = value; OnPropertyChanged("Difference");}}

        #endregion

        #region Заполнение таблиц
        public string GetExpApplText(int i, int rowIndex)
        {
            switch (i)
            {
                case 0:
                    return (rowIndex - 2).ToString();
                case 1:
                    return Nucleide.Name;
                case 2:
                    return Type;
                case 3:
                    return Number;
                case 4:
                    return Rank;
                case 5:
                    return MadeYear.ToString();
                case 6:
                    return ExtensionPeriod.ToString();
                case 7:
                    return (DocDate.AddYears(ExtensionPeriod)).ToShortDateString();
                case 8:
                    return "-";
            }
            return "";
        }

        public string GetProtApplText(int i, int rowIndex)
        {
            switch (i)
            {
                case 0:
                    return (rowIndex - 2).ToString();
                case 1:
                    return Nucleide.Name;
                case 2:
                    return Type;
                case 3:
                    return Number;
                case 4:
                    return PassportNum;
                case 5:
                    return BaseValue.ToString("F2") + " " + Unit;
                case 6:
                    return Rank;
                case 7:
                    return "Хорошее. Следов деформаций, коррозии и воздействия агрессивных веществ не обнаружено.";
                case 8:
                    return "Не более 185 Бк";
                case 9:
                    return MeasValue.ToString("F2") + " " + Unit;
                case 10:
                    return CalcedVal.ToString("F2") + " " + Unit;
                case 11:
                    return "Сохранены";
                case 12:
                    return (IsSvid ? "Свидетельство " : "Сертификат ") + "№ " + DocNumber;
            }
            return "";
        }
        #endregion

        public SourceViewModel(Source source, DateTime docDate)
        {
            if (source == null) return;

            Id = source.id;
            DocDate = docDate;

            _type = source.type;
            _number = source.number;
            _passportNum = source.passport;
            _rank = source.rank;
            _madeYear = source.madeYear;
            _extensionPeriod = source.extensionPeriod;
            _baseValueDate = source.baseValueDate;
            _baseValue = source.baseValue;
            _unit = source.unit;
            _measValue = source.measValue;
            _measDate = source.measDate;
            _docNumber = source.docNumber;
            _isSvid = source.isSvid;
            _nucleide = Collections.NucleidesCollection.SingleOrDefault(x => x.Id == source.nucleideId);
        }


    }
}
