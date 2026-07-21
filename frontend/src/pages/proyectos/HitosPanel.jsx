import { useState, useEffect, useCallback } from "react";
import Button from "../../components/ui/Button";
import InputField from "../../components/ui/InputField";
import FormMessage from "../../components/ui/FormMessage";
import Badge from "../../components/ui/Badge";
import { getHitosByProyecto, crearHitosLote, completarHito, aprobarHito, rechazarHito } from "../../services/hitoService";
import { useAuth } from "../../context/AuthContext";
import { getErrorMessage } from "../../utils/errors";

export default function HitosPanel({ proyecto }) {
  const { rol } = useAuth();
  const [hitos, setHitos] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  // Para creación masiva
  const [modoEdicion, setModoEdicion] = useState(false);
  const [nuevosHitos, setNuevosHitos] = useState([]);

  const cargarHitos = useCallback(async () => {
    try {
      setLoading(true);
      const data = await getHitosByProyecto(proyecto.id);
      setHitos(data);
      if (data.length === 0 && (rol === "Coordinador" || rol === "Administrador")) {
        setModoEdicion(true);
      }
    } catch (err) {
      setError("No se pudieron cargar los hitos.");
    } finally {
      setLoading(false);
    }
  }, [proyecto.id]);

  useEffect(() => {
    cargarHitos();
  }, [cargarHitos]);

  const handleAddFila = () => {
    setNuevosHitos([...nuevosHitos, { nombreHito: "", pesoPorcentual: 0, fechaCompromiso: "", orden: nuevosHitos.length + 1 }]);
  };

  const handleRemoveFila = (index) => {
    setNuevosHitos(nuevosHitos.filter((_, i) => i !== index));
  };

  const handleInputChange = (index, field, value) => {
    const updated = [...nuevosHitos];
    updated[index][field] = field === "pesoPorcentual" ? parseInt(value) || 0 : value;
    setNuevosHitos(updated);
  };

  const guardarHitos = async () => {
    setError("");

    for (const h of nuevosHitos) {
      if (!h.nombreHito.trim()) {
        setError("Todos los hitos deben tener un nombre.");
        return;
      }
      if (!h.fechaCompromiso) {
        setError("Todos los hitos deben tener una fecha de compromiso.");
        return;
      }
    }

    const suma = nuevosHitos.reduce((acc, h) => acc + h.pesoPorcentual, 0);
    if (suma !== 100) {
      setError(`La suma de pesos debe ser exactamente 100% (actual: ${suma}%).`);
      return;
    }

    try {
      await crearHitosLote(proyecto.id, nuevosHitos);
      setSuccess("Hitos definidos correctamente.");
      setModoEdicion(false);
      cargarHitos();
    } catch (err) {
      setError(getErrorMessage(err, "Error al guardar hitos."));
    }
  };

  const handleAccion = async (hitoId, accion, payload = null) => {
    setError("");
    setSuccess("");
    try {
      if (accion === "completar") {
        const formData = new FormData();
        if (payload) formData.append("Evidencia", payload);
        await completarHito(hitoId, formData);
      } else if (accion === "aprobar") {
        await aprobarHito(hitoId);
      } else if (accion === "rechazar") {
        await rechazarHito(hitoId, payload);
      }
      cargarHitos();
    } catch (err) {
      setError(getErrorMessage(err, `Error al ${accion} hito.`));
    }
  };

  if (loading) return <p className="p-4 text-body-sm">Cargando hitos...</p>;

  return (
    <div className="p-4 space-y-4">
      <FormMessage type="error" message={error} />
      <FormMessage type="success" message={success} />

      {modoEdicion ? (
        <div className="space-y-4">
          <p className="text-body-sm text-on-surface-variant">
            Define la lista de hitos. La suma de pesos debe ser 100%.
          </p>
          <div className="space-y-2">
            {nuevosHitos.map((h, i) => (
              <div key={i} className="flex gap-2 items-end border-b border-outline-variant pb-2">
                <div className="flex-1">
                  <InputField label="Nombre" value={h.nombreHito} onChange={(e) => handleInputChange(i, "nombreHito", e.target.value)} />
                </div>
                <div className="w-20">
                  <InputField label="%" type="number" value={h.pesoPorcentual} onChange={(e) => handleInputChange(i, "pesoPorcentual", e.target.value)} />
                </div>
                <div className="w-40">
                  <InputField label="Fecha" type="date" value={h.fechaCompromiso} onChange={(e) => handleInputChange(i, "fechaCompromiso", e.target.value)} />
                </div>
                <Button variant="secondary" onClick={() => handleRemoveFila(i)}>
                  <span className="material-symbols-outlined text-[18px]">delete</span>
                </Button>
              </div>
            ))}
          </div>
          <div className="flex justify-between items-center">
            <Button variant="secondary" onClick={handleAddFila}>+ Agregar hito</Button>
            <div className="flex gap-2">
              {hitos.length > 0 && <Button variant="secondary" onClick={() => setModoEdicion(false)}>Cancelar</Button>}
              <Button onClick={guardarHitos}>Guardar todos</Button>
            </div>
          </div>
        </div>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full text-left text-body-sm border-collapse">
            <thead>
              <tr className="border-b border-outline-variant">
                <th className="py-2 font-semibold">Hito</th>
                <th className="py-2 font-semibold">Peso</th>
                <th className="py-2 font-semibold">Fecha</th>
                <th className="py-2 font-semibold">Estado</th>
                <th className="py-2 font-semibold text-right">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {hitos.map((h) => (
                <tr key={h.id} className="border-b border-outline-variant hover:bg-surface-container-low transition-colors">
                  <td className="py-3">{h.nombreHito}</td>
                  <td className="py-3">{h.pesoPorcentual}%</td>
                  <td className="py-3">{new Date(h.fechaCompromiso).toLocaleDateString()}</td>
                  <td className="py-3">
                    <Badge variant={h.estadoHito === "Aprobado" ? "success" : h.estadoHito === "PendienteAprobacion" ? "warning" : "neutral"}>
                      {h.estadoHito === "PendienteAprobacion" ? "Pendiente de aprobación" : h.estadoHito}
                    </Badge>
                  </td>
                  <td className="py-3 text-right">
                    <div className="flex justify-end gap-2">
                      {rol === "Asesor" && (h.estadoHito === "Pendiente" || h.estadoHito === "EnProgreso" || h.estadoHito === "Correcciones") && (
                        <Button variant="secondary" onClick={() => handleAccion(h.id, "completar")}>Completar</Button>
                      )}
                      {(rol === "Coordinador" || rol === "Administrador") && h.estadoHito === "PendienteAprobacion" && (
                        <>
                          <Button variant="secondary" onClick={() => {
                            const motivo = prompt("Motivo de rechazo:");
                            if (motivo) handleAccion(h.id, "rechazar", motivo);
                          }}>Rechazar</Button>
                          <Button onClick={() => handleAccion(h.id, "aprobar")}>Aprobar</Button>
                        </>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {hitos.length === 0 && (
            <div className="py-8 text-center text-gray-500 italic">
              El Coordinador aún no ha definido los hitos para este proyecto.
            </div>
          )}
          {(rol === "Coordinador" || rol === "Administrador") && hitos.length > 0 && (
            <div className="mt-4">
              <Button variant="secondary" onClick={() => {
                setNuevosHitos(hitos.map(h => ({ ...h, fechaCompromiso: h.fechaCompromiso.split('T')[0] })));
                setModoEdicion(true);
              }}>Editar estructura</Button>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
