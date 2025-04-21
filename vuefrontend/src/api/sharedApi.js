import api from "../utils/api"

export async function filterDepartmentTickets(params) {
    const response = await api.get('/shared/querytickets', { params });
    return response.data;
  }

  export async function getSharedTicketById(ticketId) {
    const response = await api.get(`/shared/ticket/${ticketId}`);
    return response.data;
  }

  export async function postTicketResponse(payload) {
    await api.post('/shared/response', payload); 
  }
  
  export async function getMyResponses(params = {}) {
    const query = new URLSearchParams();
  
    for (const key in params) {
      if (params[key]) {
        query.append(key, params[key]);
      }
    }
  
    const res = await api.get(`/shared/sentresponses?${query.toString()}`);
    return res.data;
  }
  
  export async function assignTicketToMe(ticketId) {
    const res = await api.patch(`/shared/assigntome/${ticketId}`);
    return res.data;
  }
  