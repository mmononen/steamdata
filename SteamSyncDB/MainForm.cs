using System.Data;
using System.Net.Http.Json;
using System.Text;

namespace SteamSyncDB
{
    public partial class MainForm : Form
    {
        private DataGridView dataGridView;
        private Button btnImportCSV;
        private Button btnExportCSV;
        private Button btnFetchSteamData;
        private Button btnTestDBConnection;
        private TextBox txtNameFilter;
        //private TextBox txtTagFilter;
        private TextBox txtGenreTagFilter;
        private TextBox consoleView;
        private DatabaseHandler dbHandler;
        private DataTable gameTable;
        private DataView gameView;
        private Label rowCountLabel;

        public MainForm()
        {
            InitializeComponent();
            CustomInitialize();
            InitializeDataTable();
            dbHandler = new DatabaseHandler(this);
        }

        private void CustomInitialize()
        {
            this.consoleView = new TextBox();

            MainFormSettings();
            DataGridView();
            ImportCSVBtn();
            ExportCSVBtn();
            FetchSteamDataBtn();
            TestDBConnectionBtn();
            NameFilterTextBox();
            Formcontrols();
            //TagsTextBox();
            GenreTagTextBox();
            ConsoleView();
            InitializeRowCountLabel();

            Log("Form initialization complete");

            //SampleDataSetup();
            //SampleDataLoad();
        }

        private void InitializeDataTable()
        {
            gameTable = new DataTable();
            gameView = gameTable.DefaultView;
            dataGridView.DataSource = gameView;

            gameTable.Columns.Add("AppID", typeof(int));
            gameTable.Columns.Add("Name", typeof(string));
            gameTable.Columns.Add("ReleaseDate", typeof(DateTime));
            gameTable.Columns.Add("Price", typeof(decimal));
            gameTable.Columns.Add("Windows", typeof(bool));
            gameTable.Columns.Add("Mac", typeof(bool));
            gameTable.Columns.Add("Linux", typeof(bool));
            gameTable.Columns.Add("MetacriticScore", typeof(int));
            gameTable.Columns.Add("Recommendations", typeof(int));
            gameTable.Columns.Add("Categories", typeof(string));
            gameTable.Columns.Add("Genres", typeof(string));
            gameTable.Columns.Add("Positive", typeof(int));
            gameTable.Columns.Add("Negative", typeof(int));
            gameTable.Columns.Add("EstimatedOwners", typeof(string));
            gameTable.Columns.Add("AveragePlaytimeForever", typeof(int));
            gameTable.Columns.Add("PeakCcu", typeof(int));
            gameTable.Columns.Add("Tags", typeof(string));
            gameTable.Columns.Add("PctPosTotal", typeof(decimal));
            gameTable.Columns.Add("NumReviewsTotal", typeof(int));
        }


        private void MainFormSettings()
        {
            this.Text = "Steam Game Data";
            this.Size = new System.Drawing.Size(1920, 1080);
        }
        private void Formcontrols()
        {
            Log("Initializing form controls");
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.btnImportCSV);
            this.Controls.Add(this.btnExportCSV);
            this.Controls.Add(this.btnFetchSteamData);
            this.Controls.Add(this.txtNameFilter);
            this.Controls.Add(this.btnTestDBConnection);
        }
        private void InitializeRowCountLabel()
        {
            rowCountLabel = new Label();
            rowCountLabel.Location = new System.Drawing.Point(20, 850);
            rowCountLabel.Size = new System.Drawing.Size(200, 20);
            this.rowCountLabel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            rowCountLabel.Text = $"Row count: ";
            this.Controls.Add(rowCountLabel);
        }
        //private void UpdateRowCount(bool defer = false)
        //{
        //    if (defer)
        //    {
        //        // Use BeginInvoke to delay the update until the UI thread is ready
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

        private void DataGridView()
        {
            Log("Initializing data grid view");
            this.dataGridView = new DataGridView();
            this.dataGridView.Location = new System.Drawing.Point(20, 50);
            this.dataGridView.Size = new System.Drawing.Size(1860, 800);
            this.dataGridView.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //this.dataGridView.VirtualMode = true;
            this.dataGridView.AllowUserToAddRows = false;
        }
        private void ImportCSVBtn()
        {
            Log("Initializing import button");
            this.btnImportCSV = new Button();
            this.btnImportCSV.Text = "Import CSV";
            this.btnImportCSV.Location = new System.Drawing.Point(580, 20);
            this.btnImportCSV.Click += new EventHandler(this.btnImportCSV_Click);
        }
        private void ExportCSVBtn()
        {
            Log("Initializing export button");
            this.btnExportCSV = new Button();
            this.btnExportCSV.Text = "Export CSV";
            this.btnExportCSV.Location = new System.Drawing.Point(680, 20);
            this.btnExportCSV.Click += new EventHandler(this.btnExportCSV_Click);
        }
        private void FetchSteamDataBtn()
        {
            Log("Initializing fetch data button");
            this.btnFetchSteamData = new Button();
            this.btnFetchSteamData.Text = "Fetch SteamSpy Data";
            this.btnFetchSteamData.Location = new System.Drawing.Point(780, 20);
            this.btnFetchSteamData.Click += new EventHandler(this.btnFetchSteamData_Click);
        }

