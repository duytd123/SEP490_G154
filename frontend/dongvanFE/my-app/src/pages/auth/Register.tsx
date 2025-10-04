import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import type { RegisterBody } from "../../api/auth";

export default function Register({
  roleName,
  api,
}: {
  roleName: string;
  api: (body: RegisterBody) => Promise<any>;
}) {
  const nav = useNavigate();
  const [form, setForm] = useState({
    email: "",
    password: "",
    confirmPassword: "",
    fullName: "",
    phone: "",
  });
  const [error, setError] = useState<string | null>(null);
  const [ok, setOk] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setOk(null);

    if (form.password !== form.confirmPassword) {
      setError("Xác nhận mật khẩu không khớp");
      return;
    }

    try {
      setSubmitting(true);
      await api({
        email: form.email.trim().toLowerCase(),
        password: form.password,
        fullName: form.fullName.trim(),
        phone: form.phone.trim(),
      });
      setOk(`Đăng ký ${roleName} thành công!`);
      setTimeout(() => nav("/login"), 1000);
    } catch (err: any) {
      console.error("❌ API Error:", err);

      if (err.response) {
        const res = err.response;
        const data = res.data;

        // ✅ Ưu tiên lấy lỗi validation ASP.NET (400)
        if (res.status === 400 && data?.errors) {
          const errorsObj = data.errors;
          const allMessages = Object.values(errorsObj)
            .flat()
            .filter(Boolean)
            .join("\n");
          setError(
            allMessages || "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại."
          );
          return;
        }

        // ✅ Nếu backend trả về message/title riêng
        if (data?.message || data?.Message || data?.title) {
          const msg =
            data.message ||
            data.Message ||
            data.title ||
            "Đăng ký thất bại. Vui lòng thử lại.";
          setError(msg);
          return;
        }

        // ✅ Fallback — nếu vẫn không có gì rõ ràng
        setError("Đăng ký thất bại. Vui lòng thử lại.");
      } else if (err.request) {
        // Không kết nối được tới backend
        setError("⚠️ Không thể kết nối tới máy chủ. Vui lòng thử lại sau.");
      } else {
        setError(err.message || "Đã xảy ra lỗi không xác định.");
      }
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-amber-50 p-6">
      <div className="bg-white rounded-2xl shadow-xl p-8 w-full max-w-md">
        <h2 className="text-2xl font-bold text-gray-900">Đăng ký {roleName}</h2>
        {error && (
          <div className="mt-3 rounded-lg border border-red-300 bg-red-50 p-3 text-red-700 text-sm whitespace-pre-line">
             {error}
          </div>
        )}

        {ok && <p className="text-green-600 mt-2">{ok}</p>}

        <form onSubmit={onSubmit} className="mt-4 space-y-4">
          <input
            name="fullName"
            placeholder="Họ tên"
            value={form.fullName}
            onChange={onChange}
            required
            className="w-full border p-3 rounded-lg"
          />
          <input
            name="phone"
            placeholder="Số điện thoại"
            value={form.phone}
            onChange={onChange}
            required
            className="w-full border p-3 rounded-lg"
          />
          <input
            name="email"
            type="email"
            placeholder="Email"
            value={form.email}
            onChange={onChange}
            required
            className="w-full border p-3 rounded-lg"
          />
          <input
            name="password"
            type="password"
            placeholder="Mật khẩu"
            value={form.password}
            onChange={onChange}
            required
            className="w-full border p-3 rounded-lg"
          />
          <input
            name="confirmPassword"
            type="password"
            placeholder="Xác nhận mật khẩu"
            value={form.confirmPassword}
            onChange={onChange}
            required
            className="w-full border p-3 rounded-lg"
          />

          <button
            type="submit"
            disabled={submitting}
            className="w-full bg-amber-600 text-white py-3 rounded-lg hover:bg-amber-700"
          >
            {submitting ? "Đang xử lý..." : "Đăng ký"}
          </button>
        </form>

        <p className="text-sm mt-4 text-center">
          Đã có tài khoản?{" "}
          <Link to="/login" className="text-amber-600">
            Đăng nhập
          </Link>
        </p>
      </div>
    </div>
  );
}
