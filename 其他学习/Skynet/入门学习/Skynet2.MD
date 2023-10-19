# Skynet框架应用 搭建

## Ubuntu搭建

从码云上cloneskynet，按理来说，打开后直接make就行。但是鉴于资本主义和社会主义的隔阂，有个库make时要下载，GitHub上太慢。打开skynet项目里面的3rd，手动从码云clone那个库。（要说贸易保护，中国国家防火墙世界第一！）  

## 目录介绍

1. 3rd：第三方支持库，包括LUA虚拟机和jmalloc。
2. lualib：lua语言封装的常用库，包括http，md5等。
3. lualib-src：捆绑一些c语言库的lua库，例如数据库驱动，bson，机密算法等。
4. service：使用lua写的skynet的服务模块。
5. service-src：使用C语言写的skynet的服务模块。
6. test：使用lua写的一些测试代码。
7. example：示例。
8. Makefile：编译规则。
9. platform.mk：编译与平台相关的设置。

## 编译和运行

### 编译

略；

### 运行第一个skynet节点

运行skynet 后面是配置文档  
`$ ./skynet example/config`

运行一个模拟客户端  
`/3rd/lua/lua examples/client.lua`

