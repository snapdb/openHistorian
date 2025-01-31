//******************************************************************************************************
//  BitGuidReadWrite.cs - Gbtc
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
//  01/31/2025 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Gemstone.Configuration;
using Gemstone.Diagnostics;
using Gemstone.Timeseries;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using openHistorian.Net;
using openHistorian.Snap;
using SnapDB.Snap;
using SnapDB.Snap.Services;
using SnapDB.Snap.Services.Reader;
using Random = Gemstone.Security.Cryptography.Random;
// ReSharper disable InconsistentNaming

namespace openHistorian.UnitTests;

[TestFixture]
public class BitGuidReadWrite
{
    private const string InstanceName = "PPA";
    private const string WorkingDirectory = @"..\..\..\..\..\build\Development\net8.0\Archive\";

    private const string Guid0 = "acb07384-d9a0-4c9e-8f4b-1a2b3c4d5e6f";
    private const string Guid1 = "d3b07384-d9a0-4c9e-8f4b-1a2b3c4d5e6f";
    private const string Guid2 = "e4b07384-d9a0-4c9e-8f4b-1a2b3c4d5e6f";
    private const string Guid3 = "f5b07384-d9a0-4c9e-8f4b-1a2b3c4d5e6f";
    private const string Guid4 = "a6b07384-d9a0-4c9e-8f4b-1a2b3c4d5e6f";
    private const string Guid5 = "b7b07384-d9a0-4c9e-8f4b-1a2b3c4d5e6f";
    private const string Guid6 = "c8b07384-d9a0-4c9e-8f4b-1a2b3c4d5e6f";
    private const string Guid7 = "d9b07384-d9a0-4c9e-8f4b-1a2b3c4d5e6f";
    private const string Guid8 = "eab07384-d9a0-4c9e-8f4b-1a2b3c4d5e6f";
    private const string Guid9 = "fbb07384-d9a0-4c9e-8f4b-1a2b3c4d5e6f";

    private readonly Guid[] SignalIDs =
    [
        Guid.Parse(Guid0),
        Guid.Parse(Guid1),
        Guid.Parse(Guid2),
        Guid.Parse(Guid3),
        Guid.Parse(Guid4),
        Guid.Parse(Guid5),
        Guid.Parse(Guid6),
        Guid.Parse(Guid7),
        Guid.Parse(Guid8),
        Guid.Parse(Guid9)
    ];

    private readonly ulong TestTime;

    public BitGuidReadWrite()
    {
        Settings settings = new()
        {
            INIFile = ConfigurationOperation.ReadWrite,
            SQLite = ConfigurationOperation.Disabled
        };

        // Define settings for service components
        DiagnosticsLogger.DefineSettings(settings);

        // Bind settings to configuration sources
        settings.Bind(new ConfigurationBuilder().ConfigureGemstoneDefaults(settings));

        TestTime = (ulong)DateTime.UtcNow.Ticks;
    }

    [Test]
    public void Test01BitGuidWrite()
    {
        string workingDirectory = Path.GetFullPath(WorkingDirectory);
        HistorianServerDatabaseConfig archiveInfo = new(InstanceName, workingDirectory, true);
        using HistorianServer server = new(archiveInfo);
        using SnapClient client = SnapClient.Connect(server.Host);
        using ClientDatabaseBase<HistorianKey, HistorianValue> database = 
            client.GetDatabase<HistorianKey, HistorianValue>(InstanceName)!;

        // Write a few points to the historian
        HistorianKey key = new();
        HistorianValue value = new();

        for (int i = 0; i < SignalIDs.Length; i++)
        {
            key.Timestamp = TestTime;
            key.PointID = (ulong)(1 + i);

            bool alarmed = Random.Boolean;
            Guid signalID = SignalIDs[i];
            MeasurementStateFlags stateFlags = alarmed ? 
                MeasurementStateFlags.AlarmLow : MeasurementStateFlags.Normal;

            value.AsAlarm = (alarmed, signalID, stateFlags);

            database.Write(key, value);
        }
    }

    [Test]
    public void Test02BitGuidRead()
    {
        string workingDirectory = Path.GetFullPath(WorkingDirectory);
        HistorianServerDatabaseConfig archiveInfo = new(InstanceName, workingDirectory, false);
        using HistorianServer server = new(archiveInfo);
        using SnapClient client = SnapClient.Connect(server.Host);
        using ClientDatabaseBase<HistorianKey, HistorianValue> database =
            client.GetDatabase<HistorianKey, HistorianValue>(InstanceName)!;

        // Read points from the historian
        HistorianKey key = new();
        HistorianValue value = new();
        IEnumerable<ulong> pointIDs = Enumerable.Range(1, SignalIDs.Length).Select(id => (ulong)id);
        using TreeStream<HistorianKey, HistorianValue> stream = database.Read(TestTime, TestTime + 1, pointIDs);

        while (stream.Read(key, value))
        {
            int index = (int)(key.PointID - 1);
            (bool alarmed, Guid signalID, MeasurementStateFlags stateFlags) = value.AsAlarm;

            Assert.AreEqual(SignalIDs[index], signalID);
            Assert.AreEqual(alarmed, stateFlags == MeasurementStateFlags.AlarmLow);
        }
    }
}
