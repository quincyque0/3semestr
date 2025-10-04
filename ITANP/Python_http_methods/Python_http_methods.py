from http.server import BaseHTTPRequestHandler, HTTPServer
import urllib.parse
import json
import os

class MyHandler(BaseHTTPRequestHandler):
    
    def do_GET(self):   
        if self.path == '/':
            self.send_form()
        elif self.path == '/success':
            self.send_success_page()
        elif self.path == "/about":
            self.send_about_page()
        elif self.path == "/contact":
            self.send_contact_page()
        else: 
            self.send_error(404, "Page not found")

    
    def do_POST(self):
        if self.path == '/submit':
            self.handle_form_submission()
    # Открытие стандартной формы


    def send_form(self):
        try:
            self.send_response(200)
            self.send_header('Content-type', 'text/html; charset=utf-8')
            self.end_headers()
            
            with open("templates/main.html", "r", encoding="utf-8") as html_file:
                html = html_file.read()
            
            self.wfile.write(html.encode('utf-8'))
        except FileNotFoundError:
            self.send_error(404, "Main page not found")

    #сохраненив типо бд(json)

    def Jsonparse(self, data):
        with open('data.json','w+') as file:
            json.dump(data,file)
    
    def handle_form_submission(self):
        try:
            content_length = int(self.headers['Content-Length'])
            post_data = self.rfile.read(content_length).decode('utf-8')
            
            # Парсинг данных формы
            form_data = urllib.parse.parse_qs(post_data)
            
            # Извлекаем значения (parse_qs возвращает списки)
            name = form_data.get('name', [''])[0]
            email = form_data.get('email', [''])[0]
            message = form_data.get('message', [''])[0]
            
            # Валидация
            if not name or not email:
                self.send_error(400, "Имя и email обязательны")
                return
            
            # Обработка данных
            user_data = {
                'name': name,
                'email': email,
                'message': message
            }
            
            print(f"Получены данные: {user_data}")
            self.Jsonparse(user_data)
            
            # Перенаправление на страницу успеха
            self.send_response(302)
            self.send_header('Location', '/success')
            self.end_headers()
        except Exception as e:
            self.send_error(500, f"Ошибка: {str(e)}")

    # Открытие страницы успешной регистрации

    def send_success_page(self):
        self.send_response(200)
        self.send_header('Content-type', 'text/html; charset=utf-8')
        self.end_headers()
        html = '''
        <html>
            <head>
                <title>Успешно</title>
                <meta charset="utf-8">
            </head>
            <body>
                <h1>Вы успешно зарегистрировались</h1>
                <a href="/">Вернуться на главную</a>
            </body>
        </html>
        '''
        self.wfile.write(html.encode('utf-8'))

    # Открытие страницы о нас

    def send_about_page(self):
        try:
            self.send_response(200)
            self.send_header('Content-type', 'text/html; charset=utf-8')
            self.end_headers()
            
            with open("templates/about.html", "r", encoding="utf-8") as html_file:
                html = html_file.read()
            
            self.wfile.write(html.encode('utf-8'))
        except FileNotFoundError:
            self.send_error(404, "About page not found")

    # Открытие страницы контактов
    def send_contact_page(self):
        try:
            self.send_response(200)
            self.send_header('Content-type', 'text/html; charset=utf-8')
            self.end_headers()
            
            with open("templates/contact.html", "r", encoding="utf-8") as html_file:
                html = html_file.read()
            
            self.wfile.write(html.encode('utf-8'))
        except FileNotFoundError:
            self.send_error(404, "Contact page not found")


# Запуск сервера
if __name__ == '__main__':
    server = HTTPServer(('localhost', 8000), MyHandler)
    print("Сервер запущен на http://localhost:8000")
    server.serve_forever()