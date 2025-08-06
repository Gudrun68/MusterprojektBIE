using Microsoft.EntityFrameworkCore;
using MusterprojektBie.Model;

namespace MusterprojektBie.Classes.Data
{
    /// <summary>
    /// ApplicationDataContext ist die zentrale Klasse für den Datenbankzugriff
    /// Diese Klasse erbt von DbContext und stellt die Verbindung zur Datenbank her
    /// Hier werden alle DbSets (Tabellen) definiert und die Datenbankverbindung konfiguriert
    /// </summary>
    public class ApplicationDataContext : DbContext
    {
        /// <summary>
        /// DbSet für die Debitor-Entitäten
        /// Repräsentiert die Debitor-Tabelle in der Datenbank
        /// </summary>
        public DbSet<Debitor> Debitoren { get; set; }

        /// <summary>
        /// Konstruktor für den ApplicationDataContext
        /// Nimmt DbContextOptions entgegen, um die Datenbankverbindung zu konfigurieren
        /// </summary>
        /// <param name="options">Konfigurationsoptionen für den DbContext</param>
        public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) : base(options)
        {
        }

        /// <summary>
        /// Standardkonstruktor für Design-Time-Services
        /// Wird hauptsächlich für Migrations und Design-Time-Tools verwendet
        /// </summary>
        public ApplicationDataContext()
        {
        }

        /// <summary>
        /// Konfiguriert die Datenbankverbindung wenn keine Optionen im Konstruktor übergeben wurden
        /// Diese Methode wird aufgerufen, wenn der parameterlose Konstruktor verwendet wird
        /// </summary>
        /// <param name="optionsBuilder">Builder für die Datenbankoptionen</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Fallback-Konfiguration falls keine Optionen im Konstruktor übergeben wurden
            if (!optionsBuilder.IsConfigured)
            {
                // Beispiel für SQL Server LocalDB
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MusterprojektBie;Trusted_Connection=true;");
                
                // Alternative: SQLite für einfache lokale Entwicklung
                // optionsBuilder.UseSqlite("Data Source=musterprojekt.db");
            }

            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// Konfiguriert die Entitäts-Modelle und deren Beziehungen
        /// Hier können Fluent API Konfigurationen vorgenommen werden
        /// </summary>
        /// <param name="modelBuilder">Builder für das Datenmodell</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Beispiel-Konfiguration für die Debitor-Entität
            modelBuilder.Entity<Debitor>(entity =>
            {
                // Primärschlüssel definieren
                entity.HasKey(d => d.Id);

                // Eigenschaften konfigurieren
                entity.Property(d => d.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(d => d.Email)
                    .HasMaxLength(300);

                // Index für bessere Performance bei Suchanfragen
                entity.HasIndex(d => d.Email)
                    .IsUnique();

                // Tabellennamen explizit setzen (optional)
                entity.ToTable("Debitoren");
            });

            // Hier können weitere Entitäts-Konfigurationen hinzugefügt werden
            // modelBuilder.Entity<NächsteEntität>()...
        }

        /// <summary>
        /// Wird vor dem Speichern von Änderungen aufgerufen
        /// Kann für automatische Zeitstempel, Validierungen etc. verwendet werden
        /// </summary>
        /// <returns>Anzahl der betroffenen Datensätze</returns>
        public override int SaveChanges()
        {
            // Beispiel: Automatische Zeitstempel setzen
            UpdateTimestamps();
            
            return base.SaveChanges();
        }

        /// <summary>
        /// Asynchrone Version von SaveChanges
        /// </summary>
        /// <param name="cancellationToken">Token für die Abbruchkontrolle</param>
        /// <returns>Task mit der Anzahl der betroffenen Datensätze</returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Beispiel: Automatische Zeitstempel setzen
            UpdateTimestamps();
            
            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Hilfsmethode zum automatischen Setzen von Zeitstempeln
        /// Setzt CreatedAt und UpdatedAt Felder automatisch
        /// </summary>
        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                // Beispiel für automatische Zeitstempel (falls die Entitäten entsprechende Properties haben)
                if (entry.Entity is IAuditableEntity auditableEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        auditableEntity.CreatedAt = DateTime.UtcNow;
                    }
                    auditableEntity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }

    /// <summary>
    /// Interface für Entitäten mit automatischen Zeitstempeln
    /// Implementieren Sie dieses Interface in Ihren Entitäten für automatische Audit-Felder
    /// </summary>
    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}
