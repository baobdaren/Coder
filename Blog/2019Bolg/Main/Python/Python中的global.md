有时候，我们再一个模块中定义了一个变量，此时如果我们在该模块中调用一个方法来修改这个变量，则会发现这个变量并没有改变：
如下代码
```
datas = "default"
print("mod datas id = " + str(id(datas)))

def fcn():
    # global datas
    # datas = "new value"
    print("fcn datas id = " + str(id(datas)))

fcn()
print("mod datas id = " + str(id(datas)))
```
>* 直接执行上面的代码
>所有的datas的id是相同的

>* 取消`datas = "new value"`的注释
>我们发现，方法fcn中的datas的id和方法外的不相同

>* 取消`global datas`的注释
>这时，我们才修改了模块中变量，但是global这里只是起到了声明的作用，可以说定义覆盖

如下代码，global在方法中声名一个全局变量
```
def fcn():
    global g_para
    g_para = "g in fcn"

fcn()
print(g_para)
```
>global调用fcn方法后，g_para是一个全局变量
>也就是可以理解为，global声明变量为全局变量，如果全局没有这个变量，就定义一个
那么这样能否在只声明后打印呢？并不能，动态语言真tm烦！

