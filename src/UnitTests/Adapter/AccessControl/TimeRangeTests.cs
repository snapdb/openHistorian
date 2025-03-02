//******************************************************************************************************
//  TimeRangeTests.cs - Gbtc
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
//  11/01/2023 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using Gemstone;
using Gemstone.Identity;
using NUnit.Framework;
using openHistorian.Net;
using openHistorian.Snap;
using SnapDB.Snap;
using SnapDB.Snap.Collection;
using SnapDB.Snap.Services;
using SnapDB.Snap.Services.Net;
using SnapDB.Snap.Services.Reader;
using SnapDB.Snap.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace openHistorian.UnitTests.AccessControl;

[TestFixture]
public class TimeRangeTests
{
    [Test]
    public void ReadWithNoTimeRangeRestriction()
    {
        string archivePath = CreateLocalArchive();

        HistorianKey key = new();
        HistorianValue value = new();

        using HistorianServer server = new(new HistorianServerDatabaseConfig("PPA", archivePath, true), 12345);
        using HistorianClient client = new("127.0.0.1", 12345);
        using ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>("PPA");
        
        using (TreeStream<HistorianKey, HistorianValue> stream = database.Read(0, (ulong)DateTime.MaxValue.Ticks, new ulong[] { 1, 2, 3, 4, 5, 6 }))
        {
            ulong pointID = 1;

            while (stream.Read(key, value))
            {
                if (key.PointID != pointID++)
                    throw new Exception("Point ID out of order");
            }
        }

        // Max point ID is 1000, so this should only return 2 points
        using (TreeStream<HistorianKey, HistorianValue> stream = database.Read(0, (ulong)DateTime.MaxValue.Ticks, new ulong[] { 65, 953, 5562 }))
        {
            int pointCount = 0;

            while (stream.Read(key, value))
                pointCount++;

            if (pointCount != 2)
                throw new Exception("Point count is not 2");

            Console.WriteLine(pointCount);
        }
    }

    [Test]
    public void ReadWithTimeRangeRestriction()
    {
        DateTime startTime = DateTime.UtcNow;
        
        string archivePath = CreateLocalArchive(startTime);

        SnapSocketListenerSettings<HistorianKey, HistorianValue> settings = new()
        {
            LocalTcpPort = 12345,

            // Setting all the following values to false will force user authentication on the socket.
            // This is a test of access control for a time range with fake users, so this is skipped.
            DefaultUserCanRead = true,
            DefaultUserCanWrite = true,
            DefaultUserIsAdmin = false
        };

        settings.Users.Add("johndoe");
        settings.Users.Add("janedoe");

        Dictionary<string, Range<DateTime>> timeRangeRights = new()
        {
            { UserInfo.UserNameToSID("johndoe") , new Range<DateTime>(startTime, startTime.AddDays(50)) },
            { UserInfo.UserNameToSID("janedoe"), new Range<DateTime>(startTime.AddDays(900), startTime.AddDays(1100)) }
        };

        // Function parameters are:
        // string UserId - The user security ID (SID) of the user attempting to seek.
        // TKey instance - The key of the record being sought.
        // AccessControlSeekPosition - The position of the seek. i.e., Start or End.
        settings.UserCanSeek = (userID, key, pos) => timeRangeRights[userID].Contains(key.TimestampAsDate);

        TestUser("johndoe", 50, 0);
        TestUser("janedoe", 0, 100);

        void TestUser(string userName, int expectedCount1, int expectedCount2)
        {
            settings!.DefaultUser = userName;

            HistorianKey key = new();
            HistorianValue value = new();

            using HistorianServer server = new(new HistorianServerDatabaseConfig("PPA", archivePath, true), settings);
            using HistorianClient client = new("127.0.0.1", 12345, false);
            using ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>("PPA");
            
            using (TreeStream<HistorianKey, HistorianValue> stream = database.Read(startTime, startTime.AddDays(50), Enumerable.Range(1, 50).Select(val => (ulong)val)))
            {
                ulong pointID = 1;

                while (stream.Read(key, value))
                {
                    if (key.PointID != pointID++)
                        throw new Exception("Point ID out of order");
                }

                if (pointID != (ulong)expectedCount1 + 1)
                    throw new Exception($"Point count is not {expectedCount1}");
            }

            // Max time range is 1000 days, so this should only return 100 points
            using (TreeStream<HistorianKey, HistorianValue> stream = database.Read(startTime.AddDays(900), startTime.AddDays(1100), Enumerable.Range(900, 200).Select(val => (ulong)val)))
            {
                int pointCount = 0;

                while (stream.Read(key, value))
                    pointCount++;

                if (pointCount != expectedCount2)
                    throw new Exception($"Point count is not {expectedCount2}");

                Console.WriteLine(pointCount);
            }
        }
    }

    private static string CreateLocalArchive(DateTime startTime = default, int totalPointCount = 1000)
    {
        const string archivePath = @"C:\Temp\TimeACLTestFiles\";
        const string fileName = $"{archivePath}ArchiveFile.d2";

        if (startTime == default)
            startTime = DateTime.UtcNow;

        if (!Directory.Exists(archivePath))
            Directory.CreateDirectory(archivePath);

        if (File.Exists(fileName))
            File.Delete(fileName);

        SortedPointBuffer<HistorianKey, HistorianValue> points = new(totalPointCount, true);
        HistorianKey key = new();
        HistorianValue value = new();

        for (int x = 0; x < totalPointCount; x++)
        {
            key.TimestampAsDate = startTime.AddDays(x);
            key.PointID = (ulong)x;
            points.TryEnqueue(key, value);
        }

        points.IsReadingMode = true;

        using SortedTreeFile file = SortedTreeFile.CreateFile(fileName);
        using SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenOrCreateTable<HistorianKey, HistorianValue>(EncodingDefinition.FixedSizeCombinedEncoding);
        using SortedTreeTableEditor<HistorianKey, HistorianValue> editor = table.BeginEdit();

        editor.AddPoints(points);
        editor.Commit();

        return archivePath;
    }
}
