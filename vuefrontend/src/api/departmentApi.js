import api from "../utils/api"

export async function getAllDepartments() {
    const response = await api.get("http://localhost:5172/shared/departments");
    return response.data;
  }
  

  export async function getDepartmentById(id) {
    const response = await api.get(`http://localhost:5172/shared/departments/${id}`);
    return response.data;
  }
  

  export async function getMyAssignedDepartments() {
    const response = await api.get("http://localhost:5172/shared/mydepartments");
    return response.data
  }