import apiClient from "./apiClient";

export function getSkills() {
  return apiClient.get("/skills");
}

export function getSkillById(id) {
  return apiClient.get(`/skills/${id}`);
}

export function createSkill(data) {
  return apiClient.post("/skills", data);
}

export function updateSkill(id, data) {
  return apiClient.put(`/skills/${id}`, data);
}

export function deleteSkill(id) {
  return apiClient.delete(`/skills/${id}`);
}