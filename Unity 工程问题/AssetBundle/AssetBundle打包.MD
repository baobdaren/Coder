# AssetBundle打包

## 1. 步骤翻译官方文档

> 想要打包一个资源到一个Asset bundle执行以下步骤
>
> 1. 在Project面板中选择对象；
> 2. 在Inspector中核对资源；
> 3. 在Inspector中最下方，点击名称，点击New，创建包名或选择已有包名（如果有的话）。
> 4. 然后选择刚创建的名称，以后可以重复选择。（创建包名可以使用‘/’创建目录）
> 5. 后缀可以不选，一般选‘.unity3D’。（据说可以用来区分版本）

## 2. 打包代码

创建一个编辑器脚本，如下代码。  
点击构建后，会把所有标记的资源打包好，并存放到输出路径。

```cs
[MenuItem("打包/构建 AssetBundle")]
public static void BuildAsset()
{
    string path = "Assets/AssetBundlesFolder";
    if (!Directory.Exists(path))
    {
        Directory.CreateDirectory(path);
    }
    BuildPipeline.BuildAssetBundles(
        path, //输出路径
        BuildAssetBundleOptions.None, // 有一些可选项
        BuildTarget.StandaloneWindows // 输出平台
    );
}
```

## 3. 如果需要，上传到网站

可选

## 4. load AssetBundle和Asset

ab的加载方式官方文档只给了两种
（Using AssetBundles Natively章节升入讲解四个加载方式）

```cs
/// <summary>
/// 第一种方式：从本地加载 AssetBundle.LoadFromFile
/// </summary>
public void TestLoadFromFile()
{
    AssetBundle ab = AssetBundle.LoadFromFile("");
    ab.LoadAsset<Texture>("");
}

/// <summary>
/// 第二种方式：从网络或本地加载，url也可以是本地
/// </summary>
/// <returns></returns>
public IEnumerator TestLoadFromWeb()
{
    UnityWebRequest webReq = UnityWebRequestAssetBundle.GetAssetBundle("");
    yield return webReq.SendWebRequest();
    AssetBundle ab = DownloadHandlerAssetBundle.GetContent(webReq);
    ab.LoadAsset<Texture>("");
}
```

以上两种方式加载的ab包，最终都需要使用`ab.LoadAsset<T>("")`从ab包的内存镜像中加载指定的资源。

## 5. AB包打包策略

### 1. 类型分组

把类型类似的资源分配给单个AssetBundle，比如Audio或者语言。  
在构建多个平台的工程时这是个很好的策略。例如在win和mac之间，Audio的压缩格式相同，则可以将Audio全部打包到一个AssetBundle。然而对于shader往往有很多针对平台的选项编译（就是说不同平台不一样），所以针对mac的shader的AssetBundle，不能再win上使用。（此外不同资源修改频率不同，这对于不同版本Unity打包很友好）。

### 2. 并发内容分组

即把同时加载和使用的资源捆绑到一起。类似于基于等级的游戏，每一个等级有完全唯一的字符，纹理，音乐等。但是必须要确定，某个资源使用时其他资产也被使用，因为AssetBundle包必须整体下载。

典型用例就是场景，这种策略中，每个场景都包含了大部分或所有场景的依赖项。

官方建议：

1. 频繁更新的对象于几乎不更改的对象需要分开。
2. 将同时加载的资源分为同一组，如一个模型，模型的动画，模型的纹理。
3. 多个AssetBundle包都引用的资源，放在共享包里，以减少重复。
4. 对于几乎不会同时使用的需要分开，如高清资源和标清资源。
5. 如果单个包里少于一般的资源频繁使用，则考虑拆分。
6. 5-10个资源，且同时加载并使用，考虑合并一组
7. variant：一组对象只是一个对象的不同版本，考虑使用variant。？？？

## 6. AssetBundle Dependencies

ab的引用问题，如果一个被打包的对象引用一个未被打包的纹理，则纹理会被复制一份到对象所在的包。多个对象则会复制多份。Unity不会主动加载依赖，需要手动判断，并预先加载。

## 7. Using AssetBundle Natively

