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
                    {
                        ingredients.Add(reader["Name"].ToString());
                    }

                    IngredientsComboBox.ItemsSource = ingredients;
                }
            }
        }

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
                    {
                        einheiten.Add(reader["Name"].ToString());
                    }

                    EinheitComboBox.ItemsSource = einheiten;
                }
            }
        }

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
                    LEFT JOIN Einheit en ON e.EinheitID = en.EinheitID
                    ORDER BY z.Name";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    List<EinkaufsEintragViewModel> liste =
                        new List<EinkaufsEintragViewModel>();

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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string zutat = IngredientsComboBox.Text.Trim();
            string mengeText = MengeTextBox.Text.Trim();
            string einheit = EinheitComboBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(zutat))
            {
                MessageBox.Show("Bitte eine Zutat eingeben.");
                return;
            }

            if (!double.TryParse(mengeText, out double menge))
            {
                MessageBox.Show("Bitte eine gültige Menge eingeben.");
                return;
            }

            AddEinkaufsEintrag(zutat, menge, einheit);

            IngredientsComboBox.Text = "";
            MengeTextBox.Clear();
            EinheitComboBox.Text = "";

            LoadIngredientsFromDatabase();
            LoadEinheitenFromDatabase();
            LoadEinkaufsEintraege();
        }

        private void AddEinkaufsEintrag(string zutatName, double menge, string einheitName)
        {
            string connectionString = $"Data Source={App.DatabasePath};Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                int zutatId = GetOrCreateZutat(connection, zutatName);
                int einheitId = GetOrCreateEinheit(connection, einheitName);

                string query = @"
                    INSERT INTO EinkaufsListeEintrag
                    (EinkaufsListeID, ZutatID, Menge, EinheitID)
                    VALUES
                    (1, @ZutatID, @Menge, @EinheitID)";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ZutatID", zutatId);
                    command.Parameters.AddWithValue("@Menge", menge);
                    command.Parameters.AddWithValue("@EinheitID", einheitId);

                    command.ExecuteNonQuery();
                }
            }
        }

        private int GetOrCreateZutat(SQLiteConnection connection, string name)
        {
            string selectQuery =
                "SELECT ZutatID FROM Zutat WHERE Name = @Name";

            using (SQLiteCommand command =
                   new SQLiteCommand(selectQuery, connection))
            {
                command.Parameters.AddWithValue("@Name", name);

                object result = command.ExecuteScalar();

                if (result != null)
                    return Convert.ToInt32(result);
            }

            string insertQuery =
                "INSERT INTO Zutat (Name) VALUES (@Name)";

            using (SQLiteCommand command =
                   new SQLiteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.ExecuteNonQuery();
            }

            return (int)connection.LastInsertRowId;
        }

        private int GetOrCreateEinheit(SQLiteConnection connection, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return -1;

            string selectQuery =
                "SELECT EinheitID FROM Einheit WHERE Name = @Name";

            using (SQLiteCommand command =
                   new SQLiteCommand(selectQuery, connection))
            {
                command.Parameters.AddWithValue("@Name", name);

                object result = command.ExecuteScalar();

                if (result != null)
                    return Convert.ToInt32(result);
            }

            string insertQuery =
                "INSERT INTO Einheit (Name) VALUES (@Name)";

            using (SQLiteCommand command =
                   new SQLiteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.ExecuteNonQuery();
            }

            return (int)connection.LastInsertRowId;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button == null)
                return;

            int id = Convert.ToInt32(button.Tag);

            DeleteEinkaufsEintrag(id);

            LoadEinkaufsEintraege();
        }

        private void DeleteEinkaufsEintrag(int id)
        {
            string connectionString = $"Data Source={App.DatabasePath};Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query =
                    "DELETE FROM EinkaufsListeEintrag WHERE EintragID = @ID";

                using (SQLiteCommand command =
                       new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    command.ExecuteNonQuery();
                }
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