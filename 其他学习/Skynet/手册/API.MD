# Skynet API手册

函数|参数|总结
--:|:--|:--
skynet.uniqueservice(n, ...)|`n`：服务的脚本名，无后缀。  `...`：启动参数|启动一个全局唯一服务并返回七地址，如果已启动则返回已启动的服务地址。
skynet.queryservice(n)|n：要查询的服务的名|查询一个由uniqueservice启动的唯一服务，如果未创建则阻塞等待其创建。
skynet.newservice(n,...)||启动一个名为n的服务
skynet.localname(n)||返回同一进程内，用registe注册的具体服务的地址