import axiosClient from "./axiosClient";

export async function getMe() {
  const { data } = await axiosClient.get("/users/me");
  return data;
}
export async function updateMe(body: { displayName: string; avatarUrl?: string; bio?: string }) {
  const { data } = await axiosClient.put("/users/me", body);
  return data;
}
