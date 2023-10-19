# 打印调用栈

## 函数

`firstFrame.f_back`就是调用本函数的函数，应为本函数只负责打印，所以最少要追溯一次，才能找到调用该函数的函数

## 代码

```py
def STACKTRACE_MSG(args, z = False, traceCount = 1):  
    """
    traceCount:调用栈信息个数
    z:追溯两次
    """
    fileName = ""
    funcName = ""
    lineNumber = 0
    firstFrame = sys._getframe()
    for traceIndex in range(0,traceCount):
        if z:
            fileName = firstFrame.f_back.f_back.f_code.co_filename
            funcName = firstFrame.f_back.f_back.f_code.co_name
            lineNumber = firstFrame.f_back.f_back.f_lineno
        else:
            fileName = firstFrame.f_back.f_code.co_filename
            funcName = firstFrame.f_back.f_code.co_name
            lineNumber = firstFrame.f_back.f_lineno
        msg = "{0}-- {1}  <fun:{2}> ({3}) >>> ".format(traceIndex, fileName.split('scripts')[1][1:],funcName,lineNumber)
        print(msg, end="")
        if traceIndex == 0:
            print(args)
        else:
            print("", end="\n")
        firstFrame = firstFrame.f_back
```

>0-- cell\skills\base\SkillInitiative.py  <fun:canUse> (97) >>> 消息  
>1-- cell\skills\SkillAttack.py  <fun:canUse> (31) >>>  
>2-- cell\interfaces\SkillBox.py  <fun:useSkill> (110) >>>  
