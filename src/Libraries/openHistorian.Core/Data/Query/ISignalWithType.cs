﻿//******************************************************************************************************
//  ISignalWithTypeConversion.cs - Gbtc
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
//  12/12/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//  10/12/2023 - Lillian Gensolin
//       Converted code to .NET core.
//
//******************************************************************************************************

using openHistorian.Data.Types;

namespace openHistorian.Data.Query;

/// <summary>
/// An interface that allows the results of DatabaseMethods.ExecuteQuery
/// to be strong typed.
/// </summary>
public interface ISignalWithType
{
    #region [ Properties ]

    /// <summary>
    /// A set of functions that will properly convert the value type
    /// from its native format
    /// </summary>
    TypeBase Functions { get; }

    /// <summary>
    /// The Id value of the historian point.
    /// Null means that the point is not in the historian
    /// </summary>
    ulong? HistorianId { get; }

    #endregion
}