AssetBundle包的加载方式有四种，这四种的行为的多样性基于加载该包的平台和构建时的压缩方式（uncompressed LZMA LZ4）。

> 压缩方式比较（LZMA 和 LZ4）  
> 解压速度LZ4快
> 压缩比LZ4高
> 加载速度LZMA快

```cs
/// <summary>
/// 从内存中读取;
/// 注意：可以传递CRC;
/// 注意：加载LZMA时先解压，而LZ4则在压缩状态下加载;
/// </summary>
/// <returns></returns>
public IEnumerator TestLoadFromMemory()
{
    AssetBundleCreateRequest req = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(""));
    yield return req;
    Texture t = req.assetBundle.LoadAsset<Texture>("");
}

/// <summary>
/// 从文件加载
/// 在非压缩的本地AssetBundle中，效率最好。  
/// 加载LZMA时会先解压，再加载。而对于非压缩或LZ4则直接加载。
/// </summary>
/// <returns></returns>
public IEnumerator TestLoadFromFile()
{
    AssetBundleCreateRequest v = AssetBundle.LoadFromFileAsync("");
    yield return v;
    v.assetBundle.LoadAsset<Texture>("");
}

/// <summary>
/// UnityWebRequestAssetBundle加载
/// DownloadHandlerAssetBundle.GetContent(req)，和LoadFromFile一样的效率
/// 优势是可以让开发这更灵活的处理下载数据，而避免内存浪费
/// 网络加载的方式
/// </summary>
/// <returns></returns>
public IEnumerator WebGet()
{
    using (UnityWebRequest req = UnityWebRequestAssetBundle.GetAssetBundle(""))
    {
        yield return req.SendWebRequest();
        AssetBundle r =  DownloadHandlerAssetBundle.GetContent(req);
        r.LoadAsset<Texture>("");
    }
}
```

## 8. Loading Assets From AssetBundles

从AssetBundle包加载，比较单一

* `bundleLoaded.LoadAsset<T>(string name)`
* `bundleLoader.LoadAllAsset<T>(string name)`

上面的方法都有自己的异步和无类型的重载。针对异步重载函数，其返回类型是一个`AssetBundleRequest`，使用携程加载的等待方式为`yield return assetBundleRequest`。

## 9. Loading AssetBundle Manifests

Manifests文件再解决AssetBundle依赖是很有用。  
要获得AssetBundleManifest对象，则需要加载一个AssetBundle附加文件（在AssetBundle打包目录下，和打包的包名同名）。  
加载方式和加载其他AssetBundle包方式相同。（类型选择AssetBundleManifest）  

如果要使用A包中的一个资源，而其有引用B包中的一个资源，则需要先加载依赖。也就是，先加载AssetBundle的附加文件，判断需要加载对象的所有依赖，再加载依赖，最后加载对象自身。

## 10. Manage Loaded AssetBundles

Unity不会自动卸载不被当前场景使用的资源，他们的卸载再特定时刻触发，也可以主动触发。加载和卸载资源时机很重要，不当的卸载，会导致对象重复存在于内存或者不希望的结果，如：纹理丢失。  

AssetBundle.Unload(bool unloadAllLoadedObject)用于卸载当前的AssetBundle，如果传递参数true，则同时卸载所有已被加载的Asset；但是如果传递true，则会只卸载AssetBundle，已加载的Asset不会被卸载，再次加载这个AssetBundle时，也不会引用到旧的Asset。

官方建议使用unload(true)，为了防止再次使用时资源重复：

1. >Having well-defined points during the application’s lifetime at which transient AssetBundles are unloaded, such as between levels or during a loading screen.  
（这句话大概是说，明确卸载资源的时机，如等级提升或场景加载。也就是说，接下来肯定不会再加载这个AssetBundle了）。
2. 维护一个引用计数，再所有资源都不再使用时卸载AssetBundle和已加载的Asset。

如果非要使用unload(false)  
1. 取消对想卸载资源的场景和代码引用，再调用`Resources.UnloadUnusedAssets()`。
2. 在非累加场景加载中，将会自动场景所有对象并调用`Resources.UnloadUnusedAssets()`


