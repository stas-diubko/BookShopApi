using System.Collections.Generic;
using System.Linq;
using BookShopApi.Models;
using MongoDB.Driver;

namespace BookShopApi.Services
{
    public class BookService
    {
        private readonly IMongoCollection<Book> _books;

        public BookService(IBookstoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _books = database.GetCollection<Book>("books");
        }

        public List<Book> Get() =>
            _books.Find(book => true).ToList();

        public Book Get(string id) =>
            _books.Find<Book>(book => book._id == id).FirstOrDefault();

        public Book Create(Book book)

        {
            _books.InsertOne(book);
            return book;
        }

        public void Update(string id, Book bookIn) =>
            _books.ReplaceOne(book => book._id == id, bookIn);

        public void Remove(Book bookIn) =>
            _books.DeleteOne(book => book._id == bookIn._id);

        public void Remove(string id) =>
            _books.DeleteOne(book => book._id == id);
    }
}
