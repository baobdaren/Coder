# 七 服务间消息通信

skynet的每个服务都运行在一个独立的LUA虚拟机上，所以它们之间的通信不能使用传统意义上的全局变量。  
再skynet中服务之间的消息可以通过skynet的消息调度机制来完成通信，skynet中的服务是基于Actor模型设计出来的，每个服务都可以发送消息，处理消息和回应消息。  
每条skynet消息由6部分组成：消息类型，session，发送消息服务地址，接收消息服务地址，消息C指针，消息长度。  

## 消息类型

再skynet中消息分为不同类别，对应也有不同的编码方式。消息类型的宏定义在skynet.h中。  

```c
#define PTYPE_TEXT 0
#define PTYPE_RESPONSE 1    //表示一个回应包
#define PTYPE_MULTICAST 2   //广播消息
#define PTYPE_CLIENT 3      //用来处理网络客户端的请求消息
#define PTYPE_SYSTEM 4      //系统消息
#define PTYPE_HARBOR 5      //跨节点消息
#define PTYPE_SOCKET 6    //套接字消息
#define PTYPE_ERROR 7     //错误消息，一般服务退出的时候会发送error消息给关联的服务
#define PTYPE_QUEUE 8
#define PTYPE_DEBUG 9
#define PTYPE_LUA 10   //lua类型的消息，最常用
#define PTYPE_SNAX 11  //snax服务消息
​
#define PTYPE_TAG_DONTCOPY 0x10000
#define PTYPE_TAG_ALLOCSESSION 0x20000
```

上面的消息有多种，但是最常使用的是PTYPE_LUA，对应到Lua层，叫做lua消息，大部分服务端一般使用这种消息。默认的PTYPE_REPSONSE、PTYPE_ERROR、PTYPE_LUA，这三种消息类型已注册，其他的需要自己显示注册。  

## 7.2 注册消息处理函数

当我们需要在我们的服务中监听某个类型的消息，那么我们首先必须在服务启动时注册消息监听函数，通常在入口函数skynet.start中注册，使用接口skynet.dispatch()来注册：

## 7.3 消息的打包和解包

skynet的消息发送前必须进行打包，接收到消息后也必须进行解包才能使用。通常使用打包函数`pack`和解包函数`unpack`。  

`skynet.pack(...)`：打包后，返回指向消息数据的C指针和数据包长度。消息数据内存是动态申请的。

`skynet.unpack(msg,sz)`：解包后，返回一个参数列表。注意这个时候消息数据的内存不会被释放。如果指向消息的指针有实际用途，skynet会在适合的地方释放，如果没有也可以使用`skynet.trash(msg,sz)`显示释放。  

pack函数返回的指针和长度不是实际长度。  

## 7.4 发送消息的方法

### 7.4.1 发送无需响应的消息

```lua
-- 发送未打包的type类型消息，干函数自动把...打包，默认使用skynet.pack。第一个参数可以是别名或句柄
skynet.send(name/handle, type, ...)

-- 发送已经打包好的type类型的消息，第一个参数可以是别名或句柄
skynet.rawsend(name/handle, type, msg, sz)
```

* 这是接收任务

```lua
-- test7.lua
local skynet = require "skynet"
require "skynet.manager"

skynet.start(function ()
    skynet.register(".ser")  -- 当前服务别名

    skynet.timeout(100,function () -- 启动另外一个发送消息的服务
        skynet.error("准备启动 test7_2")
        skynet.newservice("test7_2")
    end)

    skynet.dispatch("lua",function (sesssion, address, ...) -- 注册接收当前服的所有lua类型的消息
        skynet.error("接收到新的消息 ==========")
        local msg = {...}
        for _k,_v in pairs(msg) do
            skynet.error(_k,":",_v)
        end
        skynet.error("接收到新的消息 ===== over")
    end)
end)
```

* 这是发送任务

```lua
-- test7_2.lua
local skynet = require "skynet"
require "skynet.manager"

skynet.start(function ()
    local r = skynet.send(".ser","lua" , "这是未打包消息。", "消息内容：一首交货")
    skynet.error("发送完毕1" , r)

    local packedmsg, sz = skynet.pack("打包消息", "消息内容，一手交钱")
    local r = skynet.rawsend(".ser","lua",packedmsg, sz)
    skynet.error("发送完毕2" , r)
end)
```

* 运行结果

