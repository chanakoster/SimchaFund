using System;
using System.Data.Common;

namespace SimchaFund.Data
{
    public static class Extensions
    {
        public static T GetOrNull<T>(this DbDataReader reader, string column)
        {
            object value = reader[column];
            if (value == DBNull.Value)
            {
                return default;
            }

            return (T)value;
        }
    }
}
