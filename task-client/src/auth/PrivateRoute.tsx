import { Navigate } from "react-router-dom";
import { useAuth } from "./AuthContext";
import { JSX } from "react";

export const PrivateRoute: React.FC<{ children: JSX.Element }> = ({ children }) => {
  const { accessToken } = useAuth();
  return accessToken ? children : <Navigate to="/Login" replace />;
};
