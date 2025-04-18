//******************************************************************************************************
//  FluentExtension.cs - Gbtc
//
//  Copyright © 2025, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  02/04/2025 - Christoph Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using Azure.Core;
using FluentMigrator;
using FluentMigrator.Builders;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.Execute;
using FluentMigrator.Runner;
using Gemstone.Configuration;
using Gemstone.Data;
using Gemstone.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using SchemaDefinition.Migrations;
using System.CodeDom.Compiler;

namespace SchemaDefinition;

/// <summary>
/// Extension class for FluentMigrator
/// </summary>
public static class FluentExtension
{
    internal static IMigrationRunnerBuilder UseAdoConnectionDatabase(this IMigrationRunnerBuilder migrationRunner, dynamic settings)
    {
        string connectionString = settings.System.ConnectionString;
        string dataProviderString = settings.System.DataProviderString;

        using (AdoDataConnection connection = new(connectionString, dataProviderString))
        {
            if (connection.IsMySQL)
                return migrationRunner.AddMySql5().WithGlobalConnectionString(connectionString);
            if (connection.IsPostgreSQL)
                return migrationRunner.AddPostgres().WithGlobalConnectionString(connectionString);
            if (connection.IsSqlite)
                return migrationRunner.AddSQLite().WithGlobalConnectionString(connectionString);
            if (connection.IsOracle)
                return migrationRunner.AddOracle().WithGlobalConnectionString(connectionString);
            if (connection.IsSQLServer)
                return migrationRunner.AddSqlServer().WithGlobalConnectionString(connectionString);
        }
        return migrationRunner;

    }

    public static void DeleteView(this IExecuteExpressionRoot execute, string viewName)
    {
        execute.Sql(string.Format("DROP VIEW IF EXISTS {0};", viewName));
    }

    public static ICreateTableColumnOptionOrWithColumnSyntax WithCreatedBy(this ICreateTableWithColumnSyntax tableWithColumnSyntax)
    {
             return tableWithColumnSyntax.WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("");
    }

    public static void AddRunTimeSync(this Migration baseClass, string tableName)
    {
        baseClass.IfDatabase("Sqlite").Execute.Sql(string.Format(@" CREATE TRIGGER {0}_RuntimeSync_Insert
            AFTER INSERT ON {0}
            FOR EACH ROW 
            BEGIN 
                INSERT INTO Runtime (SourceID, SourceTable) 
                VALUES(NEW.ID, '{0}'); 
            END;
        ", tableName));

        baseClass.IfDatabase("Sqlite").Execute.Sql(string.Format(@" CREATE TRIGGER {0}_RuntimeSync_Delete
            BEFORE DELETE ON {0}
            FOR EACH ROW 
            BEGIN 
                DELETE FROM Runtime 
                WHERE SourceID = OLD.ID 
                  AND SourceTable = '{0}'; 
            END;
        ", tableName));

        baseClass.IfDatabase("SqlServer").Execute.Sql(string.Format(@" CREATE TRIGGER {0}_RuntimeSync_Insert ON {0}
            AFTER INSERT AS
            BEGIN 
                SET NOCOUNT ON;
                INSERT INTO Runtime (SourceID, SourceTable) 
                SELECT ID, '{0}' FROM INSERTED; 
            END;
        ", tableName));

        baseClass.IfDatabase("SqlServer").Execute.Sql(string.Format(@" CREATE TRIGGER {0}_RuntimeSync_Delete ON {0}
            AFTER DELETE AS
            BEGIN 
               SET NOCOUNT ON;
               DELETE FROM Runtime WHERE SourceID IN (SELECT ID FROM DELETED) AND SourceTable = '{0}'
            END;
        ", tableName));
    }

}