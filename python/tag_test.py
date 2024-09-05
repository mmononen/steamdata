import json
import requests
import time

def get_game_data(app_id):
    ''' Use SteamSpy API to get app/game details by app_id. Return data as a JSON object. '''
    request_data = requests.get(f"https://steamspy.com/api.php?request=appdetails&appid={app_id}")
    return request_data.json()

def read_json_database(filename):
    ''' Read a JSON database from file and return a dict.'''
    with open(filename, "r") as datafile:
        return json.load(datafile)

def write_json_database(data, filename):
    ''' Write a JSON database from a dict. '''
    with open(filename, "w") as datafile:
        json.dump(data, datafile)

games = read_json_database('steamspy_top100.json')

for game in games:
        try:
            test_tags = games[game]['tags']
        except KeyError:
            games[game]['tags'] = get_game_data(game)['tags']
            write_json_database(games, 'steamspy_top100.json')
            time.sleep(2)
        print(games[game]['tags'])
