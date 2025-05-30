﻿//******************************************************************************************************
//  GrafanaAuthProxyController.cs - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
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
//  12/02/2017 - J. Ritchie Carroll
//       Generated original version of source code.
//  01/24/2020 - C. Lackner
//       Updated to avoid issues with accessing panel json directly for alarm setup.
//
//******************************************************************************************************
// ReSharper disable NotAccessedField.Local
// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Local
#pragma warning disable CS0649
#pragma warning disable CS8618
#pragma warning disable CA1416

// If proxy needs cookies to be forwarded to Grafana, e.g., for grafana-session management, uncomment:
//#define ProxyCookies

using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Gemstone;
using Gemstone.Collections.CollectionExtensions;
using Gemstone.Configuration;
using Gemstone.Data;
using Gemstone.Diagnostics;
using Gemstone.IO;
using Gemstone.Security.Cryptography;
using Gemstone.StringExtensions;
using Gemstone.Threading.SynchronizedOperations;
using Gemstone.Timeseries.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using openHistorian.Adapters;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using AcceptVerbsAttribute = Microsoft.AspNetCore.Mvc.AcceptVerbsAttribute;
using CancellationToken = System.Threading.CancellationToken;
using Http = System.Net.WebRequestMethods.Http;

namespace openHistorian.WebUI.Controllers;

/// <summary>
/// Creates a reverse proxy to a hosted Grafana instance that handles integrated authentication.
/// </summary>
[ApiController]
[Route("grafana")]
public class GrafanaAuthProxyController : ControllerBase, IDefineSettings
{
    #region [ Members ]

    // Nested Types
    private class UserDetail
    {
        public int id;
        public string email;
        public string name;
        public string login;
        public string theme;
        public int orgId;
        public bool isGrafanaAdmin;
    }

    private class OrgUserDetail
    {
        public int orgId;
        public int userId;
        public string email;
        public string login;
        public string role;
    }

    // Constants

    /// <summary>
    /// Defines the default installation server path for Grafana.
    /// </summary>
    public const string DefaultServerPath = "Grafana\\bin\\grafana-server.exe";

    /// <summary>
    /// Default URL for the hosted Grafana process.
    /// </summary>
    public const string DefaultHostedURL = "http://localhost:8185";

    /// <summary>
    /// Default timeout, in seconds, for system initialization.
    /// </summary>
    public const int DefaultInitializationTimeout = 30;

    /// <summary>
    /// Default Grafana organization ID.
    /// </summary>
    public const int DefaultOrganizationID = 1;

    /// <summary>
    /// Default cookie name for last visited Grafana dashboard.
    /// </summary>
    public const string DefaultLastDashboardCookieName = "x-last-dashboard";

    /// <summary>
    /// Grafana admin role name.
    /// </summary>
    public const string GrafanaAdminRoleName = "GrafanaAdmin";

    /// <summary>
    /// Default settings category for Grafana hosting.
    /// </summary>
    public const string DefaultSettingsCategory = "grafanaHosting";

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Handle proxy of the root Grafana URL, e.g., http://localhost:8180/grafana.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Proxied response.</returns>
    [AcceptVerbs(Http.Get, Http.Head, Http.Post, Http.Put, Http.MkCol), HttpDelete, HttpPatch]
    [Route("")]
    //[EnableCors]
    public Task ProxyRoot(CancellationToken cancellationToken)
    {
        return ProxyPage("", cancellationToken);
    }

