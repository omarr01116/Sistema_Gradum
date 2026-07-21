import api from "./api";

// Coincide con LoginRequestDto / LoginResponseDto del backend.
export async function login(nombreUsuario, password) {
  const response = await api.post("/Auth/login", { nombreUsuario, password });
  return response.data; // { token, nombreUsuario, rol, asesorId, expiraEn }
}