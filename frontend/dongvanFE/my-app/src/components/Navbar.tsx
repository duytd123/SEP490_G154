import { Link, useNavigate } from "react-router-dom";

export default function Navbar() {
  const nav = useNavigate();
  const s = localStorage.getItem("auth");
  const role = s ? JSON.parse(s).role : null;
  const logout = () => { localStorage.removeItem("auth"); nav("/login"); };

  return (
    <div className="nav">
      <Link to="/">Feed</Link>
      <Link to="/profile">Profile</Link>
      {role === "Admin" && <Link to="/admin/moderation">Moderation</Link>}
      <button onClick={logout}>Logout</button>
    </div>
  );
}
