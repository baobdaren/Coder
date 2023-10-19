```
	fileName = sys._getframe().f_back.f_code.co_filename   #获取调用方法所在文件
	funcName = sys._getframe().f_back.f_code.co_name     #获取调用方法名
	lineNumber = sys._getframe().f_back.f_lineno                 #获取调用的行号
```

> 	print("{0}<fun:{1}> ({2}): ".format(fileName.split('scripts')[-1],funcName,lineNumber), end="")
