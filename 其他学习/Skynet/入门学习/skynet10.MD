# 十 socketChannel

在与外部服务交互时，请求回应模式是最常用的模式之一。一般有两种设计方法。

1. 每个请求包对应一个回应包，有TCP保持时序。
2. 每个请求带上唯一的session标识，回应时带上这个标识。这样设计，可以不要求每个请求都有回应，而且无需遵循“先请求先回应”的规则。

用skynet的API实现相对复杂，提供了socketchannel。

## 10.1 第一种模式的 socketchannel

> 上一个请求的回应还没有收到的时候，下一个请求就没法发送。
> 注意这里response函数的要求

```lua
local skynet = require "skynet"
require "skynet.manager"
local sc = require "skynet.socketchannel"

local this = {}
local channel = nil

-- 这是个特殊的函数，参数skt有两个方法，read和readlin
-- 该函数的返回值必须是两个
-- 第一个参数：为true时表示解析正常；
-- 为false时表示协议出错，这会导致连接断开且request的调用者也会获得一个error
-- 该函数内的任何error都会抛给request的调用者
function this.reponse(skt)
    return true, skt:read()
end

function this.task()
    local i = 0
    while i <= 3 do
        --第一参数是需要发送的请求，第二个参数是一个函数，用来接收响应的数据。
        --调用channel:request会自动连接指定的TCP服务，并且发送请求消息。
        --该函数阻塞，返回读到的内容。
        local resp = channel:request("channel request - "..i, this.reponse)
        skynet.error("收到回应", resp) -- 上一个请求的回应没有收到时，无法发送下一个请求
        i = i + 1
    end
    skynet.exit()
end

skynet.start(function ()
    skynet.timeout(100, function ()
        channel = sc.channel{
            host = "172.16.214.62",
            port = "8888",
        } -- socketchannel对象
        skynet.error(channel.__host)
        skynet.fork(this.task)
    end)
end)
```

## 10.2 第二种模式

```lua
local skynet = require "skynet"
require "skynet.manager"
local sc = require "skynet.socketchannel"

local self = {}

local channel = sc.channel{
    host = "115.29.240.135",
    port = "8888",
    response = self.dispatch
}

function self.dispatch(skt)
    skynet.error("dispatch")
    local rmsg,rend = skt:readline()
    local session = tonumber(string.sub(rmsg, 1, 1)) -- 从第一个字符开始，长度为1
    skynet.error("debug session =", session)
    return session, true, rmsg -- 模式2需返回三个参数，新增第一个参数是个session
end



function self.sendtask()
    local i = 0
    while i <= 3 do
        skynet.fork(function (session) -- 开启协程发送，携带session
            skynet.error("client send")
            local resp = channel:request(session.."channel request".."\n", session)
            skynet.error("cleint收到回应", resp, session)
        end, i)
        i = i + 1
    end
end

skynet.start(function ()
    skynet.timeout(40, function ()
        skynet.error(channel.__host)
        skynet.fork(self.sendtask)
    end)
end)
```

> 第二种模式倒是没理解，我猜是：发送时不等回应，而一次发送多个请求，然后又收到多个请求，最后以session对应请求和回应。

## 客户端代码

```lua
local skynet = require "skynet"
local socket = require("skynet.socket")


skynet.start(function ()
    local address = "172.16.214.62"
    skynet.error("监听 ", address)
    local socketid = socket.listen(address, 8888)
    assert(socketid)
    socket.start(socketid, Acceptfunc) -- 设置该id的socket在有连接时的回调（参数1：接入socketid，参数2：接入socket的地址IP）
    skynet.timeout(100, function ()
        skynet.newservice("test10_1")
        -- skynet.newservice("test10_2")
    end)
end)

function Acceptfunc(connsocketid, connectaddress)
    skynet.error("server 有新的连接 id=", connsocketid, " IP=", connectaddress)
    skynet.fork(function (cid, cadd) -- 开启新的协程来处理这个连接
        socket.start(cid)
        while true do
            skynet.error("server 等待读取...")
            local msg,endstr = socket.readline(cid)
            if msg then
                if msg:sub(1,1) == "2" then
                    skynet.error("sleep")
                    skynet.sleep(300)
                end
                skynet.error("server 收到：", msg)
                socket.write(cid, string.upper(msg).. "--ser\n")
            else
                socket.close(cid)
                skynet.error("连接断开", cadd)
                return
            end
        end
    end, connsocketid, connectaddress)
end

```