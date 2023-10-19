# 希尔排序是把记录按下标的一定增量分组，对每组使用直接插入排序算法排序；
# 随着增量逐渐减少，每组包含的关键词越来越多，
# 当增量减至1时，整个文件恰被分成一组，算法便终止。
import random

arr = []
for i in range(100):
    arr.append(random.randint(1,10))
print(arr)

def shell():
    moveCount = 0
    n = len(arr)
    gap = n//2
    while gap > 0: # 步长
        for i in range(0, n-gap): # 这个步长的分组下，对每一个分组内部进行插入排序
            while(i>=0 and arr[i]>arr[i+gap]): # 当需要交换时并且防止往前推出首元素
                arr[i],arr[i+gap] = arr[i+gap],arr[i] # 交换
                i -= gap # 交换后需要往前继续检测，以保证整个组内的大小顺序
        gap //= 2

shell()
print(arr)