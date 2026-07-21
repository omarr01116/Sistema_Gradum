import { useState, useEffect } from "react";
import InputField from "../../components/ui/InputField";
import Button from "../../components/ui/Button";
import FormMessage from "../../components/ui/FormMessage";
import { getClientes } from "../../services/clienteService";
import { getAsesores } from "../../services/asesorService";
import { createProyecto, updateProyecto } from "../../services/proyectoService";
import { getErrorMessage } from "../../utils/errors";

const TIPOS_PROYECTO = ["Tesis", "Trabajo de investigación", "Artículo científico"];

const initialForm = {
  clienteId: "",
  universidad: "",
  carrera: "",
  programa: "",
  tipoProyecto: TIPOS_PROYECTO[0],
  tema: "",
  asesorId: "",
  fechaInicio: "",
  fechaEntregaComprometida: "",
};

export default function ProyectoForm({ proyecto, onSuccess, onCancel }) {
  const esEdicion = !!proyecto;

  const [clientes, setClientes] = useState([]);
  const [asesores, setAsesores] = useState([]);
  const [loadingCatalogos, setLoadingCatalogos] = useState(true);
  const [formData, setFormData] = useState(initialForm);
  const [message, setMessage] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    async function cargarCatalogos() {
      try {
        const [clientesData, asesoresData] = await Promise.all([getClientes(), getAsesores()]);
        setClientes(clientesData.filter((c) => c.activo));
        setAsesores(asesoresData.filter((a) => a.activo));
      } catch {
        setMessage("No se pudieron cargar clientes/asesores.");
      } finally {
        setLoadingCatalogos(false);
      }
    }
    cargarCatalogos();
  }, []);

  useEffect(() => {
    if (proyecto) {
      setFormData({
        clienteId: proyecto.clienteId,
        universidad: proyecto.universidad,
        carrera: proyecto.carrera,
        programa: proyecto.programa,
        tipoProyecto: proyecto.tipoProyecto,
        tema: proyecto.tema,
        asesorId: proyecto.asesorId ?? "",
        fechaInicio: proyecto.fechaInicio?.slice(0, 10) ?? "",
        fechaEntregaComprometida: proyecto.fechaEntregaComprometida?.slice(0, 10) ?? "",
      });
    }
  }, [proyecto]);

  const handleChange = (event) => {
    const { name, value } = event.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    setMessage("");
    setIsSubmitting(true);

    try {
      if (esEdicion) {
        // RN-01: cliente y fecha de inicio no se tocan tras crear.
        const { clienteId, fechaInicio, ...payload } = formData;
        await updateProyecto(proyecto.id, {
          ...payload,
          asesorId: Number(payload.asesorId),
        });
      } else {
        await createProyecto({
          ...formData,
          clienteId: Number(formData.clienteId),
          asesorId: Number(formData.asesorId),
        });
      }
      onSuccess();
    } catch (error) {
      setMessage(getErrorMessage(error, "No se pudo guardar el proyecto."));
    } finally {
      setIsSubmitting(false);
    }
  };

  if (loadingCatalogos) {
    return <p className="text-body-sm text-on-surface-variant">Cargando clientes y asesores...</p>;
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <label className="block mb-2 font-bold text-body-sm text-on-surface">Cliente</label>
        <select
          name="clienteId"
          value={formData.clienteId}
          onChange={handleChange}
          disabled={esEdicion}
          required
          className="w-full border border-outline-variant rounded px-3 py-2 bg-surface text-on-surface disabled:bg-surface-container disabled:text-on-surface-variant"
        >
          <option value="">Selecciona un cliente...</option>
          {clientes.map((c) => (
            <option key={c.id} value={c.id}>{c.nombres} {c.apellidos}</option>
          ))}
        </select>
        {esEdicion && (
          <p className="text-body-sm text-on-surface-variant mt-1">
            El cliente no se puede cambiar después de crear el proyecto (RN-01).
          </p>
        )}
      </div>

      <div className="grid grid-cols-2 gap-4">
        <InputField label="Universidad" name="universidad" value={formData.universidad} onChange={handleChange} required />
        <InputField label="Carrera" name="carrera" value={formData.carrera} onChange={handleChange} required />
      </div>

      <div className="grid grid-cols-2 gap-4">
        <InputField label="Programa" name="programa" value={formData.programa} onChange={handleChange} required />
        <div>
          <label className="block mb-2 font-bold text-body-sm text-on-surface">Tipo de proyecto</label>
          <select
            name="tipoProyecto"
            value={formData.tipoProyecto}
            onChange={handleChange}
            required
            className="w-full border border-outline-variant rounded px-3 py-2 bg-surface text-on-surface"
          >
            {TIPOS_PROYECTO.map((tipo) => (
              <option key={tipo} value={tipo}>{tipo}</option>
            ))}
          </select>
        </div>
      </div>

      <InputField label="Tema" name="tema" value={formData.tema} onChange={handleChange} required />

      <div>
        <label className="block mb-2 font-bold text-body-sm text-on-surface">Asesor</label>
        <select
          name="asesorId"
          value={formData.asesorId}
          onChange={handleChange}
          required
          className="w-full border border-outline-variant rounded px-3 py-2 bg-surface text-on-surface"
        >
          <option value="">Selecciona un asesor...</option>
          {asesores.map((a) => (
            <option key={a.id} value={a.id}>{a.nombres} {a.apellidos} — {a.especialidad}</option>
          ))}
        </select>
        <p className="text-body-sm text-on-surface-variant mt-1">
          Si el asesor ya alcanzó su límite de proyectos (RN-03), el backend rechaza el guardado.
        </p>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div>
          <label className="block mb-2 font-bold text-body-sm text-on-surface">Fecha de inicio</label>
          <input
            type="date"
            name="fechaInicio"
            value={formData.fechaInicio}
            onChange={handleChange}
            disabled={esEdicion}
            required
            className="w-full border border-outline-variant rounded px-3 py-2 bg-surface text-on-surface disabled:bg-surface-container disabled:text-on-surface-variant"
          />
        </div>
        <div>
          <label className="block mb-2 font-bold text-body-sm text-on-surface">Entrega comprometida</label>
          <input
            type="date"
            name="fechaEntregaComprometida"
            value={formData.fechaEntregaComprometida}
            onChange={handleChange}
            required
            className="w-full border border-outline-variant rounded px-3 py-2 bg-surface text-on-surface"
          />
        </div>
      </div>

      <FormMessage type="error" message={message} />

      <div className="flex justify-end gap-3 pt-2">
        <Button type="button" variant="secondary" onClick={onCancel}>Cancelar</Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Guardando..." : esEdicion ? "Guardar cambios" : "Crear proyecto"}
        </Button>
      </div>
    </form>
  );
}