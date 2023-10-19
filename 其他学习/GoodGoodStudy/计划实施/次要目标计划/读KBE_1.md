# kbengine源代码读

## 再次回到主干流程

* `g_componentID = genUUID64();`
这里给组件生成一个随机id

* `parseMainCommandArgs(argc, argv);`  
这里解析主函数的参数  

> （比如设定指定的组件id，以及gus，我也还没了解到gus搞啥用的。。。不影响我们阅读整体流程，不细究）

* 下面的语句进行crash处理，文档说不影响主流程阅读，😝

```cpp
char dumpname[MAX_BUF] = {0};
kbe_snprintf(dumpname, MAX_BUF, "%" PRAppID, g_componentID);
KBEngine::exception::installCrashHandler(1, dumpname);
```

* 下面的语句就是一个标准的main函数转向

```cpp
int retcode = -1;
THREAD_TRY_EXECUTION;
retcode = kbeMain(argc, argv);
THREAD_HANDLE_CRASH;
```

暂记：每个组件的主函数都在KbeMaintT这个模板的run函数终止，run之后的代码则是关闭流程了。哈哈😁
