﻿using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using NpgsqlTypes;

namespace EFCoreUtil.COPY
{
  public class PostgreSQLCopyHelper<TEntity> 
    {
        private TableDefinition Table { get; set; }

        private List<ColumnDefinition<TEntity>> Columns { get; set; }

        public PostgreSQLCopyHelper(string tableName)
            : this(string.Empty, tableName)
        {
        }

        public PostgreSQLCopyHelper(string schemaName, string tableName)
        {
            Table = new TableDefinition
            {
                Schema = schemaName,
                TableName = tableName
            };

            Columns = new List<ColumnDefinition<TEntity>>();
        }

        public void SaveAll(NpgsqlConnection connection, IEnumerable<TEntity> entities)
        {
            using (var binaryCopyWriter = connection.BeginBinaryImport(GetCopyCommand()))
            {
                WriteToStream(binaryCopyWriter, entities);
                
                binaryCopyWriter.Complete();
            }
        }

        public PostgreSQLCopyHelper<TEntity> Map<TProperty>(string columnName, Func<TEntity, TProperty> propertyGetter, NpgsqlDbType type)
        {
            return AddColumn(columnName, (writer, entity) => writer.Write(propertyGetter(entity), type));
        }

        private void WriteToStream(NpgsqlBinaryImporter writer, IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                writer.StartRow();

                foreach (var columnDefinition in Columns)
                {
                    columnDefinition.Write(writer, entity);
                }
            }
        }
        
        private PostgreSQLCopyHelper<TEntity> AddColumn(string columnName, Action<NpgsqlBinaryImporter, TEntity> action)
        {
            Columns.Add(new ColumnDefinition<TEntity>
            {
                ColumnName = columnName,
                Write = action
            });

            return this;
        }

        private string GetCopyCommand()
        {
            var commaSeparatedColumns = string.Join(", ", Columns.Select(x => x.ColumnName));

            return string.Format("COPY {0}({1}) FROM STDIN BINARY;",
                Table.GetFullQualifiedTableName(),
                commaSeparatedColumns);
        }
    }
}