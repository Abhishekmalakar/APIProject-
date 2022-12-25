using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _astoriaTrainingAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace _astoriaTrainingAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class EmployeeMastersController : ControllerBase
    {
        private readonly astoriaTraining80Context _context;

        public EmployeeMastersController(astoriaTraining80Context context)
        {
            _context = context;
        }

        // GET: api/EmployeeMasters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeMaster>>> GetEmployeeMaster()
        {


            return await _context.EmployeeMaster.ToListAsync();
        }

        //GET :api/allEmployeeList
        
        [HttpGet("allEmployeeList")]
        [ProducesResponseType(typeof(IEnumerable<Employee>),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeeList()
        {
            var Emp = (from em in _context.EmployeeMaster
                       join cm in _context.CompanyMaster on em.EmpCompanyId equals cm.CompanyId
                       join dm in _context.DesignationMaster on em.EmpDesignationId equals dm.DesignationId

                       select new Employee()
                       {
                           EmployeeKey = em.EmployeeKey,
                           EmployeeID = em.EmployeeId,
                           EmployeeName = em.EmpFirstName + " " + em.EmpLastName,
                           Company = cm.CompanyName,
                           Designation = dm.DesignationName,
                           JoiningDate = em.EmpJoiningDate,
                           Gender = em.EmpGender
                       }).ToListAsync();
            //  return await Emp;
           var emp = await Emp;
            if (emp.Count > 0)
            {
                //var emp = await emp;
                // return ok();
                return Ok(emp);
            }
            else
            {
                return NoContent();
                //   return await emp;
            }
        }
        

        // GET: api/EmployeeMasters/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IEnumerable<Employee>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<EmployeeMaster>>> GetEmployeeMaster(long id)
        {
             //   var employeeMaster1 =  _context.EmployeeMaster.Where(e => e.EmployeeKey == id).ToList();
          
            var employeeMaster = await _context.EmployeeMaster.FindAsync(id);
            if (employeeMaster == null)
            {
                return NotFound();
            }
            return Ok(employeeMaster);
        } 
            
           

     [HttpGet("EmployeeIDExists")]

        public async Task<ActionResult<bool>>GetEmployeeIdExists(string EmployeeID,long employeeKey)
        {
            bool IsEmployeeIDExists = await _context.EmployeeMaster.AnyAsync(e => e.EmployeeKey != employeeKey && e.EmployeeId.Trim().ToLower() == EmployeeID.Trim().ToLower());
            return IsEmployeeIDExists;
        }

    [HttpGet("allCompanies")]
        [ProducesResponseType(typeof(IEnumerable<Employee>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CompanyMaster>>> GetCompanyMaster()
        {

           var  CmCount= _context.CompanyMaster.Count();
            if (CmCount > 0)
            {
                return await _context.CompanyMaster.ToListAsync();
            }
            else
            {
                return NoContent();
            }
            
        }
        

        [HttpGet("allDesignation")]
        [ProducesResponseType(typeof(IEnumerable<Employee>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DesignationMaster>>> GetDesignationMaster()
        
        {
            var DmCount = _context.DesignationMaster.Count();
            if(DmCount > 0)
            {
                return await _context.DesignationMaster.ToListAsync();

            }
            else
            {
                return NoContent();
            }
        }


      // PUT: api/EmployeeMasters/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(IEnumerable<Employee>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<object>> PutEmployeeMaster(long id, EmployeeMaster employeeMaster)
        {
            
            try
            {
                employeeMaster.ModifiedDate = DateTime.Now;
                var empKey = _context.EmployeeMaster.Any(e => e.EmployeeKey == id);
                if (!empKey)
                {
                    return NotFound();
                }
                 else
                  {
                if (string.IsNullOrEmpty(employeeMaster.EmployeeId) ||
                string.IsNullOrEmpty(employeeMaster.EmpFirstName) ||
                string.IsNullOrEmpty(employeeMaster.EmpLastName) ||
               string.IsNullOrEmpty(employeeMaster.EmpGender) ||
                employeeMaster.EmpJoiningDate == null ||
                employeeMaster.CreationDate == null ||
                employeeMaster.ModifiedDate == null ||
                employeeMaster.EmployeeId.Length > 20 ||
               employeeMaster.EmpFirstName.Length > 100 ||
               employeeMaster.EmpLastName.Length > 100 ||
               employeeMaster.EmpJoiningDate > employeeMaster.EmpResignationDate)

               {
                    return BadRequest();
                }

               else
                {
                    var empMaster = _context.EmployeeMaster.Any(x => x.EmployeeId == employeeMaster.EmployeeId && x.EmployeeKey != employeeMaster.EmployeeKey);
                    if (empMaster)
                        return Conflict("EmployeeId Should not be Dublicated");
                }
                    _context.Entry(employeeMaster).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return Ok();
              }   
            }
            catch (Exception ex)
            {
                throw ex;
            }

            ////return Ok("Employee Details Updated Successfully");
        }

        // POST: api/EmployeeMasters
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<Employee>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EmployeeMaster>> PostEmployeeMaster(EmployeeMaster employeeMaster)
        {
            try
            {
                if(string.IsNullOrEmpty(employeeMaster.EmployeeId)||
                   string.IsNullOrEmpty(employeeMaster.EmpFirstName)||
                   string.IsNullOrEmpty(employeeMaster.EmpLastName)||
                   string.IsNullOrEmpty(employeeMaster.EmpGender)||
                    employeeMaster.EmpJoiningDate == null ||
                   employeeMaster.EmployeeId.Length >20 ||
                   employeeMaster.EmpFirstName.Length>100 ||
                   employeeMaster.EmpLastName.Length >100)
                {
                    return BadRequest();
                }
                else if(employeeMaster.EmpJoiningDate > employeeMaster.EmpResignationDate)
                {
                    return BadRequest();
                }
                else
                {
                    var empMaster = _context.EmployeeMaster.Where(x => x.EmployeeId == employeeMaster.EmployeeId).Select(x => x).FirstOrDefault();
                    if (empMaster != null)
                        return Conflict("EmployeeId Should Be Dublicate");
                }
                employeeMaster.CreationDate = DateTime.Now;
                employeeMaster.ModifiedDate = DateTime.Now;
                _context.EmployeeMaster.Add(employeeMaster);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            

         //  return Ok( CreatedAtAction("GetEmployeeMaster", new { id = employeeMaster.EmployeeKey }, employeeMaster));
          //   return Ok();
        }

        
        /// <summary>
        /// This method used to Delete Employee from DataBase
        /// </summary>
        /// <param name="id"> Interger Parameter is required</param>
        /// <returns> Return EmployeeMaster Entity</returns>
        // DELETE: api/EmployeeMasters/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(IEnumerable<Employee>), StatusCodes.Status200OK)]
      
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<object>> DeleteEmployeeMaster(long id)
        {
            try
            {
                var employeeMaster = await _context.EmployeeMaster.FindAsync(id);
                if (employeeMaster == null)
                {
                    return NotFound();
                }
                bool empInUse =await _context.EmployeeAllowanceDetail.AnyAsync(e => e.EmployeeKey == id);
                bool empKeyInUse = await _context.EmployeeAttendance.AnyAsync(e => e.EmployeeKey == id);
              
                if (empInUse || empKeyInUse)
                {
                    return Conflict("EmployeeID in use");
                }
            

                _context.EmployeeMaster.Remove(employeeMaster);
                await _context.SaveChangesAsync();
                
                return Ok();
            }
            catch(Exception ex)
            {
                throw ex;
            }
           // return Ok();
        }
        [HttpGet("EmployeeIdinUse")]
        [ProducesResponseType(typeof(IEnumerable<Employee>), StatusCodes.Status200OK)]
       
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> EmployeIdinUse(long employeeKey)
        {
            try
            {
                var employeeMaster = await _context.EmployeeMaster.FindAsync(employeeKey);
                if (employeeMaster == null)
                {
                    return NotFound();
                }

                bool isUse = await _context.EmployeeAttendance.AnyAsync(e => e.EmployeeKey == employeeKey );
                if (!isUse)
                {
                   isUse = await _context.EmployeeAllowanceDetail.AnyAsync(e => e.EmployeeKey == employeeKey);
                }
                return Ok(isUse);

              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
    }
}
