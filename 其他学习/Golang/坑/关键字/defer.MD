# defer

[参考](https://www.cnblogs.com/vikings-blog/p/7099023.html)

defer 声明的语句会在return之后调用，一般用来在函数结束时关闭一些资源，如文件。`defer fd.Close()`  

* 声明的地方，参数会被实时解析
  也就是说，执行到defer语句时，参数会被记录一次。return语句之后则直接使用该值。  

* 值的记录以栈为结构，先进后厨
  就是说，多个defer语句，会依次入栈，执行时逐个出栈执行。  
  
  ```go
  i := 0
  defer fmt.Println(i)
  i = 1
  defer fmt.Println(i)
  // 1 2
  ```

* 可以读取有名返回值

  ```go
    func c() (i int) {
        defer func() { i++ }()
        return 1
    }
    // 2
  ```

  >上面的函数中，i是一个有名返回值（指定返回类型时也指定呢名称）。该值可以被defer访问。