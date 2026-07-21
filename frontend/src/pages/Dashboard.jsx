import { useState, useEffect } from "react";
import MainLayout from "../layouts/MainLayout";
import { useAuth } from "../context/AuthContext";
import { getDashboardData } from "../services/dashboardService";
import { Users, FolderKanban, Clock, CheckCircle, FileText, PlusCircle, Settings, Bell } from "lucide-react";
import Card from "../components/ui/Card";
import Badge from "../components/ui/Badge";
import { Link } from "react-router-dom";

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
      <div className="flex items-center justify-center h-full">Cargando...</div>
    </MainLayout>
  );

  const isAsesor = rol === "Asesor";

  return (
    <MainLayout>
      <div className="flex justify-between items-end mb-8">
        <div>
          <h2 className="text-3xl font-bold text-gray-800 mb-2">Dashboard Overview</h2>
          <p className="text-gray-500 text-sm">
            {isAsesor ? "Aquí tienes tus próximas entregas y avances." : "Resumen general del estado de GRADUM."}
          </p>
        </div>
      </div>

      {!isAsesor && data && (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-6 mb-6">
          <MetricCard 
            label="TOTAL CLIENTS" 
            value={data.totalClientes} 
            icon={Users} 
            trend="Activos" 
            trendColor="text-green-600 bg-green-100" 
            iconColor="text-blue-600 bg-blue-50" 
          />
          <MetricCard 
            label="TOTAL PROJECTS" 
            value={data.totalProyectos} 
            icon={FolderKanban} 
            trend="Gestión" 
            trendColor="text-blue-600 bg-blue-100" 
            iconColor="text-indigo-600 bg-indigo-50" 
          />
          <MetricCard 
            label="PENDING APPROVAL" 
            value={data.hitosPendientesAprobacion} 
            icon={Clock} 
            trend="Acción Req." 
            trendColor="text-yellow-700 bg-yellow-100" 
            iconColor="text-yellow-600 bg-yellow-50" 
          />
          <MetricCard 
            label="FINISHED PROJECTS" 
            value={data.proyectosFinalizados} 
            icon={CheckCircle} 
            trend="Completados" 
            trendColor="text-green-700 bg-green-100" 
            iconColor="text-green-600 bg-green-50" 
          />
          <MetricCard 
            label="DOCUMENTS UPLOADED" 
            value={data.documentosCargados} 
            icon={FileText} 
            trend="Total" 
            trendColor="text-purple-700 bg-purple-100" 
            iconColor="text-purple-600 bg-purple-50" 
          />
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <Card className="col-span-1 lg:col-span-2 p-6">
          <div className="flex justify-between items-center mb-6">
            <div>
              <h3 className="text-lg font-bold text-gray-800">
                {isAsesor ? "Próximas Entregas de Hitos" : "Projects by Status"}
              </h3>
              <p className="text-sm text-gray-500">
                {isAsesor ? "Tus hitos próximos a vencer" : "Distribution of all current active academic projects."}
              </p>
            </div>
          </div>
          
          {isAsesor ? (
            <div className="space-y-3">
              {data?.proximasEntregas?.length > 0 ? (
                data.proximasEntregas.map((h, i) => (
                  <div key={i} className="flex justify-between items-center p-4 border border-gray-100 rounded-lg bg-slate-50">
                    <div>
                      <p className="font-bold text-gray-800">{h.nombreHito}</p>
                      <p className="text-xs text-gray-500 mt-1">{h.codigoProyecto} · {h.temaProyecto}</p>
                    </div>
                    <div className="text-right">
                      <p className="text-sm font-bold text-[#7A0B2E] mb-1">
                        {new Date(h.fechaCompromiso).toLocaleDateString()}
                      </p>
                      <Badge label="Próximo" variant="warning" />
                    </div>
                  </div>
                ))
              ) : (
                <p className="text-sm text-gray-500 italic py-4">No hay entregas próximas en los siguientes 7 días.</p>
              )}
            </div>
          ) : (
            <div className="h-64 flex items-end justify-around border-b border-l border-gray-200 pb-4 pl-4 relative">
              {/* Chart lines */}
              <div className="absolute left-0 bottom-4 w-full border-t border-gray-100"></div>
              <div className="absolute left-0 bottom-16 w-full border-t border-gray-100"></div>
              <div className="absolute left-0 bottom-28 w-full border-t border-gray-100"></div>
              <div className="absolute left-0 bottom-40 w-full border-t border-gray-100"></div>
              
              {/* Dynamic Bars based on data */}
              {data?.proyectosPorEstado && Object.entries(data.proyectosPorEstado).map(([estado, cantidad], idx) => {
                const total = Math.max(data.totalProyectos, 1);
                const percentage = (cantidad / total) * 100;
                const colors = ["bg-[#3b82f6]", "bg-[#7A0B2E]", "bg-yellow-500", "bg-emerald-500", "bg-[#5B6275]"];
                const color = colors[idx % colors.length];
                
                return (
                  <div key={estado} className={`w-12 ${color} rounded-t-sm relative z-10 flex flex-col justify-end items-center transition-all duration-500`} style={{ height: `${Math.max(percentage, 5)}%` }}>
                    <span className="text-xs font-bold text-white mb-2">{cantidad}</span>
                    <span className="text-[10px] absolute -bottom-6 text-gray-600 font-medium whitespace-nowrap">{estado}</span>
                  </div>
                );
              })}
            </div>
          )}
        </Card>
        
        {/* Acciones Rápidas restores previous functionality */}
        <Card className="flex flex-col h-[400px]">
          <div className="p-5 border-b border-gray-100 flex justify-between items-center bg-slate-50/50 rounded-t-xl">
            <h3 className="font-bold text-gray-800">Acciones rápidas</h3>
          </div>
          <div className="flex-1 overflow-y-auto p-4 flex flex-col gap-3">
            <QuickAction icon={PlusCircle} label="Nuevo Proyecto" path="/proyectos" />
            <QuickAction icon={Bell} label="Ver Alertas" path="/alertas" />
            {rol === "Administrador" && <QuickAction icon={Settings} label="Configuración" path="/configuracion" />}
          </div>
        </Card>
      </div>
    </MainLayout>
  );
}

function MetricCard({ label, value, icon: IconComponent, trend, trendColor, iconColor }) {
  return (
    <Card className="p-5 flex flex-col relative overflow-hidden group hover:border-[#7A0B2E]/30 transition-colors">
      <div className="flex justify-between items-start mb-4">
        <div className={`p-2 rounded-lg ${iconColor}`}>
           <IconComponent size={20} />
        </div>
        <span className={`text-[10px] font-bold px-2 py-1 rounded-full ${trendColor}`}>{trend}</span>
      </div>
      <p className="text-[10px] font-bold text-gray-400 tracking-wider mb-1">{label}</p>
      <h3 className="text-3xl font-bold text-gray-800">{value}</h3>
    </Card>
  );
}

function QuickAction({ icon: IconComponent, label, path }) {
  return (
    <Link to={path} className="flex items-center gap-3 p-3 rounded-lg hover:bg-slate-50 border border-transparent hover:border-gray-200 transition-colors text-sm font-medium text-gray-700">
      <IconComponent size={20} className="text-[#7A0B2E]" />
      {label}
    </Link>
  );
}
