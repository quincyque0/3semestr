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
    def do_POST(self):

        if self.path == '/submit':
            self.handle_form_submission()
    def send_form(self):

        self.send_response(200)
        self.send_header('Content-type', 'text/html; charset=utf-8')
        self.end_headers()
        
        html = '''
        <html>
            <head><title>Форма обратной связи</title></head>
            <body>
                <h1>Обратная связь</h1>
                <form action="/submit" method="post">
                    <p>
                        <label>Имя:</label><br>
                        <input type="text" name="name" required>
                    </p>
                    <p>
                        <label>Email:</label><br>
                        <input type="email" name="email" required>
                    </p>
                    <p>
                        <label>Сообщение:</label><br>
                        <textarea name="message" rows="5" cols="50"></textarea>
                    </p>
                    <p>
                        <button type="submit">Отправить</button>
                    </p>
                </form>
            </body>
        </html>
        '''
        
        self.wfile.write(html.encode('utf-8'))

    def Jsonparse(self, data):
        with open('data.json','w+') as file:
            json.dump(data,file)         
    
    def handle_form_submission(self):
        content_length = int(self.headers['Content-Length'])
        post_data = self.rfile.read(content_length).decode('utf-8')
        
        # Парсинг данных формы
        form_data = urllib.parse.parse_qs(post_data)
        
        
        # Валидация
        if not form_data.get('name') or not form_data.get('email'):
            self.send_error(400, "Имя и email обязательны")
            return
        
        # Обработка данных (в реальном приложении - сохранение в БД)
        print(f"Получены данные: {form_data}")
        self.Jsonparse(form_data)
        
        # Перенаправление на страницу успеха
        self.send_response(302)
        self.send_header('Location', '/success')
        self.end_headers()
    def send_success_page(self):
        
        self.send_response(200)
        self.send_header('Content-type', 'text/html; charset=utf-8')
        self.end_headers()
        html = '''
        <html>
            <head><title>Успешно</title></head>
            <body>
                <h1>Вы успешно зарегестрировались</h1>
                <button type="button">check</button>
                
            </body>
        </html>
        '''

        
server = HTTPServer(('localhost', 8000), MyHandler)
print("Сервер запущен на http://localhost:8000")
server.serve_forever()