# 限制名称，大小写字母数字下划线开头，长度为6-18

import re
pattern = "^[A-Za-z0-9_]{6,18}$"
Names = [
    "Lynyangyang21",
    "_lynyangyang21",
    "111",
    "*cjkdls"
    ]

for fName in Names:
    r = re.match(pattern, fName)
    if r:
        print(r.string)
# monster_1101.json
# monster_1102.json
# monster_3101.json