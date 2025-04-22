import api from "../utils/api"

export async function getAllDepartments() {
  const response = await api.get("/shared/departments");
  return response.data;
}

export async function getDepartmentById(id) {
  const response = await api.get(`/shared/departments/${id}`);
  return response.data;
}

export async function getMyAssignedDepartments() {
  const response = await api.get("/shared/mydepartments");
  return response.data;
}