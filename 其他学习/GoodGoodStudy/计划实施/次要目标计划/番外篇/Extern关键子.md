# 番外篇：extern关键子

## 介绍

* extern有两种用法  
  * `extern "C"`：源于C++有函数的重载，C++编译后的函数名和C编译后的函数名不一样
  * `extern int arg;`：

## 背景

* cpp代码生成包含了预编译-编译-汇编-链接四个步骤，在链接之前编译器无法得知其他文件的内容，所以如果我们要使用其他文件的内容，则无法通过编译器语法检查。

* 解决方法1：在被使用的对象所在文件的头文件提供接口
* 解决方法2：在使用对象的文件中使用extern关键字告知编译器这个对象没在本文件声明。

## extern

* 格式：extern 类型 变量名;
* 例如：`extern int arg;`  
如下代码：

```cpp
//main.cpp
#include<iostream>
using namespace std;

extern int TestExternProp;

int main(int argc, char**argv) {
    cout << TestExternProp;
    getchar();
}
```

```cpp
//other.cpp
int TestExternProp = 10;
```

* 上面的cpp工程中只有这两个文件，可见在main.cpp中我们使用了other.cpp中的变量。  

* 上面的代码中，如果我们注释掉extern声明，则无法预编译，错误为：  
    >error C2065: “TestExternProp”: 未声明的标识符
* 上面的代码中，如果我们注释掉other.cpp中的定义，会发生错误：
    >main.obj : error LNK2001: 无法解析的外部符号 "int TestExternProp" (?TestExternProp@@3HA)  
    >X:\vsWork\CPPTest\Debug\TestProject.exe : fatal error LNK1120: 1 个无法解析的外部命令```

* 在vs中生成错误有不同类型[VisualStdio文档-编译器和生成工具的错误和警告](https://docs.microsoft.com/zh-cn/cpp/error-messages/compiler-errors-1/c-cpp-build-errors?view=vs-2019)
上面的两种错误C0000代表编译错误或警告（C999-C5999），LNK开头的则是链接错误或警告。
