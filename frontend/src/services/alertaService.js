import api from "./api";

export const getAlertas = async () => {
  const response = await api.get("/Alerta");
  return response.data;
};

export const marcarAlertaLeida = async (id) => {
  const response = await api.patch(`/Alerta/${id}/leida`);
  return response.data;
};
