import api from "./api";

// get user dto from the api, and set roles from dto in localstorage
export const refreshUserRoles = () => {
    const token = localStorage.getItem('accessToken');
    if (!token) return Promise.resolve();
  
    return fetch('http://localhost:5172/shared/myprofile', {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
      .then(res => {
        if (!res.ok) throw new Error("Failed to fetch profile");
        return res.json();
      })
      .then(user => {
        localStorage.setItem('roles', JSON.stringify(user.roles));
        localStorage.setItem('userFullName', user.fullName);
      });
  };
  