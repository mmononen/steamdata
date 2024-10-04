using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto;
using System.Data;
using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using SteamSyncDB;

namespace SteamSyncDB
{
    public partial class MainForm : Form
    {
        private DataGridView dataGridView;
        private DataGridView dbDataGridView;
        private Button btnApplyFilters;
        private Button btnClearFilters;
        private Button btnImportCSV;
        private Button btnExportCSV;
        private Button btnFetchSteamData;
        private Button btnTestDBConnection;
        private Button btnValidateGameData;
        private Button btnPullDBData;
        private Button btnPushUpdateDBData;
        private Button btnBatchPushDBData;
        private Button btnPurgeDBData;
        private TextBox txtNameFilter;
        //private TextBox txtTagFilter;
        private TextBox txtGenreTagFilter;
        private DateTimePicker startDatePicker;
        private DateTimePicker endDatePicker;
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

        // Enum for validation method
        public enum ValidationResult
        {
            Valid,
            Invalid,
            Duplicate
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
            InitializeDatePickers();
            InitializeLayout();

            Log("Form initialization complete");

            //SampleDataSetup();
            //SampleDataLoad();
        }

        private void InitializeButtons()
        {
            FilterBtns();
            ImportCSVBtn();
            ExportCSVBtn();
            FetchSteamDataBtn();
            TestDBConnectionBtn();
            ValidateGameDataBtn();
            PullDBDataBtn();
            PushDBUpdateDataBtn();
            BatchPushDBDataBtn();
            PurgeDBDataBtn();
        }

        private void InitializeTable(DataTable table)
        {
            if (table.Columns.Count == 0)
            {
                table.Columns.Add("AppID", typeof(int));
                table.Columns.Add("name", typeof(string));
                table.Columns.Add("release_date", typeof(string));
                table.Columns.Add("price", typeof(string));
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
                table.Columns.Add("pct_pos_total", typeof(string));
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
            rowCountLabel.Text = $"Imported row count: ";
            this.Controls.Add(rowCountLabel);
        }

        // Too Slow
        //private void UpdateRowCount(bool defer = false)
        //{
        //    if (defer)
        //    {
        //        BeginInvoke((MethodInvoker)delegate {
        //            int rowCount = dataGridView.Rows.Cast<DataGridViewRow>().Count(row => row.Visible);
        //            rowCountLabel.Text = $"Imported row count: {rowCount}";
        //        });
        //    }
        //    else
        //    {
        //        int rowCount = dataGridView.Rows.Cast<DataGridViewRow>().Count(row => row.Visible);
        //        rowCountLabel.Text = $"Imported row count: {rowCount}";
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
                ColumnCount = 20,
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

            var emptySpace1 = new Label() { Width = 30 };
            var emptySpace2 = new Label() { Width = 60 };
            var emptySpace3 = new Label() { Width = 60 };

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
            controlsPanel.Controls.Add(startDatePicker, 2, 0);
            controlsPanel.Controls.Add(endDatePicker, 3, 0);
            controlsPanel.Controls.Add(btnApplyFilters, 4, 0);
            controlsPanel.Controls.Add(btnClearFilters, 5, 0);
            controlsPanel.Controls.Add(emptySpace1, 6, 0);
            controlsPanel.Controls.Add(btnImportCSV, 7, 0);
            controlsPanel.Controls.Add(btnExportCSV, 8, 0);
            controlsPanel.Controls.Add(btnFetchSteamData, 9, 0);
            controlsPanel.Controls.Add(emptySpace2, 10, 0);
            controlsPanel.Controls.Add(btnTestDBConnection, 11, 0);
            controlsPanel.Controls.Add(btnPullDBData, 12, 0);
            controlsPanel.Controls.Add(btnValidateGameData, 13, 0);
            controlsPanel.Controls.Add(btnPushUpdateDBData, 14, 0);
            controlsPanel.Controls.Add(btnBatchPushDBData, 15, 0);
            controlsPanel.Controls.Add(emptySpace3, 16, 0);
            controlsPanel.Controls.Add(btnPurgeDBData, 17, 0);

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

        private void FilterBtns()
        {
            Log("Initializing filter buttons");
            btnApplyFilters = new Button
            {
                Text = "Apply Filters",
                Width = 100
            };
            btnApplyFilters.Click += BtnApplyFilters_Click;

            btnClearFilters = new Button
            {
                Text = "Clear Filters",
                Width = 100
            };
            btnClearFilters.Click += BtnClearFilters_Click;
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

        private void PurgeDBDataBtn()
        {
            Log("Initializing purge database button");
            this.btnPurgeDBData = new Button();
            this.btnPurgeDBData.Text = "Purge DB";
            this.btnPurgeDBData.Dock = DockStyle.Fill;
            this.btnPurgeDBData.Click += new EventHandler(this.btnPurgeDBData_Click);
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

        private void InitializeDatePickers()
        {
            Log("Initializing Date Picker");
            startDatePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Width = 100
            };

            startDatePicker.Value = DateTime.Today.AddYears(-40);

            // End Date Picker
            endDatePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Width = 100
            };
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


            Log("Suspending layout for better performance");
            dataGridView.SuspendLayout();

            importGames.Clear();

            Log($"Opening and reading CSV file: {filePath}");
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
                            game.AppID,
                            game.Name, 
                            game.ReleaseDate.ToString("yyyy-MM-dd"), 
                            game.Price.ToString("0.00", CultureInfo.InvariantCulture), 
                            game.MetacriticScore,
                            game.Recommendations, 
                            string.Join(", ", game.Categories),
                            string.Join(", ", game.Genres), 
                            game.Positive, 
                            game.Negative,
                            game.EstimatedOwners, 
                            game.AveragePlaytime, 
                            game.PeakCcu,
                            string.Join(", ", game.Tags),
                            game.PctPosTotal.ToString("0.00", CultureInfo.InvariantCulture), 
                            game.NumReviews);

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
            rowCountLabel.Text = $"Imported row count: {rowCounter}";
        }

