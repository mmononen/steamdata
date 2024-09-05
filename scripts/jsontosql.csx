#r "nuget: Newtonsoft.Json, 13.0.1"

using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

// Function to escape single quotes for SQL
string escapeString(string value)
{
    return value.Replace("'", "''");
}

public class Game
{
    public int appid { get; set; }
    public string name { get; set; }
    public string developer { get; set; }
    public string publisher { get; set; }
    public string score_rank { get; set; }
    public int positive { get; set; }
    public int negative { get; set; }
    public int userscore { get; set; }
    public string owners { get; set; }
    public int average_forever { get; set; }
    public int average_2weeks { get; set; }
    public int median_forever { get; set; }
    public int median_2weeks { get; set; }
    public string price { get; set; }
    public string initialprice { get; set; }
    public int discount { get; set; }
    public int ccu { get; set; }
}

// Load the JSON file
string jsonFilePath = "games_data.json";
var jsonData = File.ReadAllText(jsonFilePath);

// Deserialize JSON to a Dictionary of games
var games = JsonConvert.DeserializeObject<Dictionary<string, Game>>(jsonData);

// Prepare the SQL INSERT statements
List<string> sqlInserts = new List<string>();

foreach (var gameEntry in games)
{
    var game = gameEntry.Value;

    // Convert price from cents to dollars and ensure the correct decimal separator
    string price = (double.Parse(game.price) / 100).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
    string initialPrice = (double.Parse(game.initialprice) / 100).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

    // Create SQL INSERT statement
    string sql = $@"
        INSERT INTO steam_games (
            appid, 
            name, 
            developer, 
            publisher, 
            score_rank, 
            positive, 
            negative, 
            userscore, 
            owners, 
            average_forever, 
            average_2weeks, 
            median_forever, 
            median_2weeks, 
            price, 
            initialprice, 
            discount, 
            ccu) VALUES (
            {game.appid}, 
            '{escapeString(game.name)}', 
            '{escapeString(game.developer)}', 
            '{escapeString(game.publisher)}', 
            {(string.IsNullOrEmpty(game.score_rank) ? "NULL" : $"'{escapeString(game.score_rank)}'")}, 
            {game.positive}, 
            {game.negative}, 
            {game.userscore}, 
            '{escapeString(game.owners)}', 
            {game.average_forever}, 
            {game.average_2weeks}, 
            {game.median_forever}, 
            {game.median_2weeks}, 
            {price}, 
            {initialPrice}, 
            {game.discount}, 
            {game.ccu});";

    sqlInserts.Add(sql.Trim());
}


// Save the SQL statements to a .sql file
File.WriteAllLines("steam_games_insert.sql", sqlInserts);

Console.WriteLine("SQL statements generated and saved to steam_games_insert.sql!");
