import { createApp } from "vue";
import App from "./App.vue";
import "./style.css";
import "./assets/css/custom.css";
import "./assets/css/navbar.css";
import { router } from "./router.js";
import { PublicClientApplication } from "@azure/msal-browser";
import { loginRequest, msalConfig } from "./authConfig";

const msal = new PublicClientApplication(msalConfig);

(async () => {
  await msal.initialize();
  const result = await msal.handleRedirectPromise(); 

  const app = createApp(App);
  app.config.globalProperties.$msal = msal;
  app.use(router);
  app.mount("#app");

  if (result) {
    msal.setActiveAccount(result.account);

    const tokenResponse = await msal.acquireTokenSilent({
      ...loginRequest,
      account: result.account,
    });

    localStorage.setItem("accessToken", tokenResponse.accessToken);
    const accessToken = tokenResponse.accessToken;

    // Call /auth/register after login
    await fetch("http://localhost:5172/auth/register", {
      headers: {
        "Authorization": `Bearer ${accessToken}`
      }
    });

    // Fetch current user profile
    const userResponse = await fetch("http://localhost:5172/shared/myprofile", {
      headers: {
        Authorization: `Bearer ${tokenResponse.accessToken}`,
      },
    });

    // FOR FILE UPLOAD USING GRAPH - IMPORTANT
    const graphToken = await msal.acquireTokenSilent({
      scopes: ["Files.ReadWrite.All", "Sites.ReadWrite.All"],
      account: result.account,
    });
    localStorage.setItem("graphAccessToken", graphToken.accessToken);
    

    if (userResponse.ok) {
      const user = await userResponse.json();

      // 🔐 Save info to localStorage for rendering
      localStorage.setItem("userId", user.userId);
      localStorage.setItem("userFullName", user.fullName);
      localStorage.setItem("firstName", user.firstName);
      localStorage.setItem("isAdmin", JSON.stringify(user.isAdmin));
      localStorage.setItem("accessibleDepartments", JSON.stringify(user.accessibleDepartmentDtos || []));

      // Mirror backend policy logic:
      const hasDeptAccess = user.accessibleDepartmentDtos?.length > 0;
      const grantedPolicies = [];

      // AdminOnly handler logic
      if (user.isAdmin && !hasDeptAccess) {
        grantedPolicies.push("AdminOnly");
      }

      if (user.isAdmin && hasDeptAccess) {
        grantedPolicies.push("AdminAndDepartmentUser");
      }

      if (user.isAdmin || hasDeptAccess) {
        grantedPolicies.push("AdminOrDepartmentUser");
      }

      // DepartmentUserOnly handler logic
      if (!user.isAdmin && hasDeptAccess) {
        grantedPolicies.push("DepartmentUserOnly");
      }

      // RegularUserOnly handler logic
      if (!user.isAdmin && !hasDeptAccess) {
        grantedPolicies.push("RegularUserOnly");
      }

localStorage.setItem("grantedPolicies", JSON.stringify(grantedPolicies));
      // ✅ Route to proper dashboard
      if (grantedPolicies.includes("AdminOnly")) {
        router.push("/profile");
        return;
      }

      router.push("/home");
      return;
    }
  }

  if (result && window.location.pathname === "/") {
    router.push("/home");
    console.log("redirect ");
  }
})();