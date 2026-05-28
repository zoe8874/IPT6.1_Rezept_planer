using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Wpf
{
    public partial class Rezept : Window
    {
        public Rezept()
        {
            InitializeComponent();
            LoadRezepte();
        }

        private void LoadRezepte()
        {
            try
            {
                string connectionString = $"Data Source={App.DatabasePath};Version=3;";

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT 
                            r.RezeptID,
                            r.Name,
                            rk.Name AS Kategorie
                        FROM Rezept r
                        LEFT JOIN RezeptKategorie rk
                            ON r.KategorieID = rk.KategorieID
                        ORDER BY r.Name";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        List<RezeptViewModel> rezepte = new List<RezeptViewModel>();

                        while (reader.Read())
                        {
                            rezepte.Add(new RezeptViewModel
                            {
                                RezeptID = Convert.ToInt32(reader["RezeptID"]),
                                Name = reader["Name"].ToString(),
                                Kategorie = reader["Kategorie"]?.ToString()
                            });
                        }

                        RezeptListBox.ItemsSource = rezepte;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Rezepte:\n{ex.Message}");
            }
        }

        private void RezeptListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RezeptListBox.SelectedItem is RezeptViewModel rezept)
            {
                LoadRezeptDetails(rezept.RezeptID);
            }
        }

        private void LoadRezeptDetails(int rezeptID)
        {
            ZutatenListBox.Items.Clear();

            try
            {
                string connectionString = $"Data Source={App.DatabasePath};Version=3;";

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Rezeptdaten
                    string rezeptQuery = @"
                        SELECT 
                            r.Name,
                            r.Beschreibung,
                            r.Anleitung,
                            rk.Name AS Kategorie
                        FROM Rezept r
                        LEFT JOIN RezeptKategorie rk
                            ON r.KategorieID = rk.KategorieID
                        WHERE r.RezeptID = @RezeptID";

                    using (SQLiteCommand command = new SQLiteCommand(rezeptQuery, connection))
                    {
                        command.Parameters.AddWithValue("@RezeptID", rezeptID);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                TitelTextBlock.Text = reader["Name"].ToString();
                                BeschreibungTextBlock.Text = reader["Beschreibung"].ToString();
                                AnleitungTextBlock.Text = reader["Anleitung"].ToString();
                                KategorieTextBlock.Text = reader["Kategorie"].ToString();
                            }
                        }
                    }

                    // Zutaten laden
                    string zutatenQuery = @"
                        SELECT 
                            z.Name AS ZutatName,
                            rz.Menge,
                            e.Name AS EinheitName
                        FROM RezeptZutat rz
                        INNER JOIN Zutat z
                            ON rz.ZutatID = z.ZutatID
                        LEFT JOIN Einheit e
                            ON rz.EinheitID = e.EinheitID
                        WHERE rz.RezeptID = @RezeptID";

                    bool allesVorhanden = true;

                    using (SQLiteCommand command = new SQLiteCommand(zutatenQuery, connection))
                    {
                        command.Parameters.AddWithValue("@RezeptID", rezeptID);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string zutat = reader["ZutatName"].ToString();

                                double menge = Convert.ToDouble(reader["Menge"]);

                                string einheit = reader["EinheitName"]?.ToString();

                                bool vorhanden = CheckIfIngredientExists(connection, zutat);

                                ZutatenListBox.Items.Add(
                                    $"{menge} {einheit} {zutat}" +
                                    (vorhanden ? " ✓" : " ✗"));

                                if (!vorhanden)
                                {
                                    allesVorhanden = false;
                                }
                            }
                        }
                    }

                    // Status
                    if (allesVorhanden)
                    {
                        StatusTextBlock.Text = "✓ Alle Zutaten vorhanden";
                        StatusBorder.Background = Brushes.LightGreen;
                    }
                    else
                    {
                        StatusTextBlock.Text = "✗ Nicht alle Zutaten vorhanden";
                        StatusBorder.Background = Brushes.LightCoral;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Rezeptdetails:\n{ex.Message}");
            }
        }

        private bool CheckIfIngredientExists(SQLiteConnection connection, string zutatName)
        {
            string query = @"
                SELECT COUNT(*)
                FROM KuehlschrankEintrag k
                INNER JOIN Zutat z
                    ON k.ZutatID = z.ZutatID
                WHERE z.Name = @Name";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", zutatName);

                long count = (long)command.ExecuteScalar();

                return count > 0;
            }
        }
    }

    public class RezeptViewModel
    {
        public int RezeptID { get; set; }

        public string Name { get; set; }

        public string Kategorie { get; set; }
    }
}