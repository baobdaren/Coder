# 六 服务调度

## 1. 服务调度

```lua
-- API介绍
-- 服务调度相关的API

local  skynet = require("skynet")

-- 让当前人物等待一段时间，时长为：0.01*num 秒
skynet.sleep(num)

-- 启动一个新的人物执行函数func，执行结束后返回其handle
-- 如果自己开启协程取执行，会打乱skynet的工作流程
skynet.fork(func, ...)

-- 让出当前的任务，随后继续执行（使得本服务内其他任务有机会执行）
skynet.yield()

-- 让出当前任务，不再执行，直到用wakeup唤醒
skynet.wait(token)

-- 唤醒一个wait/sleep，处于等待状态的任务
skynet.wakeup(token)

-- 设定一个定时触发函数func，事件为ti*0.01 秒
skynet.timeout(ti,func)

-- 返回当前进程启动时的时间（UTC）
skynet.starttime()

-- 返回当前进程启动后经过的时间（0.01）
skynet.now()

-- 返回通过starttime和now计算出当前utc
skynet.time()
```

## 使用案例

* 使用sleep休眠

```lua
local  skynet = require "skynet"

-- 测试sleep
local  time_s = 1
skynet.start(function ()
    skynet.error("sleep start")
    -- skynet.sleep(ti * 100,token)
    skynet.sleep(time_s * 100)
    skynet.error("sleep over")
end)

```

* 开启新线程

```lua
local  skynet = require "skynet"

-- 测试开启新线程
-- ?开启后会阻塞当前任务吗：不会
skynet.start(function ()
    skynet.error("main 开始fork了")
    -- skynet.fork(task(),...)
    skynet.fork(task)
    skynet.error("main 结束fork了")
end)

function task()
    local ti = 5
    skynet.error("我是task 我要睡觉秒=", ti)
    skynet.sleep(ti * 100)
    skynet.error("我是task 我要睡觉结束了")
end
```

* 当前服务让出

```lua
local  skynet = require "skynet"

-- 测试，让长时间执行的任务让出
-- 注意：sleep状态下的任务，没有占用资源

skynet.start(function ()
    skynet.fork(task, "T1")
    skynet.fork(task, "T2")
    skynet.error("主任务fork结束")
end)

function task(name)
    skynet.error(name, "开始")
    work(name)
    skynet.error(name, "结束")
end

function work(n)
    t=1000000000
    for i = 1, t-1 do
        if i >= 100000000 and i%100000000 == 0 then
            skynet.error(n, "让出：", i)
            skynet.yield()
        end
    end
end 
```

* 延时执行函数

```lua
local  skynet = require "skynet"

-- 测试延时执行函数
-- 延时执行函数怎么传递参数？lualib里延时函数没有三参数的重载，下面使用了闭包的方式。

skynet.start(function ()
    args = "这是参数"
    skynet.timeout(500,function ()
        skynet.error(args)
    end)
end)

```

* 时间获取

```lua
local  skynet = require "skynet"

-- 获取时间
skynet.start(function ()
    skynet.error("starttime", skynet.starttime())
    skynet.error("time ===", skynet.time())
    skynet.error("now ====", skynet.now())

    skynet.sleep(1000)

    skynet.error("starttime", skynet.starttime())
    skynet.error("time ===", skynet.time())
    skynet.error("now ====", skynet.now())

end)

-- [:01000009] starttime 1593175768
-- [:01000009] time === 1593175768.95
-- [:01000009] now ==== 95
-- [:01000009] starttime 1593175768
-- [:01000009] time === 1593175778.95
-- [:01000009] now ==== 1095
-- [:01000002] KILL self
```

> 这里 starttime和now 的单位时1/100S，而time的单位是S。  

* 线程的简单同步

```lua
local  skynet = require "skynet"

-- 简单的任务同步方案。
local cur = 0

skynet.start(function ()
    skynet.timeout(100, 
    function ()
        skynet.error("开始fork所有任务")
        skynet.fork(task,5)
        skynet.fork(task,4)
        skynet.fork(task,2)
        skynet.fork(task,1)
        skynet.fork(task,3)
        skynet.error("fork完成")
    end)
end)

function task(m)
    while cur ~= m-1 do
        skynet.yield()
    end
    skynet.error("当前：", m)
    cur = m
end

-- [:01000009] 开始fork所有任务
-- [:01000009] fork完成
-- [:01000009] 当前： 1
-- [:01000009] 当前： 2
-- [:01000009] 当前： 3
-- [:01000009] 当前： 4
-- [:01000009] 当前： 5
```

> skynet.wakeup 可以唤醒wait线程和sleep线程。（和上面例子无关）

* 异常处理

```lua
local  skynet = require "skynet"

-- 异常处理
skynet.start(function ()
    skynet.fork(task1, "has err func")
    skynet.fork(task2, "right fucn")
    skynet.error("fork 完成")
end)

function task1(arg)
    skynet.error(arg, "-开始")
    skynet.sleep(1 * 100) --S
    assert(err)
    skynet.error(arg, "-结束")
end

function task2(arg)
    skynet.error(arg, "-开始")
    skynet.sleep(2 * 100) --S
    skynet.error(arg, "-结束")
end

-- 运行结果
-- [:01000009] fork 完成
-- [:01000009] has err func -开始
-- [:01000009] right fucn -开始
-- [:01000002] KILL self
-- [:01000009] lua call [0 to :1000009 : 2 msgsz = 0] error : ./lualib/skynet.lua:642: ./lualib/skynet.lua:184: ./lyndemo/test6_1.lua:121: assertion failed!
-- stack traceback:
--         [C]: in function 'assert'
--         ./lyndemo/test6_1.lua:121: in function 'task1'
--         ./lualib/skynet.lua:555: in upvalue 'f'
--         ./lualib/skynet.lua:114: in function <./lualib/skynet.lua:113>
-- stack traceback:
--         [C]: in function 'assert'
--         ./lualib/skynet.lua:642: in function 'skynet.dispatch_message'
-- [:01000009] right fucn -结束
```

> 可以看出，包含异常的服务的异常，没有影响正常任务的运行。  
> 所以说可以大胆使用assert与err。

* 如果不想把服务中断掉，使用pcall

```lua
local  skynet = require "skynet"

-- 把上面的开始函数改成这样，就不会触发异常了。
skynet.start(function ()
    skynet.fork(pcall,task1, "has err func")
    skynet.fork(pcall,task2, "right fucn")
    skynet.error("fork 完成")
end)

-- 运行结果
-- [:01000009] fork 完成
-- [:01000009] has err func -开始
-- [:01000009] right fucn -开始
-- [:01000002] KILL self
-- [:01000009] right fucn -结束
```
