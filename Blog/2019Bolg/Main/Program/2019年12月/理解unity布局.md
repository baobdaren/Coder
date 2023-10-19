# 阅读官方手册

## 注释

* 这里阅读unity官方手册中，关于布局的内容
* 中英文翻译

## 正文

* 官方描述1：Understanding Layout Elements

>The auto layout system is based on a concept of layout elements and layout controllers. A layout element is an Game Object with a Rect Transform and optionally other components as well. The layout element has certain knowledge about which size it should have. Layout elements don’t directly set their own size, but other components that function as layout controllers can use the information they provide in order to calculate a size to use for them.

* 翻译：  
    自动布局组件是建立在布局元素和布局器上的概念。布局元素是包含RectTransform组件和其他可选组件的GameObject。布局元素应该准确地知道该有地尺寸。布局元素不会直接设置它们地尺寸，但其他的布局器组件可以使用布局元素提供的信息来计算为他们使用地尺寸。

---

* 官方描述2：Understanding Layout Controllers

>Layout controllers are components that control the sizes and possibly positions of one or more layout elements, meaning Game Objects with Rect Transforms on. A layout controller may control its own layout element (the same Game Object it is on itself) or it may control child layout elements.  
>A component that functions as a layout controller may also itself function as a layout element at the same time.

* 翻译：
    布局器是控制一或多个布局元素尺寸及位置（可能）的组件，？？？。一个布局器可能控制自己的布局元素（在它自己身上的相同GameObject），或者其也许控制子布局元素。  
    布局器组件可能同时也是一个布局元素