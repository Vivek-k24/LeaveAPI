using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AngularSQLlink.Data;
using AngularSQLlink.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSQLlink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRecordsController : ControllerBase
    {
        private readonly YourDbContext _context;

        public LeaveRecordsController(YourDbContext context)
        {
            _context = context;
        }

        // GET: api/LeaveRecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRecord>>> GetLeaveRecords()
        {
            return await _context.LeaveRecords.ToListAsync();
        }

        // POST: api/LeaveRecords
        [HttpPost]
        public async Task<ActionResult<LeaveRecord>> PostLeaveRecord(LeaveRecord leaveRecord)
        {
            _context.LeaveRecords.Add(leaveRecord);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLeaveRecord), new { id = leaveRecord.LeaveRecordId }, leaveRecord);
        }

        // GET: api/LeaveRecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRecord>> GetLeaveRecord(int id)
        {
            var leaveRecord = await _context.LeaveRecords.FindAsync(id);
            if (leaveRecord == null)
            {
                return NotFound();
            }
            return leaveRecord;
        }

        // PUT: api/LeaveRecords/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeaveRecord(int id, LeaveRecord leaveRecord)
        {
            if (id != leaveRecord.LeaveRecordId)
            {
                return BadRequest();
            }

            _context.Entry(leaveRecord).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/LeaveRecords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveRecord(int id)
        {
            var leaveRecord = await _context.LeaveRecords.FindAsync(id);
            if (leaveRecord == null)
            {
                return NotFound();
            }

            _context.LeaveRecords.Remove(leaveRecord);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/LeaveRecords/filter
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<LeaveRecord>>> GetFilteredLeaveRecords([FromQuery] LeaveRecordFilter filter)
        {
            var query = _context.LeaveRecords.AsQueryable();

            if (!string.IsNullOrEmpty(filter.LeaveType))
            {
                query = query.Where(lr => lr.LeaveType == filter.LeaveType);
            }

            if (!string.IsNullOrEmpty(filter.EmployeeName))
            {
                var employees = _context.Employees.Where(e => e.Name.Contains(filter.EmployeeName)).Select(e => e.EmployeeId).ToList();
                query = query.Where(lr => employees.Contains(lr.EmployeeId));
            }

            if (filter.LeavePeriod.HasValue)
            {
                query = query.Where(lr => lr.LeavePeriod == filter.LeavePeriod.Value);
            }

            if (!string.IsNullOrEmpty(filter.Location))
            {
                var employees = _context.Employees.Where(e => e.Location == filter.Location).Select(e => e.EmployeeId).ToList();
                query = query.Where(lr => employees.Contains(lr.EmployeeId));
            }

            if (!string.IsNullOrEmpty(filter.SubUnit))
            {
                var employees = _context.Employees.Where(e => e.SubUnit == filter.SubUnit).Select(e => e.EmployeeId).ToList();
                query = query.Where(lr => employees.Contains(lr.EmployeeId));
            }

            if (!string.IsNullOrEmpty(filter.JobTitle))
            {
                var employees = _context.Employees.Where(e => e.JobTitle == filter.JobTitle).Select(e => e.EmployeeId).ToList();
                query = query.Where(lr => employees.Contains(lr.EmployeeId));
            }

            var results = await query.ToListAsync();
            return Ok(results);
        }
    }

    public class LeaveRecordFilter
    {
        public string LeaveType { get; set; }
        public string EmployeeName { get; set; }
        public int? LeavePeriod { get; set; } // Changed to LeavePeriod
        public string Location { get; set; }
        public string SubUnit { get; set; }
        public string JobTitle { get; set; }
        public bool? IncludePastEmployees { get; set; }
    }
}
