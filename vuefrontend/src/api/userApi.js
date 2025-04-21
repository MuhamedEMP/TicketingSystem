import api from "../utils/api"

export async function fetchMyTickets() {

    const response = await api.get("http://localhost:5172/user/mytickets")
    return response.data;
  }
  

export async function submitNewTicket(departmentId, categoryId, ticketData) {
    const response = await api.post(
      `/user/newticket/${departmentId}/${categoryId}`,
      ticketData,
      {
        headers: {
          'Content-Type': 'application/json',
        },
      }
    );
  
    return response.data; 
  }
  
  export async function filterMyTickets(params) {
    const response = await api.get(`/user/mytickets?${params.toString()}`);
    return { tickets: response.data };
  }


  export async function getMyTicketById(ticketId) {
    const response = await api.get(`/user/tickets/${ticketId}`);
    return response.data;
  }
  

  export async function getUserProfile() {
    const response = await api.get("http://localhost:5172/shared/myprofile");
    return response.data;
  }
  

  export async function getResponsesToMyTickets(params = {}){
    const query = new URLSearchParams();
  
    for (const key in params) {
      if (params[key]) {
        query.append(key, params[key]);
      }
    }

     
    const res = await api.get(`/user/myresponses?${query.toString()}`);
    return res.data;
  
  }
  
