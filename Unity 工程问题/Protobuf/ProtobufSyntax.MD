# Protobuf 语法

1. 字段限制  p2
  `singular`：一个良好的消息应该由1-0个该字段。
  `repeqted`：表示可重复变长字段，类似数组。重复的值的顺序会被保留。  
  `reversed`：主动保留标识符，不算在字段限制内。

2. tag  
  每个字段都有独一无二的tag，1-15是1字节编码，16-2047使用2字节编码，所以1-15给频繁使用的字段。

3. 类型  

* 数据类型列表

  PB类型|对应C#类型  |介绍
  --:|:--|:--  
  float|float|
  double|double|
  int32/int64|int/long|变长编码，负数请使用sint32/sint64
  uint32/uint64|uint/ulong|变长编码
  sint32/sint64|int/long|U变长编码，带符号，负数效率高于int
  fixed32/fixed64|uint/ulong|固定4字节，如数数值经常大于2^28/2^56，效率则大于uint32/int64
  sfixed32/sfixed64|int/long|固定4/8字节
  bool|bool|
  string|string|字符串内容应该是UTF-8或7bit-ASCLL
  bytes|byte[]|任意二进制数据

* 系统默认值

  PB类型|缺省值
  :--:|:--:
  string|空字符串[""]
  bool|false
  数值|0
  enum|第一个元素

* 枚举类型

  ```cs
  enum EnumAllowingAlias {
    option allow_alias = true;
    UNKNOWN = 0; //必须从0开始
    STARTED = 1;
    RUNNING = 1;
  }
  ```

* Oneof
  但协议中有多个字段，但是这些字段最多有一个被赋值，而其他就会被清除。则使用oneof

  ```cs
  syntax = "proto3";
  package protobuf;
  message Chat {
      oneof test_oneof{
          string O1 = 1;
          string o2 = 2;
      };
  }
  ```

  但我们赋值O1时，o2会被重置为默认值。节省内存。

* Map 映射

  ```cs
  syntax = "proto3";
  package protobuf;
  message TestMap {
      map<int32, string> info = 1;
  }
  ```

  map字段可以repeated  
  序列化之后map字段是无序。  
  重复key只使用第一个

## 包

```cs
package foo.bar  
message chat{ ... }  
```

可选，用于防止不同的消息类型命名有冲突。  
用require引用定义域  
`require foo.bar open = 1;`