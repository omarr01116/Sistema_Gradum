import api from "./api";

export const getUsuarios = async () => {
  const response = await api.get("/Usuario");
  return response.data;
};

export const getUsuarioById = async (id) => {
  const response = await api.get(`/Usuario/${id}`);
  return response.data;
};

export const crearUsuario = async (data) => {
  const response = await api.post("/Usuario", data);
  return response.data;
};

export const actualizarUsuario = async (id, data) => {
  const response = await api.put(`/Usuario/${id}`, data);
  return response.data;
};

export const desactivarUsuario = async (id) => {
  const response = await api.patch(`/Usuario/${id}/desactivar`);
  return response.data;
};
