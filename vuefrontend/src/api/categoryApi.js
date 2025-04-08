import api from "../utils/api"

export async function getAllCategories() {
    const response = await api.get("http://localhost:5172/shared/categories");
    return response.data;
  }
  
  
export async function getCategoriesByDepartment(deptId) {
    const response = await api.get(`http://localhost:5172/shared/${deptId}/categories`)
    return response.data;
  }
  

  export async function getCategoryById(id) {
    const response = await api.get(`http://localhost:5172/shared/categories/${id}`);
    return response.data;
  }