using System;
using NpgsqlTypes;

namespace EFCoreUtil.COPY.Extension
{
    public static class BooleanTypeExtensions
    {
        public static PostgreSQLCopyHelper<TEntity> MapBoolean<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, bool> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Boolean);
        }

        public static PostgreSQLCopyHelper<TEntity> MapBoolean<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, bool?> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Boolean);
        }
    }
}