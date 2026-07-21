import Badge from "../../components/ui/Badge";
import { ESTADOS_PROYECTO } from "../../constants/estadosProyecto";

export default function ProyectoCard({
  proyecto, clienteNombre, asesorNombre,
  puedeTransicionar, onCambiarEstado, onVerBitacora, onVerHitos, onVerDocumentos, onEditar,
}) {
  const config = ESTADOS_PROYECTO[proyecto.estadoProyecto];
  const transiciones = config?.transicionesValidas ?? [];

  return (
    <div className="bg-surface-container-lowest border border-outline-variant rounded-lg p-4 shadow-sm hover:shadow-md transition-shadow">
      <div className="flex justify-between items-start mb-2">
        <span className="text-label-caps text-on-surface-variant">{proyecto.codigoProyecto}</span>
        <Badge variant={config?.badgeVariant ?? "neutral"}>{config?.label ?? proyecto.estadoProyecto}</Badge>
      </div>

      <h4 className="font-semibold text-body-md text-on-surface mb-1 line-clamp-2">{proyecto.tema}</h4>
      
      <div className="w-full bg-surface-container rounded-full h-1.5 mb-3">
        <div 
          className="bg-primary h-1.5 rounded-full transition-all duration-500" 
          style={{ width: `${proyecto.porcentajeAvance || 0}%` }}
        ></div>
      </div>
      <p className="text-body-sm text-on-surface-variant mb-3">
        {clienteNombre} · {asesorNombre ?? "Sin asesor"}
      </p>
      <p className="text-body-sm text-on-surface-variant mb-3">
        Entrega: {new Date(proyecto.fechaEntregaComprometida).toLocaleDateString("es-PE")}
      </p>

      <div className="flex flex-wrap gap-3 mb-2">
        <button onClick={() => onVerHitos(proyecto)} className="text-body-sm text-primary hover:underline font-medium">
          Hitos ({proyecto.porcentajeAvance || 0}%)
        </button>
        <button onClick={() => onVerDocumentos(proyecto)} className="text-body-sm text-primary hover:underline">
          Documentos
        </button>
        <button onClick={() => onVerBitacora(proyecto)} className="text-body-sm text-primary hover:underline">
          Bitácora
        </button>
        {puedeTransicionar && (
          <button onClick={() => onEditar(proyecto)} className="text-body-sm text-primary hover:underline">
            Editar
          </button>
        )}
      </div>

      {puedeTransicionar && transiciones.length > 0 && (
        <div className="flex flex-wrap gap-2 pt-2 border-t border-outline-variant">
          {transiciones.map((estado) => (
            <button
              key={estado}
              onClick={() => onCambiarEstado(proyecto, estado)}
              className="text-body-sm px-2 py-1 rounded border border-outline-variant text-on-surface-variant hover:bg-surface-container hover:text-primary transition-colors"
            >
              → {ESTADOS_PROYECTO[estado].label}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}