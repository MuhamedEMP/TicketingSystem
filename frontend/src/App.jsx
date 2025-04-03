import { useEffect } from "react";
import { useMsal, useIsAuthenticated } from "@azure/msal-react";
import { loginRequest } from "./authConfig";

function App() {
  const { instance, accounts, inProgress } = useMsal();
  const isAuthenticated = useIsAuthenticated();

  useEffect(() => {
    const initAuth = async () => {
      try {
        const response = await instance.handleRedirectPromise(); // ✅ now safe to call

        const account = response?.account || instance.getAllAccounts()[0];
        if (account) {
          instance.setActiveAccount(account);

          const tokenResponse = await instance.acquireTokenSilent({
            ...loginRequest,
            account
          });

          const accessToken = tokenResponse.accessToken;
          console.log("✅ Got token:", accessToken);

          const res = await fetch("http://localhost:5172/register", {
            method: "GET",
            headers: {
              Authorization: `Bearer ${accessToken}`
            }
          });

          const result = await res.text();
          console.log("✅ Registered:", result);
        }
      } catch (err) {
        console.error("❌ MSAL error:", err);
      }
    };

    initAuth();
  }, [instance]);

  const handleLogin = () => {
    instance.loginRedirect(loginRequest);
  };

  return (
    <div>
      <h1>Welcome</h1>
      {!isAuthenticated && (
        <button onClick={handleLogin}>Login with Microsoft</button>
      )}
    </div>
  );
}

export default App;
