# UI和粒子效果的层级问题

问题：有两个粒子效果，一个在button下面，一个在button上面。  
解决办法：给粒子设置层级，为button单独覆写层级，为button添加图形射线检测组件。

# 解决步骤：

1. 首先创建两个粒子效果和一个button。
2. 修改canvas中相机模式为screen-camera模式，选择主相机。
3. 设置粒子效果组件中render-order in layer为1和3。【此时运行，button在粒子之下】
4. 给button组件添加一个canvas组件，并开启override-sort，设置order in layer为2。【此时运行，button正好在粒子效果中间；但是这时发现无法点击button了。】
5. 给button条件组件Graphic Raycaster组件，并使得Blocking Mask至少选中UI层级。【此时运行，button可以点击了】

