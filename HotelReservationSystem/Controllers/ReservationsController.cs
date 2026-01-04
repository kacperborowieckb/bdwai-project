using HotelReservationSystem.Data;
using HotelReservationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservationSystem.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            IQueryable<Reservation> reservations = _context.Reservations.Include(r => r.Guest).Include(r => r.Room);

            if (!User.IsInRole("Admin"))
            {
                reservations = reservations.Where(r => r.Guest.Email == User.Identity.Name);
            }

            return View(await reservations.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var reservation = await _context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Room)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reservation == null) return NotFound();

            // regular users can only see their own details
            if (!User.IsInRole("Admin") && reservation.Guest?.Email != User.Identity.Name)
            {
                return Forbid();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["RoomId"] = new SelectList(_context.Rooms.Where(r => r.IsAvailable), "Id", "RoomNumber");

            if (User.IsInRole("Admin"))
            {
                ViewData["GuestId"] = new SelectList(_context.Guests, "Id", "Email");
            }
            else
            {
                var currentGuest = _context.Guests.FirstOrDefault(g => g.Email == User.Identity.Name);
                ViewBag.CurrentGuestId = currentGuest?.Id ?? 0;
            }

            return View();
        }

        // POST: Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartDate,EndDate,RoomId,GuestId")] Reservation reservation)
        {
            var currentGuest = _context.Guests.FirstOrDefault(g => g.Email == User.Identity.Name);

            if (!User.IsInRole("Admin"))
            {
                if (currentGuest == null)
                {
                    currentGuest = new Guest
                    {
                        Email = User.Identity.Name,
                        FirstName = "Guest",
                        LastName = User.Identity.Name
                    };
                    _context.Guests.Add(currentGuest);
                    await _context.SaveChangesAsync();
                }
                reservation.GuestId = currentGuest.Id;
            }

            if (reservation.StartDate >= reservation.EndDate)
            {
                ModelState.AddModelError("", "End Date must be after Start Date.");
            }

            var isBooked = _context.Reservations.Any(r => r.RoomId == reservation.RoomId &&
                           reservation.StartDate < r.EndDate && r.StartDate < reservation.EndDate);

            if (isBooked)
            {
                ModelState.AddModelError("", "This room is already booked for these dates.");
            }

            ModelState.Remove("Room");
            ModelState.Remove("Guest");

            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", reservation.RoomId);
            ViewData["GuestId"] = new SelectList(_context.Guests, "Id", "Email", reservation.GuestId);

            return View(reservation);
        }

        // GET: Reservations/Edit/5

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            ViewData["GuestId"] = new SelectList(_context.Guests, "Id", "Email", reservation.GuestId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", reservation.RoomId);

            return View(reservation);

        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate,RoomId,GuestId")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["GuestId"] = new SelectList(_context.Guests, "Id", "Email", reservation.GuestId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", reservation.RoomId);

            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Room)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);

        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }



        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }

    }
}