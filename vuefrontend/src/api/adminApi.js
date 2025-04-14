import api from "../utils/api"

export const getAllUsers = () => {
    return api.get('/admin/users');
  };

  export const queryUsers = (queryParams) => {
    return api.get('/admin/users/query', {
      params: queryParams
    });
  };
  
  export const getUserById = (userId) => {
    return api.get(`/admin/user/${userId}`);
  };
  
  export const changeUserRole = (userId, roles) => {
    return api.patch(`/admin/changerole/${userId}`, {
      roles
    });
  };
  
  export const submitDepartment = (deptName) => {
    return api.post(`/admin/adddepartment/${deptName}`);
  };
  
  export const addNewCategory = async (newCategory) => {
    const res = await api.post('/admin/addcategory', newCategory)
    return res.data
  }