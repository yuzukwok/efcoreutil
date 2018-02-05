// Copyright (c) Arch team. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using EFCoreUtil.COPY;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Defines the interface(s) for unit of work.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        

        /// <summary>
        /// Gets the specified repository for the <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>An instance of type inherited from <see cref="IRepository{TEntity}"/> interface.</returns>
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        void PgCopy<TEntity>(PostgreSQLCopyHelper<TEntity> copymap,IEnumerable<TEntity> entities) where TEntity : class;
         
        /// <summary>
        /// Asynchronously saves all changes made in this unit of work to the database.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
        Task<int> SaveChangesAsync();
        
        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <param name="ensureAutoHistory"><c>True</c> if sayve changes ensure auto record the change history.</param>
        /// <returns>The number of state entries written to the database.</returns>
        int SaveChanges();

        /// <summary>
        /// Executes the specified raw SQL command.
        /// </summary>
        /// <param name="sql">The raw SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The number of state entities written to database.</returns>
        int ExecuteSqlCommand(string sql, params object[] parameters);
        /// <summary>
        /// 使用Dapper查询sql（无事务）
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramenters"></param>
        /// <returns></returns>
        Task<IEnumerable<TReturn>> QuerySqlAsync<TReturn>(string sql,  object parameter=null, bool newconnection = false) ;
        /// <summary>
        /// 使用Dapper查询sql ，返回IDataReader
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<IDataReader> QuerySqlDataReaderAsync(string sql, object parameter = null,bool newconnection= false);
        
        /// <summary>
        /// 使用Dapper查询sql ，返回IDataReader
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<(IDataReader,IDbConnection)> QuerySqlDataReaderWithConnectionAsync(string sql, object parameter = null);
        
        /// <summary>
        /// 使用Dapper查询sql ，返回标量值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task<T> ExecuteScalarAsync<T>(string sql, object parameter = null,bool newconnection= false);
    }
}
