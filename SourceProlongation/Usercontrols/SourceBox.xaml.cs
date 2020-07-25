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
    /// Interaction logic for SourceBox.xaml
    /// </summary>
    public partial class SourceBox : UserControl
    {
        public SourceBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty FilterSourceProperty = DependencyProperty.Register(
    "FilterSource", typeof(IList), typeof(SourceBox), new PropertyMetadata(default(IList)));

        public IList FilterSource
        {
            get { return (IList)GetValue(FilterSourceProperty); }
            set { SetValue(FilterSourceProperty, value); }
        }

        public static readonly DependencyProperty SelectedSourceProperty = DependencyProperty.Register(
            "SelectedSource", typeof(NucleideViewModel), typeof(SourceBox), new PropertyMetadata(default(NucleideViewModel)));


        public NucleideViewModel SelectedSource
        {
            get { return (NucleideViewModel)GetValue(SelectedSourceProperty); }
            set { SetValue(SelectedSourceProperty, value); }
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

                var ncl = Collections.NucleidesCollection.SingleOrDefault(x => x.Name == TextBox.Text);
                if (ncl != null)
                {
                    SelectedSource = ncl;
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
                Cursor = Cursors.Hand
            };

            // Mouse events
            block.PreviewMouseLeftButtonDown += (sender, e) =>
            {
                TextBox.Text = (sender as TextBlock).Text;
                SuggestPopup.IsOpen = false;

                var ncl = Collections.NucleidesCollection.SingleOrDefault(x => x.Name == TextBox.Text);
                if (ncl != null)
                {
                    SelectedSource = ncl;
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
