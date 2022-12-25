using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _astoriaTrainingAPI.Models;

namespace _astoriaTrainingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAllowanceDetailsController : ControllerBase
    {
        private readonly astoriaTraining80Context _context;
        private DateTime TodayDate;

        public EmployeeAllowanceDetailsController(astoriaTraining80Context context)
        {
            _context = context;
        }

        // GET: api/EmployeeAllowanceDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeAllowanceDetail>>> GetEmployeeAllowanceDetail()
        {
            return await _context.EmployeeAllowanceDetail.ToListAsync();
        }

       // GET: api/EmployeeAllowanceDetails
         [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeAllowanceDetail>> GetEmployeeAllowanceDetail(long id)
        {
            var employeeAllowanceDetail = await _context.EmployeeAllowanceDetail.FindAsync(id);

            if (employeeAllowanceDetail == null)
            {
                return NotFound();
            }

            return employeeAllowanceDetail;
        }
        [HttpGet("Userinfo")]

        public async Task<ActionResult<bool>> UserInfo(string UserName, string Password)

        {
            bool info= await _context.UserInfo.AnyAsync(e => e.UserName == UserName && e.Password == Password);

            if (info == null)
            {
                return NoContent();
            }
            else
            {
                return info;
            }
        }

        [HttpGet("allAllowanceName")]
        public async Task<ActionResult<IEnumerable<AllowanceMaster>>> GetAllowanceMaster()
        {
            var CmCount = _context.AllowanceMaster.Count();

            if (CmCount > 0)
            {
                return await _context.AllowanceMaster.ToListAsync();
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet("PresentEmployee")]
        public async Task<ActionResult<IEnumerable<AllowancesAttendance>>> GetEmployeeAttendance()
        {

         //Attendance.ModifiedDate = DateTime.Now
            var EmpAtt = from emp in _context.EmployeeMaster
                         join att in _context.EmployeeAttendance.Where(x => x.ClockDate.Date == DateTime.Now.Date)
                         on emp.EmployeeKey equals att.EmployeeKey
                         select new AllowancesAttendance
                         {
                             EmployeeKey = emp.EmployeeKey,
                           //  EmployeeID = emp.EmployeeId,
                             EmployeeName = (emp.EmpFirstName + emp.EmpLastName),
                             //ClockDate = Convert.ToDateTime(ClockDate).Date.ToString("yyyy-MM-dd"),
                         };
            return await EmpAtt.ToListAsync();
        }


        [HttpPost("PostAllowance")]
        public async Task<ActionResult<Boolean>> PostEmployeeAllowanceDetail (List<EmployeeAllowanceDetail> employeeAllowanceDetailList)
        {
            try
            {
                foreach(EmployeeAllowanceDetail empAllw in employeeAllowanceDetailList)
                {
                    if (empAllw.AllowanceAmount > 0)
                    {
                        EmployeeAllowanceDetail empAllowance = _context.EmployeeAllowanceDetail.Where(e => e.EmployeeKey == empAllw.EmployeeKey
                                                    && e.ClockDate == empAllw.ClockDate && e.AllowanceId == empAllw.AllowanceId).FirstOrDefault();


                        if (empAllowance != null)
                        {
                            empAllowance.AllowanceAmount = empAllw.AllowanceAmount;
                            empAllowance.ModifiedDate = DateTime.Now;
                            _context.Entry(empAllowance).State = EntityState.Modified;
                   
                        }
                        else
                        {
                            empAllw.CreationDate = empAllw.ModifiedDate = DateTime.Now;
                            _context.EmployeeAllowanceDetail.Add(empAllw);
                        }
                        await _context.SaveChangesAsync();
                     }
                    
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }

        }

        // PUT: api/EmployeeAllowanceDetails/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployeeAllowanceDetail(long id, EmployeeAllowanceDetail employeeAllowanceDetail)
        {
            if (id != employeeAllowanceDetail.EmployeeKey)
            {
                return BadRequest();
            }

            _context.Entry(employeeAllowanceDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeAllowanceDetailExists(id))
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

        // POST: api/EmployeeAllowanceDetails
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<EmployeeAllowanceDetail>> PostEmployeeAllowanceDetail(EmployeeAllowanceDetail employeeAllowanceDetail)
        {
            _context.EmployeeAllowanceDetail.Add(employeeAllowanceDetail);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EmployeeAllowanceDetailExists(employeeAllowanceDetail.EmployeeKey))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEmployeeAllowanceDetail", new { id = employeeAllowanceDetail.EmployeeKey }, employeeAllowanceDetail);
        }

        // DELETE: api/EmployeeAllowanceDetails/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<EmployeeAllowanceDetail>> DeleteEmployeeAllowanceDetail(long id)
        {
            var employeeAllowanceDetail = await _context.EmployeeAllowanceDetail.FindAsync(id);
            if (employeeAllowanceDetail == null)
            {
                return NotFound();
            }

            _context.EmployeeAllowanceDetail.Remove(employeeAllowanceDetail);
            await _context.SaveChangesAsync();

            return employeeAllowanceDetail;
        }

        private bool EmployeeAllowanceDetailExists(long id)
        {
            return _context.EmployeeAllowanceDetail.Any(e => e.EmployeeKey == id);
        }
    }
}
