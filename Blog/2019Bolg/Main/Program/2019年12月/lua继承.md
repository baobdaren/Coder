# lua继承

* [参考](https://www.cnblogs.com/blueberryzzz/p/8947446.html)

## lua继承的一个小问题

* lua中的类其实也是一个table实例

```lua
-- 基类
Base = {
    infos = {},
};

-- 子类
Child = {}
function Child:New(me)
    me = me or {};
    setmetatable(me, self);
    self.__index = self;
    -- 不用这种继承则取消下面的注释
    -- self.infos = {}
    return me;
end
function Child:PrintName()
    if self.infos == nil then
        return
    end
    for k,v in pairs(self.infos) do
        print(k,v)
    end
end

-- 使用这种继承
Child = Child:New(Base)

HasName = Child:New()
table.insert(HasName.infos, "我是HasName这是我的名篇")
HasName:PrintName()
NoName = Child:New()
NoName:PrintName()
```

* 上面的代码运行后可以看出，即使第二个对象没有插入信息到属性infos，但是也输出了，说明这俩对象使用同一个基类。
* 注释掉这种继承，使用非继承方式则不会出现这个问题。
* 究其原因，是应为这种继承方式，对于多个对象，都指向了同一个元表。
  也就是说，new方法在进行继承操作时，将基类设为子类的元表；而在new方法在实例化子类时，则是新建空表，将子类设为空表的元表。实例->子类->父类，这就是元表的关系。而上面代码的问题就在于所有实例都将同一个子列设为了自己元表，所以导致一个实例修改子类的值，其他类的也被影响。
* 注意 Lua 中没有类这种抽象的东西，所有的类都是一个表。

## 元表和__index元方法

* 简单继承案例

    ```lua
    parent = {
        house = {"爸爸的房子"}
    }

    function parent.Say()
        print("我是爸爸啊")
    end

    -- 必须使父对象指向自身
    parent.__index = parent

    child = {
        money = {"儿子的钱"}
    }

    setmetatable(child, parent)

    child.Say()
    ```

    在上面的代码中如果我们注释掉元方法的赋值语句 `parent.__index = parent` 则会出现子类无法调用父类方法，这是因为：虽然我们设置父类为子类的元表，但是使用子类对象查找方法时，如果在当前对象查找不到，则会调用元表的__index元方法。__index元方法可是一个表，也可以时一个函数，该表或方法，指定了查找对象的方式。

    ```lua
    parent = {
        house = {"爸爸的房子"}
    }

    function parent.Say()
        print("我是爸爸啊")
    end

    -- 使父对象的__index元方法指向一个方法
    parent.__index = function (p)
        print(p)
        return function (arg)
            print("__index 返回的函数执行 " .. arg)
        end
    end

    child = {
        money = {"儿子的钱"}
    }

    setmetatable(child, parent)

    print(child)
    child.Say("say")
    -- table: 000001B7DC3FC160      testClass2.lua:23
    -- table: 000001B7DC3FC160      testClass2.lua:11
    -- __index 返回的函数执行 say    testClass2.lua:13
    ```

    由上面的运行的案例，可以看出，查找当前对象不存在的属性或方法时：如果子类__index元方法是一个表，则自此表中查找；如果子类__index元方法是一个函数，则直接调用该函数并使用返回值。

## 元方法

* 这个东西还可以解决运算符重载的问题，如两个自定义对象相加，则会调用元方法__add。
