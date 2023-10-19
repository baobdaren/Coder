# Canvas

* 什么是Canvas

Canvas 就是UIGUI用以渲染UI的组件，所有的UI都需要充当其子物体才能被渲染到屏幕。  

canvas可以提供几种渲染模式：

1. screen space - overlay：这种模式下的UI，会直接显示在屏幕上，不需要相机。这个canvas不会被其他物体遮挡。
2. screen space - camera：这种模式下，canvas画布处于设定的相机的前方，canvas中的内容可能会被距离相机近的物体遮挡。而canvas内部UI依然是按照Hierarchy中的列表渲染，不受z轴影响。
3. world space：这种模式下的UI相当世界空间里的物体。
