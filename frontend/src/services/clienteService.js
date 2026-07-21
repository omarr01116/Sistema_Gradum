import api from "./api";

const RESOURCE = "/Cliente";

export async function getClientes() {
  const { data } = await api.get(RESOURCE);
  return data;
}

export async function getClienteById(id) {
  const { data } = await api.get(`${RESOURCE}/${id}`);
  return data;
}

export async function createCliente(dto) {
  try {
    const { data } = await api.post(RESOURCE, dto);
    return data;
  } catch (error) {
    throw new Error(error.response?.data?.mensaje || "No se pudo crear el cliente.");
  }
}

export async function updateCliente(id, dto) {
  try {
    await api.put(`${RESOURCE}/${id}`, dto);
  } catch (error) {
    throw new Error(error.response?.data?.mensaje || "No se pudo actualizar el cliente.");
  }
}

export async function desactivarCliente(id) {
  try {
    await api.delete(`${RESOURCE}/${id}`);
  } catch (error) {
    throw new Error(error.response?.data?.mensaje || "No se pudo desactivar el cliente.");
  }
}

export async function reactivarCliente(id) {
  try {
    await api.patch(`${RESOURCE}/${id}/reactivar`);
  } catch (error) {
    throw new Error(error.response?.data?.mensaje || "No se pudo reactivar el cliente.");
  }
}