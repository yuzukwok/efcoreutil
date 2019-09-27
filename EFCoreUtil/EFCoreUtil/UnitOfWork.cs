// Copyright (c) Arch team. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Dapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Data.Common;
using EFCoreUtil.COPY;
using Npgsql;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IUnitOfWork"/> and <see cref="IUnitOfWork{TContext}"/> interface.
    /// </summary>
    /// <typeparam name="TContext">The type of the db context.</typeparam>
    public class UnitOfWork<TContext> : IRepositoryFactory, IUnitOfWork<TContext>, IUnitOfWork where TContext : DbContext
    {
        private readonly TContext _context;
        private bool disposed = false;
        private Dictionary<Type, object> repositories;

        private ILogger<TContext> _logger;
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork{TContext}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UnitOfWork(TContext context, ILogger<TContext> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
        }

        /// <summary>
        /// Gets the db context.
        /// </summary>
        /// <returns>The instance of type <typeparamref name="TContext"/>.</returns>
        public TContext DbContext => _context;



        /// <summary>
        /// Gets the specified repository for the <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>An instance of type inherited from <see cref="IRepository{TEntity}"/> interface.</returns>
        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (repositories == null)
            {
                repositories = new Dictionary<Type, object>();
            }

            var type = typeof(TEntity);
            if (!repositories.ContainsKey(type))
            {
                repositories[type] = new Repository<TEntity>(_context);
            }

            return (IRepository<TEntity>)repositories[type];
        }

        public  void PgCopy<TEntity>(PostgreSQLCopyHelper<TEntity> copymap, IEnumerable<TEntity> entities) where TEntity : class
        {
            //处理连接
            IDbConnection connection = null;
            connection = SetDbConnection(true, connection);
            using (connection)
            {
                
                copymap.SaveAll(connection as NpgsqlConnection, entities);
            }
        }

        /// <summary>
        /// Executes the specified raw SQL command.
        /// </summary>
        /// <param name="sql">The raw SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The number of state entities written to database.</returns>
        public int ExecuteSqlCommand(string sql, params object[] parameters) => _context.Database.ExecuteSqlRaw(sql, parameters);

        /// <summary>
        /// EF core FromSql（返回结果必须为定义的DbSet实体，使用不便）
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="sql">The raw SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>An <see cref="IQueryable{T}" /> that contains elements that satisfy the condition specified by raw SQL.</returns>
        public IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class => _context.Set<TEntity>().FromSqlRaw(sql, parameters);



        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <param name="ensureAutoHistory"><c>True</c> if save changes ensure auto record the change history.</param>
        /// <returns>The number of state entries written to the database.</returns>
        public int SaveChanges()
        {
           return _context.SaveChanges();
        }

        /// <summary>
        /// Asynchronously saves all changes made in this unit of work to the database.
        /// </summary>
        /// <param name="ensureAutoHistory"><c>True</c> if save changes ensure auto record the change history.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Saves all changes made in this context to the database with distributed transaction.
        /// </summary>
        /// <param name="ensureAutoHistory"><c>True</c> if save changes ensure auto record the change history.</param>
        /// <param name="unitOfWorks">An optional <see cref="IUnitOfWork"/> array.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
        public async Task<int> SaveChangesAsync(params IUnitOfWork[] unitOfWorks)
        {
            // TransactionScope will be included in .NET Core v2.0
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var count = 0;
                    foreach (var unitOfWork in unitOfWorks)
                    {
                        var uow = unitOfWork as UnitOfWork<DbContext>;
                        uow.DbContext.Database.UseTransaction(transaction.GetDbTransaction());
                        count += await uow.SaveChangesAsync();
                    }

                    count += await SaveChangesAsync();

                    transaction.Commit();

                    return count;
                }
                catch (Exception ex)
                {

                    transaction.Rollback();

                    throw ex;
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">The disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // clear repositories
                    if (repositories != null)
                    {
                        repositories.Clear();
                    }

                    // dispose the db context.
                    //TODO let di dispose
                   // _context.Dispose();
                }
            }

            disposed = true;
        }

        public async Task<IEnumerable<TReturn>> QuerySqlAsync<TReturn>(string sql, object parameter = null, bool newconnection = false)
        {
            //处理连接
            IDbConnection connection = null;
            connection = SetDbConnection(newconnection, connection);

            Stopwatch watch = new Stopwatch();

            try
            {
                if (newconnection)
                {
                    using (connection)
                    {
                        return await ExcuteQuery<TReturn>(sql, parameter, connection, watch);
                    }
                }
                else
                {
                    return await ExcuteQuery<TReturn>(sql, parameter, connection, watch);
             
                }
               

            }
            catch
            {
                if (_logger != null)
                {

                    _logger.LogError($"Sql:-> {sql} " +
                        System.Environment.NewLine +
                        $" Param:->{JsonConvert.SerializeObject(parameter)}" +
                        System.Environment.NewLine +
                        $"SQL Excute Error");
                }
                throw;
            }


        }

        private async Task<IEnumerable<TReturn>> ExcuteQuery<TReturn>(string sql, object parameter, IDbConnection connection, Stopwatch watch)
        {
            watch.Start();
            var re = await connection.QueryAsync<TReturn>(sql, parameter);
            watch.Stop();
            if (_logger != null)
            {
                var querytime = watch.Elapsed.TotalMilliseconds;
                _logger.LogDebug($"Sql:-> {sql} " +
                    System.Environment.NewLine +
                    $" Param:->{JsonConvert.SerializeObject(parameter)}" +
                    System.Environment.NewLine +
                    $" Excute time:{querytime}");
            }
            return re;
        }

        public async Task<IDataReader> QuerySqlDataReaderAsync(string sql, object parameter = null,bool newconnection = false)
        {
            //处理连接
            IDbConnection connection = null;
            connection = SetDbConnection(newconnection, connection);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Stopwatch watch = new Stopwatch();
            try
            {
               return await ExcuteReader(sql, parameter, connection, watch);
            }
            catch
            {
                if (_logger != null)
                {

                    _logger.LogError($"Sql:-> {sql} " +
                        System.Environment.NewLine +
                        $" Param:->{JsonConvert.SerializeObject(parameter)}" +
                        System.Environment.NewLine +
                        $"SQL Excute Error");
                }
                throw;
            }
        }

        public async Task<(IDataReader, IDbConnection)> QuerySqlDataReaderWithConnectionAsync(string sql, object parameter = null)
        {
            //处理连接
            IDbConnection connection = null;
            connection = SetDbConnection(true, connection);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Stopwatch watch = new Stopwatch();
            try
            {
                return (await ExcuteReader(sql, parameter, connection, watch),connection);
            }
            catch
            {
                if (_logger != null)
                {

                    _logger.LogError($"Sql:-> {sql} " +
                                     System.Environment.NewLine +
                                     $" Param:->{JsonConvert.SerializeObject(parameter)}" +
                                     System.Environment.NewLine +
                                     $"SQL Excute Error");
                }
                throw;
            }
        }

        private async Task<IDataReader> ExcuteReader(string sql, object parameter, IDbConnection connection, Stopwatch watch)
        {
            watch.Start();
            var re = await connection.ExecuteReaderAsync(sql, parameter);
            watch.Stop();
            if (_logger != null)
            {
                var querytime = watch.Elapsed.TotalMilliseconds;
                _logger.LogDebug($"Sql:-> {sql} " +
                    System.Environment.NewLine +
                    $" Param:->{JsonConvert.SerializeObject(parameter)}" +
                    System.Environment.NewLine +
                    $"Excute time:{querytime}");
            }
            return re;
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, object parameter = null, bool newconnection = false)
        {
            //处理连接
            IDbConnection connection = null;
            connection = SetDbConnection(newconnection, connection);

            Stopwatch watch = new Stopwatch();

            try
            {
                if (newconnection)
                {
                    using (connection)
                    {
                        return await ExcuteScalar<T>(sql, parameter, connection, watch);
                    }
                }
                else
                {
                    return await ExcuteScalar<T>(sql, parameter, connection, watch);
                }
               


            }
            catch
            {
                if (_logger != null)
                {

                    _logger.LogError($"Sql:-> {sql} " +
                        System.Environment.NewLine +
                        $" Param:->{JsonConvert.SerializeObject(parameter)}" +
                        System.Environment.NewLine +
                        $"SQL Excute Error");
                }
                throw;
            }
        }
        
        static  IDictionary<Type ,string> connects=new Dictionary<Type, string>();

        private IDbConnection SetDbConnection(bool newconnection, IDbConnection connection)
        {
            if (newconnection)
            {
                var type = typeof(TContext);
                if (!connects.ContainsKey(type))
                {
                    connects[type] =  _context.Database.GetDbConnection().ConnectionString;
                }
                connection = new Npgsql.NpgsqlConnection(connects[type]);
            }
            else
            {
                connection = _context.Database.GetDbConnection();
            }
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection;
        }

        private async Task<T> ExcuteScalar<T>(string sql, object parameter, IDbConnection connection, Stopwatch watch)
        {
            watch.Start();
            var re = await connection.ExecuteScalarAsync<T>(sql, parameter);
            watch.Stop();
            if (_logger != null)
            {
                var querytime = watch.Elapsed.TotalMilliseconds;
                _logger.LogDebug($"Sql:-> {sql} " +
                    System.Environment.NewLine +
                    $" Param:->{JsonConvert.SerializeObject(parameter)}" +
                    System.Environment.NewLine +
                    $"Excute time:{querytime}");
            }
            return re;
        }
    }
}
