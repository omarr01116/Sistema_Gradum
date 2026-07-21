// Nombres visibles del rol (Header, badges). Las claves coinciden
// exactamente con Usuario.Rol del backend.
export const ROLE_LABELS = {
  Administrador: "Administrador / Gerencia",
  Coordinador: "Coordinador Académico",
  Asesor: "Asesor Académico",
};

// Ítems de navegación por rol.
export const MENU_POR_ROL = {
  Administrador: [
    { label: "Dashboard", icon: "LayoutDashboard", path: "/" },
    { label: "Clientes", icon: "Users", path: "/clientes" },
    { label: "Proyectos", icon: "FolderKanban", path: "/proyectos" },
    { label: "Asesores", icon: "GraduationCap", path: "/asesores" },
    { label: "Usuarios", icon: "Users", path: "/usuarios" },
    { label: "Alertas", icon: "Bell", path: "/alertas" },
    { label: "Configuración", icon: "Settings", path: "/configuracion" },
  ],
  Coordinador: [
    { label: "Dashboard", icon: "LayoutDashboard", path: "/" },
    { label: "Clientes", icon: "Users", path: "/clientes" },
    { label: "Proyectos", icon: "FolderKanban", path: "/proyectos" },
    { label: "Asesores", icon: "GraduationCap", path: "/asesores" },
    { label: "Alertas", icon: "Bell", path: "/alertas" },
  ],
  Asesor: [
    { label: "Dashboard", icon: "LayoutDashboard", path: "/" },
    { label: "Mis Proyectos", icon: "FolderKanban", path: "/proyectos" },
    { label: "Alertas", icon: "Bell", path: "/alertas" },
  ],
};
