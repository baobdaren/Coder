# 面试必问之-如何减少DrawCall

* 什么是DrawCall？
    就是CPU对底层图形接口的调用。

* fragment是啥？
    潜在像素，可能称为像素。

* batching是啥？
    将批处理之前需要很多次调用（drawcall）的问题合并，之后只需调用一次底层图形接口就行。但有一些不足。
