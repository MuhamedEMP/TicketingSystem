import { createApp } from "vue";
import App from "./App.vue";
import "./style.css";
import "./assets/css/custom.css";
import "./assets/css/navbar.css";
import { router } from "./router.js";
import { PublicClientApplication } from "@azure/msal-browser";
import { loginRequest, msalConfig } from "./authConfig";
import api from "./utils/api.js";

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

    const accessToken = tokenResponse.accessToken;
    localStorage.setItem("accessToken", accessToken);

    // 🔐 Register user on backend
    await api.get("/auth/register");

    // 🔐 Save Graph access token for SharePoint uploads
    const graphToken = await msal.acquireTokenSilent({
      scopes: ["Files.ReadWrite.All", "Sites.ReadWrite.All"],
      account: result.account,
    });
    localStorage.setItem("graphAccessToken", graphToken.accessToken);

    try {
      const { data: user } = await api.get("/shared/myprofile");

      localStorage.setItem("userId", user.userId);
      localStorage.setItem("userFullName", user.fullName);
      localStorage.setItem("firstName", user.firstName);
      localStorage.setItem("isAdmin", JSON.stringify(user.isAdmin));
      localStorage.setItem("accessibleDepartments", JSON.stringify(user.accessibleDepartmentDtos || []));

      const hasDeptAccess = user.accessibleDepartmentDtos?.length > 0;
      const grantedPolicies = [];

      if (user.isAdmin && !hasDeptAccess) grantedPolicies.push("AdminOnly");
      if (user.isAdmin && hasDeptAccess) grantedPolicies.push("AdminAndDepartmentUser");
      if (user.isAdmin || hasDeptAccess) grantedPolicies.push("AdminOrDepartmentUser");
      if (!user.isAdmin && hasDeptAccess) grantedPolicies.push("DepartmentUserOnly");
      if (!user.isAdmin && !hasDeptAccess) grantedPolicies.push("RegularUserOnly");

      localStorage.setItem("grantedPolicies", JSON.stringify(grantedPolicies));

      if (grantedPolicies.includes("AdminOnly")) {
        router.push("/profile");
      } else {
        router.push("/home");
      }
    } catch (error) {
      console.error("❌ Failed to load user profile:", error);
    }
  }

  // Fallback route for empty redirect result
  if (result && window.location.pathname === "/") {
    router.push("/home");
    console.log("✅ Redirected to home.");
  }
})();
