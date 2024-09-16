using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace SteamSyncDB
{
    public class DBConfig
    {
        public string? host { get; set; }
        public string? user { get; set; }
        public string? password { get; set; }
        public string? database { get; set; }
        public int port { get; set; }
        public SSLConfig? ssl { get; set; }

    }

    public class SSLConfig
    {
        public string? ssl_mode { get; set; }
    }

    public class DatabaseHandler
    {
        private DBConfig? dbConfig;
        private MainForm mainForm;

        public DatabaseHandler(MainForm form)
        {
            mainForm = form;
            LoadDBConfig();
        }

        private void LoadDBConfig()
        {
            try
            {
                var filePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "db-config.json");
                // Read .json
                var jsonString = File.ReadAllText(filePath);
                // Deserialize .json
                dbConfig = JsonSerializer.Deserialize<DBConfig>(jsonString);
                mainForm.Log("Configuration file read and deserialized.");
            }
            catch (FileNotFoundException)
            {
                mainForm.Log("Configuration file not found.");
            }
            catch (JsonException)
            {
                mainForm.Log("Error parsing the JSON file.");
            }
            catch (Exception ex)
            {
                mainForm.Log($"An unexpected error occurred: {ex.Message}");
            }

        }

        private MySqlConnection GetConnection()
        {
            if (dbConfig == null)
            {
                mainForm.Log("Database configuration is not loaded. Unable to connect.");
                return null;
            }

            var connectionString =
                $"Server={dbConfig.host};" +
                $"Database={dbConfig.database};" +
                $"User ID={dbConfig.user};" +
                $"Password={dbConfig.password};" +
                $"Port={dbConfig.port};" +
                $"SslMode={dbConfig.ssl.ssl_mode};";

            var connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
                mainForm.Log("Database connection successful.");
            }
            catch (Exception ex)
            {
                mainForm.Log($"Failed to connect to the database: {ex.Message}");
            }

            return connection;
        }

        public void TestConnection()
        {
            using (var connection = GetConnection())
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    mainForm.Log("Test connection open.");
                }

                connection.Close();

                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    mainForm.Log("Test connection closed.");
                }
            }
        }

        public void InsertGameData(Game game)
        {
            using (var connection = GetConnection())
            {
                if (connection == null || connection.State != System.Data.ConnectionState.Open)
                {
                    mainForm.Log("Cannot insert data. Database connection is not open.");
                    return;
                }

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert into indie_games
                        string indieGameInsertQuery = @"INSERT INTO indie_games 
                            (
                                AppID, 
                                name, 
                                release_date, 
                                price, 
                                windows, 
                                mac, 
                                linux, 
                                metacritic_score, 
                                recommendations, 
                                positive, 
                                negative, 
                                estimated_owners, 
                                average_playtime_forever, 
                                peak_ccu, 
                                pct_pos_total, 
                                num_reviews_total
                            ) 
                            VALUES 
                            (
                                @AppID, 
                                @Name, 
                                @ReleaseDate, 
                                @Price, 
                                @Windows, 
                                @Mac, 
                                @Linux, 
                                @MetacriticScore, 
                                @Recommendations, 
                                @Positive, 
                                @Negative, 
                                @EstimatedOwners, 
                                @AvgPlaytime, 
                                @PeakCcu, 
                                @PctPosTotal, 
                                @NumReviews
                            );";

                        using (var cmd = new MySqlCommand(indieGameInsertQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@AppID", game.AppID);
                            cmd.Parameters.AddWithValue("@Name", game.Name);
                            cmd.Parameters.AddWithValue("@ReleaseDate", game.ReleaseDate);
                            cmd.Parameters.AddWithValue("@Price", game.Price);
                            cmd.Parameters.AddWithValue("@Windows", game.Windows);
                            cmd.Parameters.AddWithValue("@Mac", game.Mac);
                            cmd.Parameters.AddWithValue("@Linux", game.Linux);
                            cmd.Parameters.AddWithValue("@MetacriticScore", game.MetacriticScore);
                            cmd.Parameters.AddWithValue("@Recommendations", game.Recommendations);
                            cmd.Parameters.AddWithValue("@Positive", game.Positive);
                            cmd.Parameters.AddWithValue("@Negative", game.Negative);
                            cmd.Parameters.AddWithValue("@EstimatedOwners", game.EstimatedOwners);
                            cmd.Parameters.AddWithValue("@AvgPlaytime", game.AveragePlaytime);
                            cmd.Parameters.AddWithValue("@PeakCcu", game.PeakCcu);
                            cmd.Parameters.AddWithValue("@PctPosTotal", game.PctPosTotal);
                            cmd.Parameters.AddWithValue("@NumReviews", game.NumReviews);

                            cmd.ExecuteNonQuery();
                        }

                        // Insert categories, genres, and tags, and link them with game
                        InsertCategories(game.Categories, game.AppID, connection, transaction);
                        InsertGenres(game.Genres, game.AppID, connection, transaction);
                        InsertTags(game.Tags, game.AppID, connection, transaction);

                        transaction.Commit();
                        mainForm.Log("Game data inserted successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        mainForm.Log($"Error inserting data: {ex.Message}");
                    }
                }
            }
        }

        private void InsertCategories(List<string> categories, int appId, MySqlConnection connection, MySqlTransaction transaction)
        {
            foreach (var category in categories)
            {
                int categoryId = GetOrInsertCategory(category, connection, transaction);

                string gameCategoryInsertQuery = "INSERT INTO game_categories (AppID, category_id) VALUES (@AppID, @CategoryID)";
                using (var cmd = new MySqlCommand(gameCategoryInsertQuery, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@AppID", appId);
                    cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void InsertGenres(List<string> genres, int appId, MySqlConnection connection, MySqlTransaction transaction)
        {
            foreach (var genre in genres)
            {
                int genreId = GetOrInsertGenre(genre, connection, transaction);

                string gameGenreInsertQuery = "INSERT INTO game_genres (AppID, genre_id) VALUES (@AppID, @GenreID)";
                using (var cmd = new MySqlCommand(gameGenreInsertQuery, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@AppID", appId);
                    cmd.Parameters.AddWithValue("@GenreID", genreId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void InsertTags(List<string> tags, int appId, MySqlConnection connection, MySqlTransaction transaction)
        {
            foreach (var tag in tags)
            {
                int tagId = GetOrInsertTag(tag, connection, transaction);

                string gameTagInsertQuery = "INSERT INTO game_tags (AppID, tag_id) VALUES (@AppID, @TagID)";
                using (var cmd = new MySqlCommand(gameTagInsertQuery, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@AppID", appId);
                    cmd.Parameters.AddWithValue("@TagID", tagId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private int GetOrInsertCategory(string category, MySqlConnection connection, MySqlTransaction transaction)
        {
            // Check if the category exists, if not insert it and return the id
            string selectQuery = "SELECT category_id FROM categories WHERE category_name = @CategoryName";
            using (var cmd = new MySqlCommand(selectQuery, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@CategoryName", category);
                var result = cmd.ExecuteScalar();

                if (result != null)
                    return Convert.ToInt32(result);
                else
                {
                    string insertQuery = "INSERT INTO categories (category_name) VALUES (@CategoryName)";
                    using (var insertCmd = new MySqlCommand(insertQuery, connection, transaction))
                    {
                        insertCmd.Parameters.AddWithValue("@CategoryName", category);
                        insertCmd.ExecuteNonQuery();
                        return (int)insertCmd.LastInsertedId;
                    }
                }
            }
        }

        private int GetOrInsertGenre(string genre, MySqlConnection connection, MySqlTransaction transaction)
        {
            string selectQuery = "SELECT genre_id FROM genres WHERE genre_name = @GenreName";
            using (var cmd = new MySqlCommand(selectQuery, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@GenreName", genre);
                var result = cmd.ExecuteScalar();

                if (result != null)
                    return Convert.ToInt32(result);
                else
                {
                    string insertQuery = "INSERT INTO genres (genre_name) VALUES (@GenreName)";
                    using (var insertCmd = new MySqlCommand(insertQuery, connection, transaction))
                    {
                        insertCmd.Parameters.AddWithValue("@GenreName", genre);
                        insertCmd.ExecuteNonQuery();
                        return (int)insertCmd.LastInsertedId;
                    }
                }
            }
        }

        private int GetOrInsertTag(string tag, MySqlConnection connection, MySqlTransaction transaction)
        {
            string selectQuery = "SELECT tag_id FROM tags WHERE tag_name = @TagName";
            using (var cmd = new MySqlCommand(selectQuery, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@TagName", tag);
                var result = cmd.ExecuteScalar();

                if (result != null)
                    return Convert.ToInt32(result);
                else
                {
                    string insertQuery = "INSERT INTO tags (tag_name) VALUES (@TagName)";
                    using (var insertCmd = new MySqlCommand(insertQuery, connection, transaction))
                    {
                        insertCmd.Parameters.AddWithValue("@TagName", tag);
                        insertCmd.ExecuteNonQuery();
                        return (int)insertCmd.LastInsertedId;
                    }
                }
            }
        }


    }
}

