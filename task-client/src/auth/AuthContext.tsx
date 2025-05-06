import React, { createContext, useContext, useState } from "react";
import api from "../api/http";

interface Auth {
  accessToken: string | null;
  login: (u: string, p: string) => Promise<void>;
  logout: () => void;
}

const Ctx = createContext<Auth>(null!);
export const useAuth = () => useContext(Ctx);

/* ─ helper: распаковать payload ─ */
function parseJwt<T = any>(token: string): T | null {
  try {
    const base64 = token.split(".")[1]
      .replace(/-/g, "+")
      .replace(/_/g, "/");
    const json = decodeURIComponent(
      atob(base64)
        .split("")
        .map(c => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
        .join("")
    );
    return JSON.parse(json);
  } catch {
    return null;
  }
}

/* ─ provider ─ */
export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [accessToken, setToken] = useState(localStorage.getItem("accessToken"));

  const login = async (userName: string, password: string) => {
    const { data } = await api.post<{ accessToken: string }>(
      "/auth/login",
      { userName, password },
      { withCredentials: true }
    );
  
    localStorage.setItem("accessToken", data.accessToken);
    setToken(data.accessToken);
  
    const claims = parseJwt<Record<string, string>>(data.accessToken) ?? {};
  
    let role = claims["Role"];
    console.log(claims)
  
    const uName = claims["unique_name"] ?? claims["sub"] ?? userName;
  
    sessionStorage.setItem("userRole", role);
    sessionStorage.setItem("userName", uName);
  };
  
  const logout = () => {
    localStorage.clear();
    sessionStorage.clear();
    setToken(null);
  };

  return (
    <Ctx.Provider value={{ accessToken, login, logout }}>
      {children}
    </Ctx.Provider>
  );
};
    