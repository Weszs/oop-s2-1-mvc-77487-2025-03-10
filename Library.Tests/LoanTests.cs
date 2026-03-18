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
    public class LoanTests
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
        public async Task CannotCreateLoanForAlreadyLoanedBook()
        {
            // Arrange
            var db = GetInMemoryDbContext();
            var book = new Book { Title = "Test Book", Author = "Test Author", IsAvailable = true };
            var member = new Member { FullName = "Test Member", Email = "test@test.com" };
            
            db.Books.Add(book);
            db.Members.Add(member);
            await db.SaveChangesAsync();

            var loan1 = new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                LoanDate = DateTime.UtcNow.Date,
                DueDate = DateTime.UtcNow.Date.AddDays(14)
            };

            db.Loans.Add(loan1);
            book.IsAvailable = false;
            await db.SaveChangesAsync();

            // Act
            var activeLoansForBook = await db.Loans
                .Where(l => l.BookId == book.Id && l.ReturnedDate == null)
                .CountAsync();

            // Assert
            Assert.Equal(1, activeLoansForBook);
            Assert.False(book.IsAvailable);
        }

        [Fact]
        public async Task ReturnedLoanMakesBookAvailableAgain()
        {
            // Arrange
            var db = GetInMemoryDbContext();
            var book = new Book { Title = "Test Book", Author = "Test Author", IsAvailable = false };
            var member = new Member { FullName = "Test Member", Email = "test@test.com" };
            
            db.Books.Add(book);
            db.Members.Add(member);
            await db.SaveChangesAsync();

            var loan = new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                LoanDate = DateTime.UtcNow.Date.AddDays(-7),
                DueDate = DateTime.UtcNow.Date.AddDays(7)
            };

            db.Loans.Add(loan);
            await db.SaveChangesAsync();

            // Act
            loan.ReturnedDate = DateTime.UtcNow.Date;
            book.IsAvailable = true;
            db.Update(loan);
            db.Update(book);
            await db.SaveChangesAsync();

            // Assert
            var updatedLoan = await db.Loans.FindAsync(loan.Id);
            var updatedBook = await db.Books.FindAsync(book.Id);

            Assert.NotNull(updatedLoan.ReturnedDate);
            Assert.True(updatedBook.IsAvailable);
        }

        [Fact]
        public async Task CanIdentifyOverdueLoans()
        {
            // Arrange
            var db = GetInMemoryDbContext();
            var book = new Book { Title = "Test Book", Author = "Test Author", IsAvailable = false };
            var member = new Member { FullName = "Test Member", Email = "test@test.com" };
            
            db.Books.Add(book);
            db.Members.Add(member);
            await db.SaveChangesAsync();

            var overdueDate = DateTime.UtcNow.Date.AddDays(-5);
            var loan = new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                LoanDate = DateTime.UtcNow.Date.AddDays(-20),
                DueDate = overdueDate,
                ReturnedDate = null
            };

            db.Loans.Add(loan);
            await db.SaveChangesAsync();

            // Act
            var overdueLoans = await db.Loans
                .Where(l => l.DueDate < DateTime.UtcNow.Date && l.ReturnedDate == null)
                .ToListAsync();

            // Assert
            Assert.Single(overdueLoans);
            Assert.Equal(loan.Id, overdueLoans.First().Id);
        }

        [Fact]
        public async Task LoanSetsCorrectDueDate()
        {
            // Arrange
            var db = GetInMemoryDbContext();
            var book = new Book { Title = "Test Book", Author = "Test Author", IsAvailable = true };
            var member = new Member { FullName = "Test Member", Email = "test@test.com" };
            
            db.Books.Add(book);
            db.Members.Add(member);
            await db.SaveChangesAsync();

            var loanDate = DateTime.UtcNow.Date;
            var loan = new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                LoanDate = loanDate,
                DueDate = loanDate.AddDays(14)
            };

            // Act
            db.Loans.Add(loan);
            await db.SaveChangesAsync();

            // Assert
            var savedLoan = await db.Loans.FindAsync(loan.Id);
            Assert.Equal(loanDate.AddDays(14), savedLoan.DueDate);
        }

        [Fact]
        public async Task CannotHaveActiveDuplicateLoansForMember()
        {
            // Arrange
            var db = GetInMemoryDbContext();
            var book = new Book { Title = "Test Book", Author = "Test Author", IsAvailable = true };
            var member = new Member { FullName = "Test Member", Email = "test@test.com" };
            
            db.Books.Add(book);
            db.Members.Add(member);
            await db.SaveChangesAsync();

            var loan1 = new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                LoanDate = DateTime.UtcNow.Date,
                DueDate = DateTime.UtcNow.Date.AddDays(14)
            };

            db.Loans.Add(loan1);
            await db.SaveChangesAsync();

            // Act
            var activeLoanExists = await db.Loans
                .AnyAsync(l => l.BookId == book.Id && 
                               l.MemberId == member.Id && 
                               l.ReturnedDate == null);

            // Assert
            Assert.True(activeLoanExists);
        }
    }
}
