// src/components/LoginButton.jsx

import { useMsal } from "@azure/msal-react";
import { loginRequest } from "../authConfig";

export default function LoginButton() {
  const { instance } = useMsal();

  const handleLogin = () => {
    instance.loginPopup(loginRequest).then((response) => {
      console.log("Access Token:", response.accessToken);
      localStorage.setItem("token", response.accessToken); // optional
    }).catch(err => console.error(err));
  };

  return <button onClick={handleLogin}>Login with Microsoft</button>;
}
