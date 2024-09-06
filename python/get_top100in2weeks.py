import json
import requests

top100 = requests.get("https://steamspy.com/api.php?request=top100in2weeks")
data = top100.json()
with open("steamspy_top100.json", "w") as datafile:
    json.dump(data, datafile)