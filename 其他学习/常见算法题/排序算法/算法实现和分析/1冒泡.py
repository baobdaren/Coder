# 冒泡排序
# 它重复地走访过要排序的元素列，依次比较两个相邻的元素，
# 如果顺序（如从大到小、首字母从Z到A）错误就把他们交换过来。
# 走访元素的工作是重复地进行直到没有相邻元素需要交换，也就是说该元素列已经排序完成。

import random

arr = []
for i in range(10):
    arr.append(random.randint(1,100))

# ////////////////////////////

def popSortFunc():
    for i in range(len(arr)):
        for j in range(len(arr) - i - 1):
            if arr[j] > arr[j+1]:
                arr[j],arr[j+1] = arr[j+1],arr[j]

popSortFunc()
print(arr)

# 