* 使用local引用global变量，global使用需要hash一次，local则是地址+偏移
* Lua中所有functon都是闭包
* 常量对象创建循环外
* 传参不多时，使用变量传参，而不是构建table
* 创建table时，直接初始化```table t = {1,2,3}```放置当个放入导rehash消耗。
* 缓存table长度，比pairs和ipairs更高效
* table.concat拼接字符串
