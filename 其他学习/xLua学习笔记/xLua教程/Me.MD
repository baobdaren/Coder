# xlua教程 REAMDME 笔记

## 一 Lua文件加载

xlua有两种加载lua文件的方式，建议使用第二种方式。  

1. 执行字符串

    ```cs
    luaEnv.DoString("print('hello shit lua')")
    ```

2. 加载lua文件

    ```cs
    DoString("Require 'bufile'")
    ```

    require 实际上是调用一个一个的loader去加载，成功一个后不在尝试，全失败则报错找不到。  
    xlua除了使用lua原生的loader方式之外，还添加了从Resource加载的方式。注意Resource只支持有限的后缀，放在Resources目录下的lua文件必须加上.txt后缀。  

    建议的加载lua脚本的方式是：整个工程就一个DoString("require 'main'")，然后在main.lua中加载其他脚本。

    也可以自定义loader。

3. 自定义loader
    自定义loader只涉及一个接口

    ```cs
        public delegate byte[] CustomLoader(ref string filepath);
        public void luaEnv.AddLoader(CustomLoader loader);
    ```

    通过AddLoader可以注册一个回调函数，该回调函数的参数是一个字符串。在lua代码中调用require时，参数将会透传给回调，回调就可以根据这个参数去加载指定文件。其返回值为lua文件的内容，如果没找到则为空。
    下面是教程的示例

    ```cs
    using UnityEngine;
    using System.Collections;
    using XLua;
    using System;

    namespace Tutorial
    {
        public class CustomLoader : MonoBehaviour
        {
            LuaEnv luaenv = null;
            // Use this for initialization
            void Start()
            {
                luaenv = new LuaEnv();
                luaenv.AddLoader((ref string filename) =>
                {
                    if (filename == "InMemory")
                    {
                        string script = "return {ccc = 9999}";
                        return System.Text.Encoding.UTF8.GetBytes(script);
                    }
                    return null;
                });
                luaenv.DoString("print('InMemory.ccc=', require('InMemory').ccc)");
            }

            // Update is called once per frame
            void Update()
            {
                if (luaenv != null)
                {
                    luaenv.Tick();
                }
            }

            void OnDestroy()
            {
                luaenv.Dispose();
            }
        }
    }

    ```

## 二 C#访问lua

这里就指的是C#对lua数据结构的访问。

1. 访问一个全局基本类型的数据，可以通过LuaEnv.Global就可以了，通过泛型规定返回的类型。

    ```cs
    luaenv.Global.Get<int>("a");
    ```

2. 访问一个全局的table类型

    也就是上面的get方法，指定类型不同
    * 映射到普通class或struct
        定义一个class，又对应于table的字段的属性，并且包含一个无参构造函数。这种方式下xlua会帮你new一个实例，并且吧对应字段赋值过去。
        table的字段可以多于或少于class的属性，也可以嵌套复杂类型。更需要注意的是，这个过程是值拷贝，如果class比较复杂则代价比较大。而且class中的值修改不会同步到table中，反过来也不会。
        这个功能更可以通过吧类型加到GCOptimize生成降低开销。
    * 映射到一个interface
        这种方式依赖于生成代码，代码生成器会生成这个interface的实例。如果get一个属性，生成代码会get对应的table字段，如果set属性也会设置对应的字段。甚至可以通过interface的方法访问lua的函数。
    * 更轻量级的by value方式，映射到Dictionary<> List<>
        不想定义class或interface的话，可以考虑用这个，前提是table下key和value类型相同。
    * 另一种是by ref方式：映射到LuaTable类
        不需要生成代码，但是特慢。

3. 访问一个全局的function
任然是get方法，不同的是映射类型
    * 映射到delgate
        建议这种方式，但是需要生成代码（否则报错），好处在于性能优秀，类型安全。  
        无生成代码时报错：InvalidCastException  
        delegate 要怎样声明呢？
        对于function的每个参数就声明一个输入类型参数。  
        多返回值要怎么处理？  
        从左往右映射到C#的输出参数，输出参数包括返回值，out参数，ref参数。参数，返回值类型支持所有类型。
    * 映射到LuaFunction

