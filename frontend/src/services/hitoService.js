import api from "./api";

export const getHitosByProyecto = async (proyectoId) => {
  const response = await api.get(`/Proyecto/${proyectoId}/hitos`);
  return response.data;
};

export const crearHitosLote = async (proyectoId, hitos) => {
  const response = await api.post(`/Proyecto/${proyectoId}/hitos`, hitos);
  return response.data;
};

export const actualizarHito = async (id, data) => {
  const response = await api.put(`/Hito/${id}`, data);
  return response.data;
};

export const eliminarHito = async (id) => {
  const response = await api.delete(`/Hito/${id}`);
  return response.data;
};

export const completarHito = async (id, formData) => {
  // formData debe contener "Evidencia" (archivo) si se desea subir uno
  const response = await api.patch(`/Hito/${id}/completar`, formData, {
    headers: { "Content-Type": "multipart/form-data" },
  });
  return response.data;
};

export const aprobarHito = async (id) => {
  const response = await api.patch(`/Hito/${id}/aprobar`);
  return response.data;
};

export const rechazarHito = async (id, motivo) => {
  const response = await api.patch(`/Hito/${id}/rechazar`, { motivo });
  return response.data;
};
