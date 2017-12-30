using System;
using NpgsqlTypes;

namespace EFCoreUtil.COPY.Extension
{
    public static class MonetaryTypeExtensions
    {
        public static PostgreSQLCopyHelper<TEntity> MapMoney<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Decimal> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Money);
        }

        public static PostgreSQLCopyHelper<TEntity> MapMoney<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Decimal?> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Money);
        }
    }
}