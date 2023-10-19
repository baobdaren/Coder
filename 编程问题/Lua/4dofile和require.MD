# dofile和require的区别

首先，他们都可以用来加载模块，但是不同之处在于  

1. require加载文件后，会保存在package.loaded中，防止重复加载；而dofile则不会
2. require会通过模式匹配，在系统中查找文件，而dofile则不会。
<!-- 3. dofile没有运行在保护模式下，它会把错误反馈给调用者。 -->

require加载文件前，会先去搜索package.loaded，如果已经有了，则不会再次加载，并返回该值给调用。如果没有，则会去查找加载器。