```lua
-- [:01000009] 准备启动 test7_2
-- [:0100000a] LAUNCH snlua test7_2
-- [:0100000a] 发送完毕1 0
-- [:0100000a] 发送完毕2 0
-- [:01000009] 接收到新的消息 ==========
-- [:01000009] 1 : 这是未打包消息。
-- [:01000009] 2 : 消息内容：一首交货
-- [:01000009] 接收到新的消息 ===== over
-- [:01000009] 接收到新的消息 ==========
-- [:01000009] 1 : 打包消息
-- [:01000009] 2 : 消息内容，一手交钱
-- [:01000009] 接收到新的消息 ===== over
```

### 7.4.2 发送必须响应的消息

```lua
-- 向addr发送type类型消息，并对回应消息自动解包。自动打包和解包
skynet.call(address/handle, type, ...)
-- 向addr发送type类型消息，回应消息需要自己解包，自己打包和解包
skynet.rawcall(address/handle, type, msg, sz)
```

* 例如

```lua
-- test7_3.lua
local skynet = require "skynet"
require "skynet.manager"

skynet.start(function ()
    local responsemsg = skynet.call(".ser", "lua", "你收到了吗？", "false")
    skynet.error("我问了对方收到了没，他回答说 ", responsemsg)
end)
```

## 7.5 响应消息的方法

对lua消息响应的时候，使用的是PTYPE_RESPONSE这种消息，也需要打包，打包的时候必须与接收到的消息打包方法一致。  

```lua
skyner.ret() -- 目标服务消息处理后，通过该函数回应。
skynet.retpack() -- 将消息用skynet.pack打包后，用ret发送。
```

* 示例函数

```lua
-- test7.lua
local skynet = require "skynet"
require "skynet.manager"

skynet.start(function ()
    skynet.register(".ser")  -- 当前服务别名

    skynet.timeout(100,function () -- 启动另外一个发送消息的服务
        skynet.error("准备启动 test7_3")
        skynet.newservice("test7_3")
    end)

    skynet.dispatch("lua",function (sesssion, address, ...) -- 注册接收当前服的所有lua类型的消息
        skynet.error("接收到新的消息 ==========")
        local msg = {...}
        for _k,_v in pairs(msg) do
            skynet.error(_k,":",_v)
        end
        skynet.sleep(300)
        skynet.retpack("我收到你的的消息", true)
        skynet.error("接收到新的消息 ===== over")
    end)
end)
```

注意：

1. 回应消息打包方法和接收消息打包方法必须一致
2. 回应消息时无需指定回应给哪个服务。（内部已绑定了协程和源服务地址）

## 7.8 使用skynet.response回应消息

在回应消息的方法skynet.ret/retpack使用时，必须保证接收消息的过程和回应消息的过程必须在同一协程内。否则会失败，此时可以使用skynet.response回应。  

* 回应方

```lua
-- test7_7.lua
local skynet = require "skynet"
require "skynet.manager"

skynet.start(function ()
    skynet.register(".ser")  -- 当前服务别名

    skynet.timeout(100,function () -- 启动另外一个发送消息的服务
        skynet.error("准备启动 test7_8")
        skynet.newservice("test7_8")
    end)

    skynet.dispatch("lua",function (...) -- 注册接收当前服的所有lua类型的消息
        local msg = {...}
        local rsp = skynet.response(skynet.pack)    -- rsp必须在接收消息的协程获取
        skynet.fork(ResponseFunc, msg, rsp) -- 在非接收消息的协程进行回应
    end)
end)

function ResponseFunc(msg, rsp)
    -- local rsp = skynet.response(skynet.pack) -- 注意，rsp不能在这里获取
    skynet.error("接收到新的消息 ==========")
    for _k,_v in pairs(msg) do
        skynet.error(_k,":",_v)
    end

    local sendersession = msg[1]
    local senderaddress = msg[2]
    skynet.sleep(100)
    skynet.kill(".call")

    -- local r = {rsp(true, {"收到了", "第二消息"})}    -- rsp 只能接收两个参数
    local r = {rsp(false, "错误消息")}    -- rsp 只能接收两个参数

    -- local r = {rsp("test", "收到了", "这是test")}    -- rsp 只能接收两个参数
    skynet.error("接收到新的消息 ===== over", #r, ":" ,r[1])
end
```

* 调用方

```lua
-- test7_8.lua
local skynet = require "skynet"
require "skynet.manager"

skynet.start(function ()
    skynet.register(".call")

    skynet.fork(function ()
        local rspmsgs = {skynet.call(".ser", "lua", "你收到了吗？", "巴拉巴拉小魔仙")}
        skynet.error("我问了对方收到了没，他回答说 ", rspmsgs[1], rspmsgs[2])
    end)
end)
```

* 运行结果（这里举例了我们杀掉调用方，导致调用方无法接收回应。）

