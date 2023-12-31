# 八 Multicast 组播

## 介绍

引用模块`skynet.multicast`则可以使用skynet的组播方案。可以自由创建一个频道，向其中投递任意详细，订阅者则可以接收。  

你可以使用`local channel = skynet.multicast.new`创建一个新的频道，也可以提供已创建的频道的id来绑定这个频道（获得发送权）。  

通常由一个服务创建频道，然后通知其他服务该id对象，其他服务则使用该id绑定。  
`channel.dispatch = func(session, address, ...)`：频道处理函数。  
`channel:publish(...)`：向一个频道发送消息，消息可以是任意的合法lua值。  
`channel:subscribe()`：绑定一个频道后，默认是不接收消息的（不订阅？？？）（也许你只想发送消息）。则使用该方法订阅它。  
`channel:unsubscribe()`：取消订阅。  
`channel:delete()`：删除频道。发布和订阅已删除的频道不会抛出异常，也不会接收到消息。channel的id永不复用，所以多次调用不会由问题。  

> 在使用时注意，首次创建频道时，暂时没有注意到有绑定dispatch处理函数的方式，赋值也不行。所以，要添加dispatch函数，需要再new，然后在参数列表中赋值dispatch。  

## 案例

下面是自己写的一个简单的房间案例，实现了玩家利用组播获取游戏开始信息。和实际使用不同，我们主要示例广播能力。  

* 房间服务

```lua
-- 组播
local this = {}
local  skynet = require "skynet"
local multicast = require "skynet.multicast"
local homechannel = nil  -- 房间的频道

require "skynet.manager"

skynet.start(function ()
    skynet.register(".home")  -- 给房间服务注册别名

    skynet.dispatch("lua", function(session, address, ask, args)
        if ask == "getchannel" then -- 返回对房间服务组播频道id的请求
            skynet.retpack(this.getchannel())
        end
    end)

    homechannel = multicast.new()  -- 创建频道实例

    skynet.timeout(10,function () -- 创建一个玩家
        skynet.newservice("test8_1", "疾风豪")
    end)
    skynet.timeout(20,function () -- 创建一个玩家
        skynet.newservice("test8_1", "泽拉斯")
    end)
    skynet.timeout(90,function () -- 创建一个玩家
        skynet.newservice("test8_1", "风之子")
    end)

    this.gamestart(3) -- 每秒发送倒计时
end)

function this.getchannel()
    return homechannel.channel
end

function this.gamestart(lt)
    if lt < 0 then
        return
    end
    skynet.timeout(100,function ()
        homechannel:publish("还有".. tostring(lt) .."S 开始游戏")
        this.gamestart(lt-1)
    end)
end
```

* 玩家服务

```lua
local skynet = require "skynet"
local mc = require "compat10.multicast"
require "skynet.manager"
local playername = ...

skynet.start(function ()
    local s = skynet.localname(".home") -- 获取房间服务 地址
    local r = {skynet.call(s,"lua","getchannel")} -- 请求房间频道的id
    local channelid = r[1]

    local channel = mc.new{
        channel = channelid,
        dispatch = function (add, sess, msg, ...)
            skynet.error(playername ,"接收到消息-",msg)
        end
    }
    channel:subscribe()
    channel:publish(playername .. "进入了房间")
end)
```

* 运行结果

```lua
-- [:01000002] KILL self
-- [:0100000b] LAUNCH snlua test8_1 疾风豪
-- [:0100000b] 疾风豪 接收到消息- 疾风豪进入了房间
-- [:0100000c] LAUNCH snlua test8_1 泽拉斯
-- [:0100000c] 泽拉斯 接收到消息- 泽拉斯进入了房间
-- [:0100000b] 疾风豪 接收到消息- 泽拉斯进入了房间
-- [:0100000d] LAUNCH snlua test8_1 风之子
-- [:0100000d] 风之子 接收到消息- 风之子进入了房间
-- [:0100000c] 泽拉斯 接收到消息- 风之子进入了房间
-- [:0100000b] 疾风豪 接收到消息- 风之子进入了房间
-- [:0100000c] 泽拉斯 接收到消息- 还有3S 开始游戏
-- [:0100000d] 风之子 接收到消息- 还有3S 开始游戏
-- [:0100000b] 疾风豪 接收到消息- 还有3S 开始游戏
-- [:0100000c] 泽拉斯 接收到消息- 还有2S 开始游戏
-- [:0100000d] 风之子 接收到消息- 还有2S 开始游戏
-- [:0100000b] 疾风豪 接收到消息- 还有2S 开始游戏
-- [:0100000c] 泽拉斯 接收到消息- 还有1S 开始游戏
-- [:0100000d] 风之子 接收到消息- 还有1S 开始游戏
-- [:0100000b] 疾风豪 接收到消息- 还有1S 开始游戏
-- [:0100000c] 泽拉斯 接收到消息- 还有0S 开始游戏
-- [:0100000d] 风之子 接收到消息- 还有0S 开始游戏
-- [:0100000b] 疾风豪 接收到消息- 还有0S 开始游戏
```

可以看到所有订阅者都可以接收到其他服务发布的消息。  

多节点下需要自己实现组播🙂。  
