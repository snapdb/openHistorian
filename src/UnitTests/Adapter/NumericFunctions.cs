//******************************************************************************************************
//  WriteToCSV.cs - Gbtc
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
//  10/20/2023 - Lillian Gensolin
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Gemstone.Data;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace openHistorian.UnitTests;

internal class NumericFunctions
{

    [Test]
    public void NaNMedian()
    {
        IEnumerable<double> data = new double[] { 1, 2, 3, double.NaN, 5, 6, 7, 8, 9, 10 };

        double median = data.Median();
    }
}
