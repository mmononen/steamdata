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
using System.Data;
using System.Globalization;

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

        // Cache for categories, genres and tags while inserting data
        private Dictionary<string, int> categoryCache = new Dictionary<string, int>();
        private Dictionary<string, int> genreCache = new Dictionary<string, int>();
        private Dictionary<string, int> tagCache = new Dictionary<string, int>();


        public DatabaseHandler(MainForm form)
        {
            mainForm = form;
            LoadDBConfig();
        }

        private void LoadDBConfig()
        {
            try
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var filePath = Path.Combine(baseDirectory, "config", "db-config.json");

                // Read .json
                var jsonString = File.ReadAllText(filePath);

                // Deserialize .json
                dbConfig = JsonSerializer.Deserialize<DBConfig>(jsonString);

                if (dbConfig == null)
                {
                    throw new Exception("Deserialization returned null. Check the JSON structure.");
                }

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

        public MySqlConnection GetConnection()
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
                //mainForm.Log("Database connection successful.");
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

        // Pull all games at once
        public async Task<List<Game>> PullAllGameDataAsync()
        {
            var games = new List<Game>();

            using (var connection = GetConnection())
            {
                if (connection == null || connection.State != System.Data.ConnectionState.Open)
                {
                    mainForm.Log("Cannot pull data. Database connection is not open.");
                    return games;
                }

                string query = @"
                    SELECT 
                        g.AppID, g.name, g.release_date, g.price, g.metacritic_score, 
                        g.recommendations, g.positive, g.negative, g.estimated_owners, 
                        g.average_playtime_forever, g.peak_ccu, g.pct_pos_total, g.num_reviews_total, 
                        GROUP_CONCAT(DISTINCT c.category_name ORDER BY c.category_name) AS categories,
                        GROUP_CONCAT(DISTINCT ge.genre_name ORDER BY ge.genre_name) AS genres,
                        GROUP_CONCAT(DISTINCT t.tag_name ORDER BY t.tag_name) AS tags
                    FROM 
                        indie_games g
                    LEFT JOIN 
                        game_categories gc ON g.AppID = gc.AppID
                    LEFT JOIN 
                        categories c ON gc.category_id = c.category_id
                    LEFT JOIN 
                        game_genres gg ON g.AppID = gg.AppID
                    LEFT JOIN 
                        genres ge ON gg.genre_id = ge.genre_id
                    LEFT JOIN 
                        game_tags gt ON g.AppID = gt.AppID
                    LEFT JOIN 
                        tags t ON gt.tag_id = t.tag_id
                    GROUP BY 
                        g.AppID
                    ORDER BY 
                        g.AppID;";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var dataTable = new DataTable();
                        dataTable.Load(reader);

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var game = new Game
                            {
                                AppID = row.Field<int>("AppID"),
                                Name = row.Field<string>("name"),
                                ReleaseDate = row.Field<DateTime>("release_date"),
                                Price = row.Field<decimal>("price"),
                                MetacriticScore = row.Field<int>("metacritic_score"),
                                Recommendations = row.Field<int>("recommendations"),
                                Categories = row.Field<string>("categories")?.Split(',').ToList(),
                                Genres = row.Field<string>("genres")?.Split(',').ToList(),
                                Positive = row.Field<int>("positive"),
                                Negative = row.Field<int>("negative"),
                                EstimatedOwners = row.Field<string>("estimated_owners"),
                                AveragePlaytime = row.Field<int>("average_playtime_forever"),
                                PeakCcu = row.Field<int>("peak_ccu"),
                                Tags = row.Field<string>("tags")?.Split(',').ToList(),
                                PctPosTotal = row.Field<decimal>("pct_pos_total"),
                                NumReviews = row.Field<int>("num_reviews_total")
                            };
                            games.Add(game);
                        }
                    }
                }
            }

            mainForm.Log("Data retrieval completed.");
            return games;
        }

        // Update db with induvidual transactions
        public async Task UpdateGameDataAsync(Game game)
        {
            using (var connection = GetConnection())
            {
                if (connection == null || connection.State != ConnectionState.Open)
                {
                    mainForm.Log("Cannot update data. Database connection is not open.");
                    return;
                }

                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        // Construct the query
                        string query = @"
                            INSERT INTO indie_games 
                            (
                                AppID, name, release_date, price, metacritic_score, 
                                recommendations, positive, negative, estimated_owners, 
                                average_playtime_forever, peak_ccu, pct_pos_total, num_reviews_total
                            ) 
                            VALUES 
                            (
                                @AppID, @Name, @ReleaseDate, @Price, @MetacriticScore, 
                                @Recommendations, @Positive, @Negative, @EstimatedOwners, 
                                @AvgPlaytime, @PeakCcu, @PctPosTotal, @NumReviews
                            ) 
                            ON DUPLICATE KEY UPDATE
                                name = VALUES(name),
                                release_date = VALUES(release_date),
                                price = VALUES(price),
                                metacritic_score = VALUES(metacritic_score),
                                recommendations = VALUES(recommendations),
                                positive = VALUES(positive),
                                negative = VALUES(negative),
                                estimated_owners = VALUES(estimated_owners),
                                average_playtime_forever = VALUES(average_playtime_forever),
                                peak_ccu = VALUES(peak_ccu),
                                pct_pos_total = VALUES(pct_pos_total),
                                num_reviews_total = VALUES(num_reviews_total);";

                        // Create a debug string to see the final SQL query with parameters
                        string debugQuery = $@"
                            INSERT INTO indie_games 
                            (
                                AppID, name, release_date, price, metacritic_score, 
                                recommendations, positive, negative, estimated_owners, 
                                average_playtime_forever, peak_ccu, pct_pos_total, num_reviews_total
                            ) 
                            VALUES 
                            (
                                {game.AppID}, '{game.Name}', '{game.ReleaseDate:yyyy-MM-dd}', {game.Price.ToString(CultureInfo.InvariantCulture)}, {game.MetacriticScore}, 
                                {game.Recommendations}, {game.Positive}, {game.Negative}, '{game.EstimatedOwners}', 
                                {game.AveragePlaytime}, {game.PeakCcu}, {game.PctPosTotal.ToString(CultureInfo.InvariantCulture)}, {game.NumReviews}
                            ) 
                            ON DUPLICATE KEY UPDATE
                                name = '{game.Name}',
                                release_date = '{game.ReleaseDate:yyyy-MM-dd}',
                                price = {game.Price.ToString(CultureInfo.InvariantCulture)},
                                metacritic_score = {game.MetacriticScore},
                                recommendations = {game.Recommendations},
                                positive = {game.Positive},
                                negative = {game.Negative},
                                estimated_owners = '{game.EstimatedOwners}',
                                average_playtime_forever = {game.AveragePlaytime},
                                peak_ccu = {game.PeakCcu},
                                pct_pos_total = {game.PctPosTotal.ToString(CultureInfo.InvariantCulture)},
                                num_reviews_total = {game.NumReviews};";

                        // Log the SQL query
                        mainForm.Log($"Executing SQL: {debugQuery}");

                        using (var cmd = new MySqlCommand(query, connection, transaction))
                        {
                            cmd.CommandTimeout = 60;

                            // Add parameters
                            cmd.Parameters.AddWithValue("@AppID", game.AppID);
                            cmd.Parameters.AddWithValue("@Name", game.Name);
                            cmd.Parameters.AddWithValue("@ReleaseDate", game.ReleaseDate);
                            cmd.Parameters.AddWithValue("@Price", game.Price.ToString(CultureInfo.InvariantCulture));
                            cmd.Parameters.AddWithValue("@MetacriticScore", game.MetacriticScore);
                            cmd.Parameters.AddWithValue("@Recommendations", game.Recommendations);
                            cmd.Parameters.AddWithValue("@Positive", game.Positive);
                            cmd.Parameters.AddWithValue("@Negative", game.Negative);
                            cmd.Parameters.AddWithValue("@EstimatedOwners", game.EstimatedOwners);
                            cmd.Parameters.AddWithValue("@AvgPlaytime", game.AveragePlaytime);
                            cmd.Parameters.AddWithValue("@PeakCcu", game.PeakCcu);
                            cmd.Parameters.AddWithValue("@PctPosTotal", game.PctPosTotal.ToString(CultureInfo.InvariantCulture));
                            cmd.Parameters.AddWithValue("@NumReviews", game.NumReviews);

                            // Execute the query
                            await cmd.ExecuteNonQueryAsync();
                        }

                        // Insert related data
                        await InsertCategoriesAsync(game.Categories, game.AppID, connection, transaction);
                        await InsertGenresAsync(game.Genres, game.AppID, connection, transaction);
                        await InsertTagsAsync(game.Tags, game.AppID, connection, transaction);

                        // Commit the transaction
                        await transaction.CommitAsync();

                        mainForm.Log($"Game {game.AppID} - {game.Name} updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction
                        await transaction.RollbackAsync();
                        mainForm.Log($"Error updating data: {ex.ToString()}");
                    }
                }
            }
        }

        // Update db with bulk batches
        public async Task InsertGamesInBatchAsync(List<Game> games)
        {
            using (var connection = GetConnection())
            {
                if (connection == null || connection.State != ConnectionState.Open)
                {
                    mainForm.Log("Cannot insert data. Database connection is not open.");
                    return;
                }

                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        // Building a bulk insert query
                        var insertGameValues = new StringBuilder();
                        foreach (var game in games)
                        {
                            // Escape apostrophes
                            string name = game.Name.Replace("'", "''");

                            insertGameValues.Append($@"(
                            {game.AppID}, 
                            '{name}', 
                            '{game.ReleaseDate:yyyy-MM-dd}', 
                            {game.Price.ToString(CultureInfo.InvariantCulture)}, 
                            {game.MetacriticScore}, 
                            {game.Recommendations}, 
                            {game.Positive}, 
                            {game.Negative}, 
                            '{game.EstimatedOwners}', 
                            {game.AveragePlaytime}, 
                            {game.PeakCcu}, 
                            {game.PctPosTotal.ToString(CultureInfo.InvariantCulture)}, 
                            {game.NumReviews}
                        ),");
                        }

                        // Remove the last comma
                        if (insertGameValues.Length > 0)
                            insertGameValues.Length--;

                        string indieGameInsertQuery = $@"
                        INSERT INTO indie_games (
                            AppID, name, release_date, price, metacritic_score, 
                            recommendations, positive, negative, estimated_owners, 
                            average_playtime_forever, peak_ccu, pct_pos_total, num_reviews_total
                        ) VALUES 
                        {insertGameValues.ToString()}
                        ON DUPLICATE KEY UPDATE
                            name = VALUES(name),
                            release_date = VALUES(release_date),
                            price = VALUES(price),
                            metacritic_score = VALUES(metacritic_score),
                            recommendations = VALUES(recommendations),
                            positive = VALUES(positive),
                            negative = VALUES(negative),
                            estimated_owners = VALUES(estimated_owners),
                            average_playtime_forever = VALUES(average_playtime_forever),
                            peak_ccu = VALUES(peak_ccu),
                            pct_pos_total = VALUES(pct_pos_total),
                            num_reviews_total = VALUES(num_reviews_total);";

                        using (var cmd = new MySqlCommand(indieGameInsertQuery, connection, transaction))
                        {
                            await cmd.ExecuteNonQueryAsync();
                        }

                        await InsertCategoriesBatchAsync(games, connection, transaction);
                        await InsertGenresBatchAsync(games, connection, transaction);
                        await InsertTagsBatchAsync(games, connection, transaction);

                        await transaction.CommitAsync();
                        mainForm.Log("Batch insert successful.");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        mainForm.Log($"Error inserting batch: {ex.Message}");
                    }
                }
            }
        }


        // Insert categories, tags and genres
        private async Task InsertCategoriesBatchAsync(List<Game> games, MySqlConnection connection, MySqlTransaction transaction)
        {
            var categoryValues = new StringBuilder();
            foreach (var game in games)
            {
                foreach (var category in game.Categories)
                {
                    int categoryId = await GetOrInsertCategoryAsync(category, connection, transaction);
                    categoryValues.Append($"({game.AppID}, {categoryId}),");
                }
            }

            if (categoryValues.Length > 0)
                categoryValues.Length--;

            string gameCategoryInsertQuery = $@"
                INSERT INTO game_categories (AppID, category_id) VALUES 
                {categoryValues.ToString()}
                ON DUPLICATE KEY UPDATE category_id = VALUES(category_id);";

            using (var cmd = new MySqlCommand(gameCategoryInsertQuery, connection, transaction))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }
        private async Task InsertTagsBatchAsync(List<Game> games, MySqlConnection connection, MySqlTransaction transaction)
        {
            var tagValues = new StringBuilder();
            foreach (var game in games)
            {
                foreach (var tag in game.Tags)
                {
                    int tagId = await GetOrInsertTagAsync(tag, connection, transaction);
                    tagValues.Append($"({game.AppID}, {tagId}),");
                }
            }

            if (tagValues.Length > 0)
                tagValues.Length--; // Remove the trailing comma

            string gameTagInsertQuery = $@"
                INSERT INTO game_tags (AppID, tag_id) VALUES 
                {tagValues.ToString()}
                ON DUPLICATE KEY UPDATE tag_id = VALUES(tag_id);";

            using (var cmd = new MySqlCommand(gameTagInsertQuery, connection, transaction))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }
        private async Task InsertGenresBatchAsync(List<Game> games, MySqlConnection connection, MySqlTransaction transaction)
        {
            var genreValues = new StringBuilder();
            foreach (var game in games)
            {
                foreach (var genre in game.Genres)
                {
                    int genreId = await GetOrInsertGenreAsync(genre, connection, transaction);
                    genreValues.Append($"({game.AppID}, {genreId}),");
                }
            }

            if (genreValues.Length > 0)
                genreValues.Length--; // Remove the trailing comma

            string gameGenreInsertQuery = $@"
                INSERT INTO game_genres (AppID, genre_id) VALUES 
                {genreValues.ToString()}
                ON DUPLICATE KEY UPDATE genre_id = VALUES(genre_id);";

            using (var cmd = new MySqlCommand(gameGenreInsertQuery, connection, transaction))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task InsertCategoriesAsync(List<string> categories, int appId, MySqlConnection connection, MySqlTransaction transaction)
        {
            foreach (var category in categories)
            {
                int categoryId = await GetOrInsertCategoryAsync(category, connection, transaction);

                string gameCategoryInsertQuery = "INSERT INTO game_categories (AppID, category_id) VALUES (@AppID, @CategoryID)";
                using (var cmd = new MySqlCommand(gameCategoryInsertQuery, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@AppID", appId);
                    cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        private async Task InsertGenresAsync(List<string> genres, int appId, MySqlConnection connection, MySqlTransaction transaction)
        {
            foreach (var genre in genres)
            {
                int genreId = await GetOrInsertGenreAsync(genre, connection, transaction);

                string gameGenreInsertQuery = "INSERT INTO game_genres (AppID, genre_id) VALUES (@AppID, @GenreID)";
                using (var cmd = new MySqlCommand(gameGenreInsertQuery, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@AppID", appId);
                    cmd.Parameters.AddWithValue("@GenreID", genreId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        private async Task InsertTagsAsync(List<string> tags, int appId, MySqlConnection connection, MySqlTransaction transaction)
        {
            foreach (var tag in tags)
            {
                int tagId = await GetOrInsertTagAsync(tag, connection, transaction);

                string gameTagInsertQuery = "INSERT INTO game_tags (AppID, tag_id) VALUES (@AppID, @TagID)";
                using (var cmd = new MySqlCommand(gameTagInsertQuery, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@AppID", appId);
                    cmd.Parameters.AddWithValue("@TagID", tagId);  // Ensure @TagID is properly set
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // Get or insert categories, tags and genres
        private async Task<int> GetOrInsertCategoryAsync(string category, MySqlConnection connection, MySqlTransaction transaction)
        {
            if (categoryCache.TryGetValue(category, out int categoryId))
            {
                return categoryId;
            }

            string selectQuery = "SELECT category_id FROM categories WHERE category_name = @CategoryName";
            using (var cmd = new MySqlCommand(selectQuery, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@CategoryName", category);
                var result = await cmd.ExecuteScalarAsync();

                if (result != null)
                {
                    categoryId = Convert.ToInt32(result);
                }
                else
                {
                    string insertQuery = "INSERT INTO categories (category_name) VALUES (@CategoryName)";
                    using (var insertCmd = new MySqlCommand(insertQuery, connection, transaction))
                    {
                        insertCmd.Parameters.AddWithValue("@CategoryName", category);
                        await insertCmd.ExecuteNonQueryAsync();
                        categoryId = (int)insertCmd.LastInsertedId;
                    }
                }
            }

            // Cache the result
            categoryCache[category] = categoryId;
            return categoryId;
        }
        private async Task<int> GetOrInsertGenreAsync(string genre, MySqlConnection connection, MySqlTransaction transaction)
        {
            if (genreCache.TryGetValue(genre, out int genreId))
            {
                return genreId;
            }

            string selectQuery = "SELECT genre_id FROM genres WHERE genre_name = @GenreName";
            using (var cmd = new MySqlCommand(selectQuery, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@GenreName", genre);
                var result = await cmd.ExecuteScalarAsync();

                if (result != null)
                {
                    genreId = Convert.ToInt32(result);
                }
                else
                {
                    string insertQuery = "INSERT INTO genres (genre_name) VALUES (@GenreName)";
                    using (var insertCmd = new MySqlCommand(insertQuery, connection, transaction))
                    {
                        insertCmd.Parameters.AddWithValue("@GenreName", genre);
                        await insertCmd.ExecuteNonQueryAsync();
                        genreId = (int)insertCmd.LastInsertedId;
                    }
                }
            }

            genreCache[genre] = genreId;
            return genreId;
        }
        private async Task<int> GetOrInsertTagAsync(string tag, MySqlConnection connection, MySqlTransaction transaction)
        {
            if (tagCache.TryGetValue(tag, out int tagId))
            {
                return tagId;
            }

            string selectQuery = "SELECT tag_id FROM tags WHERE tag_name = @TagName";
            using (var cmd = new MySqlCommand(selectQuery, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@TagName", tag);
                var result = await cmd.ExecuteScalarAsync();

                if (result != null)
                {
                    tagId = Convert.ToInt32(result);
                }
                else
                {
                    string insertQuery = "INSERT INTO tags (tag_name) VALUES (@TagName)";
                    using (var insertCmd = new MySqlCommand(insertQuery, connection, transaction))
                    {
                        insertCmd.Parameters.AddWithValue("@TagName", tag);
                        await insertCmd.ExecuteNonQueryAsync();
                        tagId = (int)insertCmd.LastInsertedId;
                    }
                }
            }

            tagCache[tag] = tagId;
            return tagId;
        }


        // Purge all db data
        public async Task PurgeDatabaseAsync()
        {
            string query = @"
                SET FOREIGN_KEY_CHECKS = 0;
                TRUNCATE TABLE game_tags;
                TRUNCATE TABLE game_genres;
                TRUNCATE TABLE game_categories;
                TRUNCATE TABLE indie_games;
                TRUNCATE TABLE tags;
                TRUNCATE TABLE genres;
                TRUNCATE TABLE categories;
                SET FOREIGN_KEY_CHECKS = 1;";

            try
            {
                using (var connection = GetConnection())
                {
                    if (connection == null || connection.State != ConnectionState.Open)
                    {
                        mainForm.Log("Cannot purge data. Database connection is not open.");
                        return;
                    }

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                mainForm.Log("Database purge completed successfully.");
            }
            catch (Exception ex)
            {
                mainForm.Log($"Error during database purge: {ex.Message}");
                throw; // Rethrow the exception to handle it in the calling method if necessary
            }
        }

    }
}

