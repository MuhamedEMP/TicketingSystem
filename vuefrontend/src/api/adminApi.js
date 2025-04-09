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
  