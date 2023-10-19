# C#和Lua交互原理2

1的续

## C#调用Lua

Unity中的Lua框架则是依靠luainterface的api（C语言编写，作为中间语言），而C访问Lua则使用虚拟栈。

## Lua调用CS

通过生成C#源码对应warp文件，C#调用warp文件，warp文件再调用C#。  

## Lua调用CS的Warp文件是什么？

warp 文件获取了Lua要调用的C#的函数的指针：

```cs
IntPtr fn = Marshal.GetFunctionPointerForDelegate(function);
```  

而后使用xlua提供的一个C函数

```cs
[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
public static extern void xlua_push_csharp_function(IntPtr L, IntPtr fn, int n);//[-0,+1,m]
```

将函数的指针压入：

```cs
xlua_push_csharp_function(L, fn, n);
```

提示：已经读完了博客，这里总结？interface是ToLua等框架借助的C库，interface去调用Lua？