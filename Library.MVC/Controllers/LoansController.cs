using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library_Domain;
using Library.MVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.MVC.Controllers
{
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LoansController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Loans
        public async Task<IActionResult> Index()
        {
            var loans = await _db.Loans.Include(l => l.Book).Include(l => l.Member).ToListAsync();
            return View(loans);
        }

        // GET: Loans/Create
        public async Task<IActionResult> Create()
        {
            var availableBooks = await _db.Books.Where(b => b.IsAvailable).ToListAsync();
            var members = await _db.Members.ToListAsync();

            ViewBag.AvailableBooks = availableBooks;
            ViewBag.Members = members;

            return View();
        }

        // POST: Loans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,MemberId")] Loan loan)
        {
            var book = await _db.Books.FindAsync(loan.BookId);
            if (book == null || !book.IsAvailable)
            {
                ModelState.AddModelError(string.Empty, "Book is not available for loan.");
            }

            if (ModelState.IsValid)
            {
                loan.LoanDate = DateTime.UtcNow.Date;
                loan.DueDate = loan.LoanDate.AddDays(14);
                _db.Loans.Add(loan);
                book.IsAvailable = false;
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var availableBooks = await _db.Books.Where(b => b.IsAvailable).ToListAsync();
            var members = await _db.Members.ToListAsync();

            ViewBag.AvailableBooks = availableBooks;
            ViewBag.Members = members;

            return View(loan);
        }

        // POST: Loans/Return/5
        [HttpPost]
        public async Task<IActionResult> Return(int id)
        {
            var loan = await _db.Loans.FindAsync(id);
            if (loan != null && !loan.ReturnedDate.HasValue)
            {
                loan.ReturnedDate = DateTime.UtcNow.Date;
                var book = await _db.Books.FindAsync(loan.BookId);
                if (book != null)
                {
                    book.IsAvailable = true;
                }
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
