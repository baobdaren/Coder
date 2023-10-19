# 如下代码print函数执行几次

```lua
local n = 2
for i=1,n do
    n = 10
    print(i) -- 2次
end
```

for循环从进入的时候次数就确定了，不会每一次都去检查。
