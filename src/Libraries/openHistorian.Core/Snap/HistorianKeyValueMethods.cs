﻿//******************************************************************************************************
//  HistorianKeyValueMethods.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  10/10/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//  10/12/2023 - Lillian Gensolin
//       Converted code to .NET core.
//
//******************************************************************************************************

using SnapDB.Snap.Tree;

namespace openHistorian.Snap;

/// <summary>
/// Uses the standard key and value for the historian and contains a function that copies them from a source to a destination.
/// </summary>
public class HistorianKeyValueMethods : KeyValueMethods<HistorianKey, HistorianValue>
{
    #region [ Methods ]

    /// <summary>
    /// Copies HistorianKey and HistorianValue from a specified source to a specified destination.
    /// </summary>
    /// <param name="srcKey">The source of the key.</param>
    /// <param name="srcValue">The source of the value.</param>
    /// <param name="destKey">The destination for the key.</param>
    /// <param name="dstValue">The destination for the value.</param>
    public override void Copy(HistorianKey srcKey, HistorianValue srcValue, HistorianKey destKey, HistorianValue dstValue)
    {
        destKey.Timestamp = srcKey.Timestamp;
        destKey.PointID = srcKey.PointID;
        destKey.EntryNumber = srcKey.EntryNumber;
        dstValue.Value1 = srcValue.Value1;
        dstValue.Value2 = srcValue.Value2;
        dstValue.Value3 = srcValue.Value3;
    }

    #endregion
}