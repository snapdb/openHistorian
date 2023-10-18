//******************************************************************************************************
//  HistorianStreamEncodingDefinition.cs - Gbtc
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
//  08/10/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//  10/12/2023 - Lillian Gensolin
//       Converted code to .NET core.
//
//******************************************************************************************************

using openHistorian.Core.Snap.Encoding;
using SnapDB.Snap;
using SnapDB.Snap.Definitions;
using SnapDB.Snap.Encoding;

namespace openHistorian.Core.Snap.Definitions;

/// <summary>
/// Defines an encoding definition for historian stream data.
/// </summary>
public class HistorianStreamEncodingDefinition : PairEncodingDefinitionBase
{
    #region [ Properties ]

    /// <summary>
    /// Gets the type of the key if not specified as a generic parameter.
    /// </summary>
    public override Type KeyTypeIfNotGeneric => typeof(HistorianKey);

    /// <summary>
    /// Gets the encoding method.
    /// </summary>
    public override EncodingDefinition Method => TypeGuid;

    /// <summary>
    /// Gets the type of the value if not specified as a generic parameter.
    /// </summary>
    public override Type ValueTypeIfNotGeneric => typeof(HistorianValue);

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Creates an instance of the historian stream encoding for the specified key and value types.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns>An encoding instance for historian stream data.</returns>
    public override PairEncodingBase<TKey, TValue> Create<TKey, TValue>()
    {
        return (PairEncodingBase<TKey, TValue>)(object)new HistorianStreamEncoding();
    }

    #endregion

    #region [ Static ]

    /// <summary>
    /// Sets the encoding definition.
    /// </summary>
    // {0418B3A7-F631-47AF-BBFA-8B9BC0378328}
    public static readonly EncodingDefinition TypeGuid = new(new Guid(0x0418b3a7, 0xf631, 0x47af, 0xbb, 0xfa, 0x8b, 0x9b, 0xc0, 0x37, 0x83, 0x28));

    #endregion
}