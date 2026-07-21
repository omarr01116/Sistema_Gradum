import api from "./api";

const RESOURCE = "/Proyecto";

export async function getProyectos() {
  const response = await api.get(RESOURCE);
  return response.data;
}

export async function getProyecto(id) {
  const response = await api.get(`${RESOURCE}/${id}`);
  return response.data;
}

export async function createProyecto(data) {
  const response = await api.post(RESOURCE, data);
  return response.data;
}

export async function updateProyecto(id, data) {
  const response = await api.put(`${RESOURCE}/${id}`, data);
  return response.data;
}

// CambiarEstadoDto: { nuevoEstado }
export async function cambiarEstadoProyecto(id, nuevoEstado) {
  const response = await api.patch(`${RESOURCE}/${id}/estado`, { nuevoEstado });
  return response.data;
}