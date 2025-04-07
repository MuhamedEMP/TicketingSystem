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

  const result = await msal.handleRedirectPromise(); // 👈 crucial!
  if (result) {
    msal.setActiveAccount(result.account);

    const tokenResponse = await msal.acquireTokenSilent({
      ...loginRequest,
      account: result.account,
    });

    localStorage.setItem("accessToken", tokenResponse.accessToken);

  }

  const app = createApp(App);
  app.config.globalProperties.$msal = msal;
  app.use(router);
  app.mount("#app");

  if (result && window.location.pathname === "/") {
    await router.isReady(); 
    router.push("/home");
    console.log("redirect ");
  }
})();
