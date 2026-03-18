using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Library_Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.MVC.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var db = services.GetRequiredService<ApplicationDbContext>();

            // Apply pending migrations
            try
            {
                await db.Database.MigrateAsync();
            }
            catch
            {
                // ignore migration errors in some environments
            }

            // Seed Admin role and user
            const string adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            var adminEmail = "admin@library.local";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                await userManager.CreateAsync(admin, "Admin123!");
                await userManager.AddToRoleAsync(admin, adminRole);
            }

            // Seed domain data if missing
            if (!await db.Books.AnyAsync() && !await db.Members.AnyAsync())
            {
                var categories = new[] { "Fiction", "Non-fiction", "Science", "History", "Children", "Mystery" };

                var bookFaker = new Faker<Book>()
                    .RuleFor(b => b.Title, f => f.Lorem.Sentence(3, 4))
                    .RuleFor(b => b.Author, f => f.Person.FullName)
                    .RuleFor(b => b.Isbn, f => f.Commerce.Ean13())
                    .RuleFor(b => b.Category, f => f.PickRandom(categories))
                    .RuleFor(b => b.IsAvailable, true);

                var memberFaker = new Faker<Member>()
                    .RuleFor(m => m.FullName, f => f.Person.FullName)
                    .RuleFor(m => m.Email, (f, m) => f.Internet.Email(m.FullName))
                    .RuleFor(m => m.Phone, f => f.Phone.PhoneNumber());

                var books = bookFaker.Generate(20);
                var members = memberFaker.Generate(10);

                await db.Books.AddRangeAsync(books);
                await db.Members.AddRangeAsync(members);
                await db.SaveChangesAsync();

                // Create loans: up to 15 or limited by available books
                var random = new Random();
                var availableBooks = await db.Books.Where(b => b.IsAvailable).ToListAsync();
                var loansToCreate = Math.Min(15, availableBooks.Count);

                var loans = new System.Collections.Generic.List<Loan>();
                for (int i = 0; i < loansToCreate; i++)
                {
                    var book = availableBooks[i];
                    var member = members[random.Next(members.Count)];

                    var loanDate = DateTime.UtcNow.Date.AddDays(-random.Next(1, 40));
                    var dueDate = loanDate.AddDays(14);

                    // Some returned, some active
                    DateTime? returned = null;
                    var returnedChance = random.NextDouble();
                    if (returnedChance > 0.5)
                    {
                        // returned sometime after loan date
                        returned = loanDate.AddDays(random.Next(1, 30));
                    }

                    var loan = new Loan
                    {
                        BookId = book.Id,
                        MemberId = member.Id,
                        LoanDate = loanDate,
                        DueDate = dueDate,
                        ReturnedDate = returned
                    };

                    // mark book availability
                    if (returned == null)
                        book.IsAvailable = false;

                    loans.Add(loan);
                }

                await db.Loans.AddRangeAsync(loans);
                await db.SaveChangesAsync();
            }
        }
    }
}
