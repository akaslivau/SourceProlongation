using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceProlongation.Base;

namespace SourceProlongation.ViewModel
{
    public class ExecutorViewModel:ViewModelBase
    {
        private string _position = "";
        public string Position { get { return _position;} set{ _position = value; OnPropertyChanged("Position");}}

        private string _name = "";
        public string Name { get { return _name;} set{ _name = value; OnPropertyChanged("Name");}}

        public ExecutorViewModel(string position, string name)
        {
            Position = position;
            Name = name;
        }
    }
}
