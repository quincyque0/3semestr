n = int(input())
s = 0
cur = 0
a = list(map(int,input().split()))
for i in range(n):
    cur += a[i]
    s = min(s,cur)
print(abs(s))