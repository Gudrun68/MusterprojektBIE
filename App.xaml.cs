using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MusterprojektBie.Services;

namespace MusterprojektBie
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Beispiel: Zugriff auf appsettings.json und Initialisierung des DbContext
        public static ApplicationDataContext DbContext { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Konfiguration laden (appsettings.json muss im Projektverzeichnis liegen)
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // ConnectionString auslesen
            string connectionString = config.GetConnectionString("OracleDatabase");
            if (string.IsNullOrEmpty(connectionString))
            {
                // Fallback auf SQLite, falls kein Oracle-String vorhanden
                connectionString = "Data Source=musterprojekt.db";
            }

            // DbContext-Optionen erstellen
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDataContext>();
            if (connectionString.Contains("Data Source=musterprojekt.db"))
                optionsBuilder.UseSqlite(connectionString);
            else
                optionsBuilder.UseOracle(connectionString); // Oracle-Provider erforderlich

            // DbContext initialisieren
            DbContext = new ApplicationDataContext(optionsBuilder.Options);
        }
    }

}
