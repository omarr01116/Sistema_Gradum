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
    { label: "Proyectos", icon: "folder_special", path: "/proyectos" },
    { label: "Asesores", icon: "school", path: "/asesores" }, // activado
    { label: "Usuarios", icon: "group", path: null },          // pendiente: commit nuevo
    { label: "Alertas", icon: "notifications", path: null },   // pendiente: commit nuevo
    { label: "Configuración", icon: "settings", path: null },  // pendiente: commit nuevo
  ],
  Coordinador: [
    { label: "Dashboard", icon: "dashboard", path: "/" },
    { label: "Clientes", icon: "group", path: "/clientes" },
    { label: "Proyectos", icon: "folder_special", path: "/proyectos" },
    { label: "Asesores", icon: "school", path: "/asesores" },
    { label: "Alertas", icon: "notifications", path: null },   // pendiente: commit nuevo
  ],
  Asesor: [
    { label: "Dashboard", icon: "dashboard", path: "/" },
    { label: "Mis Proyectos", icon: "folder_special", path: "/proyectos" },
    { label: "Alertas", icon: "notifications", path: null },   // pendiente: commit nuevo
  ],
};