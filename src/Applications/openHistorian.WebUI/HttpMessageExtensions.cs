//******************************************************************************************************
//  HttpMessageExtensions.cs - Gbtc
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
//  05/28/2025 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using Microsoft.Extensions.Primitives;

namespace openHistorian.WebUI;

public static class HttpMessageExtensions
{
    public static async Task<HttpRequestMessage> ConvertHTTPRequestAsync(this HttpRequest request, CancellationToken cancellationToken)
    {
        HttpRequestMessage requestMessage = new();
        requestMessage.Method = new HttpMethod(request.Method);
        requestMessage.RequestUri = new Uri($"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}");

        foreach (KeyValuePair<string, StringValues> header in request.Headers)
            requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());

        if (request.ContentLength == 0)
            return requestMessage;

        using MemoryStream stream = new();

        try
        {
            await request.Body.CopyToAsync(stream, cancellationToken);
        }
        catch (ObjectDisposedException)
        {
        }

        stream.Position = 0;
        requestMessage.Content = new StreamContent(stream);

        return requestMessage;
    }
}
