import api from "./api";

export const getDocumentosByProyecto = async (proyectoId) => {
  const response = await api.get(`/Proyecto/${proyectoId}/documentos`);
  return response.data;
};

export const subirDocumento = async (proyectoId, formData) => {
  // formData: Categoria, Archivo
  const response = await api.post(`/Proyecto/${proyectoId}/documentos`, formData, {
    headers: { "Content-Type": "multipart/form-data" },
  });
  return response.data;
};

export const getVersionesDocumento = async (documentoId) => {
  const response = await api.get(`/Documento/${documentoId}/versiones`);
  return response.data;
};

export const descargarVersion = async (documentoId, numeroVersion, nombreArchivo) => {
  const response = await api.get(`/Documento/${documentoId}/version/${numeroVersion}/descargar`, {
    responseType: "blob",
  });
  
  // Crear link de descarga
  const url = window.URL.createObjectURL(new Blob([response.data]));
  const link = document.createElement("a");
  link.href = url;
  link.setAttribute("download", nombreArchivo);
  document.body.appendChild(link);
  link.click();
  link.remove();
};
