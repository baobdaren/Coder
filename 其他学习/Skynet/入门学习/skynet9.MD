# 九 socket 网络服务

## 相关API介绍

```lua
local socket = require "skynet.socket"

-- 建立socket连接，返回id
local socketid = socket.open(address, port)

-- 阻塞等待所有coroutin读完后，关闭这个连接
socket.close(socketid)

-- 强制关闭
socket.close_fd(socketid)

-- 强制关闭，在_gc原方法中使用比close更好（gc时无法切换coroutine）。于close_fd类似
socket.shutdown(socketid)

-- 从socket上读取指定个字节的数据，返回读取的字符串
-- 如果长度不足，则返回false+读取到的字符串
-- 如果sz为nil，则阻塞读取，至少一个字节再返回。
socket.read(id,sz)

-- 从一个socket上读取所有数据，直到主动断开或其他coroutine用close关闭
socket.readall(id)

-- 从一个socket上读取一段数据，分隔符为sep，缺省值为“\n”。读取结果不含风格
-- 引用：如果另外一端就关闭了，那么这个时候会返回一个nil
-- 如果buffer中有未读数据则作为第二个返回值返回。
socket.readline(id,sep)

-- 等待一个socket可读
socket.block(id)

-- 把一个字符串添加到写入队列，框架会在可写时写入
socket.write(id, str)

-- 将字符串写入一个低优先级队列。
-- skynet优先发送正常队列，发送完毕后再处理低优先级队列，发送操作不会被打断（类似原子操作）
socket.lwrite(id, str)

-- 监听地址上的端口，返回id 供start使用
socket.listen(address, port)

-- acceptcallback(id, address) 是一个函数，每当id对应的socket上有新的连接接入时？就会调用该函数
-- 该函数可以获得socket的id和接入连接的地址
-- 手册上说：每当 accept 函数获得一个新的 socket id 后，并不会立即收到这个 socket 上的数据。
-- 这是因为，我们有时会希望把这个 socket 的操作权转让给别的服务去处理。accept(id, addr)？？？
socket.start(id, acceptcallback)

-- 任何服务在给一个socket读/写数据之前，必须调用start(id)函数
-- skynet会将socket的数据依据调用该api的位置发送的
-- socket的id在该节点里都可以使用，就是说你可以发送给别的服务取使用
socket.start(id)

-- 清除本服务内对该id对应的socket的数据结构。（不会关闭socket）
-- 比如说当你想把这个socket转交给其他服务处理时，将id发送给别的服务，然后自己再移除自己对其的“保存”

socket.abandon(id)

-- 设定一个函数warncallback回调，当id对应的socket上数据超过1M时，调用回调。  
-- 如果不对socket设置这个函数，则skynet会再每此增加64K大小时，使用skynet.error()发送一条错误信息
socket.warning(sz, warncallback)
```

## 9.1 skynet TCP监听端

> 注意，socket都需要start之后才能读写，这包括监听时连接上来的socket。  

```lua
local skynet = require "skynet"
local socket = require("skynet.socket")

local self = {}

skynet.start(function ()
    local address = "172.16.214.62"
    skynet.error("监听 ", address)
    local socketid = socket.listen(address, 8888)
    assert(socketid)
    socket.start(socketid, self.acceptfunc) -- 设置该id的socket在有连接时的回调（参数1：接入socketid，参数2：接入socket的地址IP）
end)

function self.acceptfunc(connsocketid, connectaddress)
    skynet.error("有新的连接 id=", connsocketid, " ip=", connectaddress)
    skynet.fork(function () -- 开启新的协程来处理这个连接
        socket.start(connsocketid)
        while true do
            local msg = socket.read(connsocketid)
            if msg then
                skynet.error("server 对方发送了", msg)
                socket.write(connsocketid, string.upper(msg))
            else
                socket.close(connsocketid)
                skynet.error("连接断开", connectaddress)
                return
            end
        end
    end)
end
```

该服务可以把发来的字母全部修改为大写后返回回去。  