        private Game ParseGameFromCsvLine(string line, List<int> columnIndices)
        {
            var values = ParseCsvLine(line);

            // Ensure decimal is period
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
                PctPosTotal = decimal.Parse(values[columnIndices[14]], CultureInfo.InvariantCulture),
                NumReviews = int.Parse(values[columnIndices[15]])
            };

            return game;
        }

        // Remove brackets
        private List<string> ParseBracketedList(string input)
        {
            return input.Trim('[', ']').Split(',').Select(item => item.Trim()).ToList();
        }
        // Remove curly braces
        private List<string> ParseCurlyBracedList(string input)
        {
            return input.Trim('{', '}').Split(',').Select(item => item.Split(':')[0].Trim()).ToList();
        }
        // Handle quoted fields
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

                            string formattedValue = cellValue switch
                            {
                                DateTime dateTimeValue => dateTimeValue.ToString("yyyy-MM-dd"),
                                decimal decimalValue => decimalValue.ToString("0.00", CultureInfo.InvariantCulture),
                                double doubleValue => doubleValue.ToString("0.00", CultureInfo.InvariantCulture),
                                _ => cellValue?.ToString() ?? ""
                            };

                            // Handle special characters in the CSV (comma, quotes, etc.)
                            if (formattedValue.Contains(",") || formattedValue.Contains("\"") || formattedValue.Contains("\n"))
                            {
                                formattedValue = $"\"{formattedValue.Replace("\"", "\"\"")}\"";
                            }

                            writer.Write(formattedValue);

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

        // Validate and compare import data with db data
        private async void btnValidateGameData_Click(object sender, EventArgs e)
        {
            int rowCounter = 0;
            int validCounter = 0;
            int invalidCounter = 0;
            int duplicateCounter = 0;

            List<Game> validGames = new List<Game>();

            Log("Starting data validation...");

            foreach (var importedGame in importGames.ToList())
            {
                var dbGame = dbGames.FirstOrDefault(g => g.AppID == importedGame.AppID);
                var result = ValidateGameData(importedGame, dbGame);
                rowCounter++;

                switch (result)
                {
                    case ValidationResult.Valid:
                        validGames.Add(importedGame);
                        validCounter++;
                        break;
                    case ValidationResult.Invalid:
                        invalidCounter++;
                        break;
                    case ValidationResult.Duplicate:
                        duplicateCounter++;
                        break;
                }

                if (rowCounter % 1000 == 0)
                {
                    Log($"Validated {rowCounter} rows.");
                }
            }

            importGames = validGames;


            // Update view with only valid data
            Log("Updating DataGridView with valid games. Please wait.");
            dataGridView.DataSource = null;  // Unbind the DataGridView from the data source

            //Log(gameTable.ToString());
            //if (gameTable == null) { return; }

            gameTable.Clear();  // Clear the DataTable
            gameTable.Columns.Clear();  // Clear the columns

            InitializeTable(gameTable);

            // Re-populate the DataTable with valid games
            foreach (var validGame in validGames)
            {
                //if (validGame == null)
                //{
                //    continue;
                //}

                try
                {
                    gameTable.Rows.Add(
                        validGame.AppID,
                        validGame.Name ?? string.Empty,
                        validGame.ReleaseDate.ToString("yyyy-MM-dd"),
                        validGame.Price.ToString("0.00", CultureInfo.InvariantCulture),
                        validGame.MetacriticScore,
                        validGame.Recommendations,
                        string.Join(", ", validGame.Categories ?? new List<string>()),
                        string.Join(", ", validGame.Genres ?? new List<string>()),
                        validGame.Positive,
                        validGame.Negative,
                        validGame.EstimatedOwners ?? string.Empty,
                        validGame.AveragePlaytime,
                        validGame.PeakCcu,
                        string.Join(", ", validGame.Tags ?? new List<string>()),
                        validGame.PctPosTotal.ToString("0.00", CultureInfo.InvariantCulture),
                        validGame.NumReviews
                    );
                }
                catch (Exception ex)
                {

                    Log($"Error adding game AppID={validGame.AppID}, Name={validGame.Name}: {ex.Message}");
                    continue;
                }
            }

            dataGridView.DataSource = gameView;

            Log($"Data validation complete. {validCounter} valid, {invalidCounter} invalid, {duplicateCounter} duplicates.");
            rowCountLabel.Text = $"Imported row count: {validCounter}";
        }
        public ValidationResult ValidateGameData(Game importedGame, Game dbGame)
        {
            // Basic validation
            if (importedGame.AppID <= 0 || string.IsNullOrEmpty(importedGame.Name))
            { return ValidationResult.Invalid; }

            // Assume valid if not found in db
            if (dbGame == null)
            { return ValidationResult.Valid; }

            // Sort lists for comparison
            var sortedImportedTags = importedGame.Tags.OrderBy(t => t).ToList();
            var sortedDbTags = dbGame.Tags.OrderBy(t => t).ToList();
            var sortedImportedGenres = importedGame.Genres.OrderBy(g => g).ToList();
            var sortedDbGenres = dbGame.Genres.OrderBy(g => g).ToList();
            var sortedImportedCategories = importedGame.Categories.OrderBy(g => g).ToList();
            var sortedDbCategories = dbGame.Categories.OrderBy(g => g).ToList();

            // Check for duplicate data
            if (
                sortedImportedTags.SequenceEqual(sortedDbTags) &&
                sortedImportedGenres.SequenceEqual(sortedDbGenres) &&
                sortedImportedCategories.SequenceEqual(sortedDbCategories) &&
                importedGame.Price == dbGame.Price &&
                importedGame.ReleaseDate == dbGame.ReleaseDate &&
                importedGame.MetacriticScore == dbGame.MetacriticScore &&
                importedGame.Recommendations == dbGame.Recommendations &&
                importedGame.Positive == dbGame.Positive &&
                importedGame.Negative == dbGame.Negative &&
                importedGame.EstimatedOwners == dbGame.EstimatedOwners &&
                importedGame.AveragePlaytime == dbGame.AveragePlaytime &&
                importedGame.PeakCcu == dbGame.PeakCcu &&
                importedGame.PctPosTotal == dbGame.PctPosTotal &&
                importedGame.NumReviews == dbGame.NumReviews)
            {
                return ValidationResult.Duplicate;
            }

            // Valid if no issues found
            return ValidationResult.Valid;
        }
        private async void btnPullDBData_Click(object sender, EventArgs e)
        {
            Log("Clearing existing data from DB Data Table.");
            dbGameTable.Clear();
            dbGames.Clear();

            Log("Starting data pull from the database...");

            var games = await dbHandler.PullAllGameDataAsync();

            Log("All data pulled, updating the UI...");

            foreach (var game in games)
            {
                dbGameTable.Rows.Add(
                    game.AppID,
                    game.Name,
                    game.ReleaseDate.ToString("yyyy-MM-dd"),
                    game.Price.ToString("0.00", CultureInfo.InvariantCulture),
                    game.MetacriticScore,
                    game.Recommendations,
                    string.Join(", ", game.Categories),
                    string.Join(", ", game.Genres),
                    game.Positive,
                    game.Negative,
                    game.EstimatedOwners,
                    game.AveragePlaytime,
                    game.PeakCcu,
                    string.Join(", ", game.Tags),
                    game.PctPosTotal.ToString("0.00", CultureInfo.InvariantCulture),
                    game.NumReviews
                );

                dbGames.Add(game);
            }

            Log($"Data pull complete. {games.Count} total games pulled.");
        }

        // Update db with induvidual transactions
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

        // Update db with bulk data batches
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

        private void BtnApplyFilters_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        // Apply filters with Enter
        private void ApplyFilters_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ApplyFilters();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void ApplyFilters()
        {
            string nameFilter = txtNameFilter.Text.ToLower();
            string genreTagFilter = txtGenreTagFilter.Text.ToLower();
            DateTime startDate = startDatePicker.Value.Date;
            DateTime endDate = endDatePicker.Value.Date;

            var filterList = new List<string>();

            if (!string.IsNullOrEmpty(nameFilter))
                filterList.Add($"(Name LIKE '%{nameFilter}%')");

            if (!string.IsNullOrEmpty(genreTagFilter))
                filterList.Add($"(Tags LIKE '%{genreTagFilter}%' OR Genres LIKE '%{genreTagFilter}%')");

            filterList.Add($"(release_date >= #{startDate:yyyy-MM-dd}# AND release_date <= #{endDate:yyyy-MM-dd}#)");

            gameView.RowFilter = string.Join(" AND ", filterList);

            int visibleRowCount = gameView.Count;
            rowCountLabel.Text = $"Imported row count: {visibleRowCount}";

            UpdateImportGamesList();
        }

        private void BtnClearFilters_Click(object sender, EventArgs e)
        {
            txtNameFilter.Text = string.Empty;
            txtGenreTagFilter.Text = string.Empty;
            startDatePicker.Value = DateTime.Today.AddYears(-40);
            endDatePicker.Value = DateTime.Today;
            gameView.RowFilter = string.Empty;

            rowCountLabel.Text = $"Imported row count: {gameView.Count}";

            UpdateImportGamesList();
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
                    PctPosTotal = Convert.ToDecimal(row["pct_pos_total"], CultureInfo.InvariantCulture),
                    NumReviews = Convert.ToInt32(row["num_reviews_total"])
                };

                importGames.Add(game);
            }
        }

        private async void btnPurgeDBData_Click(object sender, EventArgs e)
        {
            var confirmationResult = MessageBox.Show("Are you sure you want to delete all data from the database? This action cannot be undone.",
                                                     "Confirm Purge",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Warning);

            if (confirmationResult == DialogResult.Yes)
            {
                Log("Starting database purge...");
                try
                {
                    await dbHandler.PurgeDatabaseAsync();
                    MessageBox.Show("All data has been successfully purged from the database.", "Purge Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Log($"Error during database purge: {ex.Message}");
                    MessageBox.Show("An error occurred during the purge process. Please check the logs for more details.", "Purge Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                Log("Database purge operation was cancelled.");
            }
        }
    }
}
