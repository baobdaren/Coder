# Unity消息执行顺序

1. Awake 初始化
2. OnEnable 重新启用时会再次执行
3. Start 只有第一次激活会被执行，之后启用组件时，不再执行。
4. FixedUpdate 可能会执行多次，取决于Fixed间隔和帧间隔。
5. Update
6. LateUpdate
7. OnDisable
8. OnDestroy

## FixedUpdate和Update

FixedUpdate：总是在固定间隔时间下执行，即每一次Time.deltaTime都相等。但也不能说是现实时间。这种理解是错误的。
Update：每一次deltaTime可能不等，无法保证。
