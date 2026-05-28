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
                            z.ZutatID,
                            z.Name AS ZutatName,
                            rz.Menge,
                            rz.EinheitID,
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
                                int zutatID = Convert.ToInt32(reader["ZutatID"]);
                                string zutat = reader["ZutatName"].ToString();
                                double benoetigteMenge = Convert.ToDouble(reader["Menge"]);
                                int? einheitID = reader["EinheitID"] != DBNull.Value 
                                    ? Convert.ToInt32(reader["EinheitID"]) 
                                    : (int?)null;
                                string einheit = reader["EinheitName"]?.ToString();

                                // Prüfe, ob genug von der Zutat vorhanden ist (mit Einheitenkonvertierung)
                                bool ausreichendVorhanden = CheckIfSufficientIngredient(
                                    connection, 
                                    zutatID, 
                                    einheitID, 
                                    benoetigteMenge, 
                                    out double vorhandeneMenge,
                                    out string vorhandeneEinheit);

                                string statusIcon = ausreichendVorhanden ? " ✓" : " ✗";
                                string mengenInfo = $" (vorhanden: {vorhandeneMenge:F1} {vorhandeneEinheit})";

                                ZutatenListBox.Items.Add(
                                    $"{benoetigteMenge} {einheit} {zutat}{statusIcon}{mengenInfo}");

                                if (!ausreichendVorhanden)
                                {
                                    allesVorhanden = false;
                                }
                            }
                        }
                    }

                    // Status
                    if (allesVorhanden)
                    {
                        StatusTextBlock.Text = "✓ Alle Zutaten in ausreichender Menge vorhanden";
                        StatusBorder.Background = Brushes.LightGreen;
                    }
                    else
                    {
                        StatusTextBlock.Text = "✗ Nicht alle Zutaten in ausreichender Menge vorhanden";
                        StatusBorder.Background = Brushes.LightCoral;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Rezeptdetails:\n{ex.Message}");
            }
        }

        /// <summary>
        /// Prüft, ob eine Zutat in ausreichender Menge vorhanden ist.
        /// Berücksichtigt Einheitenkonvertierung (z.B. kg zu g).
        /// </summary>
        private bool CheckIfSufficientIngredient(
            SQLiteConnection connection, 
            int zutatID, 
            int? benoetigteEinheitID, 
            double benoetigteMenge,
            out double vorhandeneMenge,
            out string vorhandeneEinheit)
        {
            vorhandeneMenge = 0;
            vorhandeneEinheit = "";

            try
            {
                // Hole alle Kühlschrankeinträge für diese Zutat
                string query = @"
                    SELECT k.Menge, k.EinheitID, e.Name AS EinheitName
                    FROM KuehlschrankEintrag k
                    LEFT JOIN Einheit e ON k.EinheitID = e.EinheitID
                    WHERE k.ZutatID = @ZutatID";

                List<(double menge, int? einheitID, string einheitName)> eintraege = new List<(double, int?, string)>();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ZutatID", zutatID);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            double menge = Convert.ToDouble(reader["Menge"]);
                            int? einheitID = reader["EinheitID"] != DBNull.Value 
                                ? Convert.ToInt32(reader["EinheitID"]) 
                                : (int?)null;
                            string einheitName = reader["EinheitName"]?.ToString() ?? "";

                            eintraege.Add((menge, einheitID, einheitName));
                        }
                    }
                }

                if (eintraege.Count == 0)
                {
                    vorhandeneEinheit = GetEinheitName(connection, benoetigteEinheitID);
                    return false;
                }

                // Konvertiere alle Einträge zur benötigten Einheit und summiere
                double gesamtMengeInBenoetigterEinheit = 0;

                foreach (var eintrag in eintraege)
                {
                    double konvertierteMenge = KonvertiereEinheit(
                        connection, 
                        eintrag.menge, 
                        eintrag.einheitID, 
                        benoetigteEinheitID);

                    gesamtMengeInBenoetigterEinheit += konvertierteMenge;
                }

                vorhandeneMenge = gesamtMengeInBenoetigterEinheit;
                vorhandeneEinheit = GetEinheitName(connection, benoetigteEinheitID);

                return gesamtMengeInBenoetigterEinheit >= benoetigteMenge;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Prüfen der Zutatenmenge:\n{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Konvertiert eine Menge von einer Einheit in eine andere.
        /// Verwendet Umrechnungsfaktoren aus der Datenbank.
        /// </summary>
        private double KonvertiereEinheit(
            SQLiteConnection connection, 
            double menge, 
            int? vonEinheitID, 
            int? zuEinheitID)
        {
            // Wenn beide Einheiten gleich oder null sind, keine Konvertierung nötig
            if (vonEinheitID == zuEinheitID)
            {
                return menge;
            }

            // Wenn eine der Einheiten null ist, kann nicht konvertiert werden
            if (!vonEinheitID.HasValue || !zuEinheitID.HasValue)
            {
                return 0;
            }

            try
            {
                // Hole Einheitsinformationen
                string query = @"
                    SELECT EinheitID, BasisEinheitID, Umrechnungsfaktor
                    FROM Einheit
                    WHERE EinheitID = @EinheitID";

                // Von-Einheit
                int? vonBasisID = null;
                double vonFaktor = 1.0;

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EinheitID", vonEinheitID.Value);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            vonBasisID = reader["BasisEinheitID"] != DBNull.Value 
                                ? Convert.ToInt32(reader["BasisEinheitID"]) 
                                : vonEinheitID;
                            vonFaktor = Convert.ToDouble(reader["UmrechnungsFaktor"]);
                        }
                    }
                }

                // Zu-Einheit
                int? zuBasisID = null;
                double zuFaktor = 1.0;

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EinheitID", zuEinheitID.Value);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            zuBasisID = reader["BasisEinheitID"] != DBNull.Value 
                                ? Convert.ToInt32(reader["BasisEinheitID"]) 
                                : zuEinheitID;
                            zuFaktor = Convert.ToDouble(reader["UmrechnungsFaktor"]);
                        }
                    }
                }

                // Prüfe, ob beide Einheiten dieselbe Basiseinheit haben
                if (vonBasisID != zuBasisID)
                {
                    // Keine Konvertierung zwischen verschiedenen Einheitensystemen möglich
                    return 0;
                }

                // Konvertiere: Menge -> Basiseinheit -> Zieleinheit
                double mengeInBasis = menge * vonFaktor;
                double mengeInZieleinheit = mengeInBasis / zuFaktor;

                return mengeInZieleinheit;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei der Einheitenkonvertierung:\n{ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Gibt den Namen einer Einheit zurück.
        /// </summary>
        private string GetEinheitName(SQLiteConnection connection, int? einheitID)
        {
            if (!einheitID.HasValue)
            {
                return "";
            }

            try
            {
                string query = "SELECT Name FROM Einheit WHERE EinheitID = @EinheitID";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EinheitID", einheitID.Value);
                    object result = command.ExecuteScalar();
                    return result?.ToString() ?? "";
                }
            }
            catch
            {
                return "";
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