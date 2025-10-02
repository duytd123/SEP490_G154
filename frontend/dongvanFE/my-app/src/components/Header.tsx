import { Link } from "react-router-dom";
import { useEffect, useState } from "react";
type HeaderProps = {
  /** Đường dẫn logo chính ở header (PNG/SVG) */ logoSrc?: string;
};
export default function Header({
  logoSrc = "/images/trendytravel-logo.png",
}: HeaderProps) {
  const [isTop, setIsTop] = useState(true);
  const [openRegister, setOpenRegister] = useState(false);
  useEffect(() => {
    const handleScroll = () => {
      setIsTop(window.scrollY === 0);
    };
    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);
  return (
    <header className="w-full shadow sticky top-0 z-50">
      {" "}
      {/* TOP BAR: xanh + Login/Register có logo - chỉ hiện khi ở top */}{" "}
      {isTop && (
        <div className="bg-sky-600 text-white text-sm py-2 px-6 flex items-center justify-between transition-all duration-300">
          {" "}
          <div className="flex items-center gap-2">
            {" "}
            <PhoneIcon className="w-4 h-4" />{" "}
            <span>Any Questions? Call Us: 1-223-355-2214</span>{" "}
          </div>{" "}
          <div className="flex items-center gap-5">
            {" "}
            <Link
              to="/login"
              className="flex items-center gap-1 hover:underline"
            >
              {" "}
              <UserIcon className="w-4 h-4" /> <span>Login</span>{" "}
            </Link>{" "}
            <div className="relative">
              {" "}
              <button
                onClick={() => setOpenRegister(!openRegister)}
                className="flex items-center gap-1 hover:underline"
              >
                {" "}
                <UserPlusIcon className="w-4 h-4" /> <span>Register</span>{" "}
              </button>{" "}
              {/* Dropdown menu */}{" "}
              {openRegister && (
                <div className="absolute right-0 mt-2 bg-white text-gray-800 rounded shadow-md w-48 z-50">
                  {" "}
                  <Link
                    to="/register/customer"
                    className="flex items-center gap-2 px-4 py-2 hover:bg-gray-100"
                    onClick={() => setOpenRegister(false)}
                  >
                    {" "}
                    <UserIcon className="w-4 h-4 text-sky-600" />{" "}
                    <span>Customer</span>{" "}
                  </Link>{" "}
                  <Link
                    to="/register/seller"
                    className="flex items-center gap-2 px-4 py-2 hover:bg-gray-100"
                    onClick={() => setOpenRegister(false)}
                  >
                    {" "}
                    <StoreIcon className="w-4 h-4 text-green-600" />{" "}
                    <span>Seller (Farmer)</span>{" "}
                  </Link>{" "}
                  <Link
                    to="/register/host"
                    className="flex items-center gap-2 px-4 py-2 hover:bg-gray-100"
                    onClick={() => setOpenRegister(false)}
                  >
                    {" "}
                    <HomeIcon className="w-4 h-4 text-yellow-600" />{" "}
                    <span>Host (Homestay)</span>{" "}
                  </Link>{" "}
                </div>
              )}{" "}
            </div>{" "}
          </div>{" "}
        </div>
      )}{" "}
      {/* MAIN NAV: nền trắng + logo bên trái */}{" "}
      <div className="bg-white flex items-center justify-between px-8 py-4">
        {" "}
        {/* Logo hình ảnh */}{" "}
        <Link to="/" className="flex items-center gap-3">
          {" "}
          <img
            src={logoSrc}
            alt="TrendyTravel"
            className="h-10 w-auto object-contain"
          />{" "}
        </Link>{" "}
        {/* Menu giữa (active Home màu vàng) */}{" "}
        <nav className="flex gap-6 font-medium text-gray-700">
          {" "}
          <Link to="/" className="text-yellow-500">
            {" "}
            Home{" "}
          </Link>{" "}
          <Link to="/hotels" className="hover:text-sky-600">
            {" "}
            Hotels{" "}
          </Link>{" "}
          <Link to="/pages" className="hover:text-sky-600">
            {" "}
            Pages{" "}
          </Link>{" "}
          <Link to="/places" className="hover:text-sky-600">
            {" "}
            Places{" "}
          </Link>{" "}
          <Link to="/gallery" className="hover:text-sky-600">
            {" "}
            Gallery{" "}
          </Link>{" "}
          <Link to="/packages" className="hover:text-sky-600">
            {" "}
            Packages{" "}
          </Link>{" "}
          <Link to="/blog" className="hover:text-sky-600">
            {" "}
            Blog{" "}
          </Link>{" "}
          <Link to="/shortcodes" className="hover:text-sky-600">
            {" "}
            Shortcodes{" "}
          </Link>{" "}
        </nav>{" "}
      </div>{" "}
    </header>
  );
}
/* ===== Inline SVG icons (không cần lib ngoài) ===== */ function PhoneIcon(
  props: React.SVGProps<SVGSVGElement>
) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" {...props}>
      {" "}
      <path
        strokeWidth="2"
        strokeLinecap="round"
        strokeLinejoin="round"
        d="M2 5a2 2 0 0 1 2-2h2l2 5-2 1a14 14 0 0 0 7 7l1-2 5 2v2a2 2 0 0 1-2 2h-1C9.163 20 4 14.837 4 8V7a2 2 0 0 1-2-2z"
      />{" "}
    </svg>
  );
}
function UserIcon(props: React.SVGProps<SVGSVGElement>) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" {...props}>
      {" "}
      <path
        strokeWidth="2"
        strokeLinecap="round"
        strokeLinejoin="round"
        d="M16 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"
      />{" "}
      <circle cx="12" cy="7" r="4" strokeWidth="2" />{" "}
    </svg>
  );
}
function UserPlusIcon(props: React.SVGProps<SVGSVGElement>) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" {...props}>
      {" "}
      <path
        strokeWidth="2"
        strokeLinecap="round"
        strokeLinejoin="round"
        d="M16 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"
      />{" "}
      <circle cx="12" cy="7" r="4" strokeWidth="2" />{" "}
      <path
        strokeWidth="2"
        strokeLinecap="round"
        strokeLinejoin="round"
        d="M19 8v6M22 11h-6"
      />{" "}
    </svg>
  );
}
function StoreIcon(props: React.SVGProps<SVGSVGElement>) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" {...props}>
      {" "}
      <path
        strokeWidth="2"
        strokeLinecap="round"
        strokeLinejoin="round"
        d="M3 9l1-5h16l1 5M4 9h16v11H4V9z"
      />{" "}
    </svg>
  );
}
function HomeIcon(props: React.SVGProps<SVGSVGElement>) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" {...props}>
      {" "}
      <path
        strokeWidth="2"
        strokeLinecap="round"
        strokeLinejoin="round"
        d="M3 9.5L12 3l9 6.5V21a1 1 0 0 1-1 1h-5v-7H9v7H4a1 1 0 0 1-1-1V9.5z"
      />{" "}
    </svg>
  );
}
