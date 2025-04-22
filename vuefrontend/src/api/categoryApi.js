import api from "../utils/api"


export async function getAllCategories() {
  const response = await api.get("/shared/categories");

  // ✅ Ensure this returns just an array
  return Array.isArray(response.data)
    ? response.data
    : response.data.categories || [];
}


export async function getCategoriesByDepartment(deptId) {
  const response = await api.get(`/shared/${deptId}/categories`);
  return response.data;
}

export async function getCategoryById(id) {
  const response = await api.get(`/shared/categories/${id}`);
  return response.data;
}