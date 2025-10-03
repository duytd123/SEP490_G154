import { useState, useEffect } from "react";
import { useNavigate, Link } from "react-router-dom";
import type { AuthSession } from "../../api/auth";

import {
  loginPassword,
  loginWithGoogleCustomer,
  loginWithFacebookCustomer,
} from "../../api/auth";

export default function DongVanLogin({
  onLogin,
  onLoginWithGoogle,
  onLoginWithFacebook,
}: {
  onLogin?: (email: string, password: string) => Promise<AuthSession> | void;
  onLoginWithGoogle?: () => Promise<AuthSession> | void;
  onLoginWithFacebook?: () => Promise<AuthSession> | void;
}) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPw, setShowPw] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const nav = useNavigate();

  // ===== INIT GOOGLE + FACEBOOK SDK =====
  useEffect(() => {
    // Google init
    if (window.google) {
      window.google.accounts.id.initialize({
        client_id: import.meta.env.VITE_GOOGLE_CLIENT_ID,
        callback: (response: any) => {
          (window as any).__GOOGLE_ID_TOKEN__ = response.credential;
          handleGoogle();
        },
      });

      window.google.accounts.id.renderButton(
        document.getElementById("google-login-btn"),
        { theme: "outline", size: "large" }
      );
    }

    // Facebook init
    if (window.FB) {
      console.log("FB SDK ready?", window.FB);

      window.FB.init({
        appId: import.meta.env.VITE_FACEBOOK_APP_ID,
        cookie: true,
        xfbml: true,
        version: "v19.0",
      });
    }
  }, []);

  // ====== HANDLERS ======
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      setError("Email không hợp lệ.");
      return;
    }
    if (password.length < 6) {
      setError("Mật khẩu tối thiểu 6 ký tự.");
      return;
    }

    try {
      setSubmitting(true);
      if (onLogin) await onLogin(email, password);
      else await loginPassword(email, password);
      nav("/home");
    } catch (err: any) {
      const apiMsg = err?.response?.data;
      if (typeof apiMsg === "string") setError(apiMsg);
      else if (apiMsg?.title) setError(apiMsg.title);
      else setError(err?.message ?? "Đăng nhập thất bại, vui lòng thử lại.");
    } finally {
      setSubmitting(false);
    }
  };

  const handleGoogle = async () => {
    try {
      setSubmitting(true);
      if (onLoginWithGoogle) {
        await onLoginWithGoogle();
      } else {
        const idToken = (window as any).__GOOGLE_ID_TOKEN__;
        if (!idToken)
          throw new Error("Chưa có Google idToken. Vui lòng thử lại.");
        await loginWithGoogleCustomer(idToken);
      }
      nav("/home");
    } catch (err: any) {
      setError(
        err?.response?.data?.message ??
          err?.message ??
          "Không thể đăng nhập bằng Google."
      );
    } finally {
      setSubmitting(false);
    }
  };

  const handleFacebook = async () => {
    try {
      setSubmitting(true);
      if (onLoginWithFacebook) {
        await onLoginWithFacebook();
      } else {
        const fbAccessToken = (window as any).__FB_ACCESS_TOKEN__;
        if (!fbAccessToken)
          throw new Error("Chưa có Facebook accessToken. Vui lòng thử lại.");
        await loginWithFacebookCustomer(fbAccessToken);
      }
      nav("/home");
    } catch (err: any) {
      setError(
        err?.response?.data?.message ??
          err?.message ??
          "Không thể đăng nhập bằng Facebook."
      );
    } finally {
      setSubmitting(false);
    }
  };

  // Hàm gọi FB SDK để login
  const handleFBLogin = () => {
    FB.login(
      (response: any) => {
        if (response.authResponse) {
          const fbAccessToken = response.authResponse.accessToken;
          (window as any).__FB_ACCESS_TOKEN__ = fbAccessToken;
          handleFacebook(); // gọi login backend
        } else {
          setError("Bạn đã huỷ đăng nhập Facebook.");
        }
      },
      { scope: "email,public_profile" }
    );
  };

  // ====== RENDER ======
  return (
    <div className="min-h-screen w-full bg-gradient-to-b from-amber-50 via-orange-50 to-rose-50 flex items-center justify-center p-4">
      <div className="grid w-full max-w-6xl grid-cols-1 md:grid-cols-2 gap-6">
        {/* ===== LEFT: Brand / Hero ===== */}
        <div className="relative overflow-hidden rounded-3xl shadow-xl bg-white/70 backdrop-blur">
          <div className="absolute inset-0 opacity-20" aria-hidden>
            <svg className="w-full h-full" xmlns="http://www.w3.org/2000/svg">
              <defs>
                <pattern
                  id="motif"
                  width="80"
                  height="80"
                  patternUnits="userSpaceOnUse"
                  patternTransform="rotate(20)"
                >
                  <path
                    d="M0 40h80M40 0v80"
                    stroke="currentColor"
                    className="text-amber-200"
                    strokeWidth="1"
                  />
                  <circle
                    cx="40"
                    cy="40"
                    r="6"
                    className="text-rose-200"
                    fill="currentColor"
                  />
                </pattern>
              </defs>
              <rect width="100%" height="100%" fill="url(#motif)" />
            </svg>
          </div>

          <div className="relative p-8 md:p-12">
            <div className="flex items-center gap-3">
              <svg
                width="40"
                height="40"
                viewBox="0 0 64 64"
                xmlns="http://www.w3.org/2000/svg"
                className="shrink-0"
              >
                <path
                  d="M8 40c0 8.837 10.745 16 24 16s24-7.163 24-16H8z"
                  fill="#fb923c"
                />
                <path
                  d="M16 40c0 5.523 7.163 10 16 10s16-4.477 16-10H16z"
                  fill="#f97316"
                />
                <path
                  d="M8 24l12-8 12 10 12-12 12 10"
                  stroke="#16a34a"
                  strokeWidth="4"
                  fill="none"
                  strokeLinecap="round"
                />
              </svg>
              <div>
                <h1 className="text-2xl md:text-3xl font-extrabold text-gray-900">
                  Đồng Văn Ecoverse
                </h1>
                <p className="text-sm text-gray-600">
                  Kết nối du lịch văn hoá & ẩm thực Đồng Văn – Hà Giang
                </p>
              </div>
            </div>

            <div className="mt-8 space-y-4 text-gray-700">
              <p className="leading-relaxed">
                Trải nghiệm bản địa: homestay đá, chợ phiên, thắng cố, phở chua,
                mật ong bạc hà…
              </p>
              <ul className="list-disc ml-5 text-sm">
                <li>Đặt phòng & tour trải nghiệm văn hoá</li>
                <li>Mua đặc sản chuẩn gốc Đồng Văn</li>
                <li>Gợi ý ẩm thực & bản đồ làng nghề</li>
              </ul>
            </div>

            <div className="mt-8 p-4 rounded-2xl bg-amber-100/60 border border-amber-200">
              <p className="text-amber-900 text-sm">
                “Giữ hồn đá – lưu vị ẩm thực – kết nối cộng đồng”
              </p>
            </div>
          </div>
        </div>

        {/* ===== RIGHT: Login Card ===== */}
        <div className="flex items-center">
          <div className="w-full rounded-3xl bg-white shadow-xl p-6 md:p-10 border border-gray-100">
            <h2 className="text-xl md:text-2xl font-bold text-gray-900">
              Đăng nhập
            </h2>
            <p className="text-sm text-gray-600 mt-1">
              Chào mừng bạn trở lại với hành trình văn hoá & ẩm thực Đồng Văn.
            </p>

            <form onSubmit={handleSubmit} className="mt-6 space-y-5" noValidate>
              {error && (
                <div className="rounded-xl border border-rose-200 bg-rose-50 p-3 text-rose-700 text-sm">
                  {error}
                </div>
              )}

              {/* Email */}
              <div>
                <label
                  htmlFor="email"
                  className="block text-sm font-medium text-gray-700"
                >
                  Email
                </label>
                <input
                  id="email"
                  type="email"
                  inputMode="email"
                  autoComplete="email"
                  placeholder="ban@vidu.com"
                  className="mt-1 w-full rounded-xl border border-gray-300 bg-white px-4 py-3 text-gray-900 placeholder:text-gray-400 focus:outline-none focus:ring-4 focus:ring-amber-200 focus:border-amber-500 transition"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  disabled={submitting}
                  required
                />
              </div>

              {/* Password */}
              <div>
                <div className="flex items-center justify-between">
                  <label
                    htmlFor="password"
                    className="block text-sm font-medium text-gray-700"
                  >
                    Mật khẩu
                  </label>
                  <button
                    type="button"
                    className="text-sm text-amber-700 hover:underline"
                    onClick={() => setShowPw((s) => !s)}
                  >
                    {showPw ? "Ẩn" : "Hiện"}
                  </button>
                </div>
                <div className="mt-1 relative">
                  <input
                    id="password"
                    type={showPw ? "text" : "password"}
                    autoComplete="current-password"
                    placeholder="••••••••"
                    className="w-full rounded-xl border border-gray-300 bg-white px-4 py-3 text-gray-900 placeholder:text-gray-400 focus:outline-none focus:ring-4 focus:ring-amber-200 focus:border-amber-500 transition pr-12"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    disabled={submitting}
                    required
                    minLength={6}
                  />
                </div>
              </div>

              {/* Submit */}
              <button
                type="submit"
                disabled={submitting}
                className="w-full rounded-xl bg-amber-600 text-white font-semibold py-3 hover:bg-amber-700 focus:outline-none focus:ring-4 focus:ring-amber-200 transition disabled:opacity-60"
              >
                {submitting ? "Đang xử lý…" : "Đăng nhập"}
              </button>

              {/* Divider */}
              <div className="relative text-center text-sm text-gray-500">
                <span className="bg-white px-3 relative z-10">hoặc</span>
                <div className="absolute inset-x-0 top-1/2 h-px bg-gray-200 -z-0" />
              </div>

              {/* Google button (render bởi SDK) */}
              <div
                id="google-login-btn"
                className="w-full flex justify-center"
              ></div>

              {/* Facebook button */}
              <button
                type="button"
                onClick={handleFBLogin}
                disabled={submitting}
                className="flex items-center justify-center gap-2 w-full rounded-xl border border-gray-300 bg-[#1877F2] py-2.5 text-white hover:brightness-95 focus:outline-none focus:ring-4 focus:ring-amber-200 transition"
              >
                Đăng nhập với Facebook
              </button>

              {/* Register link */}
              <p className="text-sm text-#16a34a text-center">
                Chưa có tài khoản?{" "}
                <Link
                  to="/register/customer"
                  className="text-amber-700 hover:underline"
                >
                  Đăng ký ngay
                </Link>
              </p>

              <p className="text-sm text-gray-600 text-center mt-2">
                <Link
                  to="/forgot-password"
                  className="text-amber-700 hover:underline"
                >
                  Quên mật khẩu?
                </Link>
              </p>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}
