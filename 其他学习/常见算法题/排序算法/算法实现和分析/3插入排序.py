# 插入排序（Insertion-Sort）的算法描述是一种简单直观的排序算法。
# 它的工作原理是通过构建有序序列，对于未排序数据，在已排序序列中从后向前扫描，找到相应位置并插入。

import random

arr = []
for i in range(10):
    arr.append(random.randint(1,100))
print(arr)
# ////////////////////////////

def InsertionSortFunc():
    for i in range(len(arr)):
        for j in range(i):
            if arr[i] > arr[j]:
                arr[i],arr[j] = arr[j],arr[i]

InsertionSortFunc()
print(arr)