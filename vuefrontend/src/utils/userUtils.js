import api from "./api";

// get user dto from the api, and set roles from dto in localstorage
export const refreshUserRoles = async () => {
  const token = localStorage.getItem('accessToken');
  if (!token) return;

  const res = await fetch('http://localhost:5172/shared/myprofile', {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!res.ok) throw new Error("Failed to refresh user profile");

  const user = await res.json();

  // Save user identity info
  localStorage.setItem("userId", user.userId);
  localStorage.setItem("userFullName", user.fullName);
  localStorage.setItem("firstName", user.firstName);
  localStorage.setItem("isAdmin", JSON.stringify(user.isAdmin));
  localStorage.setItem("accessibleDepartments", JSON.stringify(user.accessibleDepartmentDtos || []));

  // Recompute granted policies (same logic as after login)
  const hasDeptAccess = user.accessibleDepartmentDtos?.length > 0;
  const grantedPolicies = [];

  if (user.isAdmin && !hasDeptAccess) {
    grantedPolicies.push("AdminOnly");
  }

  if (user.isAdmin && hasDeptAccess) {
    grantedPolicies.push("AdminAndDepartmentUser");
  }

  if (user.isAdmin || hasDeptAccess) {
    grantedPolicies.push("AdminOrDepartmentUser");
  }

  if (!user.isAdmin && hasDeptAccess) {
    grantedPolicies.push("DepartmentUserOnly");
  }

  if (!user.isAdmin && !hasDeptAccess) {
    grantedPolicies.push("RegularUserOnly");
  }

  localStorage.setItem("grantedPolicies", JSON.stringify(grantedPolicies));
};
