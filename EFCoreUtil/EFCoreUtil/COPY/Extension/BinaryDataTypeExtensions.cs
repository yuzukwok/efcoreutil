using System;
using NpgsqlTypes;

namespace EFCoreUtil.COPY.Extension
{
    public static class BinaryDataTypeExtensions
    {
        public static PostgreSQLCopyHelper<TEntity> MapByteArray<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, byte[]> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Bytea);
        }
    }
}