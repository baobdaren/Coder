num = 78

jinZhi = 8

r = []

def trans(m, n):
    if m/n == 0:
        return
    tmp = m%n
    trans(m/n, n)
    r.append(tmp)

trans(num, jinZhi)

print(r)