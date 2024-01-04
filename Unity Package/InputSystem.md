# unity输入系统InputSystem
InputSystem是用于取代旧的InputManager的方案，采用输入映射机制。

# InputActions asset
* 该资产用于设计映射表，该表表示了某个类型的方法是哪些设备事件的调用，例如Keyborad.f和Dpad.lr都对应事件Fire
* Action 表示事件，其类型Action.type包括button（按一下），value（各种类型的值），Pass Through（）
* Action 的Type不同，其可以绑定的设备事件不同。如type=value vector2时，可以绑定方向键？？？
* Action 的Addbing可以选择多键组合 binding with one modifier：即绑定时具有一个修饰键。

# PlayerInput组件
PlayerInput组件用于接受玩家输入。  
* actions：action的 InputAction asset
* Behaviour：设置如何响应，包括发消息，广播，invokeUnityEvent，InvokeCSharpEvent。
* InvokeUnityEvent：需要在展开列表手动拖拽监听事件，响应方法必须具有一个InputAction.CallBackContext 参数。
* InvokeCSharpEvent：需要在代码中获取PlayerInput组件上的assets，并Find(str name)找到要监听的Action，并监听其事件（start performed cancel 依次调用）。
```cs
GetComponent<PlayerInput>().actions.FindAction("Fire").started += TestInputSystem_started;
```
