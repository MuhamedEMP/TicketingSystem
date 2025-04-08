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

  if (result) {
    msal.setActiveAccount(result.account);

    const tokenResponse = await msal.acquireTokenSilent({
      ...loginRequest,
      account: result.account,
    });

    localStorage.setItem("accessToken", tokenResponse.accessToken);

    const userResponse = await fetch("http://localhost:5172/shared/myprofile", {
      headers: {
        Authorization: `Bearer ${tokenResponse.accessToken}`,
      },
    });

    if (userResponse.ok){
      const user = await userResponse.json();
      localStorage.setItem("roles", JSON.stringify(user.roles));
      localStorage.setItem("userFullName", user.fullName);

      const roles = user.roles?.map(r => r.toLowerCase());

      if (roles.includes("admin")) {
 // these role based checks work
        router.push("/profile");
        return;
      } 
      router.push("/home");
      return;
      // handle HR AND IT ROLES - MAYBE DONT HARDCODE?
    } else {
      console.error("Failed to fetch user info");
      router.push("/home");
    }
  }

  const app = createApp(App);
  app.config.globalProperties.$msal = msal;
  app.use(router);
  app.mount("#app");

  if (result && window.location.pathname === "/") {
    router.push("/home");
    console.log("redirect ");
  }
})();
