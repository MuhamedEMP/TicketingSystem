export function hasPolicy(policyName) {
    const raw = localStorage.getItem("grantedPolicies") || "[]";
    const policies = JSON.parse(raw);
    return policies.includes(policyName);
  }
  