# Map

map的迭代返回无序。  

* 声明

```go
package main

import "fmt"

func main(){
    //var dic map[int]string // 只是声明，必须赋值后再使用
    dic := make(map[string]int)

    dic["lyn"] = 100
    dic["ly"] = 20
    dic["zyl"] = 19

    for _k, _v := range dic	{
        fmt.Println(_k,_v)
    }
}
```

# 强制类型转换

type_name(expression)

同C++

```go
num := 12
float32(num)
```