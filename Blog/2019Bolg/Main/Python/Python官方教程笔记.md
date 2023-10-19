* 数组切片是浅复制
* `while/for -- else` 
  当循环被`break`终止时不会调用`else`块。
  举个栗子，我们检测队伍中，所有玩家是否符合副本的等级要求：
  ```py
  teamLevels = [11,12,30,101,12]
  for item in teamLevels:
      if not 10 < item < 100:
          print("这个队伍不符合进入条件")
          break
  else:
      print("这个队伍可以进入副本")
  ```
* 多重赋值：先计算右边所有的表达式，再依次赋值
-------------------------------------------------
* `rang()`函数并不会创建一个列表:
  >[`range()`](https://docs.python.org/zh-cn/3/library/stdtypes.html#range "range") 所返回的对象在许多方        面表现得像一个列表，但实际上却并不是。此对象会在你迭代它时基于所希望的序列返回连续的项，但它没有真正生成列表，这样就能节省空间。
* `enumator()` 方法再遍历可迭代对象是，返回序号
  ```py
  l = ["hello","world","!"]
  for item in l:
      print(item)
  for index, item in enumerate(l):
      print(index, item)
  ```
* `for` 循环不会在结束时回收`item`内存
  ```py
  def fun():
      l = [1,2,3]
      for k in l:
          pass
      print(k) # 3
  fun()
  ```
* 关于global和nonlocal
  * `global` 指明该变量名称该去全局区查找。
  * `nonlocal` 知名该变量名属于上层调用块，并且该块不是全局的。
  * `python` 变量查找顺序：局部表->上层表->内置表
    `global`是对解析器的指令，只对同一时刻被解析的代码有效，这意味着`exec`中的`global`并不会对调用该块的代码产生影响。

  注意：对于一个赋值语句来说，如果局部表中没有这个名称，则会创建新的局部变量，而不去查找。
  
  注意：`nonlocal`和`global`操作的变量不能在此之前使用过。

* 函数的缺省值赋值只会执行一次 —— 官方文档
        ```py
        def Func(i, arg = []):
            arg.append(i)
            return arg

        Func(1)
        Func(2)
        Func(3)
        print(Func(4))

        # [1, 2, 3, 4]
        ```
    注意：这里的赋值一次只针对可变对象，如列表，字典，以及大多数类实例
        ```py
        def Func(i, arg = 0):
        arg = arg + i
        return arg

        Func(1)
        Func(2)
        Func(3)

        print(Func(4))
        # 4
        ```
* 元组的解包和打包，参数的解包和打包也同理
    ```py
    t = 1,2,3,4
    a,b,c,d = t

    print(t) # (1, 2, 3, 4)
    print(a,b,c,d) # 1 2 3 4
    ```
* `zip`函数，同时遍历两个序列
    ```py
    # 使用zip函数同时遍历
    A = ["A1", "A2", "A3", "A4"]
    B = ["B1", "B2", "B3", "B4"]
    for i1, i2 in zip(A, B):
        print(i1, i2)

    # A1 B1
    # A2 B2
    # A3 B3
    # A4 B4
    ```
* 使用 `_` 开头的文件中的对象，不会被导入到其他模块，相当于该文件模块私有
`from me import * `
# 模块
* >为了加速模块载入，Python在 `__pycache__` 目录里缓存了每个模块的编译后版本
* >必须要有`__init__.py` 文件才能让 Python 将包含该文件的目录当作包。
* 一个模块被` from me import * `导入时，如果有`__all__`，怎么导入其保存的方法或者对象等，而不是导入所有。
    ```py
    # model
    def Can():
        print("我可以被导入")
    def Cant():
        print("我不可以被导入")

    __all__ = ["Can", "Cant"]`
    ```
    ```py
    from model import *
    Can()
    Cant()
    ```
    >例如，文件 sound/effects/`__init__.py` 可以包含以下代码:
    `__all__` = ["echo", "surround", "reverse"]
    这意味着 from sound.effects import * 将导入 sound 包的三个命名子模块。
    如果没有定义 `__all__`，`from sound.effects import *` 语句 不 会从包 sound.effects 中导入所有子模块到当前命名空间；它只确保导入了包 sound.effects （可能运行任何在 `__init__.py` 中的初始化代码），然后导入包中定义的任何名称。这包括 `__init__.py` 定义的任何名称（以及显式加载的子模块）。它还包括由之前的 import 语句显式加载的包的任何子模块
# 7 输入输出
* 在字符串之前加字母 `f` 可以直接插入变量
    ```py
    year = 2019
    mon = 10
    data = f"{year}年{mon}月"
    print(data)
    # 2019年10月
    ```
* `f` 字符串中的格式控制 
    >bodx : 二进制 八进制 十进制 十六进  
    >c : ASCALL 输出   
    >e : 科学计数法  
    >% : 除以100,百分比输出  
    >最后,使用`<` 表示左对齐
   ``` py
    s1 = "这个数"
    print(f"{s1!r} 等于 {256:15b}")
    print(f"{s1!r} 等于 {256:15o}")
    print(f"{s1!r} 等于 {256:15X}")

    print(f"{s1!r} 等于 {256:15d}")
    print(f"{s1!r} 等于 {256:15e}")
    print(f"{s1!r} 等于 {0.25:15%}")
    print(f"{s1!r} 等于 {97:15c}")

    print(f"{s1!r} 等于 {0.25:<15%}")
    # '这个数' 等于       100000000
    # '这个数' 等于             400
    # '这个数' 等于             100
    # '这个数' 等于             256
    # '这个数' 等于    2.560000e+02
    # '这个数' 等于      25.000000%
    # '这个数' 等于               a
    # '这个数' 等于 25.000000%     
  ```
* 使用with访问文件:离开with块之后，文件会被主动关闭，尤其是访问文件异常时有用.虽然python会关闭没有引用文件描述符的打开中的文件，但依然不是立即关闭的.
    ```py
    save = None
    fPath = r"PythonTutorial\输入输出\test.txt"

    def Func():
        global save
        save = None
        # 不使用with
        fd = open(fPath)
        save = fd
        print(fd.name)
        1/0
        fd.close()

    def Func_safe():
        global save
        save = None
        # 使用with
        with open(fPath) as fd:
            save = fd
            print(fd.name)
            1/0

    def Test(f):
        try:
            f()
        except Exception as er:
            print(er.args[0])

    Test(Func)
    print(save.closed)

    Test(Func_safe)
    print(save.closed)

    # PythonTutorial\输入输出\test.txt
    # division by zero
    # False
    # PythonTutorial\输入输出\test.txt
    # division by zero
    # True
    ```
# 8 异常
* 自定义异常通常间接或直接继承自Except，多数情况下提供一些属性。但是也可以执行一般的函数。
* 注意，抛出一个异常，可以被catch语句中为异常子类的语句捕获:
    ```py
    class MyExcept(Exception):
        pass
    class MyExceptChild(MyExcept):
        pass

    try:
        raise MyExceptChild
    except MyExcept as idf:
        print("异常父类") # 异常父类
    except MyExceptChild as idf:
        print("异常子类")
    ```
    上例中，抛出的`MyExceptChild`的异常，被其父类的条件捕获了。
* raise，下例中，raise抛出当前异常
    ```py
    def Test():
        try:
            1/0
        except Exception as identifier:
            print("只是看看有异常没")
            raise

    try:
        Test()
    except Exception as identifier:
        print(f"我才是处理:{identifier}")
    # 只是看看有异常没
    # 我才是处理:division by zero
    ```
# 9 类
