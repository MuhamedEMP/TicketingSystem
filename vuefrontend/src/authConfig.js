export const msalConfig = {
    auth: {
      clientId: "137dd5ce-0c3a-40f3-af94-a03853de9572",
      authority: "https://login.microsoftonline.com/b3b27808-2559-44a8-8443-365d492c7436",
      redirectUri: "http://localhost:5173/"// Or your deployed URL
    },
    cache: {
      cacheLocation: "localStorage", // so it's persisted
      storeAuthStateInCookie: false
    }
  };
  
  export const loginRequest = {
    scopes: ["api://137dd5ce-0c3a-40f3-af94-a03853de9572/ourapi","Files.ReadWrite.All", "Sites.ReadWrite.All"]
  }; // adi promjene