# 构建服务的基础API

```lua
local skynet = require("skynet")

-- 获取config注册环境变量
skynet.getenv(varName)

-- 设置一些环境变量，注意不要设置已经存在的环境变量
skynet.setenv(key,value)

-- 打印函数
skynet.error(exceptionMsg)

-- 同func函数初始化服务，并将消息处理函数注册到C层，让该服务可以工作。
skynet.start(start_func)

-- 服务尚未初始化时：将这个函数注册为初始化函数。
-- 服务已经初始化时：立即执行这个函数。
skynet.init(func)

-- 结束当前服务
skynet.exit()

-- 获取当前服务的handle
skynet.self()

-- adde 就是skynet.self() ，这个函数将其转换成字符串
skynet.address(addr)

-- 退出skynet进程
-- 需要：require("skynet.manager")
skynet.abort()

-- address = skynet.address() 
-- 杀掉这个服务
-- 可以强制结束其他服务，但这样不安全，更安全的做法是：
-- 以通知的形式告知需要结束的服务，让其自己处理，然后自行调用skynet.exit()。
-- 否则该服务处理完一条消息后会毫无征兆地退出。
-- skynet.kill(skynet.self()) 和 skynet.exit()相比，后者更安全。
skynet.kill(address)
```

## 1. 编写一个最简单的服务

```lua
-- examples/test.lua
-- 创建一个skynet服务
local skynet = require "skynet"

-- 调用skynet.start，并传入回调函数
skynet.start(function ()
    skynet.error("call back yes ✌---你好啊刘运宁")
end)
```

config文件里只要把主函数改成test就可以了。从测试来看，查找的目录包含了examples文件夹，下面说。

写好上面的代码后：`./skynet examples/config`  
该命令必须再skynet项目下执行，config文件会获取信息。  
运行结果就是输出了err里的内容。  

## 2. 添加自己的lua脚本路径
  
1. 修改config文件中变量（luaservice），追加一个文件夹的路径。例如“lyndemo”；（luaservice的值更像是一段lua代码，这样容易理解分号，lua执行了这段值才会得到路径列表，分号是分隔符），其他内容与原版一样。然后放一份到lyndemo下。
2. 把自己的脚本写在这个文件夹下：./lyndemo/test.lua
3. 还需要一个文件：examples/config.path，放一份到我们的lyndemo下。

这一次我们这样运行：`./skynet ./lyndemo/config`  

>还是路径问题，注意这是一段lua代码。主要是以连接字符串为主；

## 3. 另外一种起服方式 —— 据说现在不用了

## 4. 设置环境变量

这里是指config中的哪些变量  
有一些特性：  

1. 预加载的环境变量是在config中设置，加载完成后所有的service都能访问。
2. 可以添加新的环境变量，但不能修改已经存在的。`skynet.setenv(n,v)`
3. 环境变量设置完成后，该节点的服务都能访问。`skynet.getenv(n)`
4. 设置环境变量后，即使服务退出，变量依然存在。

## 5. skynet init的使用

```lua
skynet.init(function() ... end)
```

`skynet.init`用以注册一个在`skynet.start`函数之前运行的函数。