```lua
-- [:01000009] 接收到新的消息 ==========
-- [:01000009] 1 : 2
-- [:01000009] 2 : 16777226
-- [:01000009] 3 : 你收到了吗？
-- [:01000009] 4 : 巴拉巴拉小魔仙
-- [:01000009] KILL :100000a
-- [:01000009] 接收到新的消息 ===== over 1 : false
```

> local rsp = skynet.response([pack]) 获取回应对象，参数为打包方式，默认skynet.pack。rsp必须在接收对象的协程获取。  
> local succed = rsp("test"/true/false, msg1, msg2, ...) 发送方法。  
> 第一参数分别代表（"test"）：测试接收方是否存在。？？？  
> 第一参数分别代表（true）：回应正确的回应消息。  
> 第一参数分别代表（"test"）：回应一个错误，这会抛出错误。  
> rsp接收方接收到时，rsp返回值为true。  
> skynet.call：返回值回应的消息，可能多个。  

## 7.9 skynet.call 失败

有时，回应协程异常终止，没有回应成功，则会自动给未答复的请求一个error消息。  

## 7.10 服务重入问题

服务的回应，并不能保证时序。先进入的消息也可能后退出。并不会按照访问的时序完成回应。  

## 7.11 服务临界区

> 这里曾被卡住好久：主要是skynet.timeout(ti,func)，func通过闭包的方式传参，没有三参数重载。

使用skynet.queue 模块避免服务冲入和伪并发引起的问题  

* 处理方

```lua
--- test.lua

local skynet = require "skynet"
require "skynet.manager"

local  queue = require "skynet.queue"
local cs = queue()

skynet.start(function ()
    skynet.register(".queuetest")
    skynet.dispatch("lua",function (session, address, ...)
        local msg = {...}
        skynet.error("接收到请求", msg[2])
        skynet.retpack(cs(function ()
            skynet.error("处理 开始", msg[2], " 耗时：", msg[1])
            skynet.sleep(msg[1])
            skynet.error("处理 完毕", msg[2], "END-")
        end))
    end)

    skynet.timeout(10,function ()
        skynet.newservice("test7_11call")
    end)
end)
```

* 调用方

```lua
-- test7_11call.lua
local skynet = require "skynet"
skynet.start(function ()
    task(10, "第1条消息", 500)
    task(20, "第2条消息", 80)
    task(30, "第3条消息", 200)
    task(40, "第4条消息", 50)
    task(50, "第5条消息", 10)
end)

-- ti：这个时间后才发送该请求
--- useti：处理这个任务需要的时间
function task(ti, con, useti)
    skynet.timeout(ti,function ()
        local rsp = skynet.call(".queuetest", "lua", useti, con)
    end)
    skynet.error("调用方：", con, "已发送")
end


```

* 运行结果

```lua
-- [:0100000a] 调用方： 第1条消息 已发送
-- [:0100000a] 调用方： 第2条消息 已发送
-- [:0100000a] 调用方： 第3条消息 已发送
-- [:0100000a] 调用方： 第4条消息 已发送
-- [:0100000a] 调用方： 第5条消息 已发送
-- [:01000009] 接收到请求 第1条消息
-- [:01000009] 处理 开始 第1条消息  耗时： 500
-- [:01000009] 接收到请求 第2条消息
-- [:01000009] 接收到请求 第3条消息
-- [:01000009] 接收到请求 第4条消息
-- [:01000009] 接收到请求 第5条消息
-- [:01000009] 处理 完毕 第1条消息 END-
-- [:01000009] 处理 开始 第2条消息  耗时： 80
-- [:01000009] 处理 完毕 第2条消息 END-
-- [:01000009] 处理 开始 第3条消息  耗时： 200
-- [:01000009] 处理 完毕 第3条消息 END-
-- [:01000009] 处理 开始 第4条消息  耗时： 50
-- [:01000009] 处理 完毕 第4条消息 END-
-- [:01000009] 处理 开始 第5条消息  耗时： 10
-- [:01000009] 处理 完毕 第5条消息 END-
```

> 由结果可以看出，即使先接收到的任务总是被先处理。（先接收到，严格意义来说是先调用skynet.retpack(f,...)的任务。）  
> 这里要注意，queue的执行顺序是先调用到queue实例的任务。  
> 从源码来看，主要是通过wait协程和wakeup来实现的。  

## 7.12 注册其他消息

忽略

## 7.13 代理服务

简而言之就是发送的消息，不直接发给接收方，而是发给一个中间方，回应时也通过代理回应。

## 7.14 伪造消息

模仿其他服务发送一个消息，可以使用skynet.redirect

## 7.15 节点间消息

节点间消息传递

```lua
local globalmsg = harbor.queryname("name")
local r = globalmsg.call(globalmsg,"lua", content)
```

