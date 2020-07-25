using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SourceProlongation.Base;

namespace SourceProlongation.ViewModel
{
    public class SourceViewModel:ViewModelBase
    {
        #region Fields
        private readonly OrderViewModel _parent;


        private bool _isChecled = false;
        public bool IsChecked { get { return _isChecled;} set{ _isChecled = value; OnPropertyChanged("IsChecked");}}


        private string _type = "";
        public string Type { get { return _type;} set{ _type = value; OnPropertyChanged("Type");}}

        private string _number = "";
        public string Number { get { return _number;} set{ _number = value; OnPropertyChanged("Number");}}

        private string _passportNum = "";
        public string PassportNum { get { return _passportNum;} set{ _passportNum = value; OnPropertyChanged("PassportNum");}}

        private string _rank = "";
        public string Rank { get { return _rank;} set{ _rank = value; OnPropertyChanged("Rank");}}

        private int _madeYear = 1960;
        public int MadeYear { get { return _madeYear;} set{ _madeYear = value; OnPropertyChanged("MadeYear");}}
        
        private int _additionalPeriod = 5;
        public int AdditionalPeriod { get { return _additionalPeriod;} set{ _additionalPeriod = value; OnPropertyChanged("AdditionalPeriod");}}

        private DateTime _baseValueDate = DateTime.Now.AddYears(-5);

        public DateTime BaseValueDate
        {
            get { return _baseValueDate; }
            set
            {
                _baseValueDate = value;
                OnPropertyChanged("BaseValueDate");
                RecalculateParameters();
            }
        }

        private double _baseValue = 0;

        public double BaseValue
        {
            get { return _baseValue; }
            set
            {
                _baseValue = value;
                OnPropertyChanged("BaseValue");
                RecalculateParameters();
            }
        }

        private string _unit = "";
        public string Unit { get { return _unit;} set{ _unit = value; OnPropertyChanged("Unit");}}


        private double _measValue = 0;

        public double MeasValue
        {
            get { return _measValue; }
            set
            {
                _measValue = value;
                OnPropertyChanged("MeasValue");
                RecalculateParameters();
            }
        }

        private DateTime _measDate = DateTime.Now;

        public DateTime MeasDate
        {
            get { return _measDate; }
            set
            {
                _measDate = value;
                OnPropertyChanged("MeasDate");
                RecalculateParameters();
            }
        }

        private string _docNumber = "";
        public string DocNumber { get { return _docNumber;} set{ _docNumber = value; OnPropertyChanged("DocNumber");}}

        private bool _isSvid = true;
        public bool IsSvid { get { return _isSvid;} set{ _isSvid = value; OnPropertyChanged("IsSvid");}}

        private bool _isScienceFormat = true;

        public bool IsScienceFormat
        {
            get { return _isScienceFormat; }
            set
            {
                _isScienceFormat = value;
                OnPropertyChanged("IsScienceFormat");
                UpdateFormat();
            }
        }

        private int _digitCount = 2;

        public int DigitCount
        {
            get { return _digitCount; }
            set
            {
                _digitCount = value;
                OnPropertyChanged("DigitCount");
                UpdateFormat();
            }
        }


        private NucleideViewModel _nucleide = null;

        public NucleideViewModel Nucleide
        {
            get { return _nucleide; }
            set
            {
                _nucleide = value;
                OnPropertyChanged("Nucleide");
                RecalculateParameters();
            }
        }

        #endregion

        #region Отклонение

        private string _format = "F2";
        public string Format { get { return _format;} set{ _format = value; OnPropertyChanged("Format");}}



        public void UpdateFormat()
        {
            if (!IsScienceFormat)
            {
                Format = "F" + DigitCount;
            }
            string reshetka = "";
            for (int i = 0; i < DigitCount; i++)
            {
                reshetka += "#";
            }
            Format = "0." + reshetka + "E+00";

            Difference = Math.Round(100 * (MeasValue / CalcedVal - 1), 1) + " % (" + CalcedVal.ToString(Format) + ")";
        }

        private double _calcedVal = 0;
        public double CalcedVal { get { return _calcedVal; } set { _calcedVal = value; OnPropertyChanged("CalcedVal"); } }

        private void RecalculateParameters()
        {
            if (Nucleide == null) return;

            var old = BaseValue;
            var days = (BaseValueDate - MeasDate).Days;
            CalcedVal = old * Math.Pow(2, days / Nucleide.HalfLife);

            Difference = Math.Round(100 * (MeasValue / CalcedVal - 1), 1) + " % (" + CalcedVal.ToString(Format) + ")";
        }
        private string _difference = "";
        public string Difference { get { return _difference;} set{ _difference = value; OnPropertyChanged("Difference");}}

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
                    return AdditionalPeriod.ToString();
                case 7:
                    return (2000 + _parent.Year + AdditionalPeriod).ToString();
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
                    return BaseValue.ToString(Format) + " " + Unit;
                case 6:
                    return Rank;
                case 7:
                    return "Хорошее. Следов деформаций, коррозии и воздействия агрессивных веществ не обнаружено.";
                case 8:
                    return "Не более 185 Бк";
                case 9:
                    return MeasValue.ToString(Format) + " " + Unit;
                case 10:
                    return CalcedVal.ToString(Format) + " " + Unit;
                case 11:
                    return "Сохранены";
                case 12:
                    return (IsSvid ? "Свидетельство " : "Сертификат ") + "№ " + DocNumber;
            }
            return "";
        }
        #endregion

        public SourceViewModel()
        {
            
        }

        public SourceViewModel(OrderViewModel order):this()
        {
            _parent = order;
        }


        public SourceViewModel(SourceViewModel source, OrderViewModel parent):this(parent)
        {
            Type = source.Type;
            Number = source.Number;
            PassportNum = source.PassportNum;
            if (source.Nucleide!=null && Collections.Nucleides.Any(x => x.Name == source.Nucleide.Name))
            {
                _nucleide = Collections.Nucleides.First(x => x.Name == source.Nucleide.Name);
            }

            MadeYear = source.MadeYear;
            _baseValue = source.BaseValue;
            _baseValueDate = source.BaseValueDate;
            Unit = source.Unit;

            _measValue = source.MeasValue;
            _measDate = source.MeasDate;
            DocNumber = source.DocNumber;
            IsSvid = source.IsSvid;

            IsScienceFormat = source.IsScienceFormat;
            DigitCount = source.DigitCount;

            AdditionalPeriod = source.AdditionalPeriod;
            Rank = source.Rank;
            RecalculateParameters();
        }
    }
}
