import { useAuth } from "../hooks/useAuth";
import { useNavigate, Link } from "react-router-dom";

export default function Home() {
  const { auth, signOut } = useAuth();
  const nav = useNavigate();

  return (
    <div className="min-h-screen flex items-center justify-center p-6">
      <div className="max-w-xl w-full rounded-2xl border shadow p-6 space-y-3">
        <h1 className="text-2xl font-bold">Xin chào, {auth?.email ?? "bạn"} </h1>
        <p>Role: <b>{auth?.role ?? "(chưa có role trong token)"}</b></p>
        <div className="flex gap-3 pt-2">
          <Link to="/login" className="px-4 py-2 rounded-lg border">Về trang đăng nhập</Link>
          <button
            onClick={() => { signOut(); nav("/login"); }}
            className="px-4 py-2 rounded-lg bg-black text-white"
          >
            Đăng xuất
          </button>
        </div>
      </div>
    </div>
  );
}
