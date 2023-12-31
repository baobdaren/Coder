# Overdraw 过度绘制
在一帧内，绘制一个屏幕像素多次，称之为overdraw。比如你开车时需要绘制远处的房子，近处的人行道，车的挡风玻璃，眼睛玻璃。  
第一个Drawcall绘制：远处的房屋。  
第二个DrawCall绘制：近处的人行道。  
第三个DrawCall绘制：汽车挡风玻璃。  
第四个DrawCall绘制：眼睛玻璃。  

Unity OverDraw是移动端和VR设备上性能最大的问题之一。    

这常见于半透明物体的渲染：先渲染不透物体，然后将结果与半透明对象混合。  
更严重的：一些屏幕后处理会将每个像素再处理一次（如： color grading），由于它重画了整个屏幕，所以OverDraw增加了。  

# 为什么OverDraw会影响性能
每次绘制，都需要传出传入数据信息，此操作非常消耗内存带宽。【所有移动平台都是tbr渲染架构，这种架构带宽问题更糟糕】  
对于VR设备，由于其本身分辨率高并且FPS必须在120以上。  
# 如何查看overdraw
* unity自带工具
* renderDoc
* 使用该开源工具（ComputeShader 量化OverDraw）[Github](https://github.com/Nordeus/Unite2017/tree/master/OverdrawMonitor)

# 如何减少overdraw
* 减少不透明OverDraw
* Batching会导致无法按照原有的顺序排序，从而增加OverDraw。（包括静态合批，动态合批，GPUInstancing）
可以禁用麻烦对象的批处理，或者手动分区域合批。
手动设置渲染队列（如将天空和地表延后渲染）？？？  
* 减少透明OverDraw
Sprite可以设置导入，增加多个顶点以较少透明像素。
Image则可以使用。。。
粒子系统也会产生多个OverDraw，因为粒子通常堆叠在一起。
如果OverDraw无法进一步减少，Additive blending比移动设备上的Alpha Blend要廉价得多。
