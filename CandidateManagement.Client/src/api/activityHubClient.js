import * as signalR from "@microsoft/signalr";

const ACTIVITY_HUB_URL = "http://localhost:5290/hubs/activity";

export function createActivityHubConnection() {
  return new signalR.HubConnectionBuilder()
    .withUrl(ACTIVITY_HUB_URL)
    .withAutomaticReconnect()
    .build();
}
