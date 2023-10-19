# 十四 登录服务

登陆服务，工作主要有两点  

1. 用户登录信息保密。  
2. 实际登陆点分配。

## 14.1 加密算法

```lua
package.cpath = "luaclib/?.so"
local crypt = require "client.crypt"
--如果在skynet中使用直接 local crypt = require "skynet.crypt"
​
--dhexchange转换8字节的key
crypt.dhexchange(key)
​
--通过key1与key2得到密钥
crypt.dhsecret(key1, key2)
```

* 示例代码

暂时略过


