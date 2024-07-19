using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AngularSQLlink.Data;
using System.Web;

namespace AngularSQLlink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRecordsController : ControllerBase
    {
        private readonly YourDbContext _context;
        private readonly ILogger<LeaveRecordsController> _logger;

        public LeaveRecordsController(YourDbContext context, ILogger<LeaveRecordsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("employee")]
        public IActionResult GetLeaveRecordsByEmployee(string employeeName, string leavePeriod)
        {
            _logger.LogInformation("Received request for employee leave records: {EmployeeName}, {LeavePeriod}", employeeName, leavePeriod);

            try
            {
                var employee = _context.Employees.FirstOrDefault(e => e.Name == employeeName);
                if (employee == null)
                {
                    _logger.LogWarning("Employee not found: {EmployeeName}", employeeName);
                    return NotFound("Employee not found.");
                }

                var leaveRecords = _context.LeaveRecords
                    .Where(lr => lr.EmployeeId == employee.EmployeeId && lr.LeavePeriod == leavePeriod)
                    .ToList();

                if (!leaveRecords.Any())
                {
                    _logger.LogInformation("No leave records found for employee: {EmployeeName}, period: {LeavePeriod}", employeeName, leavePeriod);
                    return NotFound("No leave records found for the specified employee and period.");
                }

                _logger.LogInformation("Returning {RecordCount} leave records for employee: {EmployeeName}, period: {LeavePeriod}", leaveRecords.Count, employeeName, leavePeriod);
                return Ok(leaveRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request for employee leave records: {EmployeeName}, {LeavePeriod}", employeeName, leavePeriod);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("filters")]
        public IActionResult GetLeaveRecordsByFilters(string leaveType, string leavePeriod, string location, string subUnit, string jobTitle, bool includePastEmployees)
        {
            // Decode the URL parameters (not necessary if using standard encoding)
            leaveType = HttpUtility.UrlDecode(leaveType);
            leavePeriod = HttpUtility.UrlDecode(leavePeriod);
            location = HttpUtility.UrlDecode(location);
            subUnit = HttpUtility.UrlDecode(subUnit);
            jobTitle = HttpUtility.UrlDecode(jobTitle);

            _logger.LogInformation("Received request for leave records by filters: {LeaveType}, {LeavePeriod}, {Location}, {SubUnit}, {JobTitle}, {IncludePastEmployees}",
                leaveType, leavePeriod, location, subUnit, jobTitle, includePastEmployees);

            try
            {
                var employees = _context.Employees.AsQueryable();

                if (!includePastEmployees)
                {
                    employees = employees.Where(e => e.IsCurrentEmployee);
                }

                var leaveRecords = _context.LeaveRecords
                    .Join(employees, lr => lr.EmployeeId, e => e.EmployeeId, (lr, e) => new { lr, e })
                    .Where(joined => (string.IsNullOrEmpty(leaveType) || joined.lr.LeaveType == leaveType) &&
                                     (string.IsNullOrEmpty(leavePeriod) || joined.lr.LeavePeriod == leavePeriod) &&
                                     (string.IsNullOrEmpty(location) || joined.e.Location == location) &&
                                     (string.IsNullOrEmpty(subUnit) || joined.e.SubUnit == subUnit) &&
                                     (string.IsNullOrEmpty(jobTitle) || joined.e.JobTitle == jobTitle))
                    .Select(joined => new
                    {
                        joined.lr.LeaveRecordId,
                        joined.e.Name,
                        joined.lr.LeaveType,
                        joined.lr.Entitlements,
                        joined.lr.PendingApproval,
                        joined.lr.Scheduled,
                        joined.lr.Taken,
                        joined.lr.Balance,
                        joined.lr.LeavePeriod
                    })
                    .ToList();

                if (!leaveRecords.Any())
                    return NotFound("No leave records found for the specified filters.");

                return Ok(leaveRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request for leave records by filters");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
