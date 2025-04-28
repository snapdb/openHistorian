//******************************************************************************************************
//  PointListTests.cs - Gbtc
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

namespace openHistorian.UnitTests.AccessControl;

[TestFixture]
public class PointListTests
{
    [Test]
    public void ReadWithNoPointListRestriction()
    {
        string archivePath = CreateLocalArchive();
        
        HistorianKey key = new();
        HistorianValue value = new();
        
        using HistorianServer server = new(new HistorianServerDatabaseConfig("PPA", archivePath, true), 12345);
        using HistorianClient client = new("127.0.0.1", 12345);
        using ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>("PPA");
        
        using (TreeStream<HistorianKey, HistorianValue> stream = database.Read(0, (ulong)DateTime.MaxValue.Ticks, [1, 2, 3, 4, 5, 6]))
        {
            ulong pointID = 1;

            while (stream.Read(key, value))
            {
                if (key.PointID != pointID++)
                    throw new Exception("Point ID out of order");
            }
        }

        // Max point ID is 1000, so this should only return 2 points
        using (TreeStream<HistorianKey, HistorianValue> stream = database.Read(0, (ulong)DateTime.MaxValue.Ticks, [65, 953, 5562]))
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
    public void ReadWithPointListRestriction()
    {
        string archivePath = CreateLocalArchive();

        SnapSocketListenerSettings<HistorianKey, HistorianValue> settings = new()
        {
            LocalTcpPort = 12345,

            // Setting all the following values to false will force user authentication on the socket.
            // This is a test of access control for a point list with fake users, so this is skipped.
            DefaultUserCanRead = true,
            DefaultUserCanWrite = true,
            DefaultUserIsAdmin = false
        };

        settings.Users.Add("johndoe");
        settings.Users.Add("janedoe");

        Dictionary<string, HashSet<ulong>> pointRights = new()
        {
            { UserInfo.UserNameToSID("johndoe") , [..new ulong[] { 1, 2, 3, 4, 5, 6 }] },
            { UserInfo.UserNameToSID("janedoe"), [..new ulong[] { 65, 953, 5562 }] }
        };

        // Function parameters are:
        // string UserId - The user security ID (SID) of the user attempting to match.
        // TKey instance - The key of the record being matched.
        // TValue instance - The value of the record being matched.
        settings.UserCanMatch = (userID, key, value) => pointRights[userID].Contains(key.PointID);

        TestUser("johndoe", 6, 0);
        TestUser("janedoe", 0, 2);

        void TestUser(string userName, int expectedCount1, int expectedCount2)
        {
            settings!.DefaultUser = userName;

            HistorianKey key = new();
            HistorianValue value = new();

            using HistorianServer server = new(new HistorianServerDatabaseConfig("PPA", archivePath, true), settings);
            using HistorianClient client = new("127.0.0.1", 12345);
            using ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>("PPA");
            
            using (TreeStream<HistorianKey, HistorianValue> stream = database.Read(0, (ulong)DateTime.MaxValue.Ticks, [1, 2, 3, 4, 5, 6]))
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

            // Max point ID is 1000, so this should only return 2 points
            using (TreeStream<HistorianKey, HistorianValue> stream = database.Read(0, (ulong)DateTime.MaxValue.Ticks, [65, 953, 5562]))
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

    private static string CreateLocalArchive(int totalPointCount = 1000)
    {
        const string archivePath = @"C:\Temp\PointACLTestFiles\";
        const string fileName = $"{archivePath}ArchiveFile.d2";

        if (!Directory.Exists(archivePath))
            Directory.CreateDirectory(archivePath);

        if (File.Exists(fileName))
            File.Delete(fileName);

        SortedPointBuffer<HistorianKey, HistorianValue> points = new(totalPointCount, true);
        HistorianKey key = new();
        HistorianValue value = new();

        for (int x = 0; x < totalPointCount; x++)
        {
            key.TimestampAsDate = DateTime.Now;
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
