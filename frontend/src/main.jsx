import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import "./index.css";

import { PublicClientApplication } from "@azure/msal-browser";
import { MsalProvider } from "@azure/msal-react";
import { msalConfig } from "./authConfig";

// 🧠 Create instance
const msalInstance = new PublicClientApplication(msalConfig);

// ✅ First, initialize MSAL
msalInstance.initialize().then(() => {
  // Then render your app AFTER initialization is done
  ReactDOM.createRoot(document.getElementById("root")).render(
    <React.StrictMode>
      <MsalProvider instance={msalInstance}>
        <App />
      </MsalProvider>
    </React.StrictMode>
  );
});
