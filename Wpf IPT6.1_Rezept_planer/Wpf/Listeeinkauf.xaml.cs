using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace Wpf
{
    public partial class Listeeinkauf : Window
    {
        public Listeeinkauf()
        {
            InitializeComponent();
            LoadIngredientsFromDatabase();
            LoadEinheitenFromDatabase();
            LoadEinkaufsEintraege();
        }

        // Zutaten laden
        private void LoadIngredientsFromDatabase()
        {
            string connectionString = $"Data Source={App.DatabasePath};Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT Name FROM Zutat ORDER BY Name";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    List<string> ingredients = new List<string>();

                    while (reader.Read())
                        ingredients.Add(reader["Name"].ToString());

                    IngredientsComboBox.ItemsSource = ingredients;

                    if (ingredients.Count > 0)
                        IngredientsComboBox.SelectedIndex = 0;
                }
            }
        }

        // Einheiten laden
        private void LoadEinheitenFromDatabase()
        {
            string connectionString = $"Data Source={App.DatabasePath};Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT Name FROM Einheit ORDER BY Name";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    List<string> einheiten = new List<string>();

                    while (reader.Read())
                        einheiten.Add(reader["Name"].ToString());

                    EinheitComboBox.ItemsSource = einheiten;

                    if (einheiten.Count > 0)
                        EinheitComboBox.SelectedIndex = 0;
                }
            }
        }

        // Einkaufsliste laden
        private void LoadEinkaufsEintraege()
        {
            string connectionString = $"Data Source={App.DatabasePath};Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT 
                        e.EintragID,
                        z.Name AS ZutatName,
                        e.Menge,
                        en.Name AS EinheitName
                    FROM EinkaufsListeEintrag e
                    INNER JOIN Zutat z ON e.ZutatID = z.ZutatID
                    LEFT JOIN Einheit en ON e.EinheitID = en.EinheitID";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    var liste = new List<EinkaufsEintragViewModel>();

                    while (reader.Read())
                    {
                        liste.Add(new EinkaufsEintragViewModel
                        {
                            EintragID = Convert.ToInt32(reader["EintragID"]),
                            ZutatName = reader["ZutatName"].ToString(),
                            Menge = Convert.ToDouble(reader["Menge"]),
                            EinheitName = reader["EinheitName"]?.ToString()
                        });
                    }

                    EinkaufListView.ItemsSource = liste;
                }
            }
        }

        // Hinzufügen
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string zutat = IngredientsComboBox.Text;
            string mengeText = MengeTextBox.Text;
            string einheit = EinheitComboBox.Text;

            if (string.IsNullOrWhiteSpace(zutat))
            {
                MessageBox.Show("Zutat fehlt");
                return;
            }

            if (!double.TryParse(mengeText, out double menge))
            {
                MessageBox.Show("Ungültige Menge");
                return;
            }

            AddEinkaufsEintrag(zutat, menge, einheit);

            MengeTextBox.Clear();
            LoadEinkaufsEintraege();
        }


        private void AddEinkaufsEintrag(string zutatName, double menge, string einheitName)
        {
            string connectionString = $"Data Source={App.DatabasePath};Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                int zutatId = GetIdByName(connection, "Zutat", "ZutatID", zutatName);
                int einheitId = GetIdByName(connection, "Einheit", "EinheitID", einheitName);

                string insertQuery = @"
                    INSERT INTO EinkaufsListeEintrag 
                    (EinkaufsListeID, ZutatID, Menge, EinheitID)
                    VALUES (1, @ZutatID, @Menge, @EinheitID)";

                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@ZutatID", zutatId);
                    command.Parameters.AddWithValue("@Menge", menge);
                    command.Parameters.AddWithValue("@EinheitID", einheitId);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn?.Tag != null)
            {
                int id = Convert.ToInt32(btn.Tag);

                DeleteEinkaufsEintrag(id);
                LoadEinkaufsEintraege();
            }
        }

        private void DeleteEinkaufsEintrag(int id)
        {
            string connectionString = $"Data Source={App.DatabasePath};Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "DELETE FROM EinkaufsListeEintrag WHERE EintragID = @ID";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    command.ExecuteNonQuery();
                }
            }
        }

     
        private int GetIdByName(SQLiteConnection connection, string table, string idCol, string name)
        {
            string query = $"SELECT {idCol} FROM {table} WHERE Name = @Name";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", name);

                object result = command.ExecuteScalar();

                if (result != null)
                    return Convert.ToInt32(result);

                return -1;
            }
        }
    }

    public class EinkaufsEintragViewModel
    {
        public int EintragID { get; set; }
        public string ZutatName { get; set; }
        public double Menge { get; set; }
        public string EinheitName { get; set; }
    }
}