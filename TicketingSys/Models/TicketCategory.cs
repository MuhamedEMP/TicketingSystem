﻿using System.ComponentModel.DataAnnotations;

namespace TicketingSys.Models
{
    public class TicketCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        // department this category belongs to
        public int? DepartmentId { get; set; }

        public Department? Department { get; set; }

    }
}
