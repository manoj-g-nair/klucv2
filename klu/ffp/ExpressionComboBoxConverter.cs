using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;
using ffp.TrainingDataSetTableAdapters;

namespace ffp
{
    public class ExpressionComboBoxConverter : IValueConverter
    {
        TrainingDataSet _DataSet;
        TableAdapterManager _TAM;

        /// <summary>
        /// No conversion is done here, but a list of all Expressions is returned.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Expressions</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                _TAM = new TableAdapterManager();
                _DataSet = new TrainingDataSet();
                _TAM.ExpressionTableAdapter = new ExpressionTableAdapter();

                _TAM.ExpressionTableAdapter.Fill(_DataSet.Expression);

                return _DataSet.Expression;
                
                //TableAdapterManager tam = new TableAdapterManager();
                //TrainingDataSet dataSet = new TrainingDataSet();
                //tam.ExpressionTableAdapter = new ExpressionTableAdapter();

                //tam.ExpressionTableAdapter.Fill(dataSet.Expression);               

                //ArrayList expressions = new ArrayList();

                //for (int i=0; i<dataSet.Expression.Count; i++)
                //{
                //    expressions.Add(dataSet.Expression[i].Expression);
                //}

                //return expressions;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return 1;// System.Convert.ToDateTime(value);
            }
            return value;
        }
    }
}