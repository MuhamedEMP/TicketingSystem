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
  