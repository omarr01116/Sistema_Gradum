import axios from "axios";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
});

api.interceptors.request.use((config) => {
  const raw = sessionStorage.getItem("gradum_session");
  if (raw) {
    const session = JSON.parse(raw);
    if (session?.token) {
      config.headers.Authorization = `Bearer ${session.token}`;
    }
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      sessionStorage.removeItem("gradum_session");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

export default api;