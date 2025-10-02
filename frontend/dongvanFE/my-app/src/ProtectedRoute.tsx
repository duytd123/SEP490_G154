import type { ReactElement } from "react";
import { Navigate } from "react-router-dom";

type Props = { children: ReactElement; roles?: string[] };

export default function ProtectedRoute({ children, roles }: Props) {
  const s = localStorage.getItem("auth");
  if (!s) return <Navigate to="/login" replace />;

  const role = (JSON.parse(s || "{}") as any)?.role ?? "";
  if (roles && !roles.includes(role)) return <Navigate to="/" replace />;
  return children;
}
