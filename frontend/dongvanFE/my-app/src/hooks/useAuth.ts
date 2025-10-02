import { useEffect, useState } from "react";
import { loadAuthFromStorage, logout } from "../api/auth";

type LightweightAuth = { role: string | null; email: string };

export function useAuth() {
  const [auth, setAuth] = useState<LightweightAuth | null>(null);

  useEffect(() => {
    const a = loadAuthFromStorage();
    if (a) setAuth({ role: a.role, email: a.email });
  }, []);

  const signOut = () => { logout(); setAuth(null); };

  return { auth, setAuth, signOut };
}
