using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto;
using System.Data;
using System.Globalization;
using System.Net.Http.Json;
using System.Text;

namespace SteamSyncDB
{
    public partial class MainForm : Form
    {
        private DataGridView dataGridView;
        private DataGridView dbDataGridView;
        private Button btnImportCSV;
        private Button btnExportCSV;
        private Button btnFetchSteamData;
        private Button btnTestDBConnection;
        private Button btnValidateGameData;
        private Button btnPullDBData;
        private Button btnPushUpdateDBData;
        private Button btnBatchPushDBData;
        private TextBox txtNameFilter;
        //private TextBox txtTagFilter;
        private TextBox txtGenreTagFilter;
        private TextBox consoleView;
        private DatabaseHandler dbHandler;
        private DataTable gameTable;
        private DataView gameView;
        private DataTable dbGameTable;
        private DataView dbGameView;
        private Label rowCountLabel;

        private List<Game> importGames = new List<Game>();
        private List<Game> dbGames = new List<Game>();

        public MainForm()
        {
            InitializeComponent();
            CustomInitialize();
            InitializeImportDataTable();
            InitializeDbDataTable();
            dbHandler = new DatabaseHandler(this);
        }

        private void CustomInitialize()
        {
            this.consoleView = new TextBox();

            MainFormSettings();
            InitializeButtons();
            ImportDataGridView();
            DBDataGridView();
            NameFilterTextBox();
            //TagsTextBox();
            GenreTagTextBox();
            ConsoleView();
            InitializeRowCountLabel();
            //FormControls();
            InitializeLayout();

            Log("Form initialization complete");

            //SampleDataSetup();
            //SampleDataLoad();
        }

        private void InitializeButtons()
        {
            ImportCSVBtn();
            ExportCSVBtn();
            FetchSteamDataBtn();
            TestDBConnectionBtn();
            ValidateGameDataBtn();
            PullDBDataBtn();
            PushDBUpdateDataBtn();
            BatchPushDBDataBtn();
        }

        private void InitializeTable(DataTable table)
        {
            if (table.Columns.Count == 0)
            {
                table.Columns.Add("AppID", typeof(int));
                table.Columns.Add("name", typeof(string));
                table.Columns.Add("release_date", typeof(DateTime));
                table.Columns.Add("price", typeof(decimal));
                table.Columns.Add("metacritic_score", typeof(int));
                table.Columns.Add("recommendations", typeof(int));
                table.Columns.Add("categories", typeof(string));
                table.Columns.Add("genres", typeof(string));
                table.Columns.Add("positive", typeof(int));
                table.Columns.Add("negative", typeof(int));
                table.Columns.Add("estimated_owners", typeof(string));
                table.Columns.Add("average_playtime_forever", typeof(int));
                table.Columns.Add("peak_ccu", typeof(int));
                table.Columns.Add("tags", typeof(string));
                table.Columns.Add("pct_pos_total", typeof(decimal));
                table.Columns.Add("num_reviews_total", typeof(int));
            }
        }

        private void InitializeImportDataTable()
        {
            gameTable = new DataTable();
            gameView = gameTable.DefaultView;
            dataGridView.DataSource = gameView;

            InitializeTable(gameTable);
        }

        private void InitializeDbDataTable()
        {
            dbGameTable = new DataTable();
            dbGameView = dbGameTable.DefaultView;
            dbDataGridView.DataSource = dbGameView;

            InitializeTable(dbGameTable);
        }

        private void MainFormSettings()
        {
            this.Text = "Steam Game Data";
            this.Size = new System.Drawing.Size(1920, 1080);
        }
        //private void FormControls()
        //{
        //    Log("Initializing form controls");
        //    this.Controls.Add(this.btnImportCSV);
        //    this.Controls.Add(this.btnExportCSV);
        //    this.Controls.Add(this.btnFetchSteamData);
        //    this.Controls.Add(this.txtNameFilter);
        //    this.Controls.Add(this.btnTestDBConnection);
        //    this.Controls.Add(this.btnValidateGameData);
        //}
        private void InitializeRowCountLabel()
        {
            rowCountLabel = new Label();
            rowCountLabel.Location = new System.Drawing.Point(20, 850);
            rowCountLabel.Size = new System.Drawing.Size(200, 20);
            this.rowCountLabel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            rowCountLabel.Text = $"Row count: ";
            this.Controls.Add(rowCountLabel);
        }

