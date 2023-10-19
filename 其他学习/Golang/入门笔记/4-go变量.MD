# 变量

```go
var identifier type
var identifier, identifier type
```

## 1.声明和定义

```go
// 只声明
var num int
//num = 10
fmt.Println(num) //不赋值=0，赋值=10

// 自动推断类型
var flag = true
fmt.Println(flag) // true

// 省略var，使用:=赋值，如果没有新的变量则会错误
myString := "hello world"
var eStr = "?"
//eStr := "what" // no new variables on left side of :=

fmt.Println(myString)
fmt.Println(eStr) // ?

// 简写

num = 10
s1, s2 = "hello", "world"
```

类型会被自动推断

