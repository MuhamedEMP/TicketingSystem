export async function getAllCategories() {
    const token = localStorage.getItem("accessToken");
  
    const response = await fetch("http://localhost:5172/categories", {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  
    if (!response.ok) {
      throw new Error("Failed to fetch categories");
    }
  
    return await response.json();
  }
  