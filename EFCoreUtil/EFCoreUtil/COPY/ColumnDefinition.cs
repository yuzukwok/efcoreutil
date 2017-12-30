using System;
using Npgsql;

namespace EFCoreUtil.COPY
{
    internal class ColumnDefinition<TEntity>
    {
        public string ColumnName { get; set; }

        public Action<NpgsqlBinaryImporter, TEntity> Write { get; set; }

        public override string ToString()
        {
            return string.Format("ColumnDefinition (ColumnName = {0}, Serialize = {1})", ColumnName, Write);
        }
    }
}