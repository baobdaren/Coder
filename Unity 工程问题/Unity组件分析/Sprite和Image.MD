# Sprite和Image

* 渲染方面
  Image通过CanvasRender渲染
  Sprite则通过SpriteRender渲染

* 使用上
  Image必须再Canvas下才能显示，而Sprite则可以再世界空间显示。
  Image使用RectTransform布局，方便设置布局和对齐。

* 原理上
  Image被处理成两个三角形划分的矩形，而Sprite则根据显示内容，剪裁掉大部分透明区域，然后生成一个复杂的顶点结构。这使得Sprite在渲染时较少了很多的片段，并降低了overdraw。（overdraw就是对同一个片段多次绘制）
