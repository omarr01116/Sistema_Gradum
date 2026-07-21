import api from "./api";

export const getConfiguracion = async (clave) => {
  const response = await api.get(`/Configuracion/${clave}`);
  return response.data;
};

export const actualizarConfiguracion = async (clave, valor) => {
  const response = await api.put(`/Configuracion/${clave}`, { valor });
  return response.data;
};
