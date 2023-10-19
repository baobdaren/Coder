# 番外：宏定义

* 宏定义主要以替换为逻辑，发生在预编译期间。
* 宏定义展开不占用运行时间，占用编译时间。

宏定义分为`带参数的宏定义`和`不带参宏定义`

## 不带参数宏定义

格式：#define 标识符 字符串
例如：#define PI 3.14
在程序中则替换PI为数值3.14

## 带参数的宏定义  

格式：#define 标识符(参数列表) 字符串  
例如：#define SUB(A,B) A+B  
在程序中替换如下  

```cpp
#include<iostream>
using namespace std;

#define SUB(A,B) A*B

int main(int argc, char**argv) {
    cout << "hello world!\n";

    cout << (SUB(1 + 3, 2));//7
    cout << (SUB(4, 2));//8

    getchar();
}
```

宏的替换发生在编译期间，所以上例中结果不一致。

## 宏定义中的"##"和"#"

1. 关于 `#`  
    在宏定义中，我们无法在字符串内替换,如下代码  

    ```cpp
    #include <iostream>
    using namespace std;
    #define PI 3.14
    int main(int argc, char** argv) {
        double r = 2;
        double a = PI * r*r;
        cout << "PI*R*R=" << a;
        //PI*R*R=12.56
    }
    ```

    在输出的结果中，字符串中PI并没有替换。所以通常我们使用#来将其后面的内容字符串化。

2. 关于 `##`

    ```cpp
    #include <iostream>
    #include <string>

    using namespace std;
    #define My(NAME,VALUE)  string _Global_##NAME = #VALUE;

    int main(int argc, char** argv) {
        My(Hero, 锤石);
        cout << _Global_Hero;
        return 0;
    }
    ```

    上面的代码展示了##的用处。`_Global_Hero`就是这个宏创建的字符串变量。  
