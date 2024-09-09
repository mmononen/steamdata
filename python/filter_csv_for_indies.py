# this script converts huge Steam games database in csv to a smaller indie games (released on and after 2020-01-01) set

import csv
import json

def convert_csv(filename, target_dict):
    ''' Convert CSV file to a dict.'''
    with open(filename, encoding = 'utf-8') as datafile:
        data = csv.DictReader(datafile)
        for row in data:
            if row['release_date'] >= '2020-01-01':
                if 'Indie' in row['tags']:
                    key = row['AppID']
                    game = {
                        'name' : row['name'],
                        'release_date' : row['release_date'],
                        'genres' : row['genres'],
                        'tags' : row['tags'],
                        'pct_pos_total' : row['pct_pos_total'],
                        'pct_pos_recent' : row['pct_pos_recent'],
                        'windows' : row['windows'],
                        'mac' : row['mac'],
                        'linux' : row['linux'],
                        'metacritic_score' : row['metacritic_score'],
                        'categories' : row['categories'],
                        'estimated_owners' : row['estimated_owners'],
                        'ccu' : row['peak_ccu'],
                        'price' : row['price']
                    }
                    target_dict[key] = game
    return target_dict

def write_json_database(data, filename):
    ''' Write a JSON database from a dict. '''
    with open(filename, "w") as datafile:
        json.dump(data, datafile)

if __name__ == "__main__":
    filtered_data = {}
    filtered_data = convert_csv('games_may2024_cleaned.csv', filtered_data)
    write_json_database(filtered_data, 'recent_indies.json')
