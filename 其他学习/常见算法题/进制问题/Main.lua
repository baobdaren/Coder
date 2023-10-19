local num = 78

local jinZhi = 8

local r = {}

local function trans(m, n)
    if m == 0 then
        return
    end
    local tmp = m%n
    trans(math.modf(m/n), n)
    r[#r+1] = tmp
    print(#r)
end

print("!!!")

trans(num, jinZhi)

for i=1,#r do
    print(r[i])
end