// Espeja la máquina de estados de la sección 8 del caso práctico.
// badgeVariant referencia las variantes de components/ui/Badge.jsx.
export const ESTADOS_PROYECTO = {
  Activo: {
    label: "Activo",
    badgeVariant: "info",
    transicionesValidas: ["Pausado", "Correcciones", "Sustentado"],
  },
  Pausado: {
    label: "Pausado",
    badgeVariant: "neutral",
    transicionesValidas: ["Activo"],
  },
  Correcciones: {
    label: "Correcciones",
    badgeVariant: "danger",
    transicionesValidas: ["Activo"],
  },
  Sustentado: {
    label: "Sustentado",
    badgeVariant: "warning",
    transicionesValidas: ["Finalizado"],
  },
  Finalizado: {
    label: "Finalizado",
    badgeVariant: "success",
    transicionesValidas: [],
  },
};

// Orden fijo de las columnas del tablero.
export const ORDEN_COLUMNAS = ["Activo", "Pausado", "Correcciones", "Sustentado", "Finalizado"];