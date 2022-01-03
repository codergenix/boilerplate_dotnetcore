using System;
using System.Collections.Generic;

#nullable disable

namespace DotNetCoreBoilerPlate.Models
{
    public partial class BoilerTable
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
        public string updateBy { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? deletedAt { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}
