import { createBrowserRouter, Navigate } from "react-router-dom";
import type { RouteObject } from "react-router-dom";
import Login from "./pages/auth/Login";
import Register from "./pages/auth/Register";
import Home from "./pages/Home";

const routes: RouteObject[] = [
  { 
    path: "/", 
    element: <Navigate to="/login" replace /> 
  },
  { 
    path: "/login", 
    element: <Login /> 
  },
  { 
    path: "/register", 
    element: <Register /> 
  },
  { path: "/home", element: <Home /> },
];

export const router = createBrowserRouter(routes);