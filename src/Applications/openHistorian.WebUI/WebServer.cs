﻿using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Gemstone.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using ServiceInterface;

namespace openHistorian.WebUI;

public class WebServerConfiguration
{
    public WebHosting Hosting { get; set; } = default!;

    //private class DefaultAuthenticator : IAuthenticator
    //{
    //    public Task<long?> AuthenticateAsync(string username, string password) => throw new NotImplementedException();
    //    public Task<UserData> GetUserDataAsync(long userID) => throw new NotImplementedException();
    //}

    public Func<X509Certificate2?> CertificateSelector { get; set; } = () => null;

    //public IAuthenticator Authenticator { get; set; } = new DefaultAuthenticator();
}

public class WebServer(WebServerConfiguration configuration)
{
    private Func<Task>? m_stopFunc;

    private WebServerConfiguration Configuration { get; } = configuration;

    public static IServiceCommands ServiceCommands { get; internal set; } = default!;

    /// <summary>
    /// Gets logger for the web server.
    /// </summary>
    public static ILogger Logger { get; internal set; } = default!;

    public async Task StartAsync()
    {
        using CancellationTokenSource tokenSource = new();
        TaskCompletionSource<bool> completionSource = new();

        Task stopFunc()
        {
            try
            {
                // ReSharper disable once AccessToDisposedClosure
                tokenSource.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // Ignore object disposed
            }

            return completionSource.Task;
        }

        Func<Task>? memberFunc = Interlocked.CompareExchange(ref m_stopFunc, stopFunc, null);

        if (memberFunc is not null)
            return;

        IHostBuilder hostBuilder = Host.CreateDefaultBuilder();

        string[] urls = Configuration.Hosting.HostURLs.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        string webRoot = Configuration.Hosting.WebRoot;
        hostBuilder.ConfigureWebHostDefaults(webBuilder => ConfigureWebHost(webBuilder, urls, webRoot));

        //.AddJsonOptions(options =>
        //{
        //    options.JsonSerializerOptions.TypeInfoResolver =
        //        new DefaultJsonTypeInfoResolver
        //        {
        //            // same code as above goes here
        //        };
        //});

        IHost host = hostBuilder.Build();
        await host.RunAsync(tokenSource.Token);
        completionSource.SetResult(true);
    }

    public async Task StopAsync()
    {
        Func<Task>? stopFunc = Interlocked.Exchange(ref m_stopFunc, null);
        Task stopTask = stopFunc?.Invoke() ?? Task.CompletedTask;
        await stopTask;
    }

    private void ConfigureWebHost(IWebHostBuilder webBuilder, string[] urls, string webRoot)
    {
        webBuilder.ConfigureKestrel(
            options => options.ConfigureHttpsDefaults(
                op => op.ServerCertificate = Configuration.CertificateSelector()));

#if DEBUG
        webBuilder.UseEnvironment("Development"); // Set the environment to Development
#endif

        webBuilder.UseUrls(urls);
        webBuilder.UseWebRoot(webRoot);
        webBuilder.ConfigureServices((_, serviceCollection) => ConfigureServices(serviceCollection));
        webBuilder.Configure((context, app) => Configure(app, context.HostingEnvironment));
    }

