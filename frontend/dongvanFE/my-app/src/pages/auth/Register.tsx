import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
 import {
   registerAdmin,
   registerSeller,
   registerHost,
   registerCustomer,
   type RegisterBody,
 } from "../../api/auth";

type RoleKey = "Admin" | "Seller" | "Host" | "Customer";

export default function Register() {
  const nav = useNavigate();
  const [form, setForm] = useState<RegisterBody & { confirmPassword: string }>({
    email: "",
    password: "",
    confirmPassword: "",
    fullName: "",
    phone: "",
  });
  const [role, setRole] = useState<RoleKey>("Customer");
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [ok, setOk] = useState<string | null>(null);

  const onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setForm(f => ({ ...f, [name]: value }));
  };

  const doRegister = async () => {
    const body: RegisterBody = {
      email: form.email.trim().toLowerCase(),
      password: form.password,
      fullName: form.fullName.trim(),
      phone: form.phone.trim(),
    };
    switch (role) {
      case "Admin":    return registerAdmin(body);
      case "Seller":   return registerSeller(body);
      case "Host":     return registerHost(body);
      case "Customer": return registerCustomer(body);
    }
  };

  const validate = () => {
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) return "Email không hợp lệ.";
    if (form.password.length < 6) return "Mật khẩu tối thiểu 6 ký tự.";
    if (form.password !== form.confirmPassword) return "Xác nhận mật khẩu không khớp.";
    if (!form.fullName.trim()) return "Vui lòng nhập họ tên.";
    if (!/^[0-9+\-\s]{8,20}$/.test(form.phone.trim())) return "Số điện thoại không hợp lệ.";
    return null;
  };

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setOk(null);
    const msg = validate();
    if (msg) { setError(msg); return; }

    try {
      setSubmitting(true);
      const res = await doRegister();
      setOk(`Đăng ký thành công cho ${res.email} (role: ${res.role}).`);
      setTimeout(() => nav("/login"), 800);
    } catch (err: any) {
      const apiMsg = err?.response?.data ?? err?.message;
      setError(typeof apiMsg === "string" ? apiMsg : "Đăng ký thất bại.");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen w-full bg-gradient-to-b from-amber-50 via-orange-50 to-rose-50 flex items-center justify-center p-4">
      <div className="grid w-full max-w-6xl grid-cols-1 md:grid-cols-2 gap-6">
        {/* Left - Intro */}
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
                <p className="text-sm text-gray-600">Đăng ký tài khoản để bắt đầu chuyến đi văn hoá & ẩm thực.</p>
              </div>
            </div>
          </div>
        </div>

        {/* Right - Form */}
        <div className="flex items-center">
          <div className="w-full rounded-3xl bg-white shadow-xl p-6 md:p-10 border border-gray-100">
            <h2 className="text-xl md:text-2xl font-bold text-gray-900">Đăng ký</h2>
            <p className="text-sm text-gray-600 mt-1">
              Tạo tài khoản mới cho nền tảng Đồng Văn Ecoverse.
            </p>

            <form onSubmit={onSubmit} className="mt-6 space-y-5" noValidate>
              {error && <div className="rounded-xl border border-rose-200 bg-rose-50 p-3 text-rose-700 text-sm">{error}</div>}
              {ok && <div className="rounded-xl border border-emerald-200 bg-emerald-50 p-3 text-emerald-800 text-sm">{ok}</div>}

              <div>
                <label className="block text-sm font-medium text-gray-700">Vai trò</label>
                <select
                  value={role}
                  onChange={(e) => setRole(e.target.value as RoleKey)}
                  className="mt-1 w-full rounded-xl border border-gray-300 bg-white px-4 py-3 text-gray-900 focus:outline-none focus:ring-4 focus:ring-amber-200 focus:border-amber-500 transition"
                >
                  <option value="Customer">Khách hàng (Customer)</option>
                  <option value="Host">Chủ homestay (Host)</option>
                  <option value="Seller">Người bán đặc sản (Seller)</option>
                  <option value="Admin">Quản trị (Admin)</option>
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Họ tên</label>
                <input
                  name="fullName"
                  value={form.fullName}
                  onChange={onChange}
                  placeholder="Nguyễn Văn A"
                  className="mt-1 w-full rounded-xl border border-gray-300 bg-white px-4 py-3 text-gray-900 placeholder:text-gray-400 focus:outline-none focus:ring-4 focus:ring-amber-200 focus:border-amber-500 transition"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Số điện thoại</label>
                <input
                  name="phone"
                  value={form.phone}
                  onChange={onChange}
                  placeholder="09xx xxx xxx"
                  className="mt-1 w-full rounded-xl border border-gray-300 bg-white px-4 py-3 text-gray-900 placeholder:text-gray-400 focus:outline-none focus:ring-4 focus:ring-amber-200 focus:border-amber-500 transition"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Email</label>
                <input
                  name="email"
                  type="email"
                  value={form.email}
                  onChange={onChange}
                  placeholder="ban@vidu.com"
                  className="mt-1 w-full rounded-xl border border-gray-300 bg-white px-4 py-3 text-gray-900 placeholder:text-gray-400 focus:outline-none focus:ring-4 focus:ring-amber-200 focus:border-amber-500 transition"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Mật khẩu</label>
                <input
                  name="password"
                  type="password"
                  value={form.password}
                  onChange={onChange}
                  placeholder="••••••••"
                  minLength={6}
                  className="mt-1 w-full rounded-xl border border-gray-300 bg-white px-4 py-3 text-gray-900 placeholder:text-gray-400 focus:outline-none focus:ring-4 focus:ring-amber-200 focus:border-amber-500 transition"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Xác nhận mật khẩu</label>
                <input
                  name="confirmPassword"
                  type="password"
                  value={form.confirmPassword}
                  onChange={onChange}
                  placeholder="••••••••"
                  minLength={6}
                  className="mt-1 w-full rounded-xl border border-gray-300 bg-white px-4 py-3 text-gray-900 placeholder:text-gray-400 focus:outline-none focus:ring-4 focus:ring-amber-200 focus:border-amber-500 transition"
                  required
                />
              </div>

              <button
                type="submit"
                disabled={submitting}
                className="w-full rounded-xl bg-amber-600 text-white font-semibold py-3 hover:bg-amber-700 focus:outline-none focus:ring-4 focus:ring-amber-200 transition disabled:opacity-60"
              >
                {submitting ? "Đang xử lý…" : "Tạo tài khoản"}
              </button>

              <p className="text-sm text-gray-600 text-center">
                Đã có tài khoản?{" "}
                <Link to="/login" className="text-amber-700 hover:underline">Đăng nhập</Link>
              </p>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}
