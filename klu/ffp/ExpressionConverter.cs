﻿using System;
using System.Globalization;
using System.Windows.Data;
using ffp.TrainingDataSetTableAdapters;

namespace ffp
{
    public class ExpressionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                TableAdapterManager tam = new TableAdapterManager();
                TrainingDataSet dataSet = new TrainingDataSet();
                tam.ExpressionTableAdapter = new ExpressionTableAdapter();

                tam.ExpressionTableAdapter.Fill(dataSet.Expression);               

                return dataSet.Expression.FindByExpressionOID(System.Convert.ToInt32(value)).Expression;
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
                return 666;// System.Convert.ToDateTime(value);
            }
            return value;
        }
    }
}