    /// <summary>
    /// Handle proxy of specified Grafana URL.
    /// </summary>
    /// <param name="url">URL to proxy.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Proxied response.</returns>
    [AcceptVerbs(Http.Get, Http.Head, Http.Post, Http.Put, Http.MkCol), HttpDelete, HttpPatch]
    [Route("{*url}")]
    //[EnableCors]
    public async Task ProxyPage(string url, CancellationToken cancellationToken)
    {
        //HACK: Convert all calls to use HttpRequest instead of converted HttpRequestMessage
        using HttpRequestMessage request = await Request.ConvertHTTPRequestAsync(cancellationToken);

        // Handle special URL commands
        HttpResponseMessage? response = url.ToLowerInvariant() switch
        {
            "syncusers" => HandleSynchronizeUsersRequest(request, User.Identity?.Name),
            "servertime" => HandleServerTimeRequest(request),
            "logout" => HandleGrafanaLogoutRequest(request),
            "api/login/ping" => HandleGrafanaLoginPingRequest(request, User.Identity?.Name),
            _ => null
        };

        if (response is null)
        {
            if (url.StartsWith("servervar/", StringComparison.Ordinal))
            {
                response = HandleServerVarRequest(request);
            }
            else if (url.StartsWith("avatar/", StringComparison.OrdinalIgnoreCase))
            {
                response = HandleGrafanaAvatarRequest(request);
            }
            else if (url.StartsWith("keycoordinates", StringComparison.OrdinalIgnoreCase))
            {
                response = HandleKeyCoordinatesRequest(request);
            }
            else
            {
                // Proxy all other requests
                //SecurityPrincipal? securityPrincipal = RequestContext.Principal as SecurityPrincipal;

                //if (securityPrincipal?.Identity is null)
                //    throw new SecurityException($"User \"{RequestContext.Principal?.Identity?.Name}\" is unauthorized.");

            #if ProxyCookies
                // Forward any cookies from client request to Grafana
                if (Request.Headers.Contains("Cookie"))
                {
                    Request.Headers.Remove("Cookie");
                    Request.Headers.Add("Cookie", Request.Headers.GetCookies().Select(c => c.ToString()));
                }
            #endif

                request.Headers.Add(s_authProxyHeaderName, User.Identity?.Name ?? "admin");
                request.Headers.ConnectionClose = true;
                request.RequestUri = new Uri($"{s_baseUrl}/{url}{request.RequestUri!.Query}");

                if (request.Method == HttpMethod.Get)
                    request.Content = null;

                try
                {
                    response = await s_http.SendAsync(request, cancellationToken);
                }
                catch (HttpRequestException) {}
                catch (ObjectDisposedException) {}

            #if ProxyCookies
                // Forward any Set-Cookie headers from Grafana back to client
                if (response.Headers.Contains("Set-Cookie"))
                {
                    IEnumerable<string> cookies = response.Headers.GetValues("Set-Cookie");
                    response.Headers.Remove("Set-Cookie");

                    foreach (string cookie in cookies)
                        response.Headers.Add("Set-Cookie", cookie);
                }
            #endif
            }
        }

        if (response is null)
            return;

        try
        {
            HttpStatusCode statusCode = response.StatusCode;

            if (statusCode is HttpStatusCode.NotFound or HttpStatusCode.Unauthorized)
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    string? userName = (state as ClaimsPrincipal)?.Identity?.Name;

                    if (string.IsNullOrEmpty(userName))
                        return;

                    try
                    {
                        // Validate user has a role defined in latest security context
                        Dictionary<string, string[]> securityContext = s_latestSecurityContext ?? StartUserSynchronization(userName);

                        //if (securityContext is null)
                        //    throw new InvalidOperationException("Failed to load security context");

                        string newUserMessage = securityContext.ContainsKey(userName) ? "" : $"New user \"{userName}\" encountered. ";

                        OnStatusMessage($"{newUserMessage}Security context with {securityContext.Count:N0} users and associated roles queued for Grafana user synchronization.");
                    }
                    catch (Exception ex)
                    {
                        OnStatusMessage($"ERROR: Failed while queuing Grafana user synchronization for new user \"{userName}\": {ex.Message}");
                    }
                },
                User.Identity);
            }

            // Always keep last visited Grafana dashboard in a client session cookie
            if (url.StartsWith("api/dashboards/", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(request.Headers.Referrer?.AbsolutePath))
                response.Headers.AddCookies([new CookieHeaderValue(s_lastDashboardCookieName, $"{request.Headers.Referrer.AbsolutePath}") { Path = "/" }]);

            foreach (KeyValuePair<string, IEnumerable<string>> header in response.Headers)
                HttpContext.Response.Headers[header.Key] = header.Value.ToArray();

            foreach (KeyValuePair<string, IEnumerable<string>> header in response.Content.Headers)
                HttpContext.Response.Headers[header.Key] = header.Value.ToArray();

            // Prevent ASP.NET Core from writing its own headers
            HttpContext.Response.Headers.Remove("transfer-encoding");

            // Set content type
            if (response.Content.Headers.ContentType != null)
                HttpContext.Response.ContentType = response.Content.Headers.ContentType.ToString();

            if (response.Content.Headers.ContentLength.GetValueOrDefault() > 0)
            {
                try
                {
                    await response.Content.CopyToAsync(HttpContext.Response.Body, cancellationToken);
                }
                catch (ObjectDisposedException)
                {
                }
            }
        }
        finally
        {
            response.Dispose();
        }
    }

    #endregion

    #region [ Static ]

    // Static Events

    /// <summary>
    /// Raised when the Grafana authentication proxy reports an important status message.
    /// </summary>
    public static event EventHandler<EventArgs<string>>? StatusMessage;

    // Static Fields
    private static readonly HttpClient s_http;
    private static string s_baseUrl;
    private static string s_authProxyHeaderName;
    private static int s_initializationTimeout;
    private static string s_adminUser;
    private static string s_logoutResource;
    private static string s_avatarResource;
    private static byte[]? s_avatarResourceContent;
    private static MediaTypeHeaderValue? s_avatarResourceMediaType;
    private static int s_organizationID;
    private static string s_lastDashboardCookieName;
    private static ShortSynchronizedOperation? s_synchronizeUsers;
    private static ManualResetEventSlim? s_initializationWaitHandle;
    private static Dictionary<string, string[]>? s_lastSecurityContext;
    private static Dictionary<string, string[]>? s_latestSecurityContext;
    private static bool s_manualSynchronization;

    // Static Properties

    /// <summary>
    /// Gets or sets reference top global settings.
    /// </summary>
    public static readonly GlobalSettings? GlobalSettings = new();

    // Static Constructor
    static GrafanaAuthProxyController()
    {
        // Create a shared HTTP client instance
        s_http = new HttpClient(new HttpClientHandler { 
            UseCookies = false, 
            MaxResponseHeadersLength = 64,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });

    #if DEBUG
        // Debugging adds run-time overhead, provide more time for initialization
        s_initializationTimeout *= 3;
    #endif
    }

    // Static Methods
    
    /// <inheritdoc cref="IDefineSettings.DefineSettings" />
    public static void DefineSettings(Settings settings, string settingsCategory = DefaultSettingsCategory)
    {
        dynamic section = settings[settingsCategory];

        // Make sure needed settings exist
        section.ServerPath = (DefaultServerPath, "Defines the path to the Grafana server to host - set to empty string to disable hosting.");
        section.AuthProxyHeaderName = ("X-WEBAUTH-USER", "Defines the authorization header name used for Grafana user authentication.");
        section.InitializationTimeout = (DefaultInitializationTimeout, "Defines the timeout, in seconds, for the Grafana system to initialize.");
        section.AdminUser = ("admin", "Defines the Grafana Administrator user required for managing configuration.");
        section.LogoutResource = ("Logout.cshtml", "Defines the relative URL to the logout page to use for Grafana users.");
        section.AvatarResource = ("Images/Icons/openHistorian.png", "Defines the relative URL to the 40x40px avatar image to use for Grafana users.");
        section.HostedURL = (DefaultHostedURL, "Defines the local URL to the hosted Grafana server instance. Setting is for internal use, external access to Grafana instance will be proxied via WebHostURL.");
        section.OrganizationID = (DefaultOrganizationID, "Defines the database ID of the target organization used for user synchronization.");
        section.LastDashboardCookieName = (DefaultLastDashboardCookieName, "Defines the session cookie name used to save the last visited Grafana dashboard.");

        s_authProxyHeaderName = section["AuthProxyHeaderName"];
        s_initializationTimeout = section["InitializationTimeout"] * 1000;
        s_adminUser = section["AdminUser"];
        s_logoutResource = section["LogoutResource"];
        s_avatarResource = section["AvatarResource"];
        s_baseUrl = section["HostedURL"];
        s_organizationID = section["OrganizationID"];
        s_lastDashboardCookieName = section["LastDashboardCookieName"];

        if (!System.IO.File.Exists(section["ServerPath"]))
            return;

        s_initializationWaitHandle = new ManualResetEventSlim();

        // Establish a synchronized operation for handling Grafana user synchronizations
        s_synchronizeUsers = new ShortSynchronizedOperation(SynchronizeUsers, ex =>
        {
            OnStatusMessage($"ERROR: {GetExceptionMessage(ex)}");
        });

        // Attach to event for notifications of when security context has been refreshed
        //AdoSecurityProvider.SecurityContextRefreshed += AdoSecurityProvider_SecurityContextRefreshed;
    }

    /// <summary>
    /// Signal authentication proxy that initialization is complete.
    /// </summary>
    public static void InitializationComplete()
    {
        s_initializationWaitHandle?.Set();
    }

    /// <summary>
    /// Gets a flag that determines if configured Grafana server is responding.
    /// </summary>
    /// <returns><c>true</c> if Grafana server is responding; otherwise, <c>false</c>.</returns>
    public static bool ServerIsResponding()
    {
        const string ServerCheckUrl = "/api/health";

    #if DEBUG
        const string DebugMessage = "DEBUG: ServerIsResponding check for \"{0}" + ServerCheckUrl + "\" result:\r\n{1}";

        try
        {
            // Test server response by hitting health page
            dynamic? result = CallAPIFunction(HttpMethod.Get, $"{s_baseUrl}{ServerCheckUrl}").Result;
            OnStatusMessage(string.Format(DebugMessage, s_baseUrl, result?.ToString() ?? "null"));
            return result is not null;
        }
        catch (Exception ex)
        {
            OnStatusMessage(string.Format(DebugMessage, s_baseUrl, GetExceptionMessage(ex)));
            return false;
        }
    #else
        try
        {
            // Test server response by hitting health page
            dynamic? result = CallAPIFunction(HttpMethod.Get, $"{s_baseUrl}{ServerCheckUrl}").Result;
            return result is not null;
        }
        catch
        {
            return false;
        }
    #endif
    }

    private static void OnStatusMessage(string status)
    {
        StatusMessage?.Invoke(typeof(GrafanaAuthProxyController), new EventArgs<string>(status));
    }

    //private static void AdoSecurityProvider_SecurityContextRefreshed(object? sender, EventArgs<Dictionary<string, string[]>> e)
    //{
    //    Interlocked.Exchange(ref s_latestSecurityContext, e.Argument);
    //    s_synchronizeUsers?.RunAsync();
    //}

    private static void SynchronizeUsers()
    {
        using (Logger.SuppressFirstChanceExceptionLogMessages())
        {
            Dictionary<string, string[]>? securityContext = s_latestSecurityContext;

            if (securityContext is null)
                return;

            // Skip user synchronization if security context has not changed
            if (!s_manualSynchronization && SecurityContextsAreEqual(securityContext, s_lastSecurityContext))
                return;

            s_manualSynchronization = false;
            Interlocked.Exchange(ref s_lastSecurityContext, securityContext);

            // Give initialization - which includes starting Grafana server process - a chance to complete
            if (s_initializationWaitHandle is not null && !s_initializationWaitHandle.Wait(s_initializationTimeout))
                OnStatusMessage($"WARNING: Grafana user synchronization reported timeout awaiting Grafana initialization. Timeout configured as {Ticks.FromMilliseconds(s_initializationTimeout).ToElapsedTimeString(2)}.");

            // Lookup Grafana Administrative user
            if (!LookupUser(s_adminUser, out UserDetail? userDetail, out string? message))
            {
                OnStatusMessage($"WARNING: Failed to synchronize Grafana users, cannot find Grafana Administrator \"{s_adminUser}\": {message}");
                return;
            }

            // Get user list for target organization
            OrgUserDetail[] organizationUsers = GetOrganizationUsers(s_organizationID, out message);

            if (!string.IsNullOrEmpty(message))
                OnStatusMessage($"Issue retrieving user list for default organization: {message}");

            // Make sure Grafana Administrator has an admin role in the default organization
            bool success = organizationUsers.Any(user => user.userId == userDetail?.id) ?
                UpdateUserOrganizationalRole(s_organizationID, userDetail!.id, "Admin", out message) :
                AddUserToOrganization(s_organizationID, s_adminUser, "Admin", out message);

            if (!success)
                OnStatusMessage($"Issue validating organizational admin role for Grafana Administrator \"{s_adminUser}\" - Grafana user synchronization may not succeed: {message}");

            foreach ((string? userName, string[]? roles) in securityContext)
            {
                bool createdUser = false;

                if (userName.Equals("_logout", StringComparison.OrdinalIgnoreCase))
                    continue;

                // Check if user exists
                if (!LookupUser(userName, out userDetail, out message))
                {
                    createdUser = CreateUser(userName, s_organizationID, out userDetail, out message);
                    OnStatusMessage($"Encountered new user \"{userName}\": {message}");
                }

                if (userDetail!.id == 0)
                    continue;

                // Update user's Grafana admin role status if needed
                bool userIsGrafanaAdmin = UserIsGrafanaAdmin(roles);

                if (userDetail.isGrafanaAdmin != userIsGrafanaAdmin)
                {
                    try
                    {
                        JObject content = JObject.FromObject(new
                        {
                            isGrafanaAdmin = userIsGrafanaAdmin
                        });

                        message = CallAPIFunction(HttpMethod.Put, $"{s_baseUrl}/api/admin/users/{userDetail.id}/permissions", content.ToString()).Result?.message;
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                    }

                    if (!(message?.Equals("User permissions updated", StringComparison.OrdinalIgnoreCase) ?? false))
                        OnStatusMessage($"Issue updating permissions for user \"{userName}\": {message}");
                }

                // Attempt to lookup user in default organization
                OrgUserDetail? orgUserDetail = organizationUsers.FirstOrDefault(user => user.userId == userDetail.id);

                // Get user's organizational role: Admin / Editor / Viewer
                string organizationalRole = TranslateRole(roles);

                // Update user's organizational status / role as needed
                if (orgUserDetail is null && !createdUser)
                    success = AddUserToOrganization(s_organizationID, userName, organizationalRole, out message);
                else if (createdUser || !orgUserDetail!.role.Equals(organizationalRole, StringComparison.OrdinalIgnoreCase))
                    success = UpdateUserOrganizationalRole(s_organizationID, userDetail.id, organizationalRole, out message);
                else
                    success = true;

                if (!success)
                    OnStatusMessage($"Issue assigning organizational role \"{organizationalRole}\" for user \"{userName}\": {message}");
            }

            OnStatusMessage($"Synchronized security context with {securityContext.Count:N0} users to Grafana.");
        }
    }

    private static bool LookupUser(string userName, out UserDetail? userDetail, out string? message)
    {
        userDetail = null;

        try
        {
            JObject? result;
            message = null;

            try
            {
                result = CallAPIFunction(HttpMethod.Get, $"{s_baseUrl}/api/users/lookup?loginOrEmail={userName}").Result;
            }
            catch (Exception ex)
            {
                message = GetExceptionMessage(ex);
                return false;
            }

            if (result!.TryGetValue("id", out _))
            {
                try
                {
                    userDetail = result.ToObject<UserDetail>();
                    return true;
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return false;
                }
            }

            if (result.TryGetValue("message", out JToken? token))
                message = token.Value<string>();

            return false;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return false;
        }
    }

    private static bool CreateUser(string userName, int orgID, out UserDetail? userDetail, out string? message)
    {
        try
        {
            // Create a new user
            JObject content = JObject.FromObject(new
            {
                name = userName,
                email = "",
                login = userName,
                password = PasswordGenerator.Default.GeneratePassword()
            });

            userDetail = new UserDetail
            {
                name = userName,
                login = userName,
                orgId = orgID
            };

            dynamic? result = CallAPIFunction(HttpMethod.Post, $"{s_baseUrl}/api/admin/users", content.ToString()).Result;

            message = result?.message;

            if (result?.id is null)
                return false;

            userDetail.id = (int)result.id;
            return true;
        }
        catch (Exception ex)
        {
            userDetail = null;
            message = ex.Message;
            return false;
        }
    }

    private static OrgUserDetail[] GetOrganizationUsers(int orgID, out string? message)
    {
        try
        {
            message = null;
            return ((JArray?)CallAPIFunction(HttpMethod.Get, $"{s_baseUrl}/api/orgs/{orgID}/users", responseIsArray: true).Result)?.ToObject<OrgUserDetail[]>() ?? [];
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return [];
        }
    }

    private static bool UpdateUserOrganizationalRole(int orgID, int userID, string organizationalRole, out string? message)
    {
        try
        {
            JObject content = JObject.FromObject(new
            {
                role = organizationalRole
            });

            message = CallAPIFunction(new HttpMethod("PATCH"), $"{s_baseUrl}/api/orgs/{orgID}/users/{userID}", content.ToString()).Result?.message;

            return message?.Equals("Organization user updated", StringComparison.OrdinalIgnoreCase) ?? false;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return false;
        }
    }

    private static bool AddUserToOrganization(int orgID, string userName, string organizationalRole, out string? message)
    {
        try
        {
            JObject content = JObject.FromObject(new
            {
                loginOrEmail = userName,
                role = organizationalRole
            });

            message = CallAPIFunction(HttpMethod.Post, $"{s_baseUrl}/api/orgs/{orgID}/users", content.ToString()).Result?.message;

            return message?.Equals("User added to organization", StringComparison.OrdinalIgnoreCase) ?? false;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return false;
        }
    }

    private static async Task<dynamic?> CallAPIFunction(HttpMethod method, string url, string? content = null, bool responseIsArray = false)
    {
        HttpRequestMessage request = new(method, url);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Add(s_authProxyHeaderName, s_adminUser);

        if (content is not null)
            request.Content = new StringContent(content, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await s_http.SendAsync(request);

        content = await response.Content.ReadAsStringAsync();

        if (responseIsArray)
            return JArray.Parse(content);
                
        return JObject.Parse(content);
    }

    private static bool SecurityContextsAreEqual(Dictionary<string, string[]>? left, Dictionary<string, string[]>? right)
    {
        if (left == right)
            return true;

        if (left is null || right is null)
            return false;

        if (left.Count != right.Count)
            return false;

        foreach (KeyValuePair<string, string[]> item in left)
        {
            if (!right.TryGetValue(item.Key, out string[]? value))
                return false;

            if (item.Value.CompareTo(value) != 0)
                return false;
        }

        return true;
    }

    private static bool UserIsGrafanaAdmin(string[] roles)
    {
        return roles.Any(role => role.Equals(GrafanaAdminRoleName, StringComparison.OrdinalIgnoreCase));
    }

    private static string TranslateRole(string[] roles)
    {
        if (roles.Any(role => role.Equals("Administrator", StringComparison.OrdinalIgnoreCase)))
            return "Admin";

        if (roles.Any(role => role.Equals("Editor", StringComparison.OrdinalIgnoreCase)))
            return "Editor";

        return "Viewer";
    }

    private static Dictionary<string, string[]> StartUserSynchronization(string? currentUserName)
    {
        // TODO: Re-enable security integration with context synchronization
        //Dictionary<string, string[]> userRoles = new(StringComparer.OrdinalIgnoreCase);

        //using AdoDataConnection connection = new("systemSettings");
        //using UserRoleCache userRoleCache = UserRoleCache.GetCurrentCache();
        //TableOperations<UserAccount> userAccountTable = new(connection);
        //string[] roles;

        //foreach (UserAccount user in userAccountTable.QueryRecords())
        //{
        //    string userName = user.AccountName;

        //    if (userRoleCache.TryGetUserRole(userName, out roles))
        //        userRoles[userName] = roles;
        //}

        //// Also make sure current user is added since user may have implicit rights based on group
        //if (!string.IsNullOrEmpty(currentUserName))
        //{
        //    if (!userRoles.ContainsKey(currentUserName) && userRoleCache.TryGetUserRole(currentUserName, out roles))
        //        userRoles[currentUserName] = roles;
        //}

        //if (userRoles.Count > 0)
        //{
        //    Interlocked.Exchange(ref s_latestSecurityContext, userRoles);
        //    s_manualSynchronization = true;
        //    s_synchronizeUsers.RunAsync();
        //}

        Dictionary<string, string[]> userRoles = new(StringComparer.OrdinalIgnoreCase)
        {
            { currentUserName ?? "admin", ["Administrator"] }
        };

        return userRoles;
    }

    private static HttpResponseMessage HandleSynchronizeUsersRequest(HttpRequestMessage request, string? userName)
    {
        Dictionary<string, string[]> userRoles = StartUserSynchronization(userName);

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            RequestMessage = request,
            Content = new StringContent($@"
                    <html>
                    <head>
                        <title>Grafana User Synchronization</title>
                        <link rel=""shortcut icon"" href=""@GSF/Web/Shared/Images/Icons/favicon.ico"" />
                    </head>
                    <body>
                        Security context with {userRoles.Count:N0} users and associated roles queued for Grafana user synchronization.
                    </body>
                    </html>
                ",
                Encoding.UTF8, "text/html")
        };
    }

    private static HttpResponseMessage HandleServerTimeRequest(HttpRequestMessage request)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            RequestMessage = request,
            Content = new StringContent(JObject.FromObject(new
            {
                serverTime = $"{DateTime.UtcNow:MM/dd/yyyy HH:mm:ss.fff} UTC"
            })
            .ToString(), Encoding.UTF8, "application/json")
        };
    }

    private static HttpResponseMessage HandleServerVarRequest(HttpRequestMessage request)
    {
        string variableValue = "NaN";

        if (GlobalSettings is not null)
        {
            string[] parts = request.RequestUri?.AbsolutePath.Split(['/'], StringSplitOptions.RemoveEmptyEntries) ?? [];

            if (parts.Length > 1)
            {
                string variableName = parts[^1];

                if (!string.IsNullOrWhiteSpace(variableValue))
                {
                    // We tightly control which variables to expose to prevent unnecessary data leakage
                    variableValue = variableName.ToUpperInvariant() switch
                    {
                        //"COMPANYNAME"      => GlobalSettings.CompanyName,
                        "COMPANYACRONYM"   => GlobalSettings.CompanyAcronym,
                        //"NOMINALFREQUENCY" => GlobalSettings.NominalFrequency.ToString(),
                        "SYSTEMNAME"       => GlobalSettings.SystemName,
                        _                  => "NaN"
                    };
                }
            }
        }

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            RequestMessage = request,
            Content = new StringContent(JObject.FromObject(new
            {
                value = variableValue
            })
            .ToString(), Encoding.UTF8, "application/json")
        };
    }

    private static HttpResponseMessage HandleGrafanaLogoutRequest(HttpRequestMessage request)
    {
        HttpResponseMessage response = new(HttpStatusCode.Redirect) { RequestMessage = request };
        Uri requestUri = request.RequestUri!, referrerUri = request.Headers.Referrer!;

        if (referrerUri.AbsolutePath.ToLowerInvariant().Contains("/grafana/"))
        {
            // Handle user requested logout
            response.Headers.Location = new Uri($"{requestUri.Scheme}://{requestUri.Host}:{requestUri.Port}/{s_logoutResource}");
        }
        else
        {
            // Handle automated logout, returning to original Grafana page
            string? lastDashboard = null;

            if (!string.IsNullOrWhiteSpace(referrerUri.Query))
            {
                Dictionary<string, string> parameters = referrerUri.ParseQueryString().ToDictionary();

                if (parameters.TryGetValue("referrer", out string? referrer))
                {
                    string base64Path = WebUtility.UrlDecode(referrer);
                    byte[] pathBytes = Convert.FromBase64String(base64Path);
                    referrerUri = new Uri(Encoding.UTF8.GetString(pathBytes));
                    lastDashboard = referrerUri.PathAndQuery;
                }
            }

            if (string.IsNullOrWhiteSpace(lastDashboard))
            {
                // Without knowing the original referrer, the best we can do is restore last screen only
                CookieHeaderValue? lastDashboardCookie = request.Headers.GetCookies(s_lastDashboardCookieName).FirstOrDefault();            
                lastDashboard = lastDashboardCookie?[s_lastDashboardCookieName].Value;
            }

            if (string.IsNullOrWhiteSpace(lastDashboard))
            {
                // As a last resort (i.e., no cookie or original referrer), return to the Grafana home page
                response.Headers.Location = new Uri($"{requestUri.Scheme}://{requestUri.Host}:{requestUri.Port}/grafana");
            }
            else
            {
                if (!lastDashboard.Contains('?'))
                    lastDashboard += $"?orgId={s_organizationID}";

                response.Headers.Location = new Uri($"{requestUri.Scheme}://{requestUri.Host}:{requestUri.Port}{lastDashboard}");
                OnStatusMessage($"Reloading previous Grafana dashboard: {lastDashboard}");
            }
        }

        return response;
    }

    // ReSharper disable once UnusedParameter.Local
    private static HttpResponseMessage HandleGrafanaLoginPingRequest(HttpRequestMessage request, string? userName)
    {
        // TODO: Fix user security check
        //string userLoginState = securityPrincipal?.Identity is null ? "Unauthorized" : "Logged in";
        string userLoginState = "Logged in";

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            RequestMessage = request,
            Content = new StringContent(JObject.FromObject(new
            {
                message = userLoginState
            })
            .ToString(), Encoding.UTF8, "application/json")
        };
    }

    private HttpResponseMessage HandleKeyCoordinatesRequest(HttpRequestMessage request)
    {
        //      0         1         2          3            4 
        // /grafana/KeyCoordinates/PPA/ActiveMeasurements/FILTER
        //                      instance      source      filter
        string[] parts = request.RequestUri!.AbsolutePath.Split(['/'], StringSplitOptions.RemoveEmptyEntries);
        string instanceName = "PPA";
        string source = "ActiveMeasurements";
        string filter = "";

        if (parts.Length > 2)
            instanceName = Uri.UnescapeDataString(parts[2].Trim());

        if (parts.Length > 3)
            source = Uri.UnescapeDataString(parts[3].Trim());

        if (parts.Length > 4)
            filter = Uri.UnescapeDataString(parts[4].Trim());

        if (string.IsNullOrEmpty(filter))
        {
            if (source.Equals("ActiveMeasurements", StringComparison.OrdinalIgnoreCase))
                filter = "SignalType <> 'STAT'";
            else if (source.Equals("MeasurementDetail", StringComparison.OrdinalIgnoreCase))
                filter = "SignalAcronym <> 'STAT'";
        }

        if (instanceName.Equals("keycoordinates", StringComparison.OrdinalIgnoreCase))
            instanceName = "PPA";

        DataSet? dataSource = null;

        if (LocalOutputAdapter.Instances.TryGetValue(instanceName, out LocalOutputAdapter? adapterInstance))
            dataSource = adapterInstance.DataSource;

        if (dataSource is null)
        {
            return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                RequestMessage = request,
                Content = new StringContent($"Failed to find data source for \"{instanceName}\".", Encoding.UTF8, "text/plain")
            };
        }

        DataRow[] rows;

        if (dataSource.Tables.Contains(source))
        {
            rows = string.IsNullOrEmpty(filter) ? 
                dataSource.Tables[source]!.AsEnumerable().ToArray() : 
                dataSource.Tables[source]!.Select(filter);
        }
        else
        {
            using AdoDataConnection connection = new(Settings.Instance);
            rows = string.IsNullOrEmpty(filter) ?
                connection.RetrieveData($"SELECT * FROM {source}").AsEnumerable().ToArray() :
                connection.RetrieveData($"SELECT * FROM {source} WHERE {filter}").AsEnumerable().ToArray();
        }

        JArray keyCoordinates = [];

        foreach (DataRow row in rows)
        {
            keyCoordinates.Add(JObject.FromObject(new
            {
                key = row["PointTag"].ToNonNullString("UNDEFINED"),
                lat = double.Parse(row["Latitude"].ToNonNullString("0.0")),
                lon = double.Parse(row["Longitude"].ToNonNullString("0.0")),
                name = row["Description"].ToNonNullString()
            }));
        }

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            RequestMessage = request,
            Content = new StringContent(keyCoordinates.ToString(), Encoding.UTF8, "application/json")
        };
    }

    private static HttpResponseMessage HandleGrafanaAvatarRequest(HttpRequestMessage request)
    {
        // Statically cache avatar image content and media type
        if (s_avatarResourceContent is null)
        {
            string webRoot = FilePath.AddPathSuffix(WebHosting.Default?.WebRoot ?? FilePath.GetAbsolutePath("wwwroot"));
            string imagePath = $"{webRoot}{s_avatarResource}";

            if (!System.IO.File.Exists(imagePath))
                return new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request };

            string fileExtension = FilePath.GetExtension(imagePath);
            fileExtension = string.IsNullOrWhiteSpace(fileExtension) ? "png" : fileExtension[1..].ToLowerInvariant();

            // Read the file into a byte array
            s_avatarResourceContent = System.IO.File.ReadAllBytes(imagePath);
            s_avatarResourceMediaType = new MediaTypeHeaderValue($"image/{fileExtension}");
        }

        // Return new response with image content
        HttpResponseMessage result = new(HttpStatusCode.OK)
        {
            RequestMessage = request,
            Content = new ByteArrayContent(s_avatarResourceContent)
            {
                Headers = { ContentType = s_avatarResourceMediaType }
            }
        };

        return result;
    }

    private static string GetExceptionMessage(Exception ex)
    {
        if (ex is AggregateException aggEx)
            return string.Join(", ", aggEx.InnerExceptions.Select(iex => iex.Message));

        return ex.Message;
    }

    #endregion
}