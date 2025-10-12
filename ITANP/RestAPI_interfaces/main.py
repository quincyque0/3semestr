import requests
from pydantic import BaseModel
from fastapi import FastAPI, HTTPException, status
from typing import List, Optional

class Book(BaseModel):
    id: int
    author: str
    title: str
    publication_year: int
    
class BookCreate(BaseModel):
    title: str
    author: str
    publication_year: int

app = FastAPI()


books = [
    Book(id=0, title="Ревизор", author="Гоголь", publication_year=1835),
    Book(id=1, title="Cкубиду", author="Шварцнегер", publication_year=2000),
    Book(id=2, title="Армяне и их повадки", author="Дикаприо", publication_year=2222)
]

@app.get("/books", response_model=List[Book])
def get_books_api():
    return books
@app.get("/books/{book_id}", response_model=Book)
def get_book(book_id: int):
    try:
        return books[book_id]
    except IndexError:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail="Book not found"
        )
@app.post("/books" , response_model=Book, status_code=201)
def create_book(book: BookCreate):
    _id = len(books)
    book = Book(
        id=_id,
        title=book.title,
        author=book.author,
        publication_year=book.publication_year
    )
    books.append(book)
    return book
