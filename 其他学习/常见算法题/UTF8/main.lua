function SubUTF8String(s, n)
    local dropping = string.byte(s, n+1)
    if not dropping then return s end
    if dropping >= 128 and dropping < 192 then
        return SubUTF8String(s, n-1)
    end
    return string.sub(s, 1, n)
end

local s = "fucn你手机电池"
print(SubUTF8String(s, 2))