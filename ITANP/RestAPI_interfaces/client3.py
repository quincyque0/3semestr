import requests

print("Получение списка ресурсов")
try:
    response = requests.get('http://127.0.0.1:8000')

    response.raise_for_status()
    if response.status_code == 200:
        print("Status code 200 OK")
    else:
        print(f"Status code {response.status_code}")

    data = response.json()

    for i in data:
        print(i["title"])
    print("\n")

except requests.exceptions.RequestException as a:
    print(a)




print("Получение одного ресурса")
try:
    response1 = requests.get('https://jsonplaceholder.typicode.com/posts/1')

    response1.raise_for_status()
    if response1.status_code == 200:
        print("Status code 200 OK")
    else:
        print(f"Status code {response1.status_code}")
    data1 = response1.json()


    for i in data1:
        for key, value in data1.items():
            print(f"{key}: {value}\n")
    print("\n")
except requests.exceptions.RequestException as a:
    print(a)



print("Создание нового ресурса")
try:
    headers = {
        "Content-Type": "application/json"
    }
    dic = dict(title = 'title_info', body = 'body_info', id = 0)
    response2 = requests.post("https://jsonplaceholder.typicode.com/posts", headers=headers, json=dic)

    response2.raise_for_status()
    if response2.status_code == 201:
        print("Status code 201 Created")
    else:
        print(f"Status code {response2.status_code}")
        print("\n")
except requests.exceptions.RequestException as a:
    print(a)
