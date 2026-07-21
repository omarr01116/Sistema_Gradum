import { createContext, useContext, useState, useEffect } from "react";
import { login as loginRequest } from "../services/authService";

const AuthContext = createContext(null);
const STORAGE_KEY = "gradum_session";

export function AuthProvider({ children }) {
  const [session, setSession] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const raw = sessionStorage.getItem(STORAGE_KEY);
    if (raw) {
      const stored = JSON.parse(raw);
      if (new Date(stored.expiraEn) > new Date()) {
        setSession(stored);
      } else {
        sessionStorage.removeItem(STORAGE_KEY);
      }
    }
    setLoading(false);
  }, []);

  async function login(nombreUsuario, password) {
    const data = await loginRequest(nombreUsuario, password);
    sessionStorage.setItem(STORAGE_KEY, JSON.stringify(data));
    setSession(data);
    return data;
  }

  function logout() {
    sessionStorage.removeItem(STORAGE_KEY);
    setSession(null);
  }

  const value = {
    isAuthenticated: !!session,
    nombreUsuario: session?.nombreUsuario ?? null,
    rol: session?.rol ?? null,
    asesorId: session?.asesorId ?? null,
    login,
    logout,
  };

  if (loading) return null;

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth debe usarse dentro de <AuthProvider>");
  }
  return context;
}