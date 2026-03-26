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
                        
                        IngredientsComboBox.ItemsSource = ingredients;

                        // Wenn keine Zutaten vorhanden sind, zeige eine Info
                        if (ingredients.Count == 0)
                        {
                            IngredientsComboBox.IsEditable = true;
                            // Optional: Füge Platzhalter-Text hinzu
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Zutaten:\n{ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Liest den ausgewählten oder eingegebenen Text aus der ComboBox
            string zutat = IngredientsComboBox.Text ?? string.Empty;
            
            if (!string.IsNullOrWhiteSpace(zutat))
            {
                // Hier kannst du die Zutat zum Kühlschrank hinzufügen
                MessageBox.Show($"Zutat '{zutat}' wurde hinzugefügt!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}