using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceProlongation.Base;

namespace SourceProlongation.ViewModel
{
    public class NucleideViewModel: ViewModelBase
    {
        private string _name = "";
        public string Name { get { return _name;} set{ _name = value; OnPropertyChanged("Name");}}

        private double _halfLife = 0;
        public double HalfLife { get { return _halfLife;} set{ _halfLife = value; OnPropertyChanged("HalfLife");}}

        public NucleideViewModel()
        {
            
        }

        public NucleideViewModel(string name, double halfLife):this()
        {
            Name = name;
            HalfLife = halfLife;
        }
    }
}