        private void TestDBConnectionBtn()
        {
            Log("Initializing test connection button");
            this.btnTestDBConnection = new Button();
            this.btnTestDBConnection.Text = "Test DB Connection";
            this.btnTestDBConnection.Location = new System.Drawing.Point(880, 20);
            this.btnTestDBConnection.Click += new EventHandler(this.btnTestDBConnection_Click);

        }

        private void NameFilterTextBox()
        {
            Log("Initializing name filter name box");
            this.txtNameFilter = new TextBox();
            this.txtNameFilter.PlaceholderText = "Filter by name...";
            this.txtNameFilter.Location = new System.Drawing.Point(20, 20);
            this.txtNameFilter.Size = new System.Drawing.Size(200, 20);
            this.txtNameFilter.TextChanged += new EventHandler(this.ApplyFilters);
        }
        private void GenreTagTextBox()
        {
            Log("Initializing genre/tag filter text box");
            this.txtGenreTagFilter = new TextBox();
            this.txtGenreTagFilter.Location = new System.Drawing.Point(390, 20);
            this.txtGenreTagFilter.Size = new System.Drawing.Size(150, 20);
            this.txtGenreTagFilter.PlaceholderText = "Filter by genre or tag...";
            this.txtGenreTagFilter.TextChanged += new EventHandler(this.ApplyFilters);
            this.Controls.Add(txtGenreTagFilter);
        }
        private void ConsoleView()
        {
            Log("Initializing console");
            this.consoleView.Location = new System.Drawing.Point(20, 900);
            this.consoleView.Size = new System.Drawing.Size(800, 100);
            this.consoleView.BackColor = Color.Black;
            this.consoleView.ForeColor = Color.White;
            this.consoleView.Multiline = true;
            this.consoleView.ReadOnly = true;
            this.consoleView.ScrollBars = ScrollBars.Vertical;
            this.consoleView.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);

            this.Controls.Add(consoleView);
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
            int rowCounter = 0;
            var requiredColumns = new List<string>
            {
                "AppID", "name", "release_date", "price", "windows", "mac", "linux",
                "metacritic_score", "recommendations", "categories", "genres",
                "positive", "negative", "estimated_owners", "average_playtime_forever",
                "peak_ccu", "tags", "pct_pos_total", "num_reviews_total"
            };

            Log("Clearing existing data from DataTable.");
            gameTable.Clear();
            gameTable.Columns.Clear();

            Log($"Opening and reading CSV file: {filePath}");

            // Suspend layout for better performance
            dataGridView.SuspendLayout();

            using (StreamReader reader = new StreamReader(filePath))
            {
                Log("Reading first row for header information.");
                bool isFirstRow = true;
                
                List<int> columnIndices = new List<int>();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = ParseCsvLine(line);

                    if (isFirstRow)
                    {
                        Log("Identifying required columns from header.");
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (requiredColumns.Contains(values[i].Trim()))
                            {
                                columnIndices.Add(i);
                                gameTable.Columns.Add(values[i], typeof(string));
                                Log($"Added column: {values[i]}");
                            }
                        }
                        isFirstRow = false;
                    }
                    else
                    {
                        var rowValues = columnIndices.Select(i => values[i].Trim()).ToArray();
                        gameTable.Rows.Add(rowValues);
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

            // Resume layout after import
            dataGridView.ResumeLayout();
            rowCountLabel.Text = $"Row count: {rowCounter}";
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
                    // Toggle insideQuotes when encountering a quote
                    insideQuotes = !insideQuotes;
                }
                else if (c == ',' && !insideQuotes)
                {
                    // If not inside quotes, split by comma
                    result.Add(value.ToString());
                    value.Clear();
                }
                else
                {
                    value.Append(c);
                }
            }

            // Add the final value
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
                // Write column headers
                for (int i = 0; i < dataGridView.Columns.Count; i++)
                {
                    writer.Write(dataGridView.Columns[i].Name);
                    if (i < dataGridView.Columns.Count - 1)
                        writer.Write(",");
                }
                writer.WriteLine();

                Log("Writing filtered rows");
                // Only write visible (filtered) rows
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (!row.IsNewRow && row.Visible)  // Only export visible rows
                    {
                        for (int i = 0; i < dataGridView.Columns.Count; i++)
                        {
                            var cellValue = row.Cells[i].Value?.ToString() ?? "";

                            // Quote cell if it contains a comma, quote, or new line
                            if (cellValue.Contains(",") || cellValue.Contains("\"") || cellValue.Contains("\n"))
                            {
                                // Escape any quotes by doubling them
                                cellValue = $"\"{cellValue.Replace("\"", "\"\"")}\"";
                            }

                            writer.Write(cellValue);

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
            
        }

        private async void btnTestDBConnection_Click(object sender, EventArgs e)
        {
            dbHandler.TestConnection();
        }

        private void ApplyFilters(object sender, EventArgs e)
        {
            string nameFilter = txtNameFilter.Text.ToLower();
            string genreTagFilter = txtGenreTagFilter.Text.ToLower();

            var filterList = new List<string>();

            if (!string.IsNullOrEmpty(nameFilter))
                filterList.Add($"(Name LIKE '%{nameFilter}%')");

            if (!string.IsNullOrEmpty(genreTagFilter))
                filterList.Add($"(Tags LIKE '%{genreTagFilter}%' OR Genres LIKE '%{genreTagFilter}%')");

            gameView.RowFilter = string.Join(" AND ", filterList);

            // Count visible rows after filtering
            int visibleRowCount = gameView.Count;
            rowCountLabel.Text = $"Row count: {visibleRowCount}";
        }

    }
}
