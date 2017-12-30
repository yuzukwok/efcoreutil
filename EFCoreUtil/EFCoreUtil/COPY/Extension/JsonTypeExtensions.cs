using System;
using NpgsqlTypes;

namespace EFCoreUtil.COPY.Extension
{
    public static class JsonTypeExtensions
    {
        public static PostgreSQLCopyHelper<TEntity> MapJson<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, string> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Json);
        }

        public static PostgreSQLCopyHelper<TEntity> MapJsonb<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, string> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Jsonb);
        }
    }
}