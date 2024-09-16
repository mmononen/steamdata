import mysql.connector
import json
import ast
import os

# Build the path to the db-config.json file relative to the script location
base_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))  # Get the project root
config_path = os.path.join(base_dir, 'db-config.json')

# Load the database connection details from db-config.json
print("Loading config file...")
with open(config_path, 'r') as config_file:
    config = json.load(config_file)
print("Config file loaded successfully!")

# Database connection
print("Starting database connection...")
conn = mysql.connector.connect(
    host=config['host'],
    user=config['user'],
    password=config['password'],
    database=config['database'],
    port=config['port']
)
print("Connected to the database!")

cursor = conn.cursor()

# Load the JSON data
print("Loading JSON data...")
json_path = os.path.join(base_dir, 'data', 'recent_indies.json')
with open(json_path, 'r') as json_file:
    data = json.load(json_file)
print(f"JSON file loaded: {len(data)} records found.")

insert_query = """
    INSERT INTO indie_games (appid, name, release_date, genres, tags, pct_pos_total, pct_pos_recent, 
    windows, mac, linux, metacritic_score, categories, estimated_owners, ccu, price)
    VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s);
"""

# Iterate over the data and insert each entry into the database
count = 0  # To count the number of inserted records
for appid, game in data.items():
    # Safely parse genres and tags, and convert booleans
    genres = ', '.join(ast.literal_eval(game['genres']))
    tags = ', '.join(ast.literal_eval(game['tags']).keys())
    windows = True if game['windows'] == "True" else False
    mac = True if game['mac'] == "True" else False
    linux = True if game['linux'] == "True" else False

    # Prepare data
    game_data = (
        appid,
        game['name'],
        game['release_date'],
        genres,
        tags,
        game['pct_pos_total'],
        game['pct_pos_recent'],
        windows,
        mac,
        linux,
        game['metacritic_score'],
        ', '.join(ast.literal_eval(game['categories'])),
        game['estimated_owners'],
        game['ccu'],
        game['price']
    )
    
    cursor.execute(insert_query, game_data)
    count += 1
    if count % 100 == 0:  # Print every 100 records
        print(f"Inserted {count} records...")

# Commit the transaction
conn.commit()
print(f"All {count} records inserted successfully!")

# Verify the data insertion
print("Verifying data insertion...")
cursor.execute("SELECT * FROM indie_games LIMIT 5;")
print("Sample data from the database:", cursor.fetchall())

# Close the cursor and connection
cursor.close()
conn.close()
print("Database connection closed.")
