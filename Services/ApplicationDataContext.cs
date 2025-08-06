using Microsoft.EntityFrameworkCore;
using MusterprojektBie.Model;

///ApplicationDataContext: 
/// Kapselt den Zugriff auf die Datenbank über Entity Framework (ORM), also objektorientiert.

namespace MusterprojektBie.Services
{
    /// <summary>
    /// DataContext für den Datenbankzugriff
    /// Stellt die Verbindung zur Datenbank her und definiert die verfügbaren Tabellen
    /// </summary>
    public class ApplicationDataContext : DbContext
    {
        /// <summary>
        /// Tabelle für Debitoren
        /// </summary>
        public DbSet<Debitor> Debitoren { get; set; }

        /// <summary>
        /// Konstruktor mit Konfigurationsoptionen
        /// </summary>
        public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) : base(options)
        {
        }

        /// <summary>
        /// Datenbankverbindung konfigurieren (falls keine Optionen übergeben)
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Beispiel: SQLite-Datenbank für lokale Entwicklung
                optionsBuilder.UseSqlite("Data Source=musterprojekt.db");
            }
        }
    }
}
