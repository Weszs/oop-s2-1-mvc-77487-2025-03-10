using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library_Domain;
using Library.MVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.MVC.Controllers
{
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MembersController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Members
        public async Task<IActionResult> Index()
        {
            var members = await _db.Members.ToListAsync();
            return View(members);
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Members/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Email,Phone")] Member member)
        {
            if (ModelState.IsValid)
            {
                _db.Members.Add(member);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var member = await _db.Members.FindAsync(id);
            if (member == null)
                return NotFound();

            return View(member);
        }

        // POST: Members/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Email,Phone")] Member member)
        {
            if (id != member.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(member);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var member = await _db.Members.FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
                return NotFound();

            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _db.Members.FindAsync(id);
            if (member != null)
            {
                _db.Members.Remove(member);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _db.Members.Any(e => e.Id == id);
        }
    }
}
