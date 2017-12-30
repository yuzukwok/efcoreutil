using System;
using NpgsqlTypes;

namespace EFCoreUtil.COPY.Extension
{
    public static class BitStringTypeExtensions
    {
        public static PostgreSQLCopyHelper<TEntity> MapBit<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, bool> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Bit);
        }

        public static PostgreSQLCopyHelper<TEntity> MapBit<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, bool?> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter,  NpgsqlDbType.Bit);
        }
    }
}