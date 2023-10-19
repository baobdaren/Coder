# Image和rowImage

他们都可以显示图片  

rowimage有UV坐标调整的功能，我们可以方便的实现背景滚动  
一般背景使用rowimage（）  
rowimage使用纹理texture，而image使用的是sprite
image组件可以设置image type，用于设置一些特殊的功能

1. 实现血条填充的filled。
2. tield瓷片（扩大时，如果图片不够大，则重复绘制）。
3. 以及边框切割的功能。（slice）

