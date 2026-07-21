import { useState, useEffect } from "react";
import MainLayout from "../layouts/MainLayout";
import PageContainer from "../components/layout/PageContainer";
import Button from "../components/ui/Button";
import Badge from "../components/ui/Badge";
import { getAlertas, marcarAlertaLeida } from "../services/alertaService";

export default function AlertasList() {
  const [alertas, setAlertas] = useState([]);
  const [loading, setLoading] = useState(true);

  const cargarAlertas = async () => {
    try {
      const data = await getAlertas();
      setAlertas(data);
    } catch (err) {
      console.error("Error al cargar alertas");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    cargarAlertas();
  }, []);

  const handleMarcarLeida = async (id) => {
    try {
      await marcarAlertaLeida(id);
      setAlertas(alertas.map(a => a.id === id ? { ...a, leida: true } : a));
    } catch (err) {
      console.error("Error al marcar como leída");
    }
  };

  return (
    <MainLayout>
      <PageContainer title="Centro de Alertas" subtitle="Notificaciones sobre hitos próximos, correcciones y actividad.">
        {loading ? (
          <p className="text-body-sm">Cargando alertas...</p>
        ) : alertas.length === 0 ? (
          <p className="text-body-sm text-on-surface-variant italic">No tienes alertas pendientes.</p>
        ) : (
          <div className="space-y-3">
            {alertas.map((alerta) => (
              <div 
                key={alerta.id} 
                className={`p-4 rounded-xl border transition-colors flex justify-between items-center ${
                  alerta.leida 
                    ? "bg-surface-container-lowest border-outline-variant opacity-60" 
                    : "bg-primary-container/10 border-primary/20 shadow-sm"
                }`}
              >
                <div className="flex gap-4 items-start">
                  <div className={`mt-1 w-2 h-2 rounded-full ${alerta.leida ? "bg-outline" : "bg-primary"}`}></div>
                  <div>
                    <p className={`text-body-md ${alerta.leida ? "text-on-surface-variant" : "text-on-surface font-medium"}`}>
                      {alerta.mensaje}
                    </p>
                    <p className="text-label-sm text-on-surface-variant mt-1">
                      {new Date(alerta.fechaHora).toLocaleString()} · {alerta.tipo}
                    </p>
                  </div>
                </div>
                {!alerta.leida && (
                  <Button variant="secondary" onClick={() => handleMarcarLeida(alerta.id)}>
                    Marcar leída
                  </Button>
                )}
              </div>
            ))}
          </div>
        )}
      </PageContainer>
    </MainLayout>
  );
}
