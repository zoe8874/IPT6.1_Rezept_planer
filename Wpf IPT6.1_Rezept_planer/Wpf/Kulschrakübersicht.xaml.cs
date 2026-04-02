using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wpf
{
    /// <summary>
    /// Interaktionslogik für Kulschrakübersicht.xaml
    /// </summary>
    public partial class Kulschrakübersicht : Window
    {
        public Kulschrakübersicht()
        {
            InitializeComponent();
            LoadIngredientsFromDatabase();
            LoadEinheitenFromDatabase();
            LoadKuehlschrankEintraege();
        }

        private void LoadIngredientsFromDatabase()
        {
            try
            {
                string connectionString = $"Data Source={App.DatabasePath};Version=3;";
                
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Lade alle Zutaten aus der Zutat-Tabelle
                    string query = "SELECT Name FROM Zutat ORDER BY Name";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        List<string> ingredients = new List<string>();
                        
                        while (reader.Read())
                        {
                            ingredients.Add(reader["Name"].ToString());
                        }
                        
                        // Setze die Zutaten als ItemsSource für die ComboBox
                        IngredientsComboBox.ItemsSource = ingredients;

                        // Wenn Zutaten vorhanden sind, wähle optional die erste aus
                        if (ingredients.Count > 0)
                        {
                            IngredientsComboBox.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Zutaten aus der Datenbank:\n{ex.Message}", 
                    "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadEinheitenFromDatabase()
        {
            try
            {
                string connectionString = $"Data Source={App.DatabasePath};Version=3;";
                
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    
                    // Lade alle Einheiten aus der Einheit-Tabelle
                    string query = "SELECT Name FROM Einheit ORDER BY Name";
                    
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        List<string> einheiten = new List<string>();
                        
                        while (reader.Read())
                        {
                            einheiten.Add(reader["Name"].ToString());
                        }
                        
                        // Setze die Einheiten als ItemsSource für die ComboBox
                        EinheitComboBox.ItemsSource = einheiten;

                        // Wenn Einheiten vorhanden sind, wähle optional die erste aus
                        if (einheiten.Count > 0)
                        {
                            EinheitComboBox.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Einheiten aus der Datenbank:\n{ex.Message}", 
                    "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadKuehlschrankEintraege()
        {
            try
            {
                string connectionString = $"Data Source={App.DatabasePath};Version=3;";
                
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    
                    // Lade alle Einträge mit JOIN für Zutat- und Einheitnamen
                    string query = @"SELECT 
                                        k.EintragID,
                                        z.Name AS ZutatName,
                                        k.Menge,
                                        e.Name AS EinheitName,
                                        k.Ablaufdatum
                                     FROM KuehlschrankEintrag k
                                     INNER JOIN Zutat z ON k.ZutatID = z.ZutatID
                                     INNER JOIN Einheit e ON k.EinheitID = e.EinheitID
                                     ORDER BY k.Ablaufdatum ASC";
                    
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        List<KuehlschrankEintragViewModel> eintraege = new List<KuehlschrankEintragViewModel>();
                        
                        while (reader.Read())
                        {
                            eintraege.Add(new KuehlschrankEintragViewModel
                            {
                                EintragID = Convert.ToInt32(reader["EintragID"]),
                                ZutatName = reader["ZutatName"].ToString(),
                                Menge = Convert.ToDouble(reader["Menge"]),
                                EinheitName = reader["EinheitName"].ToString(),
                                Ablaufdatum = Convert.ToDateTime(reader["Ablaufdatum"])
                            });
                        }
                        
                        // Setze die Einträge als ItemsSource für die ListBox
                        KuehlschrankListBox.ItemsSource = eintraege;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Kühlschrank-Einträge:\n{ex.Message}", 
                    "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Liest den ausgewählten oder eingegebenen Text aus den Feldern
            string zutat = IngredientsComboBox.Text ?? string.Empty;
            string mengeText = MengeTextBox.Text ?? string.Empty;
            string einheit = EinheitComboBox.Text ?? string.Empty;
            DateTime? ablaufdatum = AblaufdatumPicker.SelectedDate;
            
            // Validierung
            if (string.IsNullOrWhiteSpace(zutat))
            {
                MessageBox.Show("Bitte wählen Sie eine Zutat aus.", 
                    "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(mengeText) || !double.TryParse(mengeText, out double menge))
            {
                MessageBox.Show("Bitte geben Sie eine gültige Menge ein.", 
                    "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(einheit))
            {
                MessageBox.Show("Bitte wählen Sie eine Einheit aus.", 
                    "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!ablaufdatum.HasValue)
            {
                MessageBox.Show("Bitte wählen Sie ein Ablaufdatum aus.", 
                    "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Füge den Eintrag zur Datenbank hinzu
            try
            {
                AddKuehlschrankEintrag(zutat, menge, einheit, ablaufdatum.Value);
                
                // Felder zurücksetzen
                MengeTextBox.Clear();
                AblaufdatumPicker.SelectedDate = null;
                
                // Lade die Liste neu
                LoadKuehlschrankEintraege();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Hinzufügen der Zutat:\n{ex.Message}", 
                    "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button?.Tag != null)
            {
                int eintragID = Convert.ToInt32(button.Tag);

                MessageBoxResult result = MessageBox.Show(
                    "Möchten Sie diesen Eintrag wirklich löschen?",
                    "Löschen bestätigen",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        DeleteKuehlschrankEintrag(eintragID);
                        LoadKuehlschrankEintraege();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Fehler beim Löschen des Eintrags:\n{ex.Message}", 
                            "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void DeleteKuehlschrankEintrag(int eintragID)
        {
            string connectionString = $"Data Source={App.DatabasePath};Version=3;";
            
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM KuehlschrankEintrag WHERE EintragID = @EintragID";
                
                using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@EintragID", eintragID);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void AddKuehlschrankEintrag(string zutatName, double menge, string einheitName, DateTime ablaufdatum)
        {
            string connectionString = $"Data Source={App.DatabasePath};Version=3;";
            
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Hole ZutatID
                int zutatId = GetIdByName(connection, "Zutat", "ZutatID", zutatName);
                if (zutatId == -1)
                {
                    throw new Exception($"Zutat '{zutatName}' wurde nicht in der Datenbank gefunden.");
                }

                // Hole EinheitID
                int einheitId = GetIdByName(connection, "Einheit", "EinheitID", einheitName);
                if (einheitId == -1)
                {
                    throw new Exception($"Einheit '{einheitName}' wurde nicht in der Datenbank gefunden.");
                }

                // Füge den Eintrag in KuehlschrankEintrag hinzu
                string insertQuery = @"INSERT INTO KuehlschrankEintrag (ZutatID, Menge, EinheitID, Ablaufdatum) 
                                      VALUES (@ZutatID, @Menge, @EinheitID, @Ablaufdatum)";
                
                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@ZutatID", zutatId);
                    command.Parameters.AddWithValue("@Menge", menge);
                    command.Parameters.AddWithValue("@EinheitID", einheitId);
                    command.Parameters.AddWithValue("@Ablaufdatum", ablaufdatum.ToString("yyyy-MM-dd"));
                    
                    command.ExecuteNonQuery();
                }
            }
        }

        private int GetIdByName(SQLiteConnection connection, string tableName, string idColumnName, string name)
        {
            string query = $"SELECT {idColumnName} FROM {tableName} WHERE Name = @Name";
            
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                
                object result = command.ExecuteScalar();
                
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                
                return -1;
            }
        }
    }

    // ViewModel-Klasse für die Anzeige der Kühlschrank-Einträge
    public class KuehlschrankEintragViewModel
    {
        public int EintragID { get; set; }
        public string ZutatName { get; set; }
        public double Menge { get; set; }
        public string EinheitName { get; set; }
        public DateTime Ablaufdatum { get; set; }
    }
}