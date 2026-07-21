import api from "./api";

const RESOURCE = "/Asesor";

export async function getAsesores() {
  const { data } = await api.get(RESOURCE);
  return data;
}

export async function getAsesorById(id) {
  const { data } = await api.get(`${RESOURCE}/${id}`);
  return data;
}

export async function createAsesor(dto) {
  try {
    const { data } = await api.post(RESOURCE, dto);
    return data;
  } catch (error) {
    throw new Error(
      error.response?.data?.mensaje || error.response?.data?.message || "No se pudo crear el asesor."
    );
  }
}

export async function updateAsesor(id, dto) {
  try {
    await api.put(`${RESOURCE}/${id}`, dto);
  } catch (error) {
    throw new Error(
      error.response?.data?.mensaje || error.response?.data?.message || "No se pudo actualizar el asesor."
    );
  }
}

export async function desactivarAsesor(id) {
  try {
    await api.delete(`${RESOURCE}/${id}`);
  } catch (error) {
    throw new Error(
      error.response?.data?.mensaje || error.response?.data?.message || "No se pudo desactivar el asesor."
    );
  }
}

export async function reactivarAsesor(id) {
  try {
    await api.patch(`${RESOURCE}/${id}/reactivar`);
  } catch (error) {
    throw new Error(
      error.response?.data?.mensaje || error.response?.data?.message || "No se pudo reactivar el asesor."
    );
  }
}