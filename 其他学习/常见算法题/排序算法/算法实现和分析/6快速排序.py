# 快速排序
# 快速排序的基本思想：通过一趟排序将待排记录分隔成独立的两部分，
# 其中一部分记录的关键字均比另一部分的关键字小，
# 则可分别对这两部分记录继续进行排序，以达到整个序列有序。
import random

arr = [49, 38, 65, 97, 76, 13, 27]
# for i in range(10):
#     arr.append(random.randint(1,10))
print(arr)

def quickSort(start, end):
    if start >= end:
        return
    left,right = start,end
    mid = arr[left] # 中间元素 这里也意味着吧中间元素取出来，第一次开始的方向必须在另一边
    while(left<right):
        while left < right and arr[right] >= mid:
            right-=1
        arr[left] = arr[right]
        while left < right and arr[left] <= mid:
            left+=1
        arr[right] = arr[left]

    arr[right] = mid
    quickSort(start, left-1)
    quickSort(left+1, end)
    # 一定要注意，中间元素必须在下次排序中去除，否则会导致死循环
    # 

quickSort(0, len(arr)-1)

print(arr)