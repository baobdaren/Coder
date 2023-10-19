Python中对象分两类，可变类型和不可变类型
可变类型：元组，列表，字典，集合
不可变：数字，字符串
`id() `方法可以打印对象地址
> def id(obj)
> Return the identity of an object.
> This is guaranteed to be unique among simultaneously existing objects. (CPython uses the object's memory address.)
```
num1 = 1
for num1 in range(10):
    print(id(num1))
```
这段程序表明数字类型是不可变的，每改变一次就重新申请内存。

* 总结：== 用于判断值是否相等
* 总结：is  用于判断是否是同一个对象
```
num1 = 1
num2 = num1

print(num1 == num2)#True
print(num1 is num2)#True

num2 = 2

print(num1 == num2)#False
print(num1 is num2)#False

d1 = {"first":1, "second":2}
d2 = d1

print(d1 == d2)#True
print(d1 is d2)#True

d2 = {"first":1, "second":2}

print(d1 == d2)#True
print(d1 is d2)#False
```
注意：相同的数字，总是 is 为true
```
num1 = 1

def fcn(n):
    print(n - 1 is num1)

fcn(1)#false
fcn(2)#true
```
但是相同的字符串不一定为true，截取获得新字符串是新的对象。
```
name = "lyn"
char = "ly"

print(name[:2])#ly
print(char)#ly

print(name[:2] == char)#true
print(name[:2] is char)#false

print(id(name))#1
print(id(name[:2]))#2
print(id(char))#3
```
