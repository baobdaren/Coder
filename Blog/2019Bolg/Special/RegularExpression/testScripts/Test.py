import re
pattern = "^monster_[0-9]{4}.json$"
fsName = [
    "monster_1101.json", 
    "monster_1102.json", 
    "monster_3101.json", 
    "monster_31012.json", 
    "test.py"
    ]

for fName in fsName:
    r = re.match(pattern, fName)
    if r:
        print(r.string)
# monster_1101.json
# monster_1102.json
# monster_3101.json