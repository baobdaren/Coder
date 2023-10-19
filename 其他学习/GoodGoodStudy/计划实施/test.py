class GrandFather:
    pass

class Father(GrandFather):
    pass

class Monther:
    pass

class Child(Father, Monther):
    pass

# print(GrandFather.__subclasses__()[0].__subclasses__())
print(Child().__dir__())