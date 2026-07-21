import { useState, useEffect } from "react";
import Button from "../../components/ui/Button";
import FormMessage from "../../components/ui/FormMessage";
import { getObservaciones, createObservacion } from "../../services/observacionService";
import { getErrorMessage } from "../../utils/errors";

export default function BitacoraPanel({ proyecto, puedeEscribir }) {
  const [observaciones, setObservaciones] = useState([]);
  const [loading, setLoading] = useState(true);
  const [detalle, setDetalle] = useState("");
  const [enviando, setEnviando] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    async function cargar() {
      try {
        setLoading(true);
        setObservaciones(await getObservaciones(proyecto.id));
      } catch {
        setError("No se pudo cargar la bitácora.");
      } finally {
        setLoading(false);
      }
    }
    cargar();
  }, [proyecto.id]);

  const handleSubmit = async (event) => {
    event.preventDefault();
    if (!detalle.trim()) return;

    setError("");
    setEnviando(true);
    try {
      const nueva = await createObservacion(proyecto.id, detalle.trim());
      setObservaciones((prev) => [nueva, ...prev]);
      setDetalle("");
    } catch (err) {
      setError(getErrorMessage(err, "No se pudo registrar la observación."));
    } finally {
      setEnviando(false);
    }
  };

  return (
    <div className="space-y-4">
      {loading ? (
        <p className="text-body-sm text-on-surface-variant">Cargando bitácora...</p>
      ) : (
        <div className="max-h-80 overflow-y-auto space-y-3 pr-1">
          {observaciones.length === 0 && (
            <p className="text-body-sm text-on-surface-variant italic">Sin observaciones registradas.</p>
          )}
          {observaciones.map((obs) => (
            <div key={obs.id} className="border-l-2 border-primary/30 pl-3">
              <p className="text-body-sm text-on-surface">{obs.detalle}</p>
              <p className="text-label-caps text-on-surface-variant mt-1">
                {obs.nombreUsuario || `Usuario #${obs.usuarioId}`} · {new Date(obs.fechaHora).toLocaleString("es-PE")}
              </p>
            </div>
          ))}
        </div>
      )}

      {puedeEscribir && (
        <form onSubmit={handleSubmit} className="pt-3 border-t border-outline-variant space-y-2">
          <textarea
            value={detalle}
            onChange={(e) => setDetalle(e.target.value)}
            placeholder="Escribe una observación..."
            rows={3}
            className="w-full border border-outline-variant rounded px-3 py-2 bg-surface text-on-surface text-body-sm outline-none focus:ring-1 focus:ring-primary"
          />
          <div className="flex justify-end">
            <Button type="submit" disabled={enviando}>
              {enviando ? "Guardando..." : "Agregar observación"}
            </Button>
          </div>
        </form>
      )}

      <FormMessage type="error" message={error} />
    </div>
  );
}