export async function getAllDepartments() {
    const token = localStorage.getItem("accessToken");
  
    const response = await fetch("http://localhost:5172/shared/departments", {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  
    if (!response.ok) {
      throw new Error("Failed to fetch departments");
    }
  
    return await response.json();
  }
  