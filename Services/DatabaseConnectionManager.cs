using Oracle.ManagedDataAccess.Client;
using System.Windows;
/// -?DatabaseConnectionManager: 
/// Kapselt den direkten, technischen Datenbankzugriff (Verbindung, Fehlerbehandlung, Ausführung von SQL-Befehlen). -

namespace XRechnungLPS.Services
{
    /// <summary>
    /// Hilfsklasse
    /// enthält Methoden für Datenbank-Operationen
    /// </summary>
    public class DatabaseConnectionManager
    {
        private static bool _connectionFailed = false;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gibt an, ob die Verbindung zur Datenbank fehlgeschlagen ist.
        /// </summary>
        /// <value>
        /// <c>true</c>, wenn die Verbindung fehlgeschlagen ist; andernfalls <c>false</c>.
        /// </value>
        public static bool ConnectionFailed
        {
            get
            {
                lock (_lock)
                {
                    return _connectionFailed;
                }
            }
            private set
            {
                lock (_lock)
                {
                    _connectionFailed = value;
                }
            }
        }

        /// <summary>
        /// Führt eine SQL-Abfrage gegen eine Oracle-Datenbank aus und verarbeitet das Ergebnis mithilfe einer benutzerdefinierten Aktion.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="query">Abfragestring</param>
        /// <param name="processResult">Eine Aktion, die aufgerufen wird, um das Ergebnis mit einem <see cref="OracleDataReader"/> zu verarbeiten.</param>
        /// <exception cref = "OracleException" > Wird ausgelöst, wenn bei der Abfrage ein Fehler auftritt.</exception>
        public static void ExecuteQuery(string connectionString, string query, Action<OracleDataReader> processResult)
        {
            if (ConnectionFailed)
            {
                Console.WriteLine("Die Datenbankverbindung ist bereits fehlgeschlagen. Abbruch weiterer Verbindungsversuche.");
                return;
            }

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    OracleCommand command = new OracleCommand(query, connection);
                    connection.Open();  // Verbindung zur Datenbank öffnen

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        // Übergabe des OracleDataReaders an die Methode, die das Ergebnis verarbeitet
                        processResult(reader);
                    }

                    Console.WriteLine("Abfrage erfolgreich ausgeführt.");
                }
                catch (OracleException ex) when (ex.Number == 12170 || ex.Number == 12535)
                {
                    Console.WriteLine("Timeout beim Versuch, eine Verbindung zur Datenbank herzustellen.");
                    ConnectionFailed = true;  // Verbindungsstatus aktualisieren
                }
                catch (OracleException ex)  // Behandlung anderer Oracle-spezifischer Fehler
                {
                    Console.WriteLine($"Oracle-Fehler: {ex.Message}, Fehlernummer: {ex.Number}");
                    if (ex.Message.Contains("ORA-50000"))
                    {
                        Console.WriteLine("ORA-50000: Connection request timed out.");
                        ConnectionFailed = true;  // Verbindungsstatus aktualisieren
                    }
                    MessageBox.Show("Das Programm kann keine Verbindung zur Datenbank herstellen!", "Mitteilung", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("Ein Fehler ist aufgetreten: " + ex.Message);
                    ConnectionFailed = true;  // Verbindungsstatus aktualisieren bei einem allgemeinen Fehler
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();  // Verbindung sicher schließen
                    }
                }
            }
        }

        /// <summary>
        /// Führt eine SQL-Abfrage ohne Rückgabe gegen eine Oracle-Datenbank aus, z. B. für INSERT, UPDATE oder DELETE-Anweisungen.
        /// Ermöglicht das Festlegen von Parametern für die Abfrage über eine benutzerdefinierte Aktion.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="query">Abfragestring</param>
        /// <param name="parameterizeCommand">Eine Aktion, die einen <see cref="OracleCommand"/> übergeben bekommt,
        /// um Parameter für die Abfrage festzulegen.</param>
        /// <exception cref="OracleException">Wird ausgelöst, wenn bei der Abfrage ein Fehler auftritt.</exception>

        // Methode zur Ausführung von Non-Query-Operationen (wie UPDATE, INSERT, DELETE)
        public static void ExecuteNonQuery(string connectionString, string query, Action<OracleCommand> parameterizeCommand)
        {
            if (ConnectionFailed)
            {
                Console.WriteLine("Die Datenbankverbindung ist bereits fehlgeschlagen. Abbruch weiterer Verbindungsversuche.");
                return;
            }

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        // Parameter zur Verfügung stellen
                        parameterizeCommand(command);

                        // Non-Query ausführen (wie UPDATE)
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"{rowsAffected} Zeilen betroffen.");
                    }
                }
                catch (OracleException ex) when (ex.Number == 12170 || ex.Number == 12535)
                {
                    Console.WriteLine("Timeout beim Versuch, eine Verbindung zur Datenbank herzustellen.");
                    ConnectionFailed = true;  // Verbindungsstatus aktualisieren
                }
                catch (OracleException ex)
                {
                    Console.WriteLine($"Oracle-Fehler: {ex.Message}, Fehlernummer: {ex.Number}");
                    if (ex.Message.Contains("ORA-50000"))
                    {
                        Console.WriteLine("ORA-50000: Connection request timed out.");
                        ConnectionFailed = true;  // Verbindungsstatus aktualisieren
                    }
                }
                catch (System.Exception ex)
                {
                    if (ex != null)
                    {
                        Console.WriteLine($"Ein allgemeiner Fehler ist aufgetreten: {ex.Message}");
                        if (ex.Message.Contains("ORA-50000"))
                        {
                            Console.WriteLine("ORA-50000: Connection request timed out.");
                            ConnectionFailed = true;  // Verbindungsstatus aktualisieren
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ein unbekannter Fehler ist aufgetreten, aber die Ausnahme-Instanz ist null.");
                    }
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();  // Verbindung sicher schließen
                    }
                }
            }
        }

    }


}
