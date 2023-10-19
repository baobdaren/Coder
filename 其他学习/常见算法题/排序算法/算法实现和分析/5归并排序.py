# 归并排序

import random

arr = []
for i in range(10):
    arr.append(random.randint(1,100))
print(arr)
# ////////////////////////////

def sort(ls):
    ln = len(ls)
    if len(ls) <= 1:
        return ls
    l = sort(ls[0:(ln//2)])
    r = sort(ls[(ln//2):])
    return mergeSort(l,r)

def mergeSort(larr, rarr):
    narr = []
    lindex,rindex = 0,0
    while lindex<len(larr) and rindex<len(rarr):
        if larr[lindex] < rarr[rindex]:
            narr.append(larr[lindex])
            lindex+=1
        else:
            narr.append(rarr[rindex])
            rindex+=1
    narr += larr[lindex:]
    narr += rarr[rindex:]
    return narr

result = sort(arr)

print(result)