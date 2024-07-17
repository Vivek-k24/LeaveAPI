using System;

namespace AngularSQLlink.Models
{
    public class LeaveRecord
    {
        public int LeaveRecordId { get; set; }
        public int EmployeeId { get; set; }
        public string LeaveType { get; set; }
        public int Entitlements { get; set; }
        public int PendingApproval { get; set; }
        public int Scheduled { get; set; }
        public int Taken { get; set; }
        public int Balance { get; set; }
        public int LeavePeriod { get; set; }
    }
}
