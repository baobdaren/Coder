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
