# ShaderLab命令 Blend
确定GPU如何将片元着色器的输出和渲染目标的合并。  
启用混合会禁用一些优化（删除隐藏表面/Early-z）  
启用混合后，会发生以下四种情况
* 未设置BlendOP，则默认为add
* 如果混合操作是add，sub，revsub，min和max，片元着色器输出乘以源系数
* 如果混合操作是add，sub，revsub，min和max，渲染目标颜色乘以目标系数
* 执行混合操作
混合公式为：
```cs
finalValue = srcFactor * srcValue operation destinationFactor * destintionValue
```
* finalValue：是GPU最终写入缓冲区的值
* srcFactor和destinationFactor：在Blend中定义
* srcValue：片元着色器的输出值
* destination：目标缓冲区当前值
* operation：操作命令

## factor
* One/Zero：数值1/0
* SrcAlpha/SrcColor：源的透明通道/源的颜色
* DstAlpha/DstColor：目标的透明通道/目标的颜色
* OneMinus...：1-x，x可以是上面两项

## Blend Op
用例
```hlsl
BlendOp Add
```
OP很多，常用以下
* Add/Sub：相加/相减
* RevSub：目标-源
* Min/Max：去最小/最大值
