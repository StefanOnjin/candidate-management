import apiClient from "./apiClient";

export function getCandidates(page = 1, pageSize = 10) {
  return apiClient.get("/candidates", {
    params: { page, pageSize },
  });
}

export function getCandidateById(id) {
  return apiClient.get(`/candidates/${id}`);
}

export function createCandidate(data) {
  return apiClient.post("/candidates", data);
}

export function updateCandidate(id, data) {
  return apiClient.put(`/candidates/${id}`, data);
}

export function deleteCandidate(id) {
  return apiClient.delete(`/candidates/${id}`);
}

export function searchCandidates(data) {
  return apiClient.post("/candidates/search", data);
}

export function removeSkillFromCandidate(candidateId, skillId) {
  return apiClient.delete(`/candidates/${candidateId}/skills/${skillId}`);
}
