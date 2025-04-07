export async function fetchMyTickets() {
    const token = localStorage.getItem("accessToken");
  
    if (!token) throw new Error("Access token missing");
  
    const response = await fetch("http://localhost:5172/user/mytickets", {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  
    if (!response.ok) {
      throw new Error("Failed to fetch tickets");
    }
  
    return await response.json();
  }
  


export async function submitNewTicket(ticket) {
  const token = localStorage.getItem("accessToken");
  if (!token) throw new Error("Access token missing");

  const response = await fetch('http://localhost:5172/user/newticket', {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(ticket),
  });

  if (!response.ok) {
    const errorText = await response.text(); // 👈 log raw error response
  console.error('Failed to submit ticket:', errorText)
  }

  return await response.json();
}



export async function filterMyTickets(params) {
  const token = localStorage.getItem("accessToken");

  try {
    const response = await fetch(`http://localhost:5172/user/mytickets?${params.toString()}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (response.ok) {
      const result = await response.json();
      return { tickets: result }; 
    } else {
      console.error('Unexpected error:', response.status);
      return { tickets: [] };
    }
  } catch (err) {
    console.error('Request failed:', err);
    return { tickets: [] };
  }
}


export async function getMyTicketById(ticketId) {
  const token = localStorage.getItem("accessToken");

  const response = await fetch(`http://localhost:5172/user/tickets/${ticketId}`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || `Failed to fetch ticket ${ticketId}`);
  }

  return await response.json();
}