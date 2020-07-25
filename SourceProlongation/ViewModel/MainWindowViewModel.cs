using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Newtonsoft.Json;
using SourceProlongation.Base;

namespace SourceProlongation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        private ObservableCollection<OrderViewModel> _items;
        public ObservableCollection<OrderViewModel> Items { get => _items;
            set{ _items = value; OnPropertyChanged("Items");}}

        private OrderViewModel _selected = null;
        public OrderViewModel Selected { get { return _selected;} set{ _selected = value; OnPropertyChanged("Selected");}}
        

        #endregion 

        public ICommand WindowClosingCommand { get; private set; }

        public MainWindowViewModel()
        {
            WindowClosingCommand = new RelayCommand(a =>
            {
               //TODO: Save All
            });
            Items = new ObservableCollection<OrderViewModel>();
            using (var cntx = new SqlDataContext(Connection.ConnectionString))
            {
                var orders = cntx.GetTable<Order>();
                foreach (var order in orders)
                {
                    Items.Add(new OrderViewModel(order));
                }

                if (Items.Any()) Selected = Items[0];
            }

            /*            var tmp = Directory.GetFiles(Strings.ORDER_FOLDER).
                            Select(json => JsonConvert.DeserializeObject<OrderViewModel>
                                (File.ReadAllText(json))).
                            Select(order => new OrderViewModel(order)).
                            OrderBy(x => x.Year).ThenBy(x => x.Number).
                            ToList();

                        var lst = new List<string>();
                        foreach (var item in tmp)
                        {
                            if(lst.Contains(item.Customer)) continue;
                            lst.Add(item.Customer);
                        }
                        File.WriteAllLines("text.txt", lst);
                        return;*/

        }
    }
}
