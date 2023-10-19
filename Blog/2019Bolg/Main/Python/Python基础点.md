* 2019年6月6日
>注意：类的属性必须赋初始值，同时一般不将类变量用作实例的变量使用，实例变量在使用时定义。
>注意：一个类类型也可以用`id(class)`来取得其内存地址。
>注意：定义在类中的属性，每个实例都有一个独立的值。
>注意：类中的方法/属性名称以双下划线`__name`开头，则为类的私有属性，否则为公有。
***
***
* 2019年6月14日
>* 注意：Python中类的方法都要传入类的实例为第一个参数，建议使用self。
>* 注意：无论是在类的外部，还是类的方法内部，访问属性都必须通过实例变量访问。
>* 方法：super(type, typeInstance)，使用该函数调用父类的构造函数
>```
>class parent:
>    def __init__(self, arg):
>        print("父类的构造函数:" + arg)
>
>class child(parent):
>    def __init__(self, arg):
>        super(child, self).__init__("子类给父类的参数")
>        print("子类的构造函数:" + arg)
>
>xm = child("构造参数")
>```
>* 新建：类方法，实现如下：
>  ```
>class Animal:
>    name = "null"
>    def __init__(self, name):
>        print("Animal:" + name)
>        self.name = name
>    @classmethod
>    def ClassFcn(SELF, arg):
>        print(arg + SELF.name)
>    @staticmethod
>    def StaticFcn(SELF, arg):
>       print(arg + SELF.name)
>an = Animal("wangcai")
>Animal.ClassFcn("地球生物")
>Animal.StaticFcn(an, "静态函数")
>```
>可以看出，普通方法隐藏传递类实例，类方法隐藏传递类类型，静态方法不隐藏传递。
