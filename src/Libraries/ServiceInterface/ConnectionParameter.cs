//******************************************************************************************************
//  ConnectionParameter.cs - Gbtc
//
//  Copyright © 2024, Grid Protection Alliance.  All Rights Reserved.
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
//  07/28/2024 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using Gemstone.Expressions.Model;
using Gemstone.Reflection.MemberInfoExtensions;
using Gemstone.StringExtensions;
using System.ComponentModel;
using System.Reflection;

namespace ServiceInterface;

/// <summary>
/// Represents a connection parameter.
/// </summary>
/// <remarks>
/// Class is intended to provide UI with a structured representation of custom
/// protocol connection parameters.
/// </remarks>
public class ConnectionParameter
{
    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    public string Name { get; init; } = default!;

    /// <summary>
    /// Gets the category of the parameter.
    /// </summary>
    /// <remarks>
    /// Category is used for UI grouping of parameters.
    /// </remarks>
    public string Category { get; init; } = default!;

    /// <summary>
    /// Gets the description of the parameter.
    /// </summary>
    public string Description { get; init; } = default!;

    /// <summary>
    /// Gets the basic type of the parameter.
    /// </summary>
    public DataType DataType { get; init; }

    /// <summary>
    /// Gets the available values, e.g., when <see cref="DataType"/> is "Enum".
    /// </summary>
    public string[] AvailableValues { get; init; } = [];

    /// <summary>
    /// Gets the default value of the parameter.
    /// </summary>
    public string DefaultValue { get; init; } = default!;

    /// <summary>
    /// Gets or sets the parameter value.
    /// </summary>
    public string Value { get; set; } = default!;

    /// <summary>
    /// Static Constructor that returns a <see cref="ConnectionParameter"/> based on the <see cref="PropertyInfo"/>
    /// </summary>
    public static ConnectionParameter GetConnectionParameter(PropertyInfo info, string connectionString)
    {
        Dictionary<string, string> settings = connectionString.ParseKeyValuePairs();

        return new ConnectionParameter() 
        {
            Name = info.Name,
            Category = getCategory(info),
            Description = getDescription(info),
            DataType = getDataType(info),
            DefaultValue = getDefaultValue(info)?.ToString() ?? "",
            AvailableValues = getAvailableValues(info),
            Value = settings.TryGetValue(info.Name, out string? value) ? value : (getDefaultValue(info)?.ToString() ?? "")
        };

        static string getCategory(PropertyInfo value)
        {
            return value.TryGetAttribute(out CategoryAttribute? attribute) ? attribute.Category : "General";
        }

        static string getDescription(PropertyInfo value)
        {
            return value.TryGetAttribute(out DescriptionAttribute? attribute) ? attribute.Description : string.Empty;
        }

        static object? getDefaultValue(PropertyInfo value)
        {
            if (value.TryGetAttribute(out DefaultValueExpressionAttribute? expressionAttribute))
            {
                ValueExpressionParser parser = new(expressionAttribute?.Expression ?? "");
                return parser.ExecuteFunction();
            }
            return value.TryGetAttribute(out DefaultValueAttribute? attribute) ? attribute.Value : null;
        }

        static DataType getDataType(PropertyInfo value)
        {
            return value.PropertyType switch
            {
                { } type when type == typeof(string) => DataType.String,
                { } type when type == typeof(short) => DataType.Int16,
                { } type when type == typeof(ushort) => DataType.UInt16,
                { } type when type == typeof(int) => DataType.Int32,
                { } type when type == typeof(uint) => DataType.UInt32,
                { } type when type == typeof(long) => DataType.Int64,
                { } type when type == typeof(ulong) => DataType.UInt64,
                { } type when type == typeof(float) => DataType.Single,
                { } type when type == typeof(double) => DataType.Double,
                { } type when type == typeof(DateTime) => DataType.DateTime,
                { } type when type == typeof(bool) => DataType.Boolean,
                { IsEnum: true } => DataType.Enum,
                _ => DataType.String
            };
        }

        static string[] getAvailableValues(PropertyInfo value)
        {
            return value.PropertyType.IsEnum ? Enum.GetNames(value.PropertyType) : [];
        }

    }
}