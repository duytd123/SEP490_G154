import axiosClient, { setAuthTokens } from "./axiosClient";

const STORAGE_KEY = import.meta.env.VITE_TOKEN_STORAGE_KEY ?? "auth";

/* ===== TYPES ===== */
export type AuthSession = {
  accessToken: string;
  email: string;
  role: string | null;
  expiresAt: string;
};

export type RegisterBody = {
  email: string;
  password: string;
  fullName: string;
  phone: string;
};

axiosClient.interceptors.request.use(config => {
  console.log("📡 Sending request:", config.method, config.url);
  return config;
});

/* ===== JWT HELPERS ===== */
function base64UrlToJson<T>(b64url: string): T {
  try {
    const b64 = b64url.replace(/-/g, "+").replace(/_/g, "/");
    const json = decodeURIComponent(
      atob(b64)
        .split("")
        .map(c => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
        .join("")
    );
    return JSON.parse(json) as T;
  } catch {
    return {} as T;
  }
}

function parseJwt(token: string): any {
  const parts = token?.split(".") ?? [];
  if (parts.length !== 3) return {};
  return base64UrlToJson(parts[1]);
}

function extractRole(payload: any): string | null {
  const uri = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
  const r = payload?.role ?? payload?.Role ?? payload?.[uri];
  if (!r) return null;
  return Array.isArray(r) ? (r[0] ?? null) : r;
}

function toSession(token: string, emailFallback?: string): AuthSession {
  const p = parseJwt(token);
  const role = extractRole(p);
  const expMs = p?.exp ? p.exp * 1000 : Date.now() + 2 * 60 * 60 * 1000;
  const email = p?.email ?? p?.Email ?? emailFallback ?? "";
  return {
    accessToken: token,
    email,
    role: role ?? null,
    expiresAt: new Date(expMs).toISOString(),
  };
}

function saveSession(session: AuthSession) {
  setAuthTokens(session.accessToken);
  localStorage.setItem(STORAGE_KEY, JSON.stringify(session));
}

/* ===== LOGIN ===== */
export async function loginPassword(email: string, password: string): Promise<AuthSession> {
  const { data } = await axiosClient.post<{
    success: boolean;
    message: string;
    email?: string;
    role?: string;
    token?: string;
  }>("/Login/Login", { email: email.trim().toLowerCase(), password });

  if (!data.success) {
    throw new Error(data.message || "Đăng nhập thất bại");
  }

  const session = toSession(data.token!, data.email);
  saveSession(session);
  return session;
}

/* ===== GOOGLE LOGIN ===== */
async function handleLoginSocial(endpoint: string, payload: any) {
  const { data } = await axiosClient.post<{
    success: boolean;
    message: string;
    email?: string;
    role?: string;
    token?: string;
  }>(endpoint, payload);

  if (!data.success) throw new Error(data.message);

  const session = toSession(data.token!, data.email);
  saveSession(session);
  return session;
}

export function loginWithGoogleCustomer(idToken: string) {
  return handleLoginSocial("/Login/LoginWithGoogleCustomer", { idToken });
}
export function loginWithGoogleSeller(idToken: string) {
  return handleLoginSocial("/Login/LoginWithGoogleSeller", { idToken });
}
export function loginWithGoogleHost(idToken: string) {
  return handleLoginSocial("/Login/LoginWithGoogleHost", { idToken });
}

/* ===== FACEBOOK LOGIN ===== */
export function loginWithFacebookCustomer(accessToken: string) {
  return handleLoginSocial("/Login/LoginWithFacebookCustomer", { accessToken });
}
export function loginWithFacebookSeller(accessToken: string) {
  return handleLoginSocial("/Login/LoginWithFacebookSeller", { accessToken });
}
export function loginWithFacebookHost(accessToken: string) {
  return handleLoginSocial("/Login/LoginWithFacebookHost", { accessToken });
}

/* ===== REGISTER ===== */
export async function registerAdmin(body: RegisterBody) {
  const { data } = await axiosClient.post("/Login/RegisterAdmin", body);
  return data;
}
export async function registerSeller(body: RegisterBody) {
  const { data } = await axiosClient.post("/Login/RegisterSeller", body);
  return data;
}
export async function registerHost(body: RegisterBody) {
  const { data } = await axiosClient.post("/Login/RegisterHost", body);
  return data;
}
export async function registerCustomer(body: RegisterBody) {
  const { data } = await axiosClient.post("/Login/RegisterCustomer", body);
  return data;
}

/* ===== SESSION HELPERS ===== */
export function loadAuthFromStorage(): AuthSession | null {
  const s = localStorage.getItem(STORAGE_KEY);
  if (!s) return null;
  try {
    const parsed = JSON.parse(s) as AuthSession;
    if (!parsed?.accessToken) return null;
    setAuthTokens(parsed.accessToken);
    return parsed;
  } catch {
    return null;
  }
}

export function logout() {
  localStorage.removeItem(STORAGE_KEY);
  setAuthTokens(null);
}

/* ===== OTHERS ===== */
export async function forgotPassword(email: string) {
  const { data } = await axiosClient.post("/Login/ForgotPassword", { email });
  return data;
}
export async function resetPassword(email: string, newPassword: string, otp: string) {
  const { data } = await axiosClient.post("/Login/ResetPasswordWithOtp", {
    email,
    newPassword,
    otp,
  });
  return data;
}
export async function changePassword(currentPassword: string, newPassword: string) {
  const { data } = await axiosClient.post("/Login/ChangePassword", {
    currentPassword,
    newPassword,
  });
  return data;
}
