import * as signalR from "@microsoft/signalr";

export function createFeedConnection(): signalR.HubConnection {
  const s = localStorage.getItem("auth");
  const token = s ? JSON.parse(s).accessToken : null;

  const conn = new signalR.HubConnectionBuilder()
    .withUrl(`${import.meta.env.VITE_API_BASE_URL}/hubs/feed`, {
      accessTokenFactory: () => token ?? "",
    })
    .withAutomaticReconnect()
    .build();

  return conn;
}
