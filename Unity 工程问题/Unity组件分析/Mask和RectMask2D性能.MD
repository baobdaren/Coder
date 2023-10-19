# 有关Mask和RectMask2D性能分析

[参考]("https://blog.csdn.net/qq_40093529/article/details/85230487")

## RectMask2D

该组件不需要依赖Image组件，其剪裁区域就是RectTransform的rect大小。

* 性质1：RectMask2D节点下的所有孩子都不能与外界UI节点合批，且多个RectMask2D之间不能合批。
* 性质2：计算depth的时候，所有RectMask2D都按照一般UI节点对待，但是它没有CanvasRender组件，所以不是任何UI的bottomUI。

## Mask组件

Maks组件需要依赖一个Image组件，其剪裁掉Image组件之外的区域。

* 性质1：Mask组件会在首尾增加两个DrawCall，并且这两个DrawCall不能合并。多个mask间可以合并。
* 性质2：计算depth时，当遍历到一个mask的首（首：mask组件对象，尾：content内容遍历完后的DrawCall）把它当作一个不可合批的节点，但是可以作为子节点的bottomUI。
* 性质3：mask内的UI节点不能和非mask组件的之外的节点合并。（A再mask内，B再mask外，并且B不在其他mask内）

## 使用

mask组件最少有两个DrawCall，rectask2D则没有。  

根据二者的特性：

1. 当界面上只用1次mask时，使用rectmask2D；
2. 当界面上只用2次mask时，二者没有区别；
3. 当界面上只用3以上次mask时，使用mask； 

总结：它们都不能和非mask外部合批，但是mask可以和其他mask合批。maks自带两个DrawCall
