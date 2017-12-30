using System;
using NpgsqlTypes;

namespace EFCoreUtil.COPY.Extension
{
    public static class UUIDTypeExtensions
    {
        public static PostgreSQLCopyHelper<TEntity> MapUUID<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Guid> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Uuid);
        }

        public static PostgreSQLCopyHelper<TEntity> MapUUID<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Guid?> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Uuid);
        }
    }
}