    private void ConfigureServices(IServiceCollection services)
    {
        //HttpAuthenticator httpAuthenticator = new HttpAuthenticator(Configuration.Authenticator);

        //services.AddSingleton(typeof(HttpAuthenticator), httpAuthenticator);
        //services.AddSingleton(typeof(IAuthenticator), Configuration.Authenticator);

        // TODO: Add login page and authentication services
        //services
        //    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        //    .AddCookie(options =>
        //    {
        //        options.LoginPath = "/Login";
        //        options.AccessDeniedPath = "/Forbidden";
        //    });

        const string Administrators = nameof(Administrators);
        const string Editors = nameof(Editors);
        const string Viewers = nameof(Viewers);

        //services.AddAuthorization(options =>
        //{
        //    string administratorRole = Role.Administrator.ToString();
        //    string editorRole = Role.Editor.ToString();
        //    string viewerRole = Role.Viewer.ToString();
        //    options.AddPolicy(Administrators, policy => policy.RequireRole(administratorRole, editorRole, viewerRole));
        //    options.AddPolicy(Editors, policy => policy.RequireRole(editorRole, viewerRole));
        //    options.AddPolicy(Viewers, policy => policy.RequireRole(viewerRole));
        //    options.FallbackPolicy = options.GetPolicy(Administrators);
        //});

        services.AddMvc().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals | JsonNumberHandling.AllowReadingFromString;
            // Add option to only treat properties as required if they have a JsonRequired attribute,
            // this allows 'record' types with 'required' properties to deserialize normally
            options.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers =
                {
                    static typeInfo =>
                    {
                        foreach (JsonPropertyInfo info in typeInfo.Properties)
                        {
                            if (info.IsRequired)
                            {
                                info.IsRequired = info.AttributeProvider?
                                    .IsDefined(typeof(JsonRequiredAttribute), inherit: false)
                                    ?? false;
                            }
                        }
                    }
                }
            };
        });

        services.AddRazorPages(options =>
        {
            //options.Conventions.AuthorizePage("/Index");
            options.Conventions.AllowAnonymousToPage("/Index");
            options.Conventions.AddPageRoute("/Index", "System");
            options.Conventions.AddPageRoute("/Index", "DeviceOverview");
            options.Conventions.AddPageRoute("/Index", "Devices/{id?}");
            options.Conventions.AddPageRoute("/Index", "Phasors/{id?}");
            options.Conventions.AddPageRoute("/Index", "Measurements/{id?}");
            options.Conventions.AddPageRoute("/Index", "AddDeviceWizard");
            options.Conventions.AddPageRoute("/Index", "ActionAdapters/{id?}");
            options.Conventions.AddPageRoute("/Index", "InputAdapters/{id?}");
            options.Conventions.AddPageRoute("/Index", "OutputAdapters/{id?}");
            options.Conventions.AddPageRoute("/Index", "FilterAdapters/{id?}");
            options.Conventions.AddPageRoute("/Index", "Companies/{id?}");
            options.Conventions.AddPageRoute("/Index", "Vendors/{id?}");
            options.Conventions.AddPageRoute("/Index", "VendorDevices/{id?}");
            options.Conventions.AddPageRoute("/Index", "Interconnections/{id?}");
            options.Conventions.AddPageRoute("/Index", "SignalType/{id?}");
            options.Conventions.AddPageRoute("/Index", "Export");
            options.Conventions.AddPageRoute("/Index", "Trend");
            options.Conventions.AddPageRoute("/Index", "Event");
            options.Conventions.AddPageRoute("/Index", "Reports");
            options.Conventions.AddPageRoute("/Index", "Themes/{id?}");
            options.Conventions.AddPageRoute("/Index", "SignalTypes/{id?}");
            options.Conventions.AddPageRoute("/Index", "DeviceStates/{id?}");
            options.Conventions.AddPageRoute("/Index", "Alarms/{id?}");

            //options.Conventions.AuthorizePage("/Devices", Viewers);
            //options.Conventions.AuthorizePage("/Device", Editors);
            //options.Conventions.AuthorizePage("/Users", Administrators);
            //options.Conventions.AuthorizePage("/Groups", Administrators);

            //options.Conventions.AllowAnonymousToPage("/Forbidden");
        });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseAuthentication();

        if (!TryUseStaticFiles(app, env))
        {
            string webRoot = Configuration.Hosting.WebRoot;
            app.UseEmbeddedResources(routes => routes.MapWebRoot(webRoot));
        }

        app.UseEmbeddedResources(routes => routes.MapResourceRoot());

        // Create branch to the export data handler. 
        // All requests ending in .export will follow this branch.
        app.MapWhen(context => context.Request.Path.ToString().EndsWith(".export"),
            appBranch => {
                appBranch.UseExportDataHandler();
            });

        //app.UseHttpsRedirection();

        app.UseWebSockets();
        app.UseRouting();
        

        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
        });
    }

    private static bool TryUseStaticFiles(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
            return false;

        StaticFileOptions options = new();
        string wwwroot = Settings.Default.WebHosting.WebRoot;
        options.FileProvider = new PhysicalFileProvider(wwwroot);
        app.UseStaticFiles(options);

        return true;
    }
}
