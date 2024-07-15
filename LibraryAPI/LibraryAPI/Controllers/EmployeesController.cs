using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Data;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public EmployeesController(ApplicationContext context,UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser>signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
          if (_context.Employees == null)
          {
              return NotFound();
          }
            return await _context.Employees.ToListAsync();
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(string id)
        {
          if (_context.Employees == null)
          {
              return NotFound();
          }
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(string id, Employee employee,string? currentPassword=null)
        {
            ApplicationUser applicationUser = _userManager.FindByIdAsync(id).Result;

            if (id != employee.Id)
            {
                return BadRequest();
            }

            //Normalde buralar if li kontrol şeklinde olmalıdır.

            applicationUser.Address = employee.ApplicationUser!.Address;
            applicationUser.BirthDate = employee.ApplicationUser!.BirthDate;
            applicationUser.Email = employee.ApplicationUser!.Email;
            //...Hepsi yapılmalıdır.

            _userManager.UpdateAsync(applicationUser).Wait(); //Frontedten alınan bilgiler ile güncelledik.

            if (currentPassword!= null)
            {
                _userManager.ChangePasswordAsync(applicationUser, currentPassword, applicationUser.Password).Wait();
            }
            employee.ApplicationUser = null;


            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee,string? category=null)
        {
            Claim claim;
          if (_context.Employees == null)
          {
              return Problem("Entity set 'ApplicationContext.Employees'  is null.");
          }
            _userManager.CreateAsync(employee.ApplicationUser!,employee.ApplicationUser!.Password).Wait();
            _userManager.AddToRoleAsync(employee.ApplicationUser!, "Worker").Wait();
            if (category != null)
            {
                claim = new Claim("Category", category);
                _userManager.AddClaimAsync(employee.ApplicationUser,claim).Wait();
            }
            /*
            foreach(string category in categories)
            {
                claim = new Claim("Category",category);
                _userManager.AddClaimsAsync(employee.ApplicationUser,claim).Wait();
            }
            */
            employee.Id = employee.ApplicationUser!.Id;
            employee.ApplicationUser = null;
            _context.Employees.Add(employee);  //Employeenin içine sadece employee'nin özelliklerini yazar.
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EmployeeExists(employee.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            if (_context.Employees == null)
            {
                return NotFound();
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("Login")]
        public ActionResult Login(string userName,string password)
        {
            ApplicationUser applicationUser = _userManager.FindByNameAsync(userName).Result;
            Microsoft.AspNetCore.Identity.SignInResult signInResult;

            if( applicationUser != null)  //Kullanıcı kayıtlı olup olmadığını kontrol ediyor.
            {
                signInResult = _signInManager.PasswordSignInAsync(applicationUser, password, false, false).Result;
                if (signInResult.Succeeded == true)
                {
                    return Ok();

                }
            }
            return Unauthorized();
        }

        [Authorize]   //Bu nitelik, bu yöntemin yalnızca kimlik
                      //doğrulaması yapılmış (yani oturum açmış)
                      //kullanıcılar tarafından erişilebilir olmasını sağlar.
                      //Herhangi bir login işlemi yapılmadıysa 404 not found hatası gönderecektir.

        [HttpGet("Logout")]
        public ActionResult LogOut()
        {
            _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpPost("ForgetPassword")]
        public ActionResult<string> ForgetPassword(string userName)
        {
            ApplicationUser applicationUser = _userManager.FindByNameAsync(userName).Result;

            string token= _userManager.GeneratePasswordResetTokenAsync(applicationUser).Result;
            System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage("abc@abc",applicationUser.Email,"Şifre sıfırlama",token);

            //Bu kod, temel anlamda bir e - posta mesajını SMTP
            //sunucusu aracılığıyla göndermek için kullanılan
            //standart bir işlemi temsil eder.
            System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient("http://smtp.domain.com");
            smtpClient.Send(mailMessage);
            return token;
        }

        [HttpPost("ResetPassword")]
        public ActionResult ResetPassword(string userName,string newPassword)
        {
            ApplicationUser applicationUser = _userManager.FindByNameAsync(userName).Result;
            string token = _userManager.GeneratePasswordResetTokenAsync(applicationUser).Result;

            try
            {
                _userManager.ResetPasswordAsync(applicationUser, token, newPassword).Wait();

            }
            catch
            {
                return Unauthorized();
            }

            return Ok();
        }

        private bool EmployeeExists(string id)
        {
            return (_context.Employees?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
