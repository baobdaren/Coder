class Sth:
    def __init__(self, n):
        print("INIT " + n)
        super().__init__()
        self.name = n
    
    def setName(self, n):
        self.name = n
    

arr = [Sth("jack"), Sth("tom"), Sth("jerry")]

n = arr[0:1]

print(n[0])
print(arr[0])