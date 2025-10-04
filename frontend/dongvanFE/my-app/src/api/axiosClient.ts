import axios from "axios";

const axiosClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL + "/api",
  withCredentials: false,
});

let accessToken: string | null = null;
let refreshToken: string | null = null;
let isRefreshing = false;
let queue: Array<() => void> = [];

/* ===== SET TOKENS ===== */
export function setAuthTokens(at: string | null, rt?: string | null) {
  accessToken = at;
  refreshToken = rt ?? refreshToken; 
}

/* ===== REQUEST INTERCEPTOR ===== */
axiosClient.interceptors.request.use((config) => {
  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  return config;
});

/* ===== RESPONSE INTERCEPTOR ===== */
axiosClient.interceptors.response.use(
  (res) => res,
  async (error) => {
    const original = error.config;

    // === 401 UNAUTHORIZED ===
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
        localStorage.removeItem(import.meta.env.VITE_TOKEN_STORAGE_KEY ?? "auth");
        return Promise.reject({
          message: "Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại.",
        });
      } finally {
        isRefreshing = false;
      }
    }

    // === XỬ LÝ LỖI KHÁC ===
    if (error.response) {
      return Promise.reject(error);
    }

    if (error.request) {
      return Promise.reject({
        message: "⚠️ Không thể kết nối tới máy chủ. Vui lòng thử lại sau.",
        request: error.request,
      });
    }

    return Promise.reject({
      message: error.message || "Đã xảy ra lỗi không xác định.",
    });
  }
);


export default axiosClient;
