n = int(input())
a = list(map(int,input().split()))
j = 1
cur = 0
for i in a:
    if(i - j > 0):
        cur += i - j
        j+=1
print(cur)