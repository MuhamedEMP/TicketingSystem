﻿using System.Runtime.CompilerServices;
using System.Xml;
using TicketingSys.Dtos.TicketDtos;
using TicketingSys.Models;

namespace TicketingSys.Mappers
{
    public static class TicketMapper
    {
        // maps to a model without ticketAttachments because that table needs 
        // Ticket Id, they will be mapped to the Ticket in AttachmentMapper
        public static Ticket NewDtoToModel(this NewTicketDto dto, string createdById, int categoryId, int departmentId)
        {
            return new Ticket
            {
                SubmittedById = createdById,
                CategoryId = categoryId,
                DepartmentId = departmentId,
                Status = Enums.TicketStatusEnum.Open,
                Title = dto.Title,
                Urgency = dto.Urgency,
                CreatedAt = DateTime.UtcNow,
                Description = dto.Description,
                Attachments = new List<TicketAttachment>()
            };
        }

        public static ViewTicketDto modelToViewDto(this Ticket ticket)
        {
            return new ViewTicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status.ToString(),
                Urgency = ticket.Urgency.ToString(),
                SubmittedById = ticket.SubmittedById,
                SubmittedByName = ticket.SubmittedBy?.fullName ?? "Unknown",
                AssignedToId = ticket.AssignedToId,
                AssignedToName = ticket.AssignedTo?.fullName ?? "Unassigned",
                DepartmentName = ticket.Department?.Name ?? "Unknown", 
                CategoryName = ticket.Category?.Name ?? "Unknown", 
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                AttachmentPaths = ticket.Attachments?.Select(a => a.Path).ToList() ?? new List<string>()
            };
        }


        // this overload adds list of related responses to the ticket dto if the ticket query includes them
        public static ViewTicketDto modelToViewDto(this Ticket ticket, bool includesResponses)
        {
            var dto = ticket.modelToViewDto();

            if (includesResponses && ticket.Responses is not null)
            {
                dto.ViewResponses = ticket.Responses
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => r.ToViewDto( includeTicket: false))
                    .ToList();

                dto.responsesCount = ticket.Responses.Count();
            }

            return dto;

        }

        public static List<ViewTicketDto> modelToViewDtoList(this List<Ticket> tickets)
        {
            return tickets.Select(t => t.modelToViewDto()).ToList();
        }

    }
}