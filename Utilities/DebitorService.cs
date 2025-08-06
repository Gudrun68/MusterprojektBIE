using System;
using System.Windows;

namespace MusterprojektBie.Utilities
{
    /// <summary>
    /// Beispielklasse für den Classes-Ordner
    /// 
    /// Der Classes-Ordner ist gedacht für:
    /// - UI-Handler (wie ButtonHandler, EventHandler)
    /// - Utility-Klassen (StringHelper, MathHelper)
    /// - Business Logic-Klassen (ValidationHandler, Calculator)
    /// - Custom Controls und Behaviors
    /// - Enums und Konstanten-Klassen
    /// - Factory-Klassen
    /// - Custom Exceptions
    /// 
    /// Diese Klasse zeigt ein typisches Beispiel für einen ButtonHandler
    /// </summary>
    public class ButtonHandler
    {
        #region Private Fields

        /// <summary>
        /// Connection String für die Oracle-Datenbank
        /// In produktiven Anwendungen sollte dieser aus einer Konfigurationsdatei gelesen werden
        /// </summary>
        private readonly string _connectionString;

        #endregion

        #region Constructor

        /// <summary>
        /// Konstruktor der DebitorService-Klasse
        /// </summary>
        /// <param name="connectionString">Verbindungsstring für die Datenbank</param>
        /// <exception cref="ArgumentNullException">Wird ausgelöst, wenn connectionString null oder leer ist</exception>
        public DebitorService(string connectionString)
        {
            // Eingabevalidierung - wichtig für robuste Anwendungen
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), 
                    "Connection String darf nicht null oder leer sein.");
            }

            _connectionString = connectionString;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Lädt alle Debitoren aus der Datenbank
        /// Demonstriert die Verwendung des DatabaseConnectionManagers für SELECT-Operationen
        /// </summary>
        /// <returns>Liste aller Debitoren</returns>
        /// <exception cref="InvalidOperationException">Wird ausgelöst, wenn die Datenbankverbindung fehlgeschlagen ist</exception>
        public List<Debitor> GetAllDebitoren()
        {
            // Prüfung des Verbindungsstatus vor der Ausführung
            if (DatabaseConnectionManager.ConnectionFailed)
            {
                throw new InvalidOperationException(
                    "Datenbankverbindung ist fehlgeschlagen. Bitte versuchen Sie es später erneut.");
            }

            var debitoren = new List<Debitor>();
            
            // SQL-Query definieren - immer mit Parametern arbeiten!
            string query = "SELECT ID, NAME, EMAIL FROM DEBITOREN ORDER BY NAME";

            try
            {
                // Verwendung des DatabaseConnectionManagers
                DatabaseConnectionManager.ExecuteQuery(_connectionString, query, reader =>
                {
                    // Daten aus dem Reader in Objekte umwandeln
                    while (reader.Read())
                    {
                        var debitor = new Debitor
                        {
                            // Sichere Datentyp-Konvertierung mit Null-Checks
                            Id = reader.IsDBNull("ID") ? 0 : reader.GetInt32("ID"),
                            Name = reader.IsDBNull("NAME") ? string.Empty : reader.GetString("NAME"),
                            Email = reader.IsDBNull("EMAIL") ? string.Empty : reader.GetString("EMAIL")
                        };
                        
                        debitoren.Add(debitor);
                    }
                });

                Console.WriteLine($"{debitoren.Count} Debitoren erfolgreich geladen.");
                return debitoren;
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung mit aussagekräftigen Meldungen
                Console.WriteLine($"Fehler beim Laden der Debitoren: {ex.Message}");
                throw new InvalidOperationException("Fehler beim Laden der Debitoren aus der Datenbank.", ex);
            }
        }

        /// <summary>
        /// Sucht einen Debitor anhand seiner ID
        /// Demonstriert parametrisierte Abfragen und Null-Handling
        /// </summary>
        /// <param name="id">Die ID des gesuchten Debitors</param>
        /// <returns>Der gefundene Debitor oder null, wenn nicht vorhanden</returns>
        /// <exception cref="ArgumentException">Wird ausgelöst, wenn die ID ungültig ist</exception>
        public Debitor GetDebitorById(int id)
        {
            // Eingabevalidierung
            if (id <= 0)
            {
                throw new ArgumentException("Die Debitor-ID muss größer als 0 sein.", nameof(id));
            }

            Debitor foundDebitor = null;
            
            // Parametrisierte Abfrage - Schutz vor SQL-Injection
            string query = "SELECT ID, NAME, EMAIL FROM DEBITOREN WHERE ID = :id";

            try
            {
                DatabaseConnectionManager.ExecuteQuery(_connectionString, query, reader =>
                {
                    if (reader.Read())
                    {
                        foundDebitor = new Debitor
                        {
                            Id = reader.GetInt32("ID"),
                            Name = reader.IsDBNull("NAME") ? string.Empty : reader.GetString("NAME"),
                            Email = reader.IsDBNull("EMAIL") ? string.Empty : reader.GetString("EMAIL")
                        };
                    }
                });

                return foundDebitor;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Suchen des Debitors mit ID {id}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Erstellt einen neuen Debitor in der Datenbank
        /// Demonstriert die Verwendung von ExecuteNonQuery für INSERT-Operationen
        /// </summary>
        /// <param name="debitor">Der zu erstellende Debitor</param>
        /// <returns>True, wenn erfolgreich erstellt, sonst False</returns>
        /// <exception cref="ArgumentNullException">Wird ausgelöst, wenn debitor null ist</exception>
        public bool CreateDebitor(Debitor debitor)
        {
            // Null-Check und Validierung
            if (debitor == null)
            {
                throw new ArgumentNullException(nameof(debitor), "Debitor darf nicht null sein.");
            }

            if (string.IsNullOrWhiteSpace(debitor.Name))
            {
                throw new ArgumentException("Debitor-Name darf nicht leer sein.", nameof(debitor));
            }

            try
            {
                string insertQuery = "INSERT INTO DEBITOREN (NAME, EMAIL) VALUES (:name, :email)";

                // Verwendung von ExecuteNonQuery für Datenänderungen
                DatabaseConnectionManager.ExecuteNonQuery(_connectionString, insertQuery, command =>
                {
                    // Parameter hinzufügen - wichtig für Sicherheit!
                    command.Parameters.Add(":name", OracleDbType.Varchar2).Value = debitor.Name;
                    command.Parameters.Add(":email", OracleDbType.Varchar2).Value = debitor.Email ?? string.Empty;
                });

                Console.WriteLine($"Debitor '{debitor.Name}' erfolgreich erstellt.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Erstellen des Debitors: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Demonstriert LINQ-Operationen mit einer Liste von Debitoren
        /// Zeigt verschiedene Filtermethoden und Datenverarbeitung
        /// </summary>
        /// <param name="searchTerm">Suchbegriff für die Filterung</param>
        /// <returns>Gefilterte und sortierte Liste von Debitoren</returns>
        public List<Debitor> SearchAndFilterDebitoren(string searchTerm = "")
        {
            try
            {
                // Alle Debitoren laden
                var allDebitoren = GetAllDebitoren();

                // LINQ-Operationen für Filterung und Sortierung
                var filteredDebitoren = allDebitoren
                    .Where(d => string.IsNullOrEmpty(searchTerm) || 
                               d.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                               (!string.IsNullOrEmpty(d.Email) && d.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                    .OrderBy(d => d.Name)
                    .ToList();

                // Statistiken ausgeben
                Console.WriteLine($"Gefunden: {filteredDebitoren.Count} von {allDebitoren.Count} Debitoren");
                
                return filteredDebitoren;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler bei der Debitor-Suche: {ex.Message}");
                return new List<Debitor>();
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Hilfsmethode zur Validierung von E-Mail-Adressen
        /// Demonstriert einfache Validierungslogik
        /// </summary>
        /// <param name="email">Die zu validierende E-Mail-Adresse</param>
        /// <returns>True, wenn gültig, sonst False</returns>
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Einfache E-Mail-Validierung
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Demonstriert das Singleton-Pattern (statische Instanz)
        /// Hinweis: In echten Anwendungen sollte Dependency Injection verwendet werden
        /// </summary>
        private static DebitorService _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Singleton-Instanz des DebitorService
        /// Demonstration des Singleton-Patterns mit Thread-Safety
        /// </summary>
        public static DebitorService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            // Hier sollte der echte Connection String verwendet werden
                            _instance = new DebitorService("Data Source=localhost:1521/XE;User Id=user;Password=pass;");
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion
    }
}
