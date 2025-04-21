export const msalConfig = {
    auth: {
      clientId: "b8d1f791-7b53-4678-b40f-162ad2d1b0d9",
      authority: "https://login.microsoftonline.com/b3b27808-2559-44a8-8443-365d492c7436",
      redirectUri: "http://localhost:5173/"// Or your deployed URL
    },
    cache: {
      cacheLocation: "localStorage", // so it's persisted
      storeAuthStateInCookie: false
    }
  };
  
  export const loginRequest = {
    scopes: ["api://b8d1f791-7b53-4678-b40f-162ad2d1b0d9/ourapi","Files.ReadWrite.All", "Sites.ReadWrite.All"]
  };