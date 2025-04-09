import api from "./api";

export const refreshUserRoles = () => {
  console.error("REFRESH USER ROLES CALLED");

  const token = localStorage.getItem('accessToken');
  if (!token) return Promise.resolve();

  return api.get('/shared/myprofile', {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })
    .then(response => {
      const user = response.data;

      if (!user || !user.roles) {
        throw new Error("Invalid user data");
      }

      localStorage.setItem('roles', JSON.stringify(user.roles));
      localStorage.setItem('userFullName', user.fullName);
    });
};
