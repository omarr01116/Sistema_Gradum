import { useState, useEffect, useCallback } from "react";
import MainLayout from "../../layouts/MainLayout";
import Modal from "../../components/ui/Modal";
import ProyectoForm from "./ProyectoForm";
import BitacoraPanel from "./BitacoraPanel";
import HitosPanel from "./HitosPanel";
import DocumentosPanel from "./DocumentosPanel";
import { getProyectos, cambiarEstadoProyecto } from "../../services/proyectoService";
import { getClientes } from "../../services/clienteService";
import { getAsesores } from "../../services/asesorService";
import { ESTADOS_PROYECTO } from "../../constants/estadosProyecto";
import { useAuth } from "../../context/AuthContext";
import { getErrorMessage } from "../../utils/errors";
import Card from "../../components/ui/Card";
import Badge from "../../components/ui/Badge";
import { Plus, FileText, Calendar, BookOpen, Edit, ArrowRight } from "lucide-react";

export default function ProyectosBoard() {
  const { rol } = useAuth();
  const puedeGestionar = rol === "Coordinador" || rol === "Administrador";

  const [proyectos, setProyectos] = useState([]);
  const [clientes, setClientes] = useState([]);
  const [asesores, setAsesores] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const [modalFormOpen, setModalFormOpen] = useState(false);
  const [proyectoEditando, setProyectoEditando] = useState(null);
  const [proyectoBitacora, setProyectoBitacora] = useState(null);
  const [proyectoHitos, setProyectoHitos] = useState(null);
  const [proyectoDocumentos, setProyectoDocumentos] = useState(null);

  const cargarDatos = useCallback(async () => {
    try {
      setLoading(true);
      const proyectosData = await getProyectos();
      setProyectos(proyectosData);

      try {
        const [clientesData, asesoresData] = await Promise.all([
          getClientes(),
          getAsesores(),
        ]);
        setClientes(clientesData);
        setAsesores(asesoresData);
      } catch (catalogError) {
        console.warn("No se pudieron cargar los catálogos.");
      }
    } catch {
      setError("No se pudieron cargar los proyectos.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    cargarDatos();
  }, [cargarDatos]);

  const nombreCliente = (clienteId) => {
    const c = clientes.find((cl) => cl.id === clienteId);
    return c ? `${c.nombres} ${c.apellidos}` : "—";
  };

  const nombreAsesor = (asesorId) => {
    if (!asesorId) return null;
    const a = asesores.find((as) => as.id === asesorId);
    return a ? `${a.nombres} ${a.apellidos}` : "—";
  };

  const handleCambiarEstado = async (proyecto, nuevoEstado) => {
    setError("");
    try {
      await cambiarEstadoProyecto(proyecto.id, nuevoEstado);
      setProyectos((prev) =>
        prev.map((p) => (p.id === proyecto.id ? { ...p, estadoProyecto: nuevoEstado } : p))
      );
    } catch (err) {
      setError(getErrorMessage(err, "No se pudo cambiar el estado del proyecto."));
    }
  };

  const abrirCrear = () => {
    setProyectoEditando(null);
    setModalFormOpen(true);
  };

  const abrirEditar = (proyecto) => {
    setProyectoEditando(proyecto);
    setModalFormOpen(true);
  };

  const handleSuccessForm = () => {
    setModalFormOpen(false);
    cargarDatos();
  };

  return (
    <MainLayout>
      <div className="flex justify-between items-end mb-8">
        <div>
          <h2 className="text-3xl font-bold text-gray-800 mb-2">Projects Management</h2>
          <p className="text-gray-500 text-sm">Gestiona y supervisa todos los proyectos académicos de GRADUM.</p>
        </div>
        <div className="flex gap-3">
          <button onClick={cargarDatos} className="flex items-center gap-2 bg-white border border-gray-200 text-gray-700 px-4 py-2 rounded-lg hover:bg-slate-50 font-medium text-sm transition-colors">
            Actualizar
          </button>
          {puedeGestionar && (
            <button onClick={abrirCrear} className="flex items-center gap-2 bg-[#7A0B2E] text-white px-4 py-2 rounded-lg hover:bg-[#610824] font-medium text-sm transition-colors">
              <Plus size={16} /> Nuevo Proyecto
            </button>
          )}
        </div>
      </div>

      {error && (
        <div className="mb-4 p-3 bg-red-50 border border-red-200 text-red-600 text-sm rounded-lg">
          {error}
        </div>
      )}

      <Card className="overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm">
            <thead className="bg-slate-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-4 font-semibold text-gray-600">CODE & THEME</th>
                <th className="px-6 py-4 font-semibold text-gray-600">CLIENT / ASSESSOR</th>
                <th className="px-6 py-4 font-semibold text-gray-600">STATUS</th>
                <th className="px-6 py-4 font-semibold text-gray-600">PROGRESS</th>
                <th className="px-6 py-4 font-semibold text-gray-600">DEADLINE</th>
                <th className="px-6 py-4 font-semibold text-gray-600 text-center">ACTIONS</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {loading ? (
                <tr>
                  <td colSpan="6" className="text-center py-8 text-gray-500">Cargando proyectos...</td>
                </tr>
              ) : proyectos.length === 0 ? (
                <tr>
                  <td colSpan="6" className="text-center py-8 text-gray-500 italic">No se encontraron proyectos.</td>
                </tr>
              ) : (
                proyectos.map((proyecto) => {
                  const config = ESTADOS_PROYECTO[proyecto.estadoProyecto];
                  const transiciones = config?.transicionesValidas ?? [];
                  
                  return (
                    <tr key={proyecto.id} className="hover:bg-slate-50/50 transition-colors group">
                      <td className="px-6 py-4">
                        <p className="font-bold text-gray-800">{proyecto.codigoProyecto}</p>
                        <p className="text-gray-500 text-xs mt-1 w-64 truncate">{proyecto.tema}</p>
                      </td>
                      <td className="px-6 py-4">
                        <p className="font-medium text-gray-700">{nombreCliente(proyecto.clienteId)}</p>
                        <p className="text-xs text-gray-500 mt-1">{nombreAsesor(proyecto.asesorId) || "Sin asesor"}</p>
                      </td>
                      <td className="px-6 py-4">
                        <Badge label={config?.label ?? proyecto.estadoProyecto} variant={config?.badgeVariant ?? "neutral"} />
                      </td>
                      <td className="px-6 py-4">
                        <div className="flex items-center gap-3">
                          <div className="w-full max-w-[100px] bg-gray-200 rounded-full h-2 overflow-hidden">
                            <div 
                              className="bg-[#7A0B2E] h-2 rounded-full transition-all duration-500" 
                              style={{ width: `${proyecto.porcentajeAvance || 0}%` }}
                            ></div>
                          </div>
                          <span className="text-xs font-bold text-gray-600">{proyecto.porcentajeAvance || 0}%</span>
                        </div>
                      </td>
                      <td className="px-6 py-4">
                        <p className="font-medium text-gray-700">
                          {proyecto.fechaEntregaComprometida ? new Date(proyecto.fechaEntregaComprometida).toLocaleDateString() : "—"}
                        </p>
                      </td>
                      <td className="px-6 py-4">
                        <div className="flex items-center justify-center gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                          <button onClick={() => setProyectoHitos(proyecto)} title="Ver Hitos" className="p-2 text-gray-400 hover:text-[#7A0B2E] hover:bg-[#7A0B2E]/10 rounded-lg transition-colors">
                            <Calendar size={18} />
                          </button>
                          <button onClick={() => setProyectoDocumentos(proyecto)} title="Documentos" className="p-2 text-gray-400 hover:text-[#7A0B2E] hover:bg-[#7A0B2E]/10 rounded-lg transition-colors">
                            <FileText size={18} />
                          </button>
                          <button onClick={() => setProyectoBitacora(proyecto)} title="Bitácora" className="p-2 text-gray-400 hover:text-[#7A0B2E] hover:bg-[#7A0B2E]/10 rounded-lg transition-colors">
                            <BookOpen size={18} />
                          </button>
                          {puedeGestionar && (
                            <>
                              <button onClick={() => abrirEditar(proyecto)} title="Editar Proyecto" className="p-2 text-gray-400 hover:text-[#7A0B2E] hover:bg-[#7A0B2E]/10 rounded-lg transition-colors">
                                <Edit size={18} />
                              </button>
                              
                              {transiciones.length > 0 && (
                                <div className="relative group/menu">
                                  <button className="p-2 text-gray-400 hover:text-[#7A0B2E] hover:bg-[#7A0B2E]/10 rounded-lg transition-colors flex items-center">
                                    <ArrowRight size={18} />
                                  </button>
                                  <div className="absolute right-0 top-full mt-1 hidden group-hover/menu:block bg-white border border-gray-200 shadow-lg rounded-lg z-10 w-48 py-1">
                                    {transiciones.map(estado => (
                                      <button 
                                        key={estado}
                                        onClick={() => handleCambiarEstado(proyecto, estado)}
                                        className="w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-slate-50"
                                      >
                                        → {ESTADOS_PROYECTO[estado].label}
                                      </button>
                                    ))}
                                  </div>
                                </div>
                              )}
                            </>
                          )}
                        </div>
                      </td>
                    </tr>
                  );
                })
              )}
            </tbody>
          </table>
        </div>
      </Card>

      <Modal
        isOpen={modalFormOpen}
        onClose={() => setModalFormOpen(false)}
        title={proyectoEditando ? "Editar proyecto" : "Nuevo proyecto"}
      >
        <ProyectoForm proyecto={proyectoEditando} onSuccess={handleSuccessForm} onCancel={() => setModalFormOpen(false)} />
      </Modal>

      <Modal
        isOpen={!!proyectoBitacora}
        onClose={() => setProyectoBitacora(null)}
        title={`Bitácora — ${proyectoBitacora?.codigoProyecto ?? ""}`}
      >
        {proyectoBitacora && (
          <BitacoraPanel proyecto={proyectoBitacora} puedeEscribir={rol !== "Administrador"} />
        )}
      </Modal>

      <Modal
        isOpen={!!proyectoHitos}
        onClose={() => setProyectoHitos(null)}
        title={`Hitos y Avance — ${proyectoHitos?.codigoProyecto ?? ""}`}
      >
        {proyectoHitos && (
          <HitosPanel proyecto={proyectoHitos} />
        )}
      </Modal>

      <Modal
        isOpen={!!proyectoDocumentos}
        onClose={() => setProyectoDocumentos(null)}
        title={`Documentos — ${proyectoDocumentos?.codigoProyecto ?? ""}`}
      >
        {proyectoDocumentos && (
          <DocumentosPanel proyecto={proyectoDocumentos} />
        )}
      </Modal>
    </MainLayout>
  );
}