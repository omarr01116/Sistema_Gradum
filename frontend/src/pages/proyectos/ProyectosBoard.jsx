import { useState, useEffect, useCallback } from "react";
import MainLayout from "../../layouts/MainLayout";
import PageContainer from "../../components/layout/PageContainer";
import Modal from "../../components/ui/Modal";
import Button from "../../components/ui/Button";
import FormMessage from "../../components/ui/FormMessage";
import ProyectoCard from "./ProyectoCard";
import ProyectoForm from "./ProyectoForm";
import BitacoraPanel from "./BitacoraPanel";
import HitosPanel from "./HitosPanel";
import DocumentosPanel from "./DocumentosPanel";
import { getProyectos, cambiarEstadoProyecto } from "../../services/proyectoService";
import { getClientes } from "../../services/clienteService";
import { getAsesores } from "../../services/asesorService";
import { ESTADOS_PROYECTO, ORDEN_COLUMNAS } from "../../constants/estadosProyecto";
import { useAuth } from "../../context/AuthContext";
import { getErrorMessage } from "../../utils/errors";

export default function ProyectosBoard() {
  const { rol } = useAuth();
  const puedeGestionar = rol === "Coordinador";

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
      
      // 1. Cargamos los proyectos independientemente
      const proyectosData = await getProyectos();
      setProyectos(proyectosData);

      // 2. Intentamos cargar los catálogos en un bloque separado
      try {
        const [clientesData, asesoresData] = await Promise.all([
          getClientes(),
          getAsesores(),
        ]);
        setClientes(clientesData);
        setAsesores(asesoresData);
      } catch (catalogError) {
        console.warn("No se pudieron cargar los catálogos (clientes/asesores).");
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
      <PageContainer title="Proyectos" subtitle="Tablero de proyectos agrupados por estado.">
        <div className="flex justify-between items-center mb-4 gap-4">
          <div className="flex-1"><FormMessage type="error" message={error} /></div>
          <div className="flex gap-2">
            <Button variant="secondary" onClick={cargarDatos}>
              <span className="material-symbols-outlined text-[18px]">refresh</span>
            </Button>
            {puedeGestionar && <Button onClick={abrirCrear}>Nuevo proyecto</Button>}
          </div>
        </div>

        {loading ? (
          <p className="text-body-sm text-on-surface-variant">Cargando proyectos...</p>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-3 xl:grid-cols-5 gap-4">
            {ORDEN_COLUMNAS.map((estado) => {
              const proyectosColumna = proyectos.filter((p) => p.estadoProyecto === estado);
              return (
                <div key={estado} className="bg-surface-container rounded-lg p-3">
                  <div className="flex justify-between items-center mb-3 px-1">
                    <h3 className="font-semibold text-body-sm text-on-surface">
                      {ESTADOS_PROYECTO[estado].label}
                    </h3>
                    <span className="text-label-caps text-on-surface-variant">{proyectosColumna.length}</span>
                  </div>
                  <div className="space-y-3">
                    {proyectosColumna.map((proyecto) => (
                      <ProyectoCard
                        key={proyecto.id}
                        proyecto={proyecto}
                        clienteNombre={nombreCliente(proyecto.clienteId)}
                        asesorNombre={nombreAsesor(proyecto.asesorId)}
                        puedeTransicionar={puedeGestionar}
                        onCambiarEstado={handleCambiarEstado}
                        onVerBitacora={setProyectoBitacora}
                        onVerHitos={setProyectoHitos}
                        onVerDocumentos={setProyectoDocumentos}
                        onEditar={abrirEditar}
                      />
                    ))}
                    {proyectosColumna.length === 0 && (
                      <p className="text-body-sm text-on-surface-variant italic px-1">Sin proyectos</p>
                    )}
                  </div>
                </div>
              );
            })}
          </div>
        )}
      </PageContainer>

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