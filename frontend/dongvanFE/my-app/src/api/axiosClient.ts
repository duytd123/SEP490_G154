import axios from "axios";

const axiosClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL + "/api",
  withCredentials: false,
});

let accessToken: string | null = null;
let refreshToken: string | null = null;
let isRefreshing = false;
let queue: Array<() => void> = [];

export function setAuthTokens(at: string | null, rt?: string | null) {
  accessToken = at;
  refreshToken = rt ?? null;
}

axiosClient.interceptors.request.use((config) => {
  if (accessToken) config.headers.Authorization = `Bearer ${accessToken}`;
  return config;
});

axiosClient.interceptors.response.use(
  (res) => res,
  async (error) => {
    const original = error.config;
    if (error.response?.status === 401 && !original._retry && refreshToken) {
      if (isRefreshing) {
        await new Promise<void>((resolve) => queue.push(resolve));
        return axiosClient(original);
      }
      original._retry = true;
      isRefreshing = true;
      try {
        const res = await axios.post(
          `${import.meta.env.VITE_API_BASE_URL}/api/auth/refresh`,
          { refreshToken }
        );
        accessToken = res.data.accessToken;
        refreshToken = res.data.refreshToken ?? refreshToken;
        queue.forEach((fn) => fn());
        queue = [];
        return axiosClient(original);
      } catch (e) {
        accessToken = null;
        refreshToken = null;
        queue = [];
        throw e;
      } finally {
        isRefreshing = false;
      }
    }
    throw error;
  }
);

export default axiosClient;
