// src/api/auth.ts
import axiosClient, { setAuthTokens } from "./axiosClient";

const STORAGE_KEY = import.meta.env.VITE_TOKEN_STORAGE_KEY ?? "auth";

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

export type RegisterResult = {
  message: string;
  userId: number;
  email: string;
  role: string;
};

function base64UrlToJson<T>(b64url: string): T {
  const b64 = b64url.replace(/-/g, "+").replace(/_/g, "/");
  const json = decodeURIComponent(
    atob(b64)
      .split("")
      .map(c => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
      .join("")
  );
  return JSON.parse(json) as T;
}
function parseJwt(token: string): any {
  const parts = token.split(".");
  if (parts.length !== 3) return {};
  return base64UrlToJson(parts[1]);
}
function extractRole(payload: any): string | null {
  const uri = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
  const r = payload?.role ?? payload?.[uri];
  if (!r) return null;
  return Array.isArray(r) ? (r[0] ?? null) : r;
}
function toSession(token: string, emailFallback?: string): AuthSession {
  const p = parseJwt(token);
  const role = extractRole(p);
  const expMs = p?.exp ? p.exp * 1000 : Date.now() + 2 * 60 * 60 * 1000;
  const email = p?.email ?? emailFallback ?? "";
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

/** 🔑 Password login – backend: POST /api/Login/Login (form-urlencoded) */
export async function loginPassword(email: string, password: string): Promise<AuthSession> {
  const params = new URLSearchParams();
  params.set("email", email.trim().toLowerCase());
  params.set("password", password);

  const { data } = await axiosClient.post<{ Email: string; Token: string }>(
    "/Login/Login",
    params,
    { headers: { "Content-Type": "application/x-www-form-urlencoded" } }
  );
  const session = toSession(data.Token, data.Email);
  saveSession(session);
  return session;
}

/** Google logins (ví dụ dùng Customer) */
export async function loginWithGoogleCustomer(idToken: string) {
  const { data } = await axiosClient.post<{ Email: string; Role: string; Token: string }>(
    "/Login/LoginWithGoogleCustomer", { idToken }
  );
  const s = toSession(data.Token, data.Email); saveSession(s); return s;
}

/** Facebook logins (ví dụ dùng Customer) */
export async function loginWithFacebookCustomer(accessToken: string) {
  const { data } = await axiosClient.post<{ Email: string; Role: string; Token: string }>(
    "/Login/LoginWithFacebookCustomer", { accessToken }
  );
  const s = toSession(data.Token, data.Email); saveSession(s); return s;
}

/** Registers */
export async function registerAdmin(body: RegisterBody): Promise<RegisterResult> {
  const { data } = await axiosClient.post("/Login/RegisterAdmin", body);
  return { message: data.message, userId: data.userId, email: data.email, role: data.role };
}
export async function registerSeller(body: RegisterBody): Promise<RegisterResult> {
  const { data } = await axiosClient.post("/Login/RegisterSeller", body);
  return { message: data.message, userId: data.userId, email: data.email, role: data.role };
}
export async function registerHost(body: RegisterBody): Promise<RegisterResult> {
  const { data } = await axiosClient.post("/Login/RegisterHost", body);
  return { message: data.message, userId: data.userId, email: data.email, role: data.role };
}
export async function registerCustomer(body: RegisterBody): Promise<RegisterResult> {
  const { data } = await axiosClient.post("/Login/RegisterCustomer", body);
  return { message: data.message, userId: data.userId, email: data.email, role: data.role };
}

/** Storage helpers */
export function loadAuthFromStorage(): AuthSession | null {
  const s = localStorage.getItem(STORAGE_KEY);
  if (!s) return null;
  try {
    const parsed = JSON.parse(s) as AuthSession;
    if (!parsed?.accessToken) return null;
    setAuthTokens(parsed.accessToken);
    return parsed;
  } catch { return null; }
}
export function logout() {
  localStorage.removeItem(STORAGE_KEY);
  setAuthTokens(null);
}
