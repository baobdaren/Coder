####List
* 数组List可以包含不同类型的元素，甚至嵌套数组

####Tuple
* 元组的索引不可以重新指向新的地址
报错
>TypeError: 'tuple' object does not support item assignment
* 可以把string看成是一个特殊的元组，所以数组也是不可变的
* string、list 和 tuple 都属于 sequence（序列）。
####Set
* 基本功能是测试成员关系和删除重复元素
* 注意：赋值空集合的方法 `mySet = set()`，`mySet = {}`这是空字典赋值
```
mySet = set()
print(type(mySet))#<class 'set'>
friend1 = "明月心"
friend2 = "雁南飞"
friend3 = "公子羽"
mySet = {friend1, friend2, friend3}
print(type(mySet))#<class 'set'>
```
####Dictionary
