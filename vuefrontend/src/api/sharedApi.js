import api from "../utils/api"

export async function filterDepartmentTickets(params) {
    const response = await api.get('/shared/querytickets', { params });
    return response.data;
  }

  export async function getSharedTicketById(ticketId) {
    const response = await api.get(`/shared/ticket/${ticketId}`);
    return response.data;
  }