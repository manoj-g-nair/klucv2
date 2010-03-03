using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace ffp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// We use this as a quick workaround so that our application doesn't crash
        /// if any of the DataGrid Validation Exceptions are thrown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Process unhandled exception
            MessageBoxResult res = MessageBox.Show(e.Exception.Message, "An unhandled exception occured!", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No);

            if (res == MessageBoxResult.No)
            {
                // Prevent default unhandled exception processing
                e.Handled = true;
            }
        }

    }
}
