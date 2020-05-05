using System.Collections.Generic;
using BookShopApi.Models;
using BookShopApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;

        public BooksController(BookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public ActionResult<List<Book>> Get() =>
            _bookService.Get();

        [HttpGet("{id:length(24)}", Name = "GetBook")]
        public ActionResult<Book> Get(string id)
        {
            var book = _bookService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{page}/{pageSize}")]
        public ActionResult<ResponseQueryBooks> GetBooksByQuery(int page, int pageSize) =>
         _bookService.GetBooksWithQuery(page, pageSize);

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult<ResponseGeneral> Create(Book book)
        {
            _bookService.Create(book);
            return new ResponseGeneral() { success = true, message = "Book added" };
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id:length(24)}")]
        public ActionResult<ResponseGeneral> Update(string id, Book bookIn)
        {
            var book = _bookService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            _bookService.Update(id, bookIn);

            return new ResponseGeneral() { success = true, message = "Book updated" };
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id:length(24)}")]
        public ActionResult<ResponseGeneral> Delete(string id)
        {
            var book = _bookService.Get(id);

            if (book == null)
            {
                return NotFound();
            }

            _bookService.Remove(book._id);

            return new ResponseGeneral() { success = true, message = "Book deleted" };
        }
    }
}
