# AssetBundle

[参考这些博客](https://www.jianshu.com/p/5226659533cd)

## AssetBundle介绍

AssetBundle时Unity的一种资源包，这种资源包可以包含几乎所有的资源：模型，纹理，预设，场景。并且可以在运行时动态加载它们。  
Assetbundle可以压缩后传输。  
使用AssetBundle可以解决一下问题：

1. 实现资源热更。
2. 减少初始包的大小，游戏可以做分包处理。压缩方式多，需要权衡。
3. 利用二进制加密。

## AssetBundle加载

原理流程

1. 首先从网络或者硬盘把AssetBundle加载到内存。[CreateFromFile / UnityWebRequest]，此时内存有了Asset Bundle的内存镜像。
2. 从Asset bundle的内存镜像中读取需要的内容，如：贴图，预制体等。[AssetBundle.Load()]，此时保存了所需内容。
3. 使用或引用读取到的Asset的。

推荐使用AssetBundle.LoadFromFiles，从性能上说是最好的，内存占用小。如果资源需要下载，则使用www或unitywebrequest下载好之后，再用上面的方法加载。

## AssetBundle卸载

AB包的卸载统一导致卸载不干净，继而导致资源的冗余。  

1. AssetBundle.Unload(false)-卸载AssetBundle的内存镜像
2. AssetBundle.Unload(true)-卸载AssetBundle的内存镜像以及已经加载的Object（就是AssetBundle.Load()得到）。
3. Resource.UnloadUnusedAssets()-卸载没有引用的assets。