        // Too Slow
        //private void UpdateRowCount(bool defer = false)
        //{
        //    if (defer)
        //    {
        //        BeginInvoke((MethodInvoker)delegate {
        //            int rowCount = dataGridView.Rows.Cast<DataGridViewRow>().Count(row => row.Visible);
        //            rowCountLabel.Text = $"Row count: {rowCount}";
        //        });
        //    }
        //    else
        //    {
        //        int rowCount = dataGridView.Rows.Cast<DataGridViewRow>().Count(row => row.Visible);
        //        rowCountLabel.Text = $"Row count: {rowCount}";
        //    }
        //}

        private void InitializeLayout()
        {

            var tableLayoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(10, 20, 10, 20),
            };

            var controlsPanel = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 12,
                RowCount = 1,
                BackColor = Color.LightGray,
            };

            var consolePanel = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.LightGray,
            };

            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

            tableLayoutPanel.Controls.Add(controlsPanel, 0, 0);
            tableLayoutPanel.Controls.Add(dataGridView, 0, 1);
            tableLayoutPanel.Controls.Add(dbDataGridView, 0, 2);
            tableLayoutPanel.Controls.Add(consolePanel, 0, 3);

            controlsPanel.Controls.Add(txtNameFilter, 0, 0);
            controlsPanel.Controls.Add(txtGenreTagFilter, 1, 0);
            controlsPanel.Controls.Add(btnImportCSV, 3, 0);
            controlsPanel.Controls.Add(btnExportCSV, 4, 0);
            controlsPanel.Controls.Add(btnFetchSteamData, 5, 0);
            controlsPanel.Controls.Add(btnTestDBConnection, 6, 0);
            controlsPanel.Controls.Add(btnValidateGameData, 7, 0);
            controlsPanel.Controls.Add(btnPullDBData, 8, 0);
            controlsPanel.Controls.Add(btnPushUpdateDBData, 9, 0);
            controlsPanel.Controls.Add(btnBatchPushDBData, 10, 0);

            consolePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            consolePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 90));

            consolePanel.Controls.Add(rowCountLabel, 0, 0);
            consolePanel.Controls.Add(consoleView, 0, 1);

            this.Controls.Add(tableLayoutPanel);
        }

        private void ImportDataGridView()
        {
            Log("Initializing data grid view");
            this.dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false
            };
        }

        private void DBDataGridView()
        {
            Log("Initializing DB data grid view");
            this.dbDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false
            };
        }
        private void ImportCSVBtn()
        {
            Log("Initializing import button");
            this.btnImportCSV = new Button();
            this.btnImportCSV.Text = "Import CSV";
            this.btnImportCSV.Dock = DockStyle.Fill;
            this.btnImportCSV.Click += new EventHandler(this.btnImportCSV_Click);
        }
        private void ExportCSVBtn()
        {
            Log("Initializing export button");
            this.btnExportCSV = new Button();
            this.btnExportCSV.Text = "Export CSV";
            this.btnExportCSV.Dock = DockStyle.Fill;
            this.btnExportCSV.Click += new EventHandler(this.btnExportCSV_Click);
        }
        private void FetchSteamDataBtn()
        {
            Log("Initializing fetch data button");
            this.btnFetchSteamData = new Button();
            this.btnFetchSteamData.Text = "Fetch SteamSpy Data";
            this.btnFetchSteamData.Dock = DockStyle.Fill;
            this.btnFetchSteamData.Click += new EventHandler(this.btnFetchSteamData_Click);
        }

        private void TestDBConnectionBtn()
        {
            Log("Initializing test connection button");
            this.btnTestDBConnection = new Button();
            this.btnTestDBConnection.Text = "Test DB Connection";
            this.btnTestDBConnection.Dock = DockStyle.Fill;
            this.btnTestDBConnection.Click += new EventHandler(this.btnTestDBConnection_Click);

        }

        private void ValidateGameDataBtn()
        {
            Log("Initializing validate game data button");
            this.btnValidateGameData = new Button();
            this.btnValidateGameData.Text = "Validate Game Data";
            this.btnValidateGameData.Dock = DockStyle.Fill;
            this.btnValidateGameData.Click += new EventHandler(this.btnValidateGameData_Click);
        }

        private void PullDBDataBtn()
        {
            Log("Initializing pull database data button");
            this.btnPullDBData = new Button();
            this.btnPullDBData.Text = "Pull DB Data";
            this.btnPullDBData.Dock = DockStyle.Fill;
            this.btnPullDBData.Click += new EventHandler(this.btnPullDBData_Click);
        }
        private void PushDBUpdateDataBtn()
        {
            Log("Initializing push update to database button");
            this.btnPushUpdateDBData = new Button();
            this.btnPushUpdateDBData.Text = "Update DB Data";
            this.btnPushUpdateDBData.Dock = DockStyle.Fill;
            this.btnPushUpdateDBData.Click += new EventHandler(this.btnPushUpdateDBData_Click);
        }
        private void BatchPushDBDataBtn()
        {
            Log("Initializing batch push to database button");
            this.btnBatchPushDBData = new Button();
            this.btnBatchPushDBData.Text = "Batch Push to DB";
            this.btnBatchPushDBData.Dock = DockStyle.Fill;
            this.btnBatchPushDBData.Click += new EventHandler(this.btnBatchPushDBData_Click);
        }

        private void NameFilterTextBox()
        {
            Log("Initializing name filter name box");
            this.txtNameFilter = new TextBox();
            this.txtNameFilter.PlaceholderText = "Filter by name...";
            this.txtNameFilter.Dock = DockStyle.None;
            this.txtNameFilter.Size = new System.Drawing.Size(200, 20);
            this.txtNameFilter.KeyDown += new KeyEventHandler(this.ApplyFilters_KeyDown);
        }
        private void GenreTagTextBox()
        {
            Log("Initializing genre/tag filter text box");
            this.txtGenreTagFilter = new TextBox();
            this.txtGenreTagFilter.Dock = DockStyle.None;
            this.txtGenreTagFilter.Size = new System.Drawing.Size(150, 20);
            this.txtGenreTagFilter.PlaceholderText = "Filter by genre or tag...";
            this.txtGenreTagFilter.KeyDown += new KeyEventHandler(this.ApplyFilters_KeyDown);
            //this.Controls.Add(txtGenreTagFilter);
        }
        private void ConsoleView()
        {
            Log("Initializing console");
            this.consoleView.Dock = DockStyle.Fill;
            //this.consoleView.Size = new System.Drawing.Size(800, 100);
            this.consoleView.BackColor = Color.Black;
            this.consoleView.ForeColor = Color.White;
            this.consoleView.Multiline = true;
            this.consoleView.ReadOnly = true;
            this.consoleView.ScrollBars = ScrollBars.Vertical;
            //this.consoleView.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            //this.Controls.Add(consoleView);
        }

        public void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            consoleView.AppendText($"[{timestamp}] {message}{Environment.NewLine}");
        }

        //private void SampleDataSetup()
        //{
        //    this.dataGridView.ColumnCount = 3;
        //    this.dataGridView.Columns[0].Name = "Game ID";
        //    this.dataGridView.Columns[1].Name = "Game Name";
        //    this.dataGridView.Columns[2].Name = "Release Date";
        //}

        //private void SampleDataLoad()
        //{
        //    this.dataGridView.Rows.Add("1", "Portal 2", "2011-04-19");
        //    this.dataGridView.Rows.Add("2", "Half-Life", "1998-11-08");
        //    this.dataGridView.Rows.Add("3", "Dota 2", "2013-07-09");
        //    this.dataGridView.Rows.Add("4", "Team Fortress 2", "2007-10-10");
        //    this.dataGridView.Rows.Add("5", "Counter-Strike", "2000-11-01");
        //}

        // Event handlers
        private void btnImportCSV_Click(object sender, EventArgs e)
        {
            Log("Starting .csv data import");
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                openFileDialog.Title = "Import CSV File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    ImportCSV(filePath);
                    Log(".csv data import complete");
                }
            }
        }
        private void ImportCSV(string filePath)
        {
            // Clear filter fields and active filters before import
            txtNameFilter.Text = string.Empty;
            txtGenreTagFilter.Text = string.Empty;
            gameView.RowFilter = string.Empty;

            int rowCounter = 0;
            var requiredColumns = new List<string>
            {
                "AppID",
                "name",
                "release_date",
                "price", 
                "metacritic_score",
                "recommendations", 
                "categories", 
                "genres", 
                "positive",
                "negative", 
                "estimated_owners", 
                "average_playtime_forever",
                "peak_ccu", 
                "tags", 
                "pct_pos_total", 
                "num_reviews_total"
            };

            Log("Clearing existing data from Local Data Table.");
            gameTable.Clear();
            gameTable.Columns.Clear();

            Log($"Opening and reading CSV file: {filePath}");

            Log("Suspending layout for better performance");
            dataGridView.SuspendLayout();

            using (StreamReader reader = new StreamReader(filePath))
            {
                Log("Reading first row for header information.");
                bool isFirstRow = true;

                List<int> columnIndices = new List<int>();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (isFirstRow)
                    {
                        var headers = ParseCsvLine(line);
                        Log("Identifying required columns from header.");
                        for (int i = 0; i < headers.Length; i++)
                        {
                            string columnName = headers[i].Trim();
                            if (requiredColumns.Contains(columnName))
                            {
                                columnIndices.Add(i);
                                gameTable.Columns.Add(columnName, typeof(string));
                                Log($"Added column: {columnName}");
                            }
                        }
                        isFirstRow = false;
                    }
                    else
                    {
                        var game = ParseGameFromCsvLine(line, columnIndices);
                        gameTable.Rows.Add(
                            game.AppID, game.Name, game.ReleaseDate, game.Price, game.MetacriticScore,
                            game.Recommendations, string.Join(", ", game.Categories),
                            string.Join(", ", game.Genres), game.Positive, game.Negative,
                            game.EstimatedOwners, game.AveragePlaytime, game.PeakCcu,
                            string.Join(", ", game.Tags), game.PctPosTotal, game.NumReviews);

                        importGames.Add(game);
                        rowCounter++;

                        if (rowCounter % 1000 == 0)
                        {
                            Log($"Processed {rowCounter} rows.");
                        }
                    }
                }
            }

            Log("CSV import completed successfully.");
            dataGridView.DataSource = gameView;

            dataGridView.ResumeLayout();
            rowCountLabel.Text = $"Row count: {rowCounter}";
        }

        private Game ParseGameFromCsvLine(string line, List<int> columnIndices)
        {
            var values = ParseCsvLine(line);
            string priceString = values[columnIndices[3]].Replace(',', '.');

            var game = new Game
            {
                AppID = int.Parse(values[columnIndices[0]]),
                Name = values[columnIndices[1]],
                ReleaseDate = DateTime.Parse(values[columnIndices[2]]),
                Price = decimal.Parse(priceString, CultureInfo.InvariantCulture),
                MetacriticScore = int.Parse(values[columnIndices[4]]),
                Recommendations = int.Parse(values[columnIndices[5]]),
                Categories = ParseBracketedList(values[columnIndices[6]]),
                Genres = ParseBracketedList(values[columnIndices[7]]),
                Positive = int.Parse(values[columnIndices[8]]),
                Negative = int.Parse(values[columnIndices[9]]),
                EstimatedOwners = values[columnIndices[10]],
                AveragePlaytime = int.Parse(values[columnIndices[11]]),
                PeakCcu = int.Parse(values[columnIndices[12]]),
                Tags = ParseCurlyBracedList(values[columnIndices[13]]),
                PctPosTotal = decimal.Parse(values[columnIndices[14]]),
                NumReviews = int.Parse(values[columnIndices[15]])
            };

            return game;
        }

        private List<string> ParseBracketedList(string input)
        {
            return input.Trim('[', ']').Split(',').Select(item => item.Trim()).ToList();
        }

        private List<string> ParseCurlyBracedList(string input)
        {
            return input.Trim('{', '}').Split(',').Select(item => item.Split(':')[0].Trim()).ToList();
        }


        // Helper function to handle quoted fields
        private string[] ParseCsvLine(string line)
        {
            List<string> result = new List<string>();
            bool insideQuotes = false;
            StringBuilder value = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    insideQuotes = !insideQuotes;
                }
                else if (c == ',' && !insideQuotes)
                {
                    result.Add(value.ToString());
                    value.Clear();
                }
                else
                {
                    value.Append(c);
                }
            }

            result.Add(value.ToString());
            return result.ToArray();
        }

        private void btnExportCSV_Click(object sender, EventArgs e)
        {
            Log("Starting .csv data export");
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
                saveFileDialog.Title = "Save CSV File";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    ExportCSV(filePath);
                    Log(".csv data export complete");
                }
            }
        }
        private void ExportCSV(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                Log("Writing headers");
                for (int i = 0; i < dataGridView.Columns.Count; i++)
                {
                    writer.Write(dataGridView.Columns[i].Name);
                    if (i < dataGridView.Columns.Count - 1)
                        writer.Write(",");
                }
                writer.WriteLine();

                Log("Writing filtered rows");

                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (!row.IsNewRow && row.Visible)
                    {
                        for (int i = 0; i < dataGridView.Columns.Count; i++)
                        {
                            var cellValue = row.Cells[i].Value;

                            if (cellValue is decimal decimalValue)
                            {
                                writer.Write(decimalValue.ToString(CultureInfo.InvariantCulture));
                            }
                            else if (cellValue is double doubleValue)
                            {
                                writer.Write(doubleValue.ToString(CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                var stringValue = cellValue?.ToString() ?? "";

                                if (stringValue.Contains(",") || stringValue.Contains("\"") || stringValue.Contains("\n"))
                                {
                                    stringValue = $"\"{stringValue.Replace("\"", "\"\"")}\"";
                                }

                                writer.Write(stringValue);
                            }

                            if (i < dataGridView.Columns.Count - 1)
                                writer.Write(",");
                        }
                        writer.WriteLine();
                    }
                }
            }
        }

        private async void btnFetchSteamData_Click(object sender, EventArgs e)
        {
            // TO DO
        }

        private async void btnTestDBConnection_Click(object sender, EventArgs e)
        {
            dbHandler.TestConnection();
        }

        private async void btnValidateGameData_Click(object sender, EventArgs e)
        {
            int rowCounter = 0;
            int validCounter = 0;
            int invalidCounter = 0;

            List<Game> invalidGames = new List<Game>();

            Log("Starting data validation...");
            foreach (var game in importGames.ToList()) // Use .ToList() to avoid modifying the collection during iteration
            {
                bool isValid = dbHandler.ValidateGameData(game);
                rowCounter++;

                if (isValid) 
                { 
                    validCounter++; 
                } 
                else 
                {
                    Log($"Game {game.AppID} - {game.Name}: Invalid");
                    invalidCounter++;
                    invalidGames.Add(game);
                }

                if (rowCounter % 1000 == 0)
                {
                    Log($"Validated {rowCounter} rows.");
                }
            }

            Log("Removing invalid games from DataGridView and games list. Please wait.");
            foreach (var invalidGame in invalidGames)
            {
                Log($"Removing game {invalidGame.AppID.ToString()} from games list");
                importGames.Remove(invalidGame);

                dataGridView.SuspendLayout();

                Log($"Removing game {invalidGame.AppID.ToString()} from DataGridView");
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.Cells["AppID"].Value.ToString() == invalidGame.AppID.ToString())
                    {
                        dataGridView.Rows.Remove(row);
                        break;
                    }
                }

                dataGridView.ResumeLayout();
            }

            Log($"Data validation complete. {validCounter} valid, {invalidCounter} invalid.");
            rowCountLabel.Text = $"Row count: {validCounter}";
        }

        private async void btnPullDBData_Click(object sender, EventArgs e)
        {
            Log("Clearing existing data from DB Data Table.");
            dbGameTable.Clear();

            Log("Starting batch data pull from the database...");

            int batchSize = 1000;
            int totalPulled = 0;

            while (true)
            {
                Log($"Pulling next batch of {batchSize} games from the database...");
                var gamesBatch = await dbHandler.PullGameDataBatchAsync(totalPulled, batchSize);

                if (gamesBatch.Count == 0)
                {
                    Log("No more data to pull.");
                    break;
                }

                foreach (var game in gamesBatch)
                {
                    dbGameTable.Rows.Add(
                        game.AppID, game.Name, game.ReleaseDate, game.Price, game.MetacriticScore,
                        game.Recommendations, string.Join(", ", game.Categories), string.Join(", ", game.Genres),
                        game.Positive, game.Negative, game.EstimatedOwners, game.AveragePlaytime,
                        game.PeakCcu, string.Join(", ", game.Tags), game.PctPosTotal, game.NumReviews);
                }

                totalPulled += gamesBatch.Count;
                Log($"Batch pull complete. {totalPulled} total games pulled so far.");

                if (gamesBatch.Count < batchSize)
                {
                    Log("Final batch pulled. Data pull complete.");
                    break;
                }
            }
        }

        private async void btnPushUpdateDBData_Click(object sender, EventArgs e)
        {
            Log("Starting update push to the database...");

            int batchSize = 100;
            int totalGames = importGames.Count;
            int batchCount = (int)Math.Ceiling((double)totalGames / batchSize);

            for (int batch = 0; batch < batchCount; batch++)
            {
                int start = batch * batchSize;
                int end = Math.Min(start + batchSize, totalGames);

                Log($"Processing batch {batch + 1}/{batchCount}: Games {start + 1} to {end}");

                for (int i = start; i < end; i++)
                {
                    await dbHandler.UpdateGameDataAsync(importGames[i]);
                }

                Log($"Batch {batch + 1} completed: {end - start} games pushed to the database.");
            }

            Log("Data update push to the database complete.");
        }

        private async void btnBatchPushDBData_Click(object sender, EventArgs e)
        {
            Log("Starting batch push to the database...");

            int batchSize = 100;
            int totalGames = importGames.Count;
            int batchCount = (int)Math.Ceiling((double)totalGames / batchSize);

            for (int batch = 0; batch < batchCount; batch++)
            {
                int start = batch * batchSize;
                int end = Math.Min(start + batchSize, totalGames);

                Log($"Processing batch {batch + 1}/{batchCount}: Games {start + 1} to {end}");

                var batchList = importGames.Skip(start).Take(batchSize).ToList();
                await dbHandler.InsertGamesInBatchAsync(batchList);

                Log($"Batch {batch + 1}/{batchCount} pushed successfully.");
            }

            MessageBox.Show("Batch Push Completed");
            Log("Batch push to the database complete.");
        }

        private void ApplyFilters_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string nameFilter = txtNameFilter.Text.ToLower();
                string genreTagFilter = txtGenreTagFilter.Text.ToLower();

                var filterList = new List<string>();

                if (!string.IsNullOrEmpty(nameFilter))
                    filterList.Add($"(Name LIKE '%{nameFilter}%')");

                if (!string.IsNullOrEmpty(genreTagFilter))
                    filterList.Add($"(Tags LIKE '%{genreTagFilter}%' OR Genres LIKE '%{genreTagFilter}%')");

                gameView.RowFilter = string.Join(" AND ", filterList);

                int visibleRowCount = gameView.Count;
                rowCountLabel.Text = $"Row count: {visibleRowCount}";

                UpdateImportGamesList();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void UpdateImportGamesList()
        {
            importGames.Clear();

            foreach (DataRowView rowView in gameView)
            {
                DataRow row = rowView.Row;

                var game = new Game
                {
                    AppID = Convert.ToInt32(row["AppID"]),
                    Name = row["name"].ToString(),
                    ReleaseDate = Convert.ToDateTime(row["release_date"]),
                    Price = Convert.ToDecimal(row["price"], CultureInfo.InvariantCulture),
                    MetacriticScore = Convert.ToInt32(row["metacritic_score"]),
                    Recommendations = Convert.ToInt32(row["recommendations"]),
                    Categories = row["categories"].ToString().Split(',').ToList(),
                    Genres = row["genres"].ToString().Split(',').ToList(),
                    Positive = Convert.ToInt32(row["positive"]),
                    Negative = Convert.ToInt32(row["negative"]),
                    EstimatedOwners = row["estimated_owners"].ToString(),
                    AveragePlaytime = Convert.ToInt32(row["average_playtime_forever"]),
                    Tags = row["tags"].ToString().Split(',').ToList(),
                    PeakCcu = Convert.ToInt32(row["peak_ccu"]),
                    PctPosTotal = Convert.ToDecimal(row["pct_pos_total"]),
                    NumReviews = Convert.ToInt32(row["num_reviews_total"])
                };

                importGames.Add(game);
            }
        }


    }
}
