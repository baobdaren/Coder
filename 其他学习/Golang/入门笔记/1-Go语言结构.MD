# Go语言结构

* 包声明
* 引入包
* 函数
* 变量
* 语句/表达式
* 注释

hello world

```go
package main
import "fmt"
func main(){
    fmt.Println("hello world")
}
```

> 第一行 `package main` 定义了包名，必须在源文件中非注释第一行指明这个文件属于哪个包。每个go应用都要main包。
> 第二行`import "fmt"` 引入的fmt包，输入输出的包。
> `func main(){}` 每一个可执行的程序要包含main函数，一般来说启动后第一个执行，但是又init函数时，会先执行init。
> 注释同C#
> go对变量名成有特殊处理，首字母大笔时，该标识符（常量，变量，类型，函数名，结构字段等）的对象可以再其他包导入时被可见，小写时则只能在包内使用。public和private

