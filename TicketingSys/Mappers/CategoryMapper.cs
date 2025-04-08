﻿using TicketingSys.Dtos.CategoryDtos;
using TicketingSys.Models;

namespace TicketingSys.Mappers
{
    public static class CategoryMapper
    {
        public static ViewTicketCategoryDto modelToViewDto(this TicketCategory ticketCategory)
        {
          
            return new ViewTicketCategoryDto
            {
                Id = ticketCategory.Id,
                Name = ticketCategory.Name,
                Description = ticketCategory.Description,
                DepartmentId = ticketCategory.DepartmentId,
                DepartmentName = ticketCategory.Department?.Name ?? "Unknown"
            };
        }
    }
}
