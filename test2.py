n = int(input())
print((n+2)//2)
if n % 2 == 0:
    for i in range(0,n//2):
        print(1 + i*2,end =" ")
    print(n)
else:
    for i in range(0,(int(n//2) + 1)):
        print(1 + i*2,end=" ")
    