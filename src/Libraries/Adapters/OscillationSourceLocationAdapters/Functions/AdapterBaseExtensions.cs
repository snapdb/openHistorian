
//******************************************************************************************************
//  DataCleaning.cs - Gbtc
//
//  Copyright © 2025, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  05/28/2025 - G. Santos
//       Generated original version of source code.
//******************************************************************************************************

using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Reflection;
using Gemstone.StringExtensions;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using Gemstone.Timeseries.Data;

namespace DataQualityMonitoring.Functions;

public static class AdapterBaseExtensions
{
    // ToDo: This is repeated code, we should change this to intialize parameters more similarly to how is done in base classes.
    public static void InitializeParameters<T>(this ActionAdapterBase source) where T: ActionAdapterBase
    {
        List<PropertyInfo> propertiesBase = typeof(T).GetProperties().ToList();
        IEnumerable<PropertyInfo> properties = source.GetType()
            .GetProperties()
            // Only init connection string params
            .Where(p => p.GetCustomAttributes<ConnectionStringParameterAttribute>().Any())
            // Skipping base constructor properties
            .Where(p => propertiesBase.FindIndex(pB => string.Equals(pB.Name, p.Name)) < 0);
        Dictionary<string, string> settings = source.Settings;

        foreach (PropertyInfo prop in properties)
        {
            try
            {
                if (!settings.TryGetValue(prop.Name, out string? setting))
                    throw new ArgumentNullException(string.Format("{0} is missing from Settings - Example: framesPerSecond=30; lagTime=3; leadTime=1", prop.Name));

                object? value = null;
                if (prop.PropertyType.IsEnum)
                {
                    value = Enum.Parse(prop.PropertyType, setting);
                }
                else if (string.Equals(prop.PropertyType.Name, "String", StringComparison.OrdinalIgnoreCase))
                {
                    value = setting;
                }
                else
                {
                    MethodInfo? parseFunc = prop.PropertyType.GetMethod("Parse", 0, BindingFlags.Public | BindingFlags.Static, [typeof(string)]);
                    if (parseFunc is null)
                        throw new SettingsPropertyWrongTypeException(string.Format("{0} is unable to be parsed by initialization function.", prop.Name));
                    value = parseFunc.Invoke(null, [setting]);
                }

                prop.SetValue(source, value);
            }
            catch (ArgumentNullException ex)
            {
                DefaultValueAttribute? defaultValue = prop.GetCustomAttribute<DefaultValueAttribute>();
                if (defaultValue is null) throw; // No default

                prop.SetValue(source, defaultValue.Value);
            }
        }
    }
    /// <summary>
    /// Parses a measurement's point tag into its line id and substation. Point Tags are expected to be in the form substation-lineId:otherinfo. 
    /// </summary>
    /// <param name="key">Measurement key being parsed.</param>
    /// <param name="key">Measurement table name.</param>
    /// <returns>Returns a <see cref="Tuple{string,string}"/> with the form of substation first, line id second.</returns>
    public static (string, string) ParseSubstationLineID(this ActionAdapterBase adapter, MeasurementKey key, string measurementTable = nameof(DataSourceLookups.ActiveMeasurements))
    {
        DataSet? dataSource = adapter.DataSource;
        if (dataSource is null)
            throw new ArgumentNullException(nameof(dataSource));

        DataRow? record = dataSource.LookupMetadata(key.SignalID, measurementTable);

        if (record is not null)
        {
            string[] pointTag = record["PointTag"].ToNonNullString().Split(['-'], 2);
            return (pointTag[0], pointTag[1].Split(':')[0]);
        }

        return ("", "");
    }
}