# 十二 snax框架

snax框架方便skynet实现的框架（相对skynet的api而言）

使用该框架必须要在config文件种配置snax用于路径查找。每个snax服务都有一个用于启动服务的名字。推荐按lua的模块命名规则，但目前不要使用“.”。启动服务时会按路径搜索文件。

## 12.1 snax基础API

* 启动snax服务的API

```lua
-- 启动一个服务（可以多份），name为服务名，返回一控制对象，用于和服务交互
-- 多次调用会启动多个实例
-- 多个实例彼此独立，由控制对象区分
-- 返回值是对象，不是服务地址
local ctrl = snax.newservice(name, ...)

-- 和上面一样，一个节点上只会启动一份。多次调用只会返回相同的对象
local ctrl2 = snax.uniqueservice(name, ...)

-- 一样，全局唯一服务
local c3 = snax.globalservice(name, ...)
```

* 查询snax服务

```lua
-- 查询一个当前节点的具体服务，返回控制对象。如未启动，则阻塞等待
r = snax.queryservice(name)

-- 同上，查询全局节点的
r = snax.queryglobal(name)

-- 获取自己的服务的控制对象
r = snax.self()
```

* snax服务退出

```lua
-- 让一个snax服务退出
snax.kill(r)

-- 退出当前 相当于snax.kill(snax.self(),...)
snax.exit()
```

* 启动路径config-snax项配置

snax = root.."examples/?.lua;"..root.."test/?.lua;".."my_workspace/?.lua" --添加my_workspace路径

## 简单的snax服务

snax服务的启动需要使用snax.newservice/globalservice/unqueservice来启动。snax服务需要snax框架配合。  

```lua

```

## snax服务请求

```lua
local ssctrl = snax.queryservice("name")
-- cmd是具体的命令（就是对方的方法），三点是参数
-- post是无响应请求，不阻塞。post请求的方法，必须在accept下
ssctrl.post.CMD(...)
-- req是响应请求，阻塞等待。req请求的方法，必须在response下
ssctrl.req.CMD(...)
```

## snzx处理请求

```lua
-- test12_1
local snax = require "skynet.snax"
local skynet =require "skynet"

skynet.start(function ()
    local sserctrl = snax.newservice("test12_2", 1,"12_2上线吧")
    skynet.error("snax service 已启动 - ", sserctrl)

    local r = sserctrl.post.letter("这是你的信件", "掌门爱你呦！")
    skynet.error("post.letter 的返回 ", r)

    local rs = sserctrl.req.chat("上线了吗？宝贝，我年龄", 25)
    skynet.error("我12_1收到消息", rs)

    sserctrl.post.quit("你可以下线了")
end)
```

```lua
-- test12_2

local snax = require "skynet.snax"
local skynet =require "skynet"

function init(...)
    local args = {...}
    skynet.error("12_2开始启动，参数：", ...)
end

function exit(...)
    skynet.error("12_2退出 参数", ...)
end

-- accept 的接收消息，无需返回
function accept.letter(...)
    skynet.error("12_2收到信件 ", ...)
    return "12_2尝试返回"
end

-- accept 的接收消息，无需返回
function accept.quit(...)
    skynet.error("12_2接到下线命令", ...)
    snax.exit() -- 这里需要显式调用
end

-- response 下的应答消息，需要返回

function response.chat(...)
    skynet.error("12_2收到消息：", ...)
    skynet.sleep(300)
    return "我收到了".."我年龄23"
end
```

> 这里要注意的是，请求（req/post）的方法必须包含在特定table下（accept/response）。

## snax 热更

snax支持热更（skynet不支持），方式如下：

```lua
snax.hotfix(obj, patchcode) -- 向obj提交一个patch
```

* 热更功能

```lua
-- test12_3

local skynet = require "skynet"
local snax = require "skynet.snax"

skynet.start(function ()
    local snaxserctrl = snax.newservice("test12_4")
    skynet.error("第一次调用", snaxserctrl)
    snaxserctrl.post.hot()

    -- 不映射局部变量时，skynet和lnum都访问不到
    -- snax.hotfix(snaxserctrl, [[
    --     function accept.hot(arg1)
    --         print("变态盗版", lnum, gnum)
    --     end
    -- ]])

    -- 映射呢之后就可以访问到了
    snax.hotfix(snaxserctrl, [[
        local skynet
        local lnum
        function accept.hot(arg1)
            skynet.error("变态盗版", lnum, gnum)
        end
    ]])

    skynet.error("第2次调用")
    snaxserctrl.post.hot()
end)
```

* 需要热更的代码

```lua
-- test12_4
local skynet = require "skynet"
local snax = require "skynet.snax"

local lnum = 10
gnum = 11

function accept.hot(arg1)
    skynet.error("原版 hot", lnum, gnum)
end

function init(...)
    skynet.error("12_4 已经启动了")
end

function exit(...)
    skynet.error("12_4 退出")
end
```

## 修改snax线上状态

比如增加一个版本要更新的提示

```lua
-- moni game -- test12_6
local skynet = require "skynet"
local snax = require "skynet.snax"

local gametips = ""

function init(...)
    skynet.error("game starting ...")
    gametips = "服务器已经启动了"
    skynet.timeout(50, gamemain)
end

function gamemain()
    skynet.timeout(50, gamemain)
    if #gametips <= 2 then
        return
    end
    skynet.error(gametips)
    gametips = SubUTF8String(gametips,#gametips-1)
end

function accept.settips(newtips)
    gametips = newtips
end

function SubUTF8String(s, n)
    local dropping = string.byte(s, n+1)
    if not dropping then return s end
    if dropping >= 128 and dropping < 192 then
        return SubUTF8String(s, n-1)
    end
    return string.sub(s, 1, n)
end
```

```lua
local skynet = require "skynet"
local snax = require "skynet.snax"

local gamesnaxserctrl = nil

skynet.start(function ()
    gamesnaxserctrl = snax.newservice("test12_6")
    -- skynet.timeout(500, function ()
    --     gamesnaxserctrl.post.settips("准备版本更新")
    -- end)
    skynet.timeout(500, function ()
        skynet.error("游戏在运行？ = ", gamesnaxserctrl ~= nil)
        local hr = snax.hotfix(gamesnaxserctrl, [[
            local gametips
            local skynet
            function hotfix(...)
                gametips = "游戏准备热更，请保存你的数据"
            end
        ]])
        skynet.error("发热后=", hr)
    end)
end)
```
