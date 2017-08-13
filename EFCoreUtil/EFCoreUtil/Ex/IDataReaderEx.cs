using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Microsoft.EntityFrameworkCore
{
   public static class IDataReaderEx
    {
        public static  bool ColumnExists(this IDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
