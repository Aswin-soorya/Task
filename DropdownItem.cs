using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormCreation.Models
{
    public class DropdownItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class UserDetails
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int DistrictId { get; set; }

        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
    }
    public class AuditLogModel
    {
        public string TableName { get; set; }
        public string ActionType { get; set; }
        public int RecordId { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        public string DoneBy { get; set; }
        public DateTime DoneDate { get; set; } 
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}