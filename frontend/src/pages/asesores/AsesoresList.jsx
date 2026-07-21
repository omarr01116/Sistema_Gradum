import { useEffect, useState } from "react";
import MainLayout from "../../layouts/MainLayout";
import PageContainer from "../../components/layout/PageContainer";
import DataTable from "../../components/ui/DataTable";
import Badge from "../../components/ui/Badge";
import Button from "../../components/ui/Button";
import Modal from "../../components/ui/Modal";
import FormMessage from "../../components/ui/FormMessage";
import AsesorForm from "./AsesorForm";
import { useAuth } from "../../context/AuthContext";
import {
  getAsesores,
  desactivarAsesor,
  reactivarAsesor,
} from "../../services/asesorService";

export default function AsesoresList() {
  const { rol } = useAuth();

  const [asesores, setAsesores] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const [modalOpen, setModalOpen] = useState(false);
  const [asesorEditando, setAsesorEditando] = useState(null);

  const [filtroEstado, setFiltroEstado] = useState("activos");

  // Solo el Administrador puede gestionar asesores.
  const puedeGestionar = rol === "Administrador";

  async function cargarAsesores() {
    try {
      setLoading(true);
      setError("");

      const data = await getAsesores();
      setAsesores(data);
    } catch {
      setError("No se pudo cargar el listado de asesores.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    cargarAsesores();
  }, []);

  function abrirCrear() {
    setAsesorEditando(null);
    setModalOpen(true);
  }

  function abrirEditar(asesor) {
    setAsesorEditando(asesor);
    setModalOpen(true);
  }

  function cerrarModal() {
    setModalOpen(false);
    setAsesorEditando(null);
  }

  async function handleGuardado() {
    cerrarModal();
    await cargarAsesores();
  }

  async function handleToggleActivo(asesor) {
    try {
      if (asesor.activo) {
        await desactivarAsesor(asesor.id);
      } else {
        await reactivarAsesor(asesor.id);
      }

      await cargarAsesores();
    } catch (err) {
      setError(err.message);
    }
  }

  const asesoresFiltrados = asesores.filter((a) => {
    if (filtroEstado === "activos") return a.activo;
    if (filtroEstado === "inactivos") return !a.activo;
    return true;
  });

  const columns = [
    {
      key: "nombreCompleto",
      label: "Nombre",
      render: (row) => `${row.nombres} ${row.apellidos}`,
    },
    {
      key: "especialidad",
      label: "Especialidad",
    },
    {
      key: "email",
      label: "Correo",
    },
    {
      key: "telefono",
      label: "Teléfono",
    },
    {
      key: "activo",
      label: "Estado",
      render: (row) => (
        <Badge
          label={row.activo ? "Activo" : "Inactivo"}
          variant={row.activo ? "success" : "neutral"}
        />
      ),
    },
    ...(puedeGestionar
      ? [
          {
            key: "acciones",
            label: "Acciones",
            render: (row) => (
              <div className="flex gap-3">
                <button
                  onClick={() => abrirEditar(row)}
                  className="text-primary hover:underline font-medium"
                >
                  Editar
                </button>

                <button
                  onClick={() => handleToggleActivo(row)}
                  className={`font-medium ${
                    row.activo
                      ? "text-error hover:underline"
                      : "text-green-700 hover:underline"
                  }`}
                >
                  {row.activo ? "Desactivar" : "Reactivar"}
                </button>
              </div>
            ),
          },
        ]
      : []),
  ];

  return (
    <MainLayout>
      <PageContainer
        title="Asesores"
        subtitle="Gestión de asesores académicos."
      >
        <div className="flex justify-between items-center mb-4">
          <div className="flex gap-2">
            <Button
              variant={filtroEstado === "activos" ? "primary" : "secondary"}
              onClick={() => setFiltroEstado("activos")}
            >
              Activos
            </Button>

            <Button
              variant={filtroEstado === "inactivos" ? "primary" : "secondary"}
              onClick={() => setFiltroEstado("inactivos")}
            >
              Inactivos
            </Button>

            <Button
              variant={filtroEstado === "todos" ? "primary" : "secondary"}
              onClick={() => setFiltroEstado("todos")}
            >
              Todos
            </Button>
          </div>

          {puedeGestionar && (
            <Button onClick={abrirCrear}>
              <span className="material-symbols-outlined text-[18px]">
                add
              </span>
              Nuevo asesor
            </Button>
          )}
        </div>

        <FormMessage type="error" message={error} />

        {loading ? (
          <p className="text-body-sm text-on-surface-variant">
            Cargando asesores...
          </p>
        ) : (
          <DataTable
            columns={columns}
            data={asesoresFiltrados}
            emptyMessage="No hay asesores para mostrar."
          />
        )}

        <Modal
          isOpen={modalOpen}
          onClose={cerrarModal}
          title={asesorEditando ? "Editar asesor" : "Nuevo asesor"}
        >
          <AsesorForm
            asesor={asesorEditando}
            onSuccess={handleGuardado}
            onCancel={cerrarModal}
          />
        </Modal>
      </PageContainer>
    </MainLayout>
  );
}