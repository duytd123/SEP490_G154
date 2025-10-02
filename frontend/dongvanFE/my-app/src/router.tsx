import { createBrowserRouter, Navigate } from "react-router-dom";
import type { RouteObject } from "react-router-dom";

import Layout from "./components/Layout";

import HomePage from "./pages/HomePage";
import Home from "./pages/Home";
import Login from "./pages/auth/Login";
import RegisterCustomer from "./pages/auth/RegisterCustomer";
import RegisterSeller from "./pages/auth/RegisterSeller";
import RegisterHost from "./pages/auth/RegisterHost";
import ForgotPassword from "./pages/auth/ForgotPassword";

const routes: RouteObject[] = [
  {
    path: "/",
    element: <Layout />, // Layout chung (Header + Footer)
    children: [
      { index: true, element: <HomePage /> },
      { path: "home", element: <Home /> },
      { path: "register/customer", element: <RegisterCustomer /> },
      { path: "register/seller", element: <RegisterSeller /> },
      { path: "register/host", element: <RegisterHost /> },
    ],
  },

  { path: "/login", element: <Login /> },

  {path: "/forgot-password", element:<ForgotPassword />} ,

  { path: "/register", element: <Navigate to="/register/customer" replace /> },

  { path: "*", element: <Navigate to="/" replace /> },
];

export const router = createBrowserRouter(routes);
