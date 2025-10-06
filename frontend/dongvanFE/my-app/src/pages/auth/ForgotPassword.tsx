import { useState } from "react";
import { forgotPassword } from "../../api/auth";
import { useNavigate } from "react-router-dom";

export default function ForgotPassword() {
  const [step, setStep] = useState<1 | 2>(1);
  const [email, setEmail] = useState("");
  const [token, setToken] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPw, setConfirmPw] = useState("");
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const nav = useNavigate();

  // Bước 1: nhập email để lấy reset token
  const handleSendEmail = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setMessage(null);

    try {
      setLoading(true);
      const res = await forgotPassword(email);
      setMessage(res.message ?? "Mã đặt lại mật khẩu đã được tạo. Vui lòng nhập mã để tiếp tục.");
      setStep(2);
    } catch (err: any) {
      setError(err?.response?.data?.message ?? "Không thể gửi yêu cầu.");
    } finally {
      setLoading(false);
    }
  };

  // Bước 2: dùng token để reset mật khẩu
  const handleResetPassword = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setMessage(null);

    if (password !== confirmPw) {
      setError("Mật khẩu xác nhận không khớp.");
      return;
    }

    try {
      setLoading(true);
     // const res = await resetPasswordWithToken(token, password);
    //  setMessage(res.message ?? "Đặt lại mật khẩu thành công!");
      setTimeout(() => nav("/login"), 2000);
    } catch (err: any) {
      setError(err?.response?.data?.message ?? "Không thể đặt lại mật khẩu.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-b from-amber-50 via-orange-50 to-rose-50 p-4">
      <div className="w-full max-w-md bg-white rounded-3xl shadow-xl p-6 md:p-10">
        <h2 className="text-xl font-bold text-gray-900 mb-4">
          {step === 1 ? "Quên mật khẩu" : "Đặt lại mật khẩu"}
        </h2>
        <p className="text-sm text-gray-600 mb-6">
          {step === 1
            ? "Nhập email của bạn để nhận mã đặt lại mật khẩu."
            : "Nhập mã xác thực và mật khẩu mới để tiếp tục."}
        </p>

        {error && (
          <div className="mb-4 p-3 rounded-lg bg-rose-50 border border-rose-200 text-rose-700 text-sm">
            {error}
          </div>
        )}
        {message && (
          <div className="mb-4 p-3 rounded-lg bg-emerald-50 border border-emerald-200 text-emerald-700 text-sm">
            {message}
          </div>
        )}

        {step === 1 ? (
          <form onSubmit={handleSendEmail} className="space-y-4">
            <input
              type="email"
              placeholder="Nhập email của bạn"
              className="w-full rounded-xl border border-gray-300 px-4 py-3 text-gray-900 focus:outline-none focus:ring-4 focus:ring-amber-200 focus:border-amber-500"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              disabled={loading}
              required
            />
            <button
              type="submit"
              disabled={loading}
              className="w-full rounded-xl bg-amber-600 text-white font-semibold py-3 hover:bg-amber-700 focus:outline-none focus:ring-4 focus:ring-amber-200 transition disabled:opacity-60"
            >
              {loading ? "Đang gửi..." : "Gửi mã"}
            </button>
          </form>
        ) : (
          <form onSubmit={handleResetPassword} className="space-y-4">
            <input
              type="text"
              placeholder="Nhập mã reset"
              className="w-full rounded-xl border border-gray-300 px-4 py-3"
              value={token}
              onChange={(e) => setToken(e.target.value)}
              required
              disabled={loading}
            />
            <input
              type="password"
              placeholder="Mật khẩu mới"
              className="w-full rounded-xl border border-gray-300 px-4 py-3"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              disabled={loading}
              minLength={6}
            />
            <input
              type="password"
              placeholder="Xác nhận mật khẩu mới"
              className="w-full rounded-xl border border-gray-300 px-4 py-3"
              value={confirmPw}
              onChange={(e) => setConfirmPw(e.target.value)}
              required
              disabled={loading}
              minLength={6}
            />
            <button
              type="submit"
              disabled={loading}
              className="w-full rounded-xl bg-amber-600 text-white font-semibold py-3 hover:bg-amber-700 focus:outline-none focus:ring-4 focus:ring-amber-200 transition disabled:opacity-60"
            >
              {loading ? "Đang xử lý…" : "Đặt lại mật khẩu"}
            </button>
          </form>
        )}
      </div>
    </div>
  );
}
