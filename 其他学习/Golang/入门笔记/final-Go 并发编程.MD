# 大名鼎鼎的Go并发

通过关键字go来开启goroutine即可
goroutine 是轻量级线程。 goroutine的调度是由Golang运行时管理的。  
语法格式-声明函数为goroutine类型：  
`go 函数名(参数列表)`

使用：  
`函数名(参数列表)`  

例如：  

```go
func f(x,y,z){}
go f(x,y,z) //声明，之后执行该函数，会以协程方式执行
f(x,y,z) //使用
```

```go
package main
import (
    "fmt"
    "strconv"
    "time"
)

func main() {
    go response("1-time now", 2000000)
    response("2-time now", 1) //该调用会阻塞主主函数，否则会导致主函数执行完毕，后台协程均被删除
    fmt.Println("开始吧")
}

func response(msg string, waitTime uint64) {
    for i := 0; i < 5; i++ {
        time.Sleep(time.Duration(waitTime))
        fmt.Println(msg + strconv.Itoa(i))
    }
}

//2-time now0
//2-time now1
//1-time now0
//2-time now2
//2-time now3
//2-time now4
//开始吧
```

由上，可以看出，主协程执行的结束后，其他携程没有执行完并且被强制关闭。  


## 通道 channel

通道是一种数据结构，用以在两个协程之间传递数据。  
用法：  

```go
ch <- value // 把value发送到通道ch

sth := <-ch // 取出通道的值，并定义sth且赋值给sth
```

* 声明一个通道  

```go
ch = make(chan int)
catchCh := make(chan string, 10) // 带有10个缓冲空间的通道
```

带缓冲的通道无需等该值被接收后才能继续执行。  

* 遍历通道和关闭通道

```go
ch = make(chan string, 10)

for chMsg := range ch{
    // do
    // 如果ch不关闭，则会阻塞range
}

close(ch)
```