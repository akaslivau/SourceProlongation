using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SourceProlongation.Model;
using SourceProlongation.ViewModel;

namespace SourceProlongation.Usercontrols
{
    /// <summary>
    /// Interaction logic for CustomerBox.xaml
    /// </summary>
    public partial class CustomerBox : UserControl
    {
        public CustomerBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty FilterSourceProperty = DependencyProperty.Register(
      "FilterSource", typeof(IList), typeof(CustomerBox), new PropertyMetadata(default(IList)));

        public IList FilterSource
        {
            get { return (IList)GetValue(FilterSourceProperty); }
            set { SetValue(FilterSourceProperty, value); }
        }

        public static readonly DependencyProperty SelectedCustomerProperty = DependencyProperty.Register(
            "SelectedCustomer", typeof(Customer), typeof(CustomerBox), new PropertyMetadata(default(Customer), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var op = (CustomerBox) d;
            op.TextBox.Text = op.SelectedCustomer == null ? "" : op.SelectedCustomer.name;
        }


        public Customer SelectedCustomer
        {
            get { return (Customer)GetValue(SelectedCustomerProperty); }
            set { SetValue(SelectedCustomerProperty, value); }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            bool found = false;
            string query = (sender as TextBox).Text;
            if (query.Length < 2) return;

            if (query.Length == 0)
            {
                // Clear
                ResultStack.Children.Clear();
                SuggestPopup.IsOpen = false;
            }
            else
            {
                SuggestPopup.IsOpen = true;
            }

            // Clear the list
            ResultStack.Children.Clear();

            // Add the result
            foreach (var obj in FilterSource)
            {
                if (obj.ToString().ToLower().Contains(query.ToLower()))
                {
                    // The word starts with this... Autocomplete must work
                    AddItem(obj.ToString());
                    found = true;
                }
            }

            if (ResultStack.Children.Count == 1)
            {
                SuggestPopup.IsOpen = false;
                TextBox.Text = (ResultStack.Children[0] as TextBlock).Text;

                var ncl = Collections.Customers.SingleOrDefault(x => x.name == TextBox.Text);
                if (ncl != null)
                {
                    SelectedCustomer = ncl;
                    Keyboard.ClearFocus();
                }
            }


            if (!found)
            {
                ResultStack.Children.Add(new TextBlock() { Text = "Ничего не найдено" });
            }
        }

        private void AddItem(string text)
        {
            TextBlock block = new TextBlock
            {
                Text = text,
                Margin = new Thickness(2, 3, 2, 3),
                Cursor = Cursors.Hand,
                Background = Brushes.White
            };

            // Mouse events
            block.PreviewMouseLeftButtonDown += (sender, e) =>
            {
                TextBox.Text = (sender as TextBlock).Text;
                SuggestPopup.IsOpen = false;

                var ncl = Collections.Customers.SingleOrDefault(x => x.name == TextBox.Text);
                if (ncl != null)
                {
                    SelectedCustomer = ncl;
                    Keyboard.ClearFocus();
                }
            };

            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.LightGreen;
            };

            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
            };

            // Add to the panel
            ResultStack.Children.Add(block);
        }

    }
}
