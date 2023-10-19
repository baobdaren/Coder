-- teams = {{"p1", "p2"}, {"p3","p4"}, {"p5","p6"}}

-- function work(d)
-- 	local t
-- 	if d == nil then
-- 		t = math.random(#teams)
-- 	else
-- 		t = math.random(#teams-1)
-- 	end
-- 	local p = math.random(#teams[t])

-- 	local name = teams[t][p]

-- 	teams[t][p] = teams[t][#teams[t]]
-- 	table.remove(teams[t], #teams[t])

-- 	if #teams[t] == 0 then
-- 		teams[t] = teams[#teams]
-- 		table.remove(teams, #teams)
-- 	end

-- 	local tmp = teams[t]
-- 	teams[t] = teams[#teams]
-- 	teams[#teams] = tmp

-- 	if d ~= nil then
-- 		return name
-- 	end
-- 	return name, work(1)
-- end

-- while #teams > 0 do
-- 	print(work())
-- end

-- 上面的代码并不能完成需求
-- 小组匹配机制 {p1, p2} {p3,p4} {p5,p6}进行匹配，小组内不能匹配
-- 主要问题是 可能出现一个前面的小组都匹配完成，剩下一个小组两人都没有匹配，导致只能组内匹配

-- 解决方案1，先随机所有组的一个玩家进行匹配（可能剩余一个，这样就直接选择一个不是同组的玩家匹配）
-- 剩下的偶数个组内只有一个玩家，然后对他们匹配
local teams = {{"p1", "p2"}, {"p3","p4"}, {"p5","p6"}, {"p7", "p8"}, {"p9", "p10"}}
local teams = {{"p1", "p2"}, {"p3","p4"}, {"p5","p6"}, {"p7", "p8"}, {"p9", "p10"}}
function work()
    local firtPlayers = {}
    local secondPlayers = {}
    for i = 1, #teams do
        local  index = math.random(2)
        firtPlayers[i] = teams[i][index]
        table.remove(teams[i], index)
        table.insert(secondPlayers, i, teams[i][1])
    end
    -- return firtPlayers, secondPlayers

    result = {}
    function randomPlayer(set)
        while #set > 1 do
            local a = math.random(#set)
            local b = math.random(#set-1)
            local tmp = {}
            tmp[1] = set[a]
            table.remove( set, a )
            tmp[2] = set[b]
            table.remove( set, b )
            result[#result+1] = {tmp[1], tmp[2]}
        end
    end
    randomPlayer(firtPlayers)
    randomPlayer(secondPlayers)

    return result
end

local a = work()

print("ds")