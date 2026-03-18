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
    public class MemberTests
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
        public async Task CanCreateMember()
        {
            // Arrange
            var db = GetInMemoryDbContext();
            var member = new Member
            {
                FullName = "John Doe",
                Email = "john@example.com",
                Phone = "123-456-7890"
            };

            // Act
            db.Members.Add(member);
            await db.SaveChangesAsync();

            // Assert
            var savedMember = await db.Members.FirstOrDefaultAsync(m => m.FullName == "John Doe");
            Assert.NotNull(savedMember);
            Assert.Equal("john@example.com", savedMember.Email);
        }

        [Fact]
        public async Task CanRetrieveAllMembers()
        {
            // Arrange
            var db = GetInMemoryDbContext();
            var members = new List<Member>
            {
                new Member { FullName = "Member 1", Email = "member1@test.com" },
                new Member { FullName = "Member 2", Email = "member2@test.com" },
                new Member { FullName = "Member 3", Email = "member3@test.com" }
            };

            db.Members.AddRange(members);
            await db.SaveChangesAsync();

            // Act
            var allMembers = await db.Members.ToListAsync();

            // Assert
            Assert.Equal(3, allMembers.Count);
        }

        [Fact]
        public async Task MemberHasValidEmail()
        {
            // Arrange
            var db = GetInMemoryDbContext();
            var member = new Member
            {
                FullName = "Test Member",
                Email = "test@example.com"
            };

            // Act
            db.Members.Add(member);
            await db.SaveChangesAsync();

            // Assert
            var savedMember = await db.Members.FirstOrDefaultAsync(m => m.Email == "test@example.com");
            Assert.NotNull(savedMember);
            Assert.Contains("@", savedMember.Email);
        }

        [Fact]
        public async Task CanDeleteMember()
        {
            // Arrange
            var db = GetInMemoryDbContext();
            var member = new Member { FullName = "To Delete", Email = "delete@test.com" };
            db.Members.Add(member);
            await db.SaveChangesAsync();

            // Act
            db.Members.Remove(member);
            await db.SaveChangesAsync();

            // Assert
            var deletedMember = await db.Members.FirstOrDefaultAsync(m => m.FullName == "To Delete");
            Assert.Null(deletedMember);
        }

        [Fact]
        public async Task MemberCanHaveMultipleLoans()
        {
            // Arrange
            var db = GetInMemoryDbContext();
            var member = new Member { FullName = "Avid Reader", Email = "reader@test.com" };
            var book1 = new Book { Title = "Book 1", Author = "Author 1", IsAvailable = false };
            var book2 = new Book { Title = "Book 2", Author = "Author 2", IsAvailable = false };

            db.Members.Add(member);
            db.Books.AddRange(book1, book2);
            await db.SaveChangesAsync();

            var loan1 = new Loan { BookId = book1.Id, MemberId = member.Id, LoanDate = DateTime.UtcNow.Date, DueDate = DateTime.UtcNow.Date.AddDays(14) };
            var loan2 = new Loan { BookId = book2.Id, MemberId = member.Id, LoanDate = DateTime.UtcNow.Date, DueDate = DateTime.UtcNow.Date.AddDays(14) };

            db.Loans.AddRange(loan1, loan2);
            await db.SaveChangesAsync();

            // Act
            var memberLoans = await db.Loans.Where(l => l.MemberId == member.Id).ToListAsync();

            // Assert
            Assert.Equal(2, memberLoans.Count);
        }
    }
}
