import { useEffect, useMemo, useState } from "react";
import MainLayout from "../../layouts/MainLayout";
import PageContainer from "../../components/layout/PageContainer";
import DataTable from "../../components/ui/DataTable";
import Badge from "../../components/ui/Badge";
import Button from "../../components/ui/Button";
import Modal from "../../components/ui/Modal";
import FormMessage from "../../components/ui/FormMessage";
import ClienteForm from "./ClienteForm";
import { useAuth } from "../../context/AuthContext";
import {
  getClientes,
  desactivarCliente,
  reactivarCliente,
} from "../../services/clienteService";

export default function ClientesList() {
  const { rol } = useAuth();

  const [clientes, setClientes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [clienteEditando, setClienteEditando] = useState(null);

  // activos | inactivos | todos
  const [filtroEstado, setFiltroEstado] = useState("activos");

  // Según el backend
  const puedeCrear = rol === "Coordinador";
  const puedeGestionar =
    rol === "Administrador" || rol === "Coordinador";

  async function cargarClientes() {
    try {
      setLoading(true);
      setError("");

      const data = await getClientes();
      setClientes(data);
    } catch {
      setError("No se pudo cargar el listado de clientes.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    cargarClientes();
  }, []);

  const clientesFiltrados = useMemo(() => {
    switch (filtroEstado) {
      case "activos":
        return clientes.filter((c) => c.activo);

      case "inactivos":
        return clientes.filter((c) => !c.activo);

      default:
        return clientes;
    }
  }, [clientes, filtroEstado]);

  function abrirCrear() {
    setClienteEditando(null);
    setModalOpen(true);
  }

  function abrirEditar(cliente) {
    setClienteEditando(cliente);
    setModalOpen(true);
  }

  function cerrarModal() {
    setModalOpen(false);
    setClienteEditando(null);
  }

  async function handleGuardado() {
    cerrarModal();
    await cargarClientes();
  }

  async function handleToggleActivo(cliente) {
    try {
      if (cliente.activo) {
        await desactivarCliente(cliente.id);
      } else {
        await reactivarCliente(cliente.id);
      }

      await cargarClientes();
    } catch (err) {
      setError(err.message);
    }
  }

  const columns = [
    {
      key: "codigoCliente",
      label: "Código",
    },
    {
      key: "nombreCompleto",
      label: "Nombre",
      render: (row) => `${row.nombres} ${row.apellidos}`,
    },
    {
      key: "dniPasaporte",
      label: "DNI / Pasaporte",
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
      key: "estadoFinanciero",
      label: "Estado financiero",
      render: (row) => (
        <Badge
          label={
            row.estadoFinanciero === "AlDia"
              ? "Al día"
              : "Con deuda"
          }
          variant={
            row.estadoFinanciero === "AlDia"
              ? "success"
              : "danger"
          }
        />
      ),
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
                  className="text-primary hover:text-primary-container font-semibold"
                >
                  Editar
                </button>

                <button
                  onClick={() => handleToggleActivo(row)}
                  className={`font-semibold ${
                    row.activo
                      ? "text-error hover:opacity-70"
                      : "text-green-700 hover:opacity-70"
                  }`}
                >
                  {row.activo
                    ? "Desactivar"
                    : "Reactivar"}
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
        title="Clientes"
        subtitle="Gestión de tesistas y su estado financiero."
      >
        <div className="flex items-center justify-between mb-4">
          <div className="flex gap-2">
            <Button
              variant={
                filtroEstado === "activos"
                  ? "primary"
                  : "secondary"
              }
              onClick={() => setFiltroEstado("activos")}
            >
              Activos
            </Button>

            <Button
              variant={
                filtroEstado === "inactivos"
                  ? "primary"
                  : "secondary"
              }
              onClick={() => setFiltroEstado("inactivos")}
            >
              Inactivos
            </Button>

            <Button
              variant={
                filtroEstado === "todos"
                  ? "primary"
                  : "secondary"
              }
              onClick={() => setFiltroEstado("todos")}
            >
              Todos
            </Button>
          </div>

          {puedeCrear && (
            <Button onClick={abrirCrear}>
              <span className="material-symbols-outlined text-[18px]">
                add
              </span>
              Nuevo cliente
            </Button>
          )}
        </div>

        <FormMessage
          type="error"
          message={error}
        />

        {loading ? (
          <p className="text-body-sm text-on-surface-variant">
            Cargando clientes...
          </p>
        ) : (
          <DataTable
            columns={columns}
            data={clientesFiltrados}
            emptyMessage="No hay clientes para el filtro seleccionado."
          />
        )}

        <Modal
          isOpen={modalOpen}
          onClose={cerrarModal}
          title={
            clienteEditando
              ? "Editar cliente"
              : "Nuevo cliente"
          }
        >
          <ClienteForm
            cliente={clienteEditando}
            onSuccess={handleGuardado}
            onCancel={cerrarModal}
          />
        </Modal>
      </PageContainer>
    </MainLayout>
  );
}