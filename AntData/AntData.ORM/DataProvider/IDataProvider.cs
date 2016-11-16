﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using AntData.ORM.Common;

namespace AntData.ORM.DataProvider
{
	using Data;
	using Mapping;
	using SchemaProvider;
	using SqlProvider;

	public interface IDataProvider
	{
		string             Name                  { get; }
		string             ConnectionNamespace   { get; }
		Type               DataReaderType        { get; }
		MappingSchema      MappingSchema         { get; }
		SqlProviderFlags   SqlProviderFlags      { get; }

		IDbConnection      CreateConnection      (string connectionString);
		ISqlBuilder        CreateSqlBuilder      ();
		ISqlOptimizer      GetSqlOptimizer       ();
		
		object             GetConnectionInfo     (DataConnection dataConnection, string parameterName);
		Expression         GetReaderExpression   (MappingSchema mappingSchema, IDataReader reader, int idx, Expression readerExpression, Type toType);
		bool?              IsDBNullAllowed       (IDataReader reader, int idx);
		void               SetParameter          (IDbDataParameter parameter, string name, DataType dataType, object value);

        void SetParameter(CustomerParam parameter, string name, DataType dataType, object value);
		Type               ConvertParameterType  (Type type, DataType dataType);
		bool               IsCompatibleConnection(IDbConnection connection);

		ISchemaProvider    GetSchemaProvider     ();

		BulkCopyRowsCopied BulkCopy<T>           (DataConnection dataConnection, BulkCopyOptions options, IEnumerable<T> source);
		int                Merge<T>              (DataConnection dataConnection, Expression<Func<T,bool>> predicate, bool delete, IEnumerable<T> source,
		                                          string tableName, string databaseName, string schemaName)
			where T : class;

	}
}
