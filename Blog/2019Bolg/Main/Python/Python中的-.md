###python中*的用法
* 乘法 和 重复
---
* 方法参数中，表示接收任意多个参数
```
def fcn(*tp, tail="default"):
    print(type(tp))#<class 'tuple'>
    for v in tp:
        print(v)
    print(tail)

fcn(1,2,3,4, "new value")
fcn(1,2,3,4, tail="new value")
```
> 输出发现，传入的参数被装换成元组
注意：如果在方法中还要传入指定的参数，则必须指定名称，否则如同上例，将会使用默认值
---
* 调用方法时，表示“解包传递参数”
```
def fcn2(v1,v2,v3="v3 default"):
    print(v1,v2,v3)#J Q K

myT = ("J","Q","K")
fcn2(*myT)
```
>myT被分别赋值给方法的三个参数
---
* 方法参数中两个星号，表示按照字典的方式传参
```
def fcn3(**dt):
    print(type(dt))#<class 'dict'>
    for k,v in dt.items():
        print(k,v)
    """
    min A
    max Kin
    """

fcn3(min = "A", max = "King")
```
> 输出表示，dt类型为字典
