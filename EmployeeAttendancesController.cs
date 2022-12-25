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
    public class EmployeeAttendancesController : ControllerBase
    {
        private readonly astoriaTraining80Context _context;

        public EmployeeAttendancesController(astoriaTraining80Context context)
        {
            _context = context;
        }

        // GET: api/EmployeeAttendances
        [HttpGet("allattendanceList")]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetEmployeeAttendance(string FilterClockDate,int FilterCompanyID)
        {
           
            try
            {
                //Attendance.ModifiedDate = DateTime.Now;
                var EmpAtt = from emp in _context.EmployeeMaster.Where(e => e.EmpCompanyId == FilterCompanyID && (e.EmpResignationDate == null || e.EmpResignationDate >= Convert.ToDateTime(FilterClockDate)))
                             join att in _context.EmployeeAttendance.Where(x => x.ClockDate == Convert.ToDateTime(FilterClockDate).Date)
                             on emp.EmployeeKey equals att.EmployeeKey
                             into grouping
                             from g in grouping.DefaultIfEmpty()
                                 //  where emp.EmpCompanyId == FilterCompanyID
                             select new Attendance
                             {
                                 EmployeeKey = emp.EmployeeKey,
                                 EmployeeID = emp.EmployeeId,
                                 EmployeeName = (emp.EmpFirstName + emp.EmpLastName),
                                 ClockDate = Convert.ToDateTime(FilterClockDate).Date.ToString("yyyy-MM-dd"),
                                
                                 TimeIn = g.TimeIn == null ? string.Empty : g.TimeIn.ToString("hh:mm"),
                                 TimeOut = g.TimeOut == null ? string.Empty : g.TimeOut.ToString("hh:mm"),
                                 Remarks = g.Remarks == null ? string.Empty : g.Remarks
                                
                             };
                return await EmpAtt.ToListAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        // GET: api/EmployeeAttendances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeAttendance>> GetEmployeeAttendance(long id)
        {
            var employeeAttendance = await _context.EmployeeAttendance.FindAsync(id);

            if (employeeAttendance == null)
            {
                return NotFound();
            }

            return employeeAttendance;
        }

        // PUT: api/EmployeeAttendances/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployeeAttendance(long id, EmployeeAttendance employeeAttendance)
        {
            if (id != employeeAttendance.EmployeeKey)
            {
                return BadRequest();
            }

            _context.Entry(employeeAttendance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeAttendanceExists(id))
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
        [HttpPost("AddAttendance")]
        public async Task<ActionResult<EmployeeAttendance>> PostEmployeeAttendance(List<EmployeeAttendance> lstemployeeAttendance)
        {
            if(lstemployeeAttendance.Count == 0)
            {
                return BadRequest("Employee List is NULL");
            }
            try
            {
                foreach (EmployeeAttendance empA in lstemployeeAttendance)
                {
                    EmployeeAttendance empattendance = _context.EmployeeAttendance.Where(e => e.EmployeeKey == empA.EmployeeKey
                    && e.ClockDate == empA.ClockDate).FirstOrDefault();
                    //Put atterndance
                    if (empattendance != null)
                    {
                       // empattendance.employeeKey = empA.employeeKey;
                       
                        empattendance.TimeIn = empA.TimeIn;
                        empattendance.TimeOut = empA.TimeOut;
                        empattendance.Remarks = empA.Remarks;
                        empattendance.ModifiedDate = DateTime.Now;
                        _context.Entry(empattendance).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                    //Posting attendance
                    else
                    {
                        empA.CreationDate = DateTime.Now;
                        empA.ModifiedDate = DateTime.Now;
                        _context.EmployeeAttendance.Add(empA);
                        await _context.SaveChangesAsync();
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    // DELETE: api/EmployeeAttendances/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<EmployeeAttendance>> DeleteEmployeeAttendance(long id)
        {
            var employeeAttendance = await _context.EmployeeAttendance.FindAsync(id);
            if (employeeAttendance == null)
            {
                return NotFound();
            }

            _context.EmployeeAttendance.Remove(employeeAttendance);
            await _context.SaveChangesAsync();

            return employeeAttendance;
        }

        private bool EmployeeAttendanceExists(long id)
        {
            return _context.EmployeeAttendance.Any(e => e.EmployeeKey == id);
        }
    }
}
