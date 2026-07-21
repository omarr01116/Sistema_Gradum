export const ROLE_LABELS = {
  Administrador: "Administrador / Gerencia",
  Coordinador: "Coordinador Académico",
  Asesor: "Asesor Académico",
};

export const MENU_POR_ROL = {
  Administrador: [
    { label: "Dashboard", icon: "dashboard", path: "/" },
    { label: "Clientes", icon: "group", path: "/clientes" },
    { label: "Asesores", icon: "school", path: "/asesores" },
    { label: "Usuarios", icon: "manage_accounts", path: null },
    { label: "Alertas", icon: "notifications", path: null },
    { label: "Configuración", icon: "settings", path: null },
  ],
  Coordinador: [
    { label: "Dashboard", icon: "dashboard", path: "/" },
    { label: "Clientes", icon: "group", path: "/clientes" },
    { label: "Proyectos", icon: "folder_special", path: null },
    { label: "Asesores", icon: "school", path: "/asesores" },
    { label: "Alertas", icon: "notifications", path: null },
  ],
  Asesor: [
    { label: "Dashboard", icon: "dashboard", path: "/" },
    { label: "Mis Proyectos", icon: "folder_special", path: null },
    { label: "Alertas", icon: "notifications", path: null },
  ],
};