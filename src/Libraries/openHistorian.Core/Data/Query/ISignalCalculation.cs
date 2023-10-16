//******************************************************************************************************
//  ISignalCalculation.cs - Gbtc
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

namespace openHistorian.Data.Query
{
    /// <summary>
    /// Represents a signal calculation with additional properties and methods.
    /// </summary>
    public interface ISignalCalculation : ISignalWithType
    {
        /// <summary>
        /// Gets the unique identifier associated with the signal calculation.
        /// </summary>
        Guid SignalId { get; }

        /// <summary>
        /// Calculates the signal value based on the provided signal data.
        /// </summary>
        /// <param name="signals">A dictionary of signal data indexed by their unique identifiers.</param>
        void Calculate(IDictionary<Guid, SignalDataBase> signals);
    }

}