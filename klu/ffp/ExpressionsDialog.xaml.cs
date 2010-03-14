using System;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ffp.TrainingDataSetTableAdapters;
using KluSharp;
using Microsoft.Windows.Controls.Ribbon;

namespace ffp
{
    /// <summary>
    /// Interaction logic for ExpressionsDialog.xaml
    /// </summary>
    public partial class ExpressionsDialog : Window
    {
        TrainingDataSet _DataSet;

        public ExpressionsDialog(ref TrainingDataSet DataSet)
        {
            InitializeComponent();

            _DataSet = DataSet;
            ExpressionsDataGrid.ItemsSource = _DataSet.Expression;
        }
    }
}
