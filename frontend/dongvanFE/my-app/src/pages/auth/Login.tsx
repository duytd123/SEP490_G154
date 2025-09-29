import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import {
  loginPassword,
  loginWithGoogleCustomer,
  loginWithFacebookCustomer
} from "../../api/auth";

export default function DongVanLogin({
  onLogin,
  onLoginWithGoogle,
  onLoginWithFacebook,
}: {
  onLogin?: (email: string, password: string) => Promise<void> | void;
  onLoginWithGoogle?: () => Promise<void> | void;
  onLoginWithFacebook?: () => Promise<void> | void;
}) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPw, setShowPw] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const nav = useNavigate();

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
      setError(err?.response?.data ?? err?.message ?? "Đăng nhập thất bại, vui lòng thử lại.");
    } finally {
      setSubmitting(false);
    }
  };

  // DEMO: dùng Customer. Muốn Seller/Host → đổi hàm tương ứng hoặc truyền props.
  const handleGoogle = async () => {
    try {
      setSubmitting(true);
      if (onLoginWithGoogle) await onLoginWithGoogle();
      else {
        const idToken = (window as any).__GOOGLE_ID_TOKEN__;
        if (!idToken) throw new Error("Chưa có Google idToken. Tích hợp Google SDK để lấy idToken.");
        await loginWithGoogleCustomer(idToken);
      }
      nav("/home");
    } catch (err: any) {
      setError(err?.response?.data?.message ?? err?.message ?? "Không thể đăng nhập bằng Google.");
    } finally {
      setSubmitting(false);
    }
  };

  const handleFacebook = async () => {
    try {
      setSubmitting(true);
      if (onLoginWithFacebook) await onLoginWithFacebook();
      else {
        const fbAccessToken = (window as any).__FB_ACCESS_TOKEN__;
        if (!fbAccessToken) throw new Error("Chưa có Facebook accessToken. Tích hợp FB SDK để lấy accessToken.");
        await loginWithFacebookCustomer(fbAccessToken);
      }
      nav("/home");
    } catch (err: any) {
      setError(err?.response?.data?.message ?? err?.message ?? "Không thể đăng nhập bằng Facebook.");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen w-full bg-gradient-to-b from-amber-50 via-orange-50 to-rose-50 flex items-center justify-center p-4">
      <div className="grid w-full max-w-6xl grid-cols-1 md:grid-cols-2 gap-6">
        {/* Left: Brand / Hero */}
        <div className="relative overflow-hidden rounded-3xl shadow-xl bg-white/70 backdrop-blur">
          <div className="absolute inset-0 opacity-20" aria-hidden>
            <svg className="w-full h-full" xmlns="http://www.w3.org/2000/svg">
              <defs>
                <pattern id="motif" width="80" height="80" patternUnits="userSpaceOnUse" patternTransform="rotate(20)">
                  <path d="M0 40h80M40 0v80" stroke="currentColor" className="text-amber-200" strokeWidth="1" />
                  <circle cx="40" cy="40" r="6" className="text-rose-200" fill="currentColor" />
                </pattern>
              </defs>
              <rect width="100%" height="100%" fill="url(#motif)" />
            </svg>
          </div>
          <div className="relative p-8 md:p-12">
            <div className="flex items-center gap-3">
              <svg width="40" height="40" viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg" className="shrink-0">
                <path d="M8 40c0 8.837 10.745 16 24 16s24-7.163 24-16H8z" fill="#fb923c"/>
                <path d="M16 40c0 5.523 7.163 10 16 10s16-4.477 16-10H16z" fill="#f97316"/>
                <path d="M8 24l12-8 12 10 12-12 12 10" stroke="#16a34a" strokeWidth="4" fill="none" strokeLinecap="round"/>
              </svg>
              <div>
                <h1 className="text-2xl md:text-3xl font-extrabold text-gray-900">Đồng Văn Ecoverse</h1>
                <p className="text-sm text-gray-600">Kết nối du lịch văn hoá & ẩm thực Đồng Văn – Hà Giang</p>
              </div>
            </div>

            <div className="mt-8 space-y-4 text-gray-700">
              <p className="leading-relaxed">
                Trải nghiệm bản địa: homestay đá, chợ phiên, thắng cố, phở chua, mật ong bạc hà…
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

        {/* Right: Login Card */}
        <div className="flex items-center">
          <div className="w-full rounded-3xl bg-white shadow-xl p-6 md:p-10 border border-gray-100">
            <h2 className="text-xl md:text-2xl font-bold text-gray-900">Đăng nhập</h2>
            <p className="text-sm text-gray-600 mt-1">Chào mừng bạn trở lại với hành trình văn hoá & ẩm thực Đồng Văn.</p>

            <form onSubmit={handleSubmit} className="mt-6 space-y-5" noValidate>
              {error && (
                <div className="rounded-xl border border-rose-200 bg-rose-50 p-3 text-rose-700 text-sm">
                  {error}
                </div>
              )}

              <div>
                <label htmlFor="email" className="block text-sm font-medium text-gray-700">Email</label>
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

              <div>
                <div className="flex items-center justify-between">
                  <label htmlFor="password" className="block text-sm font-medium text-gray-700">Mật khẩu</label>
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
                  <svg className="absolute right-3 top-1/2 -translate-y-1/2" width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5">
                    <rect x="3" y="11" width="18" height="10" rx="2" className="text-gray-300" stroke="currentColor" />
                    <path d="M7 11V8a5 5 0 1 1 10 0v3" className="text-gray-300" stroke="currentColor" />
                  </svg>
                </div>
              </div>

              <div className="flex items-center justify-between text-sm">
                <label className="inline-flex items-center gap-2 select-none">
                  <input type="checkbox" className="h-4 w-4 rounded border-gray-300 text-amber-600 focus:ring-amber-500" />
                  Ghi nhớ đăng nhập
                </label>
                <a href="#" onClick={(e)=>e.preventDefault()} className="text-amber-700 hover:underline">Quên mật khẩu?</a>
              </div>

              <button
                type="submit"
                disabled={submitting}
                className="w-full rounded-xl bg-amber-600 text-white font-semibold py-3 hover:bg-amber-700 focus:outline-none focus:ring-4 focus:ring-amber-200 transition disabled:opacity-60"
              >
                {submitting ? "Đang xử lý…" : "Đăng nhập"}
              </button>

              <div className="relative text-center text-sm text-gray-500">
                <span className="bg-white px-3 relative z-10">hoặc</span>
                <div className="absolute inset-x-0 top-1/2 h-px bg-gray-200 -z-0" />
              </div>

              <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                <button
                  type="button"
                  onClick={handleGoogle}
                  disabled={submitting}
                  className="flex items-center justify-center gap-2 w-full rounded-xl border border-gray-300 bg-white py-2.5 hover:bg-gray-50 focus:outline-none focus:ring-4 focus:ring-amber-200 transition"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 48 48" width="20" height="20" aria-hidden>
                    <path fill="#FFC107" d="M43.611 20.083H42V20H24v8h11.303C33.621 32.262 29.226 35 24 35 16.82 35 11 29.18 11 22S16.82 9 24 9c3.59 0 6.84 1.37 9.31 3.59l5.66-5.66C35.89 3.26 30.28 1 24 1 10.75 1 0 11.75 0 25s10.75 24 24 24 24-10.75 24-24c0-1.61-.17-3.17-.389-4.917z"/>
                    <path fill="#FF3D00" d="M6.306 14.691l6.571 4.813C14.5 16.108 18.89 13 24 13c3.59 0 6.84 1.37 9.31 3.59l5.66-5.66C35.89 3.26 30.28 1 24 1 14.56 1 6.51 6.16 2.69 13.37z"/>
                    <path fill="#4CAF50" d="M24 49c6.09 0 11.64-2.34 15.8-6.16l-7.29-5.98C30.21 38.82 27.26 40 24 40c-5.2 0-9.58-2.72-11.94-6.78l-6.58 5.06C6.5 44.02 14.57 49 24 49z"/>
                    <path fill="#1976D2" d="M43.611 20.083H42V20H24v8h11.303c-1.349 3.179-4.66 5.417-8.303 5.417-5.2 0-9.58-2.72-11.94-6.78l-6.58 5.06C6.5 44.02 14.57 49 24 49c10.5 0 19.39-7.25 21.82-17z"/>
                  </svg>
                  Đăng nhập với Google
                </button>

                <button
                  type="button"
                  onClick={handleFacebook}
                  disabled={submitting}
                  className="flex items-center justify-center gap-2 w-full rounded-xl border border-gray-300 bg-[#1877F2] py-2.5 text-white hover:brightness-95 focus:outline-none focus:ring-4 focus:ring-amber-200 transition"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="18" height="18" fill="currentColor" aria-hidden>
                    <path d="M22 12.07C22 6.48 17.52 2 11.93 2S1.86 6.48 1.86 12.07c0 5.03 3.68 9.2 8.48 9.93v-7.02H7.9v-2.91h2.44V9.84c0-2.42 1.44-3.76 3.65-3.76 1.06 0 2.17.19 2.17.19v2.38h-1.22c-1.2 0-1.57.74-1.57 1.49v1.79h2.67l-.43 2.91h-2.24V22c4.8-.73 8.48-4.9 8.48-9.93z"/>
                  </svg>
                  Đăng nhập với Facebook
                </button>
              </div>

              <p className="text-sm text-gray-600 text-center">
  Chưa có tài khoản? <Link to="/register" className="text-amber-700 hover:underline">Đăng ký ngay</Link>
</p>

            </form>
          </div>
        </div>
      </div>
    </div>
  );
}
