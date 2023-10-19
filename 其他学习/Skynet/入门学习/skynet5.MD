# 服务别名

每个服务启动后，都会分配一个id，例如：01000010，也可以转换成字符串“01000010”

但是这个数字不是固定的，而是启动顺序影响。如果要方便的获取某个服务，可以设置别名。  

## 本地别名和全局别名

* 本地别名只能在当前节点使用：必须以“.”开头，比如：.ser
* 全局别名可以在所有节点中使用：必须不以“.”开头，比如：ser

## 别名注册和接口查询

定义别名

```lua
-- 给当前服务定一个全局/本地别名
skynet.register(sername)

-- 给指定的服务servicehandler定一个全局/本地别名
skynet.name(sername, servicehanlder)
```

查询别名

```lua
-- 查询本地，名为sername的服务，返回servicehandler，不存在则返回nil
skynet.localname(aliasname)

-- 查询本地或者全局服务名
-- 当查询本地名时（如：".ser"），返回servicehandler，不存在则返回nil
-- 当查询全局服务名时（如："ser"）,返回servicehandler，不存在时就阻塞等直到该服务初始化完成。
local skynet = require "skynet.harbor"
skynet.queryname(sername)
```

注意：本地名和全局名可以同时存在

## 其他

全局别名不允许二次修改。即已设定好全局别名后，不能重新设定。  
区别服务名和别名，前者是脚本名，后者是别名（queryservice queryname）
