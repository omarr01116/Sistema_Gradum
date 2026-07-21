// Extrae el mensaje que devuelve ReglaNegocioException vía 400 { mensaje: "..." }.
// Reutilizable en cualquier form/acción que hable con el backend.
export function getErrorMessage(error, fallback = "Ocurrió un error inesperado.") {
  return error?.response?.data?.mensaje ?? fallback;
}