# 服务类型

skynet中的服务分为全局唯一服务和普通服务。全局唯一服务只有一个实例。

## 普通服务

每一次调用创建接口都会产生一个实例，每个实例由唯一的id，可以创建很多个。  

```lua
-- 1. 用于启动新的普通服务，参数为脚本名称，无后缀
-- 2. 阻塞式API，只有所创建的服务的start函数返回后才能得到该服务的地址
-- 3. 如果被启动的脚本初始化是异常，该接口也会执行失败。
-- 4. 如果被启动的服务的start函数是个死循环，该接口也会阻塞
skynet.newservice(scriptName)
```

## 全局唯一服务

多次调用不会创建多个，全局唯一。  

* 创建接口

```lua
skynet.uniqueservice(servicename,...)
skynet.uniqueservice(u, servicename,...)
-- 参数u为true时，则表示该服务在所有节点之间时唯一的。第二次创建只会返回第一次创建的服务的地址
```

* 查询接口

可以通过以下接口查询这个服务是否被创建。  

```lua
skynet.queryservice(sername) -- 查询用uniqueservice创建的服务是否存在
skynet.queryservice(bool, sername) -- 第一个参数为true则查询所有节点
```

> 对于唯一服务的创建接口和查询接口，都有一个重载，多了一个bool参数。这主要用来区分查询范围。当设置为true时，表示创建/查询在所有节点中的唯一服务；而没有这个参数时，表示只查询当前节点。  
