import { useState } from "react";
import InputField from "../../components/ui/InputField";
import Button from "../../components/ui/Button";
import FormMessage from "../../components/ui/FormMessage";
import { createCliente, updateCliente } from "../../services/clienteService";

const ESTADOS_FINANCIEROS = [
  { value: "AlDia", label: "Al día" },
  { value: "ConDeuda", label: "Con deuda" },
];

export default function ClienteForm({ cliente, onSuccess, onCancel }) {
  const esEdicion = !!cliente;

  const [formData, setFormData] = useState({
    nombres: cliente?.nombres ?? "",
    apellidos: cliente?.apellidos ?? "",
    dniPasaporte: cliente?.dniPasaporte ?? "",
    telefono: cliente?.telefono ?? "",
    email: cliente?.email ?? "",
    estadoFinanciero: cliente?.estadoFinanciero ?? "AlDia",
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
        // UpdateClienteDto no incluye dniPasaporte (no es editable).
        const { dniPasaporte, ...updateDto } = formData;
        await updateCliente(cliente.id, updateDto);
      } else {
        await createCliente(formData);
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

      {!esEdicion && (
        <InputField
          label="DNI / Pasaporte"
          name="dniPasaporte"
          value={formData.dniPasaporte}
          onChange={handleChange}
          required
        />
      )}

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

      <div className="space-y-2">
        <label className="block font-bold text-body-sm text-on-surface">
          Estado financiero
        </label>
        <select
          name="estadoFinanciero"
          value={formData.estadoFinanciero}
          onChange={handleChange}
          className="w-full border border-outline-variant rounded px-3 py-2 text-body-md text-on-surface outline-none focus:ring-1 focus:ring-primary focus:border-primary"
        >
          {ESTADOS_FINANCIEROS.map((opt) => (
            <option key={opt.value} value={opt.value}>
              {opt.label}
            </option>
          ))}
        </select>
      </div>

      <FormMessage type="error" message={error} />

      <div className="flex justify-end gap-3 pt-2">
        <Button type="button" variant="secondary" onClick={onCancel}>
          Cancelar
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Guardando..." : esEdicion ? "Guardar cambios" : "Crear cliente"}
        </Button>
      </div>
    </form>
  );
}