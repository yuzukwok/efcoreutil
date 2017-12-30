using System;
using System.Net;
using System.Net.NetworkInformation;
using NpgsqlTypes;

namespace EFCoreUtil.COPY.Extension
{
    public static class NetworkAddressTypeExtensions
    {
        public static PostgreSQLCopyHelper<TEntity> MapInetAddress<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, IPAddress> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Inet);
        }

        public static PostgreSQLCopyHelper<TEntity> MapMacAddress<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, PhysicalAddress> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.MacAddr);
        }
    }
}