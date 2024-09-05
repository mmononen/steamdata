# Install mysql-connector-python (if not already installed)
# pip install mysql-connector-python

import mysql.connector

# Connection details
conn = mysql.connector.connect(
    host="86.60.209.30",
    user="remoteuser",
    password="***",
    database="indie_games_db",
    port=3306
)

cursor = conn.cursor()

# Helper function to fetch all games
def fetch_all_games():
    query = "SELECT * FROM steam_games;"
    cursor.execute(query)
    result = cursor.fetchall()
    for row in result:
        print(row)

# Helper function to fetch a game by appid
def fetch_game_by_appid(appid):
    query = "SELECT * FROM steam_games WHERE appid = %s;"
    cursor.execute(query, (appid,))
    result = cursor.fetchone()
    print(result)

# Helper function to insert a new game
# Check README.md for more information
def insert_game(game_data):
    query = """
    INSERT INTO steam_games (appid, name, developer, publisher, score_rank, positive, negative, userscore, owners, 
    average_forever, average_2weeks, median_forever, median_2weeks, price, initialprice, discount, ccu) 
    VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s);
    """
    cursor.execute(query, game_data)
    conn.commit()
    print("Game inserted successfully.")

# Helper function to update game data
def update_game(appid, field, value):
    query = f"UPDATE steam_games SET {field} = %s WHERE appid = %s;"
    cursor.execute(query, (value, appid))
    conn.commit()
    print(f"Game with appid {appid} updated successfully.")

# Helper function to delete a game by appid
def delete_game(appid):
    query = "DELETE FROM steam_games WHERE appid = %s;"
    cursor.execute(query, (appid,))
    conn.commit()
    print(f"Game with appid {appid} deleted successfully.")

# Example usage
# Uncomment these lines to use the functions
# fetch_all_games()
# fetch_game_by_appid(0000)
# insert_game((0000, 'New Game', 'New Dev', 'New Publisher', None, 000, 10, 0, '0,000,000 .. 0,000,000', 0000, 000, 00, 00, 0.00, 00.00, 0, 0000))
# update_game(0000, 'name', 'Updated Game Name')
# delete_game(0000)

# Close the connection when done
# conn.close()
