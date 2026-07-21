import { useState } from "react";
import InputField from "../../components/ui/InputField";
import Button from "../../components/ui/Button";
import FormMessage from "../../components/ui/FormMessage";
import { createAsesor, updateAsesor } from "../../services/asesorService";

export default function AsesorForm({ asesor, onSuccess, onCancel }) {
  const esEdicion = !!asesor;

  const [formData, setFormData] = useState({
    nombres: asesor?.nombres ?? "",
    apellidos: asesor?.apellidos ?? "",
    telefono: asesor?.telefono ?? "",
    email: asesor?.email ?? "",
    especialidad: asesor?.especialidad ?? "",
  });

  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  function handleChange(event) {
    const { name, value } = event.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setError("");
    setIsSubmitting(true);

    try {
      if (esEdicion) {
        await updateAsesor(asesor.id, formData);
      } else {
        await createAsesor(formData);
      }
      onSuccess();
    } catch (err) {
      setError(err.message);
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <InputField
        label="Nombres"
        name="nombres"
        value={formData.nombres}
        onChange={handleChange}
        required
      />
      <InputField
        label="Apellidos"
        name="apellidos"
        value={formData.apellidos}
        onChange={handleChange}
        required
      />
      <InputField
        label="Correo electrónico"
        name="email"
        type="email"
        value={formData.email}
        onChange={handleChange}
        required
      />
      <InputField
        label="Teléfono"
        name="telefono"
        value={formData.telefono}
        onChange={handleChange}
        required
      />
      <InputField
        label="Especialidad"
        name="especialidad"
        value={formData.especialidad}
        onChange={handleChange}
        placeholder="Ej. Inteligencia Artificial"
        required
      />

      <FormMessage type="error" message={error} />

      <div className="flex justify-end gap-3 pt-2">
        <Button type="button" variant="secondary" onClick={onCancel}>
          Cancelar
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Guardando..." : esEdicion ? "Guardar cambios" : "Crear asesor"}
        </Button>
      </div>
    </form>
  );
}