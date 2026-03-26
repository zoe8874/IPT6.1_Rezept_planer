using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Wpf
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public static string DatabasePath => System.IO.Path.GetFullPath(@"..\..\..\..\Datenbank\RP.db");
        private const string InitSqlPath = @"..\..\..\..\Datenbank\init.sql";
        private const string InsertSqlPath = @"..\..\..\..\Datenbank\Insert.sql";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                bool needsInitialization = !System.IO.File.Exists(DatabasePath);

                // Erstelle das Verzeichnis, falls es nicht existiert
                string directory = System.IO.Path.GetDirectoryName(DatabasePath);
                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }

                // Erstelle die Datenbankdatei, falls sie nicht existiert
                if (needsInitialization)
                {
                    SQLiteConnection.CreateFile(DatabasePath);
                }

                // Überprüfe, ob die Tabellen existieren
                bool tablesExist = CheckIfTablesExist();

                // Führe das init.sql Skript aus, wenn die Datenbank neu ist oder Tabellen fehlen
                if (needsInitialization || !tablesExist)
                {
                    if (System.IO.File.Exists(InitSqlPath))
                    {
                        string initSql = System.IO.File.ReadAllText(InitSqlPath, Encoding.UTF8);
                        string connectionString = $"Data Source={DatabasePath};Version=3;";

                        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                        {
                            connection.Open();

                            // Entferne Kommentare und teile das SQL-Skript in einzelne Statements auf
                            string cleanedSql = RemoveSqlComments(initSql);
                            string[] sqlStatements = cleanedSql.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string statement in sqlStatements)
                            {
                                string trimmedStatement = statement.Trim();
                                if (!string.IsNullOrWhiteSpace(trimmedStatement))
                                {
                                    using (SQLiteCommand command = new SQLiteCommand(trimmedStatement, connection))
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                }

                // Führe Insert.sql aus, wenn die Datenbank neu ist
                if (needsInitialization)
                {
                    if (System.IO.File.Exists(InsertSqlPath))
                    {
                        string insertSql = System.IO.File.ReadAllText(InsertSqlPath, Encoding.UTF8);
                        string connectionString = $"Data Source={DatabasePath};Version=3;";

                        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                        {
                            connection.Open();

                            // Entferne Kommentare und teile das SQL-Skript in einzelne Statements auf
                            string cleanedSql = RemoveSqlComments(insertSql);
                            string[] sqlStatements = cleanedSql.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string statement in sqlStatements)
                            {
                                string trimmedStatement = statement.Trim();
                                if (!string.IsNullOrWhiteSpace(trimmedStatement))
                                {
                                    using (SQLiteCommand command = new SQLiteCommand(trimmedStatement, connection))
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei der Datenbankinitialisierung:\n{ex.Message}\n\nStackTrace:\n{ex.StackTrace}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string RemoveSqlComments(string sql)
        {
            StringBuilder result = new StringBuilder();
            int i = 0;

            while (i < sql.Length)
            {
                // Prüfe auf Zeilenkommentare (--)
                if (i < sql.Length - 1 && sql[i] == '-' && sql[i + 1] == '-')
                {
                    // Springe bis zum Ende der Zeile
                    while (i < sql.Length && sql[i] != '\n')
                    {
                        i++;
                    }
                    if (i < sql.Length && sql[i] == '\n')
                    {
                        result.Append('\n');
                        i++;
                    }
                }
                else
                {
                    result.Append(sql[i]);
                    i++;
                }
            }

            return result.ToString();
        }

        private bool CheckIfTablesExist()
        {
            try
            {
                string connectionString = $"Data Source={DatabasePath};Version=3;";

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT name FROM sqlite_master WHERE type='table' AND name='Zutat'";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        object result = command.ExecuteScalar();
                        return result != null;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
