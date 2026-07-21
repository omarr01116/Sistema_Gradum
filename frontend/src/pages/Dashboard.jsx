import { useState, useEffect } from "react";
import MainLayout from "../layouts/MainLayout";
import PageContainer from "../components/layout/PageContainer";
import Badge from "../components/ui/Badge";
import { useAuth } from "../context/AuthContext";
import { getDashboardData } from "../services/dashboardService";

export default function Dashboard() {
  const { nombreUsuario, rol } = useAuth();
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const cargar = async () => {
      try {
        const res = await getDashboardData();
        setData(res);
      } catch (err) {
        console.error("Error al cargar dashboard");
      } finally {
        setLoading(false);
      }
    };
    cargar();
  }, []);

  if (loading) return (
    <MainLayout>
      <PageContainer title="Cargando..." />
    </MainLayout>
  );

  const isAsesor = rol === "Asesor";

  return (
    <MainLayout>
      <PageContainer 
        title={`¡Hola, ${nombreUsuario}!`} 
        subtitle={isAsesor ? "Aquí tienes tus próximas entregas y avances." : "Resumen general del estado de GRADUM."}
      >
        {!isAsesor && data && (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-4 mb-8">
            <MetricCard title="Clientes" value={data.totalClientes} icon="group" />
            <MetricCard title="Proyectos" value={data.totalProyectos} icon="folder_special" />
            <MetricCard title="Pendientes Aprob." value={data.hitosPendientesAprobacion} icon="pending_actions" color="text-warning" />
            <MetricCard title="Finalizados" value={data.proyectosFinalizados} icon="task_alt" color="text-success" />
            <MetricCard title="Documentos" value={data.documentosCargados} icon="description" />
          </div>
        )}

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Columna Principal */}
          <div className="lg:col-span-2 space-y-6">
            <div className="bg-surface-container-low p-6 rounded-2xl border border-outline-variant">
              <h3 className="font-semibold text-body-lg mb-4">
                {isAsesor ? "Próximas Entregas de Hitos" : "Distribución de Proyectos"}
              </h3>
              
              {isAsesor ? (
                <div className="space-y-3">
                  {data?.proximasEntregas?.length > 0 ? (
                    data.proximasEntregas.map((h, i) => (
                      <div key={i} className="flex justify-between items-center p-3 bg-surface-container rounded-lg">
                        <div>
                          <p className="font-medium text-body-md">{h.nombreHito}</p>
                          <p className="text-label-sm text-on-surface-variant">{h.codigoProyecto} · {h.temaProyecto}</p>
                        </div>
                        <div className="text-right">
                          <p className="text-body-sm font-semibold text-primary">
                            {new Date(h.fechaCompromiso).toLocaleDateString()}
                          </p>
                          <Badge variant="warning">Próximo</Badge>
                        </div>
                      </div>
                    ))
                  ) : (
                    <p className="text-body-sm text-on-surface-variant italic">No hay entregas próximas en los siguientes 7 días.</p>
                  )}
                </div>
              ) : (
                <div className="space-y-4">
                  {data?.proyectosPorEstado && Object.entries(data.proyectosPorEstado).map(([estado, cantidad]) => (
                    <div key={estado} className="space-y-1">
                      <div className="flex justify-between text-label-sm">
                        <span>{estado}</span>
                        <span className="font-bold">{cantidad}</span>
                      </div>
                      <div className="w-full bg-surface-container rounded-full h-2">
                        <div 
                          className="bg-primary h-2 rounded-full" 
                          style={{ width: `${data.totalProyectos > 0 ? (cantidad / data.totalProyectos) * 100 : 0}%` }}
                        ></div>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>

          {/* Columna Lateral (Atajos) */}
          <div className="space-y-6">
            <div className="bg-primary-container/20 p-6 rounded-2xl border border-primary/10">
              <h3 className="font-semibold text-body-md mb-3 text-on-primary-fixed-variant">Acciones rápidas</h3>
              <div className="grid grid-cols-1 gap-2">
                <QuickAction icon="add_circle" label="Nuevo Proyecto" path="/proyectos" />
                <QuickAction icon="notifications" label="Ver Alertas" path="/alertas" />
                {rol === "Administrador" && <QuickAction icon="settings" label="Configuración" path="/configuracion" />}
              </div>
            </div>
          </div>
        </div>
      </PageContainer>
    </MainLayout>
  );
}

function MetricCard({ title, value, icon, color = "text-primary" }) {
  return (
    <div className="bg-surface-container-low p-4 rounded-xl border border-outline-variant flex items-center gap-4">
      <div className={`w-10 h-10 rounded-lg bg-surface-container flex items-center justify-center ${color}`}>
        <span className="material-symbols-outlined">{icon}</span>
      </div>
      <div>
        <p className="text-label-sm text-on-surface-variant">{title}</p>
        <p className="text-headline-sm font-bold">{value}</p>
      </div>
    </div>
  );
}

function QuickAction({ icon, label, path }) {
  return (
    <a href={path} className="flex items-center gap-3 p-3 rounded-lg hover:bg-primary-container/40 transition-colors text-body-sm font-medium text-on-primary-fixed-variant">
      <span className="material-symbols-outlined text-[20px]">{icon}</span>
      {label}
    </a>
  );
}