4. 使用建议
    * 访问lua全局数据，特别是table以及function，代价比较大，建议尽量少做。初始化时把要调用的lua的function获取一次（映射到delegate），保存下来，后续直接调用该delegate即可，table也类似。

    * 如果lua侧的实现部分都以delegate和interface方式提供，使用方完全可以和xlua解耦：由一个中间模块专门负责xlua的初始化以及delegate和interface的映射，然后把他们映射到需要他们的地方去。

## 三 Lua调用C#

>LuaCallCsharp

### new C#对象

在C#中这样

```cs
var obj = new UnityEngine.Gameobject();
```

对应lua中：  

```lua
obj = CS.UnityEngine.GameObject();
```

基本类似，除了以下两个地方

1. lua中没有new关键字
2. 所有C#相关的都放到CS下，包括构造函数，静态成员属性，方法。

xlua支持重载，直接传递不同类型就行了。

### 访问C#静态属性，方法

#### 读取静态属性

`CS.UnityEngine.Time.deltaTime`

#### 写静态属性

`CS.UnityEngine.Time.timeScale = 0.5`

#### 调用静态方法

`CS.UnityEngine.GameObject.Find('name')`

>小技巧：如果一个类需要经常访问，可以使用局部变量直接引用它，这样既可以减少敲代码时间，还能提高性能。  
> ```local UnityGameObjecy = UnityEngine.GameObject();```
> ```UnityGameObject.Find("name")```

### 访问C#成员属性和方法

#### 读取成员属性

`player.score`

#### 写成员属性

`player.score = 0`

#### 调用成员方法

注意：调用成员方法，需要传递对象本身，建议使用冒号语法糖：
`player:attack()`

#### 父类属性和方法

xlua支持（通过派生类）访问基类的静态属性和静态方法。也支持（通过派生类）访问基类的成员属性和方法。

#### 参数的输入和输出属性（out ref）

Lua侧调用C#函数的参数处理：C#的普通参数和ref修饰的参数，从左往右依次对应lua调用侧的输入参数。注意out修饰的参数不算在输入参数。

Lua侧调用C#函数的返回值处理：C#函数的返回值（如果有）和ref修饰的参数和out修饰的参数，从左往右依次对应lua的多返回值。注意ref修饰的参数，既是输入也是返回值。

#### 重载方法

直接通过不同类型的参数，来实现调用不同的C#重载方法。
由于lua的数据类型不及C#丰富，可存在一对多的情况。如lua中的number对应C#中的int，float，double等。此时指挥调用排版在前面的函数。

#### 操作符

支持的操作符有：+，-，*，/，一元-，==，<=，%，[]

#### 可变参数方法

对于C#的如下方法

```cs
void VariableParamsFunc(int num, params string[] args)
```

使用下面的lua代码调用

```lua
obj:(8, 'str1', 'str2')
```

#### 使用 extension methods

在C#里定义了，lua里就能直接使用。

#### 泛型（模板）方法

不直接支持，可以通过extension methods封装后调用

#### 枚举类型

枚举就像枚举类型下的静态属性一样

#### delegate使用（调用，+，-）

lua调用C#中的delegate，和普通lua函数的调用方式一样。
+操作符：对应C#的+操作符，把两个调用串成一个调用链，右操作数可以是delegate或者lua函数
-操作符：和+相反，把一个delegate从调用链中删除
> delegate属性可以用一个luafunction来赋值

#### event

如obj里有个这样的定义：`public event Action onFunc`  
那么在lua中这样注册事件回调：`obj:onFunc('+', callBackLuaFunc)`  
那么在lua中这样移除事件回调：`obj:onFunc('-', callBackLuaFunc)`
注意这里是直接把该对象的事件视为方法。

#### 64位整数支持

#### C#复杂类型和table的自动转换

#### 获取类型（相当于C#的typeof）

比如：`typeof(CS.UnityEngine.ParticleSystem)`

#### “强”转
