import apiClient from "./apiClient";

export function getActivities(params = {}) {
  return apiClient.get("/activities", {
    params,
  });
}
