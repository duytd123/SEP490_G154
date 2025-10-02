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

/* ===== NORMAL LOGIN ===== */
export async function loginPassword(email: string, password: string): Promise<AuthSession> {
  const { data } = await axiosClient.post<{ email: string; Email?: string; token: string; Token?: string; role?: string; Role?: string }>(
    "/Login/Login",
    { email: email.trim().toLowerCase(), password }
  );

  const token = data.token ?? data.Token;
  const mail = data.email ?? data.Email;
  const session = toSession(token, mail);
  saveSession(session);
  return session;
}

/* ===== GOOGLE LOGIN ===== */
export async function loginWithGoogleCustomer(idToken: string) {
  const { data } = await axiosClient.post<{ Email: string; email?: string; Role: string; role?: string; Token: string; token?: string }>(
    "/Login/LoginWithGoogleCustomer", { idToken }
  );
  const session = toSession(data.Token ?? data.token, data.Email ?? data.email);
  saveSession(session); return session;
}

export async function loginWithGoogleSeller(idToken: string) {
  const { data } = await axiosClient.post<{ Email: string; email?: string; Role: string; role?: string; Token: string; token?: string }>(
    "/Login/LoginWithGoogleSeller", { idToken }
  );
  const session = toSession(data.Token ?? data.token, data.Email ?? data.email);
  saveSession(session); return session;
}

export async function loginWithGoogleHost(idToken: string) {
  const { data } = await axiosClient.post<{ Email: string; email?: string; Role: string; role?: string; Token: string; token?: string }>(
    "/Login/LoginWithGoogleHost", { idToken }
  );
  const session = toSession(data.Token ?? data.token, data.Email ?? data.email);
  saveSession(session); return session;
}

/* ===== FACEBOOK LOGIN ===== */
export async function loginWithFacebookCustomer(accessToken: string) {
  const { data } = await axiosClient.post<{ Email: string; email?: string; Role: string; role?: string; Token: string; token?: string }>(
    "/Login/LoginWithFacebookCustomer", { accessToken }
  );
  const session = toSession(data.Token ?? data.token, data.Email ?? data.email);
  saveSession(session); return session;
}

export async function loginWithFacebookSeller(accessToken: string) {
  const { data } = await axiosClient.post<{ Email: string; email?: string; Role: string; role?: string; Token: string; token?: string }>(
    "/Login/LoginWithFacebookSeller", { accessToken }
  );
  const session = toSession(data.Token ?? data.token, data.Email ?? data.email);
  saveSession(session); return session;
}

export async function loginWithFacebookHost(accessToken: string) {
  const { data } = await axiosClient.post<{ Email: string; email?: string; Role: string; role?: string; Token: string; token?: string }>(
    "/Login/LoginWithFacebookHost", { accessToken }
  );
  const session = toSession(data.Token ?? data.token, data.Email ?? data.email);
  saveSession(session); return session;
}

/* ===== REGISTER ===== */
export async function registerAdmin(body: RegisterBody) {
  const { data } = await axiosClient.post("/Login/RegisterAdmin", body);
  return { message: data.message, userId: data.userId, email: data.email ?? data.Email, role: data.role ?? data.Role };
}

export async function registerSeller(body: RegisterBody) {
  const { data } = await axiosClient.post("/Login/RegisterSeller", body);
  return { message: data.message, userId: data.userId, email: data.email ?? data.Email, role: data.role ?? data.Role };
}

export async function registerHost(body: RegisterBody) {
  const { data } = await axiosClient.post("/Login/RegisterHost", body);
  return { message: data.message, userId: data.userId, email: data.email ?? data.Email, role: data.role ?? data.Role };
}

export async function registerCustomer(body: RegisterBody) {
  const { data } = await axiosClient.post("/Login/RegisterCustomer", body);
  return { message: data.message, userId: data.userId, email: data.email ?? data.Email, role: data.role ?? data.Role };
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

// ====== OTHERS ======
export async function forgotPassword(email: string) {
  const { data } = await axiosClient.post("/Login/ForgotPassword", { email });
  return { message: data?.message ?? data?.Message ?? null };
}
export async function resetPassword(
  email: string,
  newPassword: string,
  otp: string
) {
  const { data } = await axiosClient.post("/Login/ResetPassword", {
    email,
    newPassword,
    otp,
  });
  return { message: data?.message ?? data?.Message ?? null };
}
export async function changePassword(
  currentPassword: string,
  newPassword: string
) {
  const { data } = await axiosClient.post("/Login/ChangePassword", {
    currentPassword,
    newPassword,
  });
  return { message: data?.message ?? data?.Message ?? null };
}


