//******************************************************************************************************
//  ConnectionStatus.cs - Gbtc
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
//  07/27/2024 - C. Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using System.Text;

namespace ServiceInterface;

/// <summary>
/// Defines the status of a connection.
/// </summary>
public class ConnectionStatus
{
    /// <summary>
    /// Gets the ID of the associated connection.
    /// </summary>
    public Guid ID { get; set; }

    /// <summary>
    /// Gets the name of the associated connection.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets current connection state of the associated connection.
    /// </summary>
    public ConnectionState Status { get; set; }

    /// <summary>
    /// Deserializes the <see cref="ConnectionStatus"/> from a <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">Source stream.</param>
    public void ReadFrom(Stream stream)
    {
        BinaryReader reader = new(stream, Encoding.UTF8, true);

        ID = new Guid(reader.ReadBytes(16));
        Name = reader.ReadString();
        Status = (ConnectionState)reader.ReadByte();
    }

    /// <summary>
    /// Serializes the <see cref="ConnectionStatus"/> to a <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">Target stream.</param>
    public void WriteTo(Stream stream)
    {
        BinaryWriter writer = new(stream, Encoding.UTF8, true);

        writer.Write(ID.ToByteArray());
        writer.Write(Name);
        writer.Write((byte)Status);
    }
}