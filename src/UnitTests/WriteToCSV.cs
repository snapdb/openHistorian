//******************************************************************************************************
//  WriteToCSV.cs - Gbtc
//
//  Copyright © 2023, Grid Protection Alliance.  All Rights Reserved.
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
//  10/20/2023 - Lillian Gensolin
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Data;
using System.IO;
using System.Linq;
using Gemstone.Data;
using NUnit.Framework;

namespace openHistorian.UnitTests;

internal class WriteToCSV
{
    private const string connectionString = @"Data Source = localhost\SQLEXPRESS; Initial Catalog = openHistorian; Integrated Security = SSPI; Connect Timeout = 5";
    private const string dataProvider = "AssemblyName=Microsoft.Data.SqlClient; ConnectionType=Microsoft.Data.SqlClient.SqlConnection";

    [Test]
    public void ExportDataToCSV()
    {
        using AdoDataConnection connection = new(connectionString, dataProvider);

        try
        {
            if (connection.IsSQLServer)
            {
                const string sqlQuery = @"            
                              SELECT
                                  SUBSTRING([ID], 5, LEN(ID) - 4) AS ID,
                                  [SignalID],
                                  [PointTag],
                                  [AlternateTag],
                                  [SignalReference],
                                  [Device],
                                  [FramesPerSecond],
                                  [Protocol],
                                  [SignalType],
                                  [EngineeringUnits],
                                  [PhasorLabel],
                                  [PhasorType],
                                  [Phase],
                                  [BaseKV],
                                  [Description]
                              FROM [openHistorian].[dbo].[ActiveMeasurement]
                              WHERE ID LIKE 'PPA:%'
                              ORDER BY ID
                ";

                DataTable dataTable = connection.RetrieveData(sqlQuery);

                string csvFilePath = @"C:\Program Files\openHistorian\Archive\2023\03\ppa-metadata.dat";

                StreamWriter writer = new(csvFilePath);
                // Write the header
                string header = string.Join(",", dataTable.Columns.Cast<DataColumn>().Select(column => $"\"{column.ColumnName}\""));
                writer.WriteLine(header);

                // Write the data
                foreach (DataRow row in dataTable.Rows)
                {
                    string line = string.Join(",", row.ItemArray.Select(field => $"\"{field}\""));
                    writer.WriteLine(line);
                }

                Console.WriteLine("CSV file has been created successfully.");
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
}
