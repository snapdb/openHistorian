using System;
using System.Windows.Forms;
using Gemstone.Configuration;
using Gemstone.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace DataExtractionUtility
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // Define settings for the service. Note that the Gemstone defaults
                // for handling INI and SQLite configuration are defined in a hierarchy
                // where the configuration settings are loaded are in the following
                // priority order, from lowest to highest:
                // - INI file (defaults.ini) - Machine Level, %programdata% folder
                // - INI file (settings.ini) - Machine Level, %programdata% folder
                // - SQLite database (settings.db) - User Level, %appdata% folder (not used by service)
                // - Environment variables - Machine Level
                // - Environment variables - User Level
                // - Command line arguments
                Settings settings = new()
                {
                    INIFile = ConfigurationOperation.Disabled,
                    SQLite = ConfigurationOperation.ReadWrite
                };

                // Define component settings for application
                DiagnosticsLogger.DefineSettings(settings);

                // Bind settings to configuration sources
                settings.Bind(new ConfigurationBuilder().ConfigureGemstoneDefaults(settings));


                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrmMain());
            }
            finally
            {
                ShutdownHandler.InitiateSafeShutdown();
            }

        }
    }
}
