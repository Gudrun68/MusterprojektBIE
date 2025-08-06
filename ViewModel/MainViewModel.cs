using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using XRechnungLPS.Services;

namespace MusterprojektBie.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string connectionString;
        private string _labelText;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string LabelText
        {
            get => _labelText;
            set
            {
                _labelText = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            // Initialen Text setzen
            LabelText = "Hier gibt es nichts zu sehen!";
        }

        public void Betriebsabfrage()
        {
            connectionString = "Data Source=localhost;User Id=myUsername;Password=myPassword;";

            string query = "SELECT * FROM BETRIEB WHERE Bedingung ORDER BY DEBITOR";
            DatabaseConnectionManager.ExecuteQuery(connectionString, query, reader =>
            {
                while (reader.Read())
                {
                    // verarbeite die Daten hier
                }
            });
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
