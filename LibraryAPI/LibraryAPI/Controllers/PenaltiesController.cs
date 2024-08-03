using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Data;
using LibraryAPI.Models;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PenaltiesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public PenaltiesController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpPost("PenaltyForMember/{memberId}")]
        public async Task<IActionResult> PenaltyForMember(string memberId)
        {

            var member = await _context.Members
                .Include(m => m.BorrowedBooks)
                .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null)
            {
                return NotFound("Member not found");
            }

            foreach(var borrowedBookCopy in member.BorrowedBooks)
            {
                TimeSpan timeDifference = (TimeSpan)(DateTime.Today - borrowedBookCopy.BorrowDate);
                int delayDays = (int)timeDifference.TotalDays;
                

                if (delayDays > 15)
                {
                    var penaltyAmount = delayDays * 1;

                    var penalty = new Penalty
                    {
                        MemberId = memberId,
                        PenaltyDate = DateTime.Today,
                        PenaltyAmount = penaltyAmount

                    };

                    _context.Penalties.Add(penalty);
                }
            }

            await _context.SaveChangesAsync();

            return Ok("Penalties added");
        }

        [HttpPost("PenaltyForEmployee/{employeeId}")]
        public async Task<IActionResult> PenaltyForEmployee(string employeeId)
        {

            var employee = await _context.Employees
                .Include(m => m.BorrowedBooks)
                .FirstOrDefaultAsync(m => m.Id == employeeId);

            if (employee == null)
            {
                return NotFound("Member not found");
            }

            foreach (var borrowedBookCopy in employee.BorrowedBooks)
            {
                TimeSpan timeDifference = (TimeSpan)(DateTime.Today - borrowedBookCopy.BorrowDate);
                int delayDays = (int)timeDifference.TotalDays;


                if (delayDays > 10)
                {
                    var penaltyAmount = delayDays * 1;

                    var penalty = new Penalty
                    {
                        EmployeeId = employeeId,
                        PenaltyDate = DateTime.Today,
                        PenaltyAmount = penaltyAmount

                    };

                    _context.Penalties.Add(penalty);
                }
            }

            await _context.SaveChangesAsync();

            return Ok("Penalties added");
        }

        [HttpGet("PenaltiesForMember/{memberId}")]
        public async Task<IActionResult> GetPenaltiesForMember(string memberId)
        {
            var penalties = await _context.Penalties
                .Where(p => p.MemberId == memberId)
                .ToListAsync();

            if (penalties == null || !penalties.Any())
            {
                return NotFound("No penalties found for the member");
            }

            return Ok(penalties);
        }

        [HttpGet("PenaltiesForEmployee/{employeeId}")]
        public async Task<IActionResult> GetPenaltiesForEmployee(string employeeId)
        {
            var penalties = await _context.Penalties
                .Where(p => p.EmployeeId == employeeId)
                .ToListAsync();

            if (penalties == null || !penalties.Any())
            {
                return NotFound("No penalties found for the employee");
            }

            return Ok(penalties);
        }

        /*
        // GET: api/Penalties
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Penalty>>> GetPenalties()
        {
          if (_context.Penalties == null)
          {
              return NotFound();
          }
            return await _context.Penalties.ToListAsync();
        }

        // GET: api/Penalties/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Penalty>> GetPenalty(int id)
        {
          if (_context.Penalties == null)
          {
              return NotFound();
          }
            var penalty = await _context.Penalties.FindAsync(id);

            if (penalty == null)
            {
                return NotFound();
            }

            return penalty;
        }

        // PUT: api/Penalties/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPenalty(int id, Penalty penalty)
        {
            if (id != penalty.PenaltyId)
            {
                return BadRequest();
            }

            _context.Entry(penalty).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PenaltyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Penalties
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Penalty>> PostPenalty(Penalty penalty)
        {
          if (_context.Penalties == null)
          {
              return Problem("Entity set 'ApplicationContext.Penalties'  is null.");
          }
            _context.Penalties.Add(penalty);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPenalty", new { id = penalty.PenaltyId }, penalty);
        }

        // DELETE: api/Penalties/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePenalty(int id)
        {
            if (_context.Penalties == null)
            {
                return NotFound();
            }
            var penalty = await _context.Penalties.FindAsync(id);
            if (penalty == null)
            {
                return NotFound();
            }

            _context.Penalties.Remove(penalty);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PenaltyExists(int id)
        {
            return (_context.Penalties?.Any(e => e.PenaltyId == id)).GetValueOrDefault();
        }
        */


    }
}
