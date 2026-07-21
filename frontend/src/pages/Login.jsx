import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function Login() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const [formData, setFormData] = useState({ nombreUsuario: "", password: "" });
  const [message, setMessage] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleChange = (event) => {
    const { name, value } = event.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    setMessage("");

    try {
      setIsSubmitting(true);
      await login(formData.nombreUsuario, formData.password);
      navigate("/");
    } catch (error) {
      setMessage("Usuario o contraseña incorrectos.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen bg-slate-50 flex items-center justify-center p-4 font-sans">
      <div className="bg-white border border-gray-200 shadow-xl rounded-2xl overflow-hidden flex w-full max-w-4xl min-h-[500px]">
        {/* Left Side - Brand */}
        <div className="hidden md:flex w-1/2 bg-slate-50 flex-col items-center justify-center p-12 border-r border-gray-100">
          <div className="w-36 h-36 bg-white rounded-full shadow-md flex items-center justify-center mb-6 overflow-hidden p-2">
            <img src="/logo.jpg" alt="Gradum Logo" className="w-full h-full object-contain rounded-full" />
          </div>
          <h1 className="text-3xl font-bold text-[#7A0B2E] tracking-tight">GRADUM</h1>
          <p className="text-xs text-gray-500 tracking-[0.2em] mt-2 text-center uppercase">Tesis & Investigación</p>
        </div>

        {/* Right Side - Form */}
        <div className="w-full md:w-1/2 p-12 flex flex-col justify-center bg-white">
          <div className="md:hidden flex flex-col items-center mb-8">
            <h1 className="text-3xl font-bold text-[#7A0B2E] tracking-tight">GRADUM</h1>
            <p className="text-xs text-gray-500 tracking-[0.2em] mt-1 text-center uppercase">Tesis & Investigación</p>
          </div>

          <h2 className="text-2xl font-bold text-gray-800 mb-2">Sign In</h2>
          <p className="text-gray-500 text-sm mb-8">Accede a tu panel de gestión académica.</p>
          
          <form onSubmit={handleSubmit} className="space-y-5">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Usuario</label>
              <input 
                type="text" 
                name="nombreUsuario"
                value={formData.nombreUsuario}
                onChange={handleChange}
                placeholder="usuario"
                required
                className="w-full px-4 py-2.5 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-[#7A0B2E]/20 focus:border-[#7A0B2E]"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Contraseña</label>
              <input 
                type="password" 
                name="password"
                value={formData.password}
                onChange={handleChange}
                placeholder="••••••••"
                required
                className="w-full px-4 py-2.5 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-[#7A0B2E]/20 focus:border-[#7A0B2E]"
              />
            </div>
            
            <div className="flex items-center justify-between">
              <label className="flex items-center gap-2 cursor-pointer">
                <input type="checkbox" className="w-4 h-4 rounded border-gray-300 text-[#7A0B2E] focus:ring-[#7A0B2E]" />
                <span className="text-sm text-gray-600">Recordarme</span>
              </label>
              <a href="#" className="text-sm text-[#7A0B2E] hover:underline font-medium">¿Olvidaste tu contraseña?</a>
            </div>

            {message && (
              <div className="p-3 bg-red-50 border border-red-200 text-red-600 text-sm rounded-lg">
                {message}
              </div>
            )}

            <button 
              type="submit" 
              disabled={isSubmitting}
              className="w-full bg-[#7A0B2E] hover:bg-[#610824] text-white font-medium py-2.5 rounded-lg transition-colors mt-2 disabled:opacity-50"
            >
              {isSubmitting ? "Ingresando..." : "Iniciar Sesión"}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}