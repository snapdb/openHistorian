﻿//******************************************************************************************************
//  CompareTo1.0.cs - Gbtc
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
//  10/16/2023 - Lillian Gensolin
//       Converted code to .NET core.
//
//******************************************************************************************************

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SnapDB.Historian;
using SnapDB.Historian.Files;
using SnapDB.IO;
using NUnit.Framework;

namespace openHistorian.Core.UnitTests.SortedTreeStore.Engine;

	[TestFixture]
	class CompareTo1_0
	{
		private const int PointsToArchive = 100000000;
		private const int MetaDataPoints = 100;

        /// <summary>
        /// Tests the speed of writing data points to the database.
        /// </summary>
    [Test]
		public void TestWriteSpeed()
		{
			foreach (string file in Directory.GetFiles("c:\\temp\\benchmark\\", "*.*", SearchOption.AllDirectories))
				File.Delete(file);

			Console.WriteLine("Creating initial archive file...");

			using (ArchiveFile file = OpenArchiveFile("c:\\temp\\benchmark\\test_archive.d"))
			{
				file.DataWriteException += (sender, e) => Console.WriteLine("Data Write Exception: {0}", e.Argument.Message);
				file.FileFull += (sender, e) => Console.WriteLine("File is full!");
				file.FutureDataReceived += (sender, e) => Console.WriteLine("Future data received");
				file.OrphanDataReceived += (sender, e) => Console.WriteLine("Orphaned data received");

				Console.WriteLine("Start file write...");
				TimeTag now = new TimeTag(DateTime.UtcNow);

				Stopwatch sw = new Stopwatch();
				sw.Start();

				for (int x = 0; x < PointsToArchive / MetaDataPoints; x++)
				{
					for (int i = 1; i <= MetaDataPoints; i++)
						file.WriteData(new ArchiveDataPoint(i, now, x, Quality.Good));

					now = new TimeTag(now.Value + 1.0M);
				}

				double totalTime = sw.Elapsed.TotalSeconds;
				Console.WriteLine("Completed write test in {0:#,##0.00} seconds at {1:#,##0.00} points per second", totalTime, PointsToArchive / totalTime);

				Console.WriteLine("      Points received = {0:#,##0}", file.Fat.DataPointsReceived);
				Console.WriteLine("      Points archived = {0:#,##0}", file.Fat.DataPointsArchived);
				Console.WriteLine("     Data blocks used = {0:#,##0}", file.Fat.DataBlocksUsed);
				Console.WriteLine("Data blocks available = {0:#,##0}", file.Fat.DataBlocksAvailable);
			}
		}

    /// <summary>
    /// Tests the speed of reading data points from the database.
    /// </summary>
    [Test]
		public void TestReadSpeed()
		{
			Console.WriteLine("Opening archive file...");

			using (ArchiveFile file = OpenArchiveFile("c:\\temp\\benchmark\\test_archive.d"))
			{
				file.DataWriteException += (sender, e) => Console.WriteLine("Data Read Exception: {0}", e.Argument.Message);

				Console.WriteLine("Start file read...");
				long pointCount = 0;

				Stopwatch sw = new Stopwatch();
				sw.Start();

				foreach (IDataPoint dataPoint in file.ReadData(Enumerable.Range(1, MetaDataPoints), TimeTag.MinValue, TimeTag.MaxValue))
				{
					//if (dataPoint.Value != 0.0F)
					//    throw new Exception("Corrupt");

					pointCount++;
				}

				double totalTime = sw.Elapsed.TotalSeconds;
				Console.WriteLine("Completed read test in {0:#,##0.00} seconds at {1:#,##0.00} points per second", totalTime, pointCount / totalTime);
				Console.WriteLine("Read points = {0:#,##0}", pointCount);

			}
		}

		private static ArchiveFile OpenArchiveFile(string sourceFileName)
		{
			const string MetadataFileName = "{0}{1}_dbase.dat";
			const string StateFileName = "{0}{1}_startup.dat";
			const string IntercomFileName = "{0}scratch.dat";

			string location = FilePath.GetDirectoryName(sourceFileName);
			string fileName = FilePath.GetFileName(sourceFileName);
			string instance = fileName.Substring(0, fileName.LastIndexOf("_archive", StringComparison.OrdinalIgnoreCase));

			ArchiveFile file = new ArchiveFile
			{
				FileName = sourceFileName,
				FileAccessMode = FileAccess.ReadWrite,
				MonitorNewArchiveFiles = false,
				PersistSettings = false,
				LeadTimeTolerance = 10.0D * 365.0D * 24.0D * 60.0D,
				CompressData = false,
				ConserveMemory = false,
				StateFile = new StateFile
				{
					FileAccessMode = FileAccess.ReadWrite,
					FileName = string.Format(StateFileName, location, instance)
				},
				IntercomFile = new IntercomFile
				{
					FileAccessMode = FileAccess.ReadWrite,
					FileName = string.Format(IntercomFileName, location)
				},
				MetadataFile = new MetadataFile
				{
					FileAccessMode = FileAccess.ReadWrite,
					FileName = string.Format(MetadataFileName, location, instance),
					LegacyMode = MetadataFileLegacyMode.Disabled,
					LoadOnOpen = true
				}
			};

			MetadataFile metadata = file.MetadataFile;

			file.MetadataFile.Open();

			if (file.MetadataFile.RecordsOnDisk == 0)
			{
				Console.WriteLine("Building Metadata file...");

				for (int i = 1; i <= MetaDataPoints; i++)
				{
					metadata.Write(i, new MetadataRecord(i, MetadataFileLegacyMode.Disabled)
					{
						Name = "point" + i,
						GeneralFlags = new MetadataRecordGeneralFlags
						{
							DataType = DataType.Analog,
							Enabled = true
						}
					});
				}

				metadata.Save();
			}

			file.Open();

			file.SynchronizeStateFile();

			return file;
		}
	}
