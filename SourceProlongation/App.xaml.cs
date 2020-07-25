using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using SourceProlongation.Properties;
using SourceProlongation.View;
using SourceProlongation.ViewModel;

namespace SourceProlongation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var conString = File.Exists("connection")
                ? File.ReadAllText("connection", Encoding.GetEncoding("windows-1251"))
                : Settings.Default.vniimConnectionString;

            Connection.Initialize(conString);

            var window = new MainWindowView();

            // Create the ViewModel to which 
            // the main window binds.
            var viewModel = new MainWindowViewModel();

            // When the ViewModel asks to be closed, 
            // close the window.
            EventHandler handler = null;
            handler = delegate
            {
                viewModel.RequestClose -= handler;
                window.Close();
            };
            viewModel.RequestClose += handler;

            // Allow all controls in the window to 
            // bind to the ViewModel by setting the 
            // DataContext, which propagates down 
            // the element tree.
            window.DataContext = viewModel;
            window.Show();
            
            
        }


    }
}
