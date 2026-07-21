import { Routes, Route } from "react-router-dom";
import ProtectedRoute from "./ProtectedRoute";
import Login from "../pages/Login";
import Dashboard from "../pages/Dashboard";
import ClientesList from "../pages/clientes/ClientesList";
import AsesoresList from "../pages/asesores/AsesoresList";
import ProyectosBoard from "../pages/proyectos/ProyectosBoard";

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/" element={<ProtectedRoute><Dashboard /></ProtectedRoute>} />
      <Route
        path="/clientes"
        element={<ProtectedRoute allowedRoles={["Administrador", "Coordinador"]}><ClientesList /></ProtectedRoute>}
      />
      <Route
        path="/asesores"
        element={<ProtectedRoute allowedRoles={["Administrador", "Coordinador"]}><AsesoresList /></ProtectedRoute>}
      />
      <Route
        path="/proyectos"
        element={
          <ProtectedRoute allowedRoles={["Administrador", "Coordinador", "Asesor"]}>
            <ProyectosBoard />
          </ProtectedRoute>
        }
      />
    </Routes>
  );
}