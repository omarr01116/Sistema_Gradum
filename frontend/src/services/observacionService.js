import api from "./api";

// Body de creación: { detalle } — según la entidad real Observacion.Detalle.
export async function getObservaciones(idProyecto) {
  const response = await api.get(`/Proyecto/${idProyecto}/observaciones`);
  return response.data;
}

export async function createObservacion(idProyecto, detalle) {
  const response = await api.post(`/Proyecto/${idProyecto}/observaciones`, { detalle });
  return response.data;
}