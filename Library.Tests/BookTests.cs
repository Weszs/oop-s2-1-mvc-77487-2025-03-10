using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library_Domain;
using Library.MVC.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Library.Tests
{
    public class BookTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task CanCreateBook()
        {
            // Arrange
            var db = GetInMemoryDbContext();
            var book = new Book
            {
                Title = "The Great Gatsby",
                Author = "F. Scott Fitzgerald",
                Isbn = "9780743273565",
                Category = "Fiction",
                IsAvailable = true
            };

            // Act
            db.Books.Add(book);
            await db.SaveChangesAsync();

            // Assert
            var savedBook = await db.Books.FirstOrDefaultAsync(b => b.Title == "The Great Gatsby");
            Assert.NotNull(savedBook);
            Assert.Equal("F. Scott Fitzgerald", savedBook.Author);
        }

        [Fact]
        public async Task CanRetrieveAllBooks()
        {
            // Arrange
            var db = GetInMemoryDbContext();
            var books = new List<Book>
            {
                new Book { Title = "Book 1", Author = "Author 1", IsAvailable = true },
                new Book { Title = "Book 2", Author = "Author 2", IsAvailable = true },
                new Book { Title = "Book 3", Author = "Author 3", IsAvailable = false }
            };

            db.Books.AddRange(books);
            await db.SaveChangesAsync();

            // Act
            var allBooks = await db.Books.ToListAsync();

            // Assert
            Assert.Equal(3, allBooks.Count);
        }

        [Fact]
        public async Task CanFilterAvailableBooks()
        {
            // Arrange
            var db = GetInMemoryDbContext();
            var books = new List<Book>
            {
                new Book { Title = "Available 1", Author = "Author 1", IsAvailable = true },
                new Book { Title = "Available 2", Author = "Author 2", IsAvailable = true },
                new Book { Title = "OnLoan", Author = "Author 3", IsAvailable = false }
            };

            db.Books.AddRange(books);
            await db.SaveChangesAsync();

            // Act
            var availableBooks = await db.Books
                .Where(b => b.IsAvailable)
                .ToListAsync();

            // Assert
            Assert.Equal(2, availableBooks.Count);
            Assert.All(availableBooks, b => Assert.True(b.IsAvailable));
        }

        [Fact]
        public async Task CanSearchBooksByTitle()
        {
            // Arrange
            var db = GetInMemoryDbContext();
            var books = new List<Book>
            {
                new Book { Title = "The Hobbit", Author = "J.R.R. Tolkien", IsAvailable = true },
                new Book { Title = "Harry Potter", Author = "J.K. Rowling", IsAvailable = true },
                new Book { Title = "The Lord of the Rings", Author = "J.R.R. Tolkien", IsAvailable = true }
            };

            db.Books.AddRange(books);
            await db.SaveChangesAsync();

            // Act
            var searchResults = await db.Books
                .Where(b => b.Title.Contains("The"))
                .ToListAsync();

            // Assert
            Assert.Equal(2, searchResults.Count);
        }

        [Fact]
        public async Task CanDeleteBook()
        {
            // Arrange
            var db = GetInMemoryDbContext();
            var book = new Book { Title = "To Delete", Author = "Author", IsAvailable = true };
            db.Books.Add(book);
            await db.SaveChangesAsync();

            // Act
            db.Books.Remove(book);
            await db.SaveChangesAsync();

            // Assert
            var deletedBook = await db.Books.FirstOrDefaultAsync(b => b.Title == "To Delete");
            Assert.Null(deletedBook);
        }
    }
}
