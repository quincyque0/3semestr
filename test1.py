n = int(input())
cur = 0
for i in range(n):
    l,r = map(int,input().split())
    if l > cur:
        cur = l + r
    else: cur += r
print(cur)