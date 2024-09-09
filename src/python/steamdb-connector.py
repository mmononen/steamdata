# Install mysql-connector-python (if not already installed)
# pip install mysql-connector-python

import mysql.connector
import json

# Load configuration from JSON
with open('db_config.json', 'r') as config_file:
    config = json.load(config_file)

# Connection details
conn = mysql.connector.connect(
    host=config['host'],
    user=config['user'],
    password=config['password'],
    database=config['database'],
    port=config['port']
)

cursor = conn.cursor()

# Helper function to fetch all games
def fetch_all_games():
    query = "SELECT * FROM indie_games;"
    cursor.execute(query)
    result = cursor.fetchall()
    for row in result:
        print(row)

# Helper function to fetch a game by appid
def fetch_game_by_appid(appid):
    query = "SELECT * FROM indie_games WHERE appid = %s;"
    cursor.execute(query, (appid,))
    result = cursor.fetchone()
    print(result)

# Helper function to insert a new game
def insert_game(game_data):
    query = """
    INSERT INTO indie_games (appid, name, release_date, genres, tags, pct_pos_total, pct_pos_recent, 
    windows, mac, linux, metacritic_score, categories, estimated_owners, ccu, price) 
    VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s);
    """
    cursor.execute(query, game_data)
    conn.commit()
    print("Game inserted successfully.")

# Helper function to update game data
def update_game(appid, field, value):
    query = f"UPDATE indie_games SET {field} = %s WHERE appid = %s;"
    cursor.execute(query, (value, appid))
    conn.commit()
    print(f"Game with appid {appid} updated successfully.")

# Helper function to delete a game by appid
def delete_game(appid):
    query = "DELETE FROM indie_games WHERE appid = %s;"
    cursor.execute(query, (appid,))
    conn.commit()
    print(f"Game with appid {appid} deleted successfully.")

# Example usage
# Uncomment these lines to use the functions
# fetch_all_games()
# fetch_game_by_appid(0000)
# insert_game((0000, 'New Game', '2021-01-01', 'Action, Adventure', 'Indie', 95.00, 93.00, 1, 1, 0, 85, 'Single-player', '10,000 - 50,000', 1000, 19.99))
# update_game(0000, 'name', 'Updated Game Name')
# delete_game(0000)

# Close the connection when done
conn.close()
