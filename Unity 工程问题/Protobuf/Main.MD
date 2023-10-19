# Protobuf和Unity

## 概念

protobuf由谷歌开发。跨语言，跨平台，可扩展的序列化结构数据的方法，可以用于数据存储和通讯协议。  
灵活，高效和自动化的序列化方法，类比XML，但是比XML小，快，简单。  
可以生成个中代码的数据结构，并进行读写。甚至更新数据结构，而不破坏由旧数据结构编译的已部署程序。？？？  

> 序列化：将结构数据或对象，转换成能够存储和传输（网络传输）的格式，同时应该要保证，序列化之后能够被重建会原来的结构数据或对象。

## 介绍protobuf-net

鉴于Unity大多数工程用的.net2.0，而谷歌官方使用了较高版本，所以我们在Unity中使用第三方的版本：protobuf-net

* 定义结构
  protobuf，使用.proto来定义序列化结构数据的形式，拥有自己的语法：  

  ```cs
  syntax = "proto3";
  package protobuf;
  message Chat {
    singular int32 SenderID = 1;
    string SendMsg = 2;
    enum Channel
    {
        world=0;
        friend=1;
        system=2;
    }
  }
  ```

* 生成协议结构对应代码
  使用release包下protogen.exe生成cs的结构数据代码。  
  命令行：`./protogen --csharp_out=./Generate ChatMsg.proto`   
  生成代码为cs类型并且输出路径：`--csharp_out=./Generate`  
  需要使用的proto文件路径：`ChatMsg.proto`  

* 在代码中创建结构类型的对象

首先我们在git上下载release包。其中包含两个主要的文件：

1. protogen.exe


## 操作

