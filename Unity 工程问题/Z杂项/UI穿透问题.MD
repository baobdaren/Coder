# UI穿透问题

* 问题
  UI的穿透问题环境是这样的：在我的世界中，我们使用左键摧毁方块，某一时刻，我们打开合成面板，想要点击使用方块，但是由于UI穿透，此时摧毁方块的代码也执行了。  
  简而言之：我们打开UI之前，鼠标右击执行世界逻辑中的代码；而我们打开UI之后，我们希望执行UI相关代码，而不再执行世界逻辑中的代码。  
  这就是UI穿透问题。这是不可以出现的问题。  
  我们必须在打开UI后，不在执行世界逻辑中的代码。

* 解决方案  
  第一种：在世界逻辑中检测是否当前鼠标下又object；  

  ```cs
    int fid = Input.GetTouch(0).fingerId;
    // fid 是多触控功能下的
    if (EventSystem.current.IsPointerOverGameObject(fid))
    {
        return;
    }
  ```

  第二种：配合EventSystem和Canvas对象上的GraphicRaycast；

  ```cs
    public bool ClickedUI()
    {
        PointerEventData eventData = new PointerEventData(Main_eventSystem);
        eventData.pressPosition = Input.mousePosition;
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        Main.Instance.Main_graphicRaycaster.Raycast(eventData, results);
        return results.Count > 0;
    }
  ```

  通过被射线检测到物体的数量来判断是否点到UI；

  以上两种方法，在canvas的camera-screen模式下，即使物体遮挡住UI，也会认为点击到UI。在world模式下则，若被遮挡则认为没有点到UI。