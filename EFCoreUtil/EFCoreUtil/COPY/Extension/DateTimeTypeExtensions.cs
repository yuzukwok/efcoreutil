using System;
using NpgsqlTypes;

namespace EFCoreUtil.COPY.Extension
{
    public static class DateTimeTypeExtensions
    {
        public static PostgreSQLCopyHelper<TEntity> MapDate<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, DateTime> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Date);
        }

        public static PostgreSQLCopyHelper<TEntity> MapDate<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, DateTime?> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Date);
        }

        public static PostgreSQLCopyHelper<TEntity> MapTimeStamp<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, DateTime> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Timestamp);
        }

        public static PostgreSQLCopyHelper<TEntity> MapTimeStamp<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, DateTime?> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Timestamp);
        }

        public static PostgreSQLCopyHelper<TEntity> MapInterval<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, TimeSpan> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Interval);
        }

        public static PostgreSQLCopyHelper<TEntity> MapInterval<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, TimeSpan?> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Interval);
        }
    }
}