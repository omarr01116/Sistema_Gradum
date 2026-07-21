// Nombres visibles del rol (Header, badges). Las claves coinciden
// exactamente con Usuario.Rol del backend.
export const ROLE_LABELS = {
  Administrador: "Administrador / Gerencia",
  Coordinador: "Coordinador Académico",
  Asesor: "Asesor Académico",
};

// Ítems de navegación por rol. `path: null` = módulo aún no
// implementado (se muestra deshabilitado en el Sidebar).
export const MENU_POR_ROL = {
  Administrador: [
    { label: "Dashboard", icon: "dashboard", path: "/" },
    { label: "Usuarios", icon: "group", path: null },
    { label: "Asesores", icon: "school", path: null },
    { label: "Alertas", icon: "notifications", path: null },
    { label: "Configuración", icon: "settings", path: null },
  ],
  Coordinador: [
    { label: "Dashboard", icon: "dashboard", path: "/" },
    { label: "Clientes", icon: "group", path: null },
    { label: "Proyectos", icon: "folder_special", path: null },
    { label: "Asesores", icon: "school", path: null },
    { label: "Alertas", icon: "notifications", path: null },
  ],
  Asesor: [
    { label: "Dashboard", icon: "dashboard", path: "/" },
    { label: "Mis Proyectos", icon: "folder_special", path: null },
    { label: "Alertas", icon: "notifications", path: null },
  ],
};