import { useAuth } from "../../context/AuthContext";
import { ROLE_LABELS } from "../../constants/roles";
import { Bell, ChevronDown } from "lucide-react";
import { useState, useEffect, useRef } from "react";
import { Link } from "react-router-dom";
import { getAlertas, marcarAlertaLeida } from "../../services/alertaService";

export default function Header() {
  const { nombreUsuario, rol, logout } = useAuth();
  const iniciales = nombreUsuario ? nombreUsuario.slice(0, 2).toUpperCase() : "U";

  const [bellOpen, setBellOpen] = useState(false);
  const [profileOpen, setProfileOpen] = useState(false);
  const [alertas, setAlertas] = useState([]);
  const headerRef = useRef(null);

  const cargarAlertas = async () => {
    try {
      const data = await getAlertas();
      setAlertas(data);
    } catch (err) {
      console.error("Error al cargar alertas en Header", err);
    }
  };

  useEffect(() => {
    cargarAlertas();
    const interval = setInterval(cargarAlertas, 30000);
    return () => clearInterval(interval);
  }, []);

  useEffect(() => {
    function handleClickOutside(event) {
      if (headerRef.current && !headerRef.current.contains(event.target)) {
        setBellOpen(false);
        setProfileOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const unreadAlerts = alertas.filter(a => !a.leida);
  const unreadCount = unreadAlerts.length;

  const handleMarcarLeida = async (id, e) => {
    e.preventDefault();
    e.stopPropagation();
    try {
      await marcarAlertaLeida(id);
      setAlertas(prev => prev.map(a => a.id === id ? { ...a, leida: true } : a));
    } catch (err) {
      console.error("Error al marcar alerta leída en Header", err);
    }
  };

  const handleToggleBell = () => {
    setBellOpen(!bellOpen);
    setProfileOpen(false);
    if (!bellOpen) {
      cargarAlertas();
    }
  };

  const handleToggleProfile = () => {
    setProfileOpen(!profileOpen);
    setBellOpen(false);
  };

  return (
    <header ref={headerRef} className="h-20 bg-white border-b border-gray-200 flex items-center justify-between px-8 sticky top-0 z-10 w-full">
      <div className="flex-1">
        {/* Placeholder for future left-side items if needed */}
      </div>
      <div className="flex items-center gap-6">
        {/* Dropdown de Alertas */}
        <div className="relative">
          <button 
            onClick={handleToggleBell}
            className="relative p-2 text-gray-600 hover:bg-slate-50 rounded-full transition-colors focus:outline-none"
          >
            <Bell size={22} />
            {unreadCount > 0 && (
              <span className="absolute top-1.5 right-1.5 w-2.5 h-2.5 bg-red-600 rounded-full border-2 border-white animate-pulse"></span>
            )}
          </button>

          {bellOpen && (
            <div className="absolute right-0 top-full mt-2 w-80 bg-white border border-gray-200 rounded-xl shadow-xl z-50 py-2">
              <div className="px-4 py-2 border-b border-gray-100 flex justify-between items-center bg-slate-50/50 rounded-t-xl">
                <span className="font-bold text-xs text-gray-800">Notificaciones ({unreadCount})</span>
                <Link 
                  to="/alertas" 
                  onClick={() => setBellOpen(false)}
                  className="text-xs text-primary hover:underline font-semibold"
                >
                  Ver todas
                </Link>
              </div>
              <div className="max-h-64 overflow-y-auto divide-y divide-gray-100">
                {unreadAlerts.length === 0 ? (
                  <p className="text-center text-xs text-gray-500 py-6 italic">No tienes alertas pendientes</p>
                ) : (
                  unreadAlerts.slice(0, 5).map((alerta) => (
                    <div key={alerta.id} className="p-3 hover:bg-slate-50 flex flex-col gap-1 transition-colors">
                      <p className="text-xs text-gray-700 font-medium leading-normal">{alerta.mensaje}</p>
                      <div className="flex justify-between items-center mt-1">
                        <span className="text-[10px] text-gray-400">{new Date(alerta.fechaHora).toLocaleDateString()}</span>
                        <button 
                          onClick={(e) => handleMarcarLeida(alerta.id, e)}
                          className="text-[10px] text-primary hover:underline font-bold"
                        >
                          Marcar leída
                        </button>
                      </div>
                    </div>
                  ))
                )}
              </div>
            </div>
          )}
        </div>

        <div className="h-8 w-px bg-gray-200" />

        {/* Dropdown de Perfil de Usuario */}
        <div className="relative">
          <div 
            onClick={handleToggleProfile}
            className="flex items-center gap-3 cursor-pointer hover:opacity-85 transition-opacity"
          >
            <div className="text-right hidden md:block select-none">
              <p className="text-sm font-bold text-gray-800 leading-tight">
                {nombreUsuario}
              </p>
              <p className="text-xs text-gray-500">
                {ROLE_LABELS[rol] ?? rol}
              </p>
            </div>
            <div className="w-10 h-10 rounded-full overflow-hidden bg-slate-100 border border-gray-200 flex items-center justify-center font-bold text-gray-600 select-none">
               {iniciales}
            </div>
            <ChevronDown size={16} className={`text-gray-400 transition-transform duration-200 ${profileOpen ? "rotate-180" : ""}`} />
          </div>

          {profileOpen && (
            <div className="absolute right-0 top-full mt-2 w-56 bg-white border border-gray-200 rounded-xl shadow-xl z-50 py-1 divide-y divide-gray-100">
              <div className="px-4 py-3">
                <p className="text-[10px] text-gray-400 font-semibold uppercase tracking-wider">Sesión iniciada como</p>
                <p className="text-sm font-bold text-gray-800 truncate mt-0.5">{nombreUsuario}</p>
                <p className="text-[10px] bg-primary/10 text-primary rounded-full px-2.5 py-0.5 font-bold tracking-wide inline-block mt-1.5 uppercase">
                  {ROLE_LABELS[rol] ?? rol}
                </p>
              </div>
              <div className="py-1">
                <Link 
                  to="/alertas" 
                  onClick={() => setProfileOpen(false)}
                  className="flex items-center px-4 py-2.5 text-sm text-gray-700 hover:bg-slate-50 transition-colors"
                >
                  Ver Alertas
                </Link>
                {rol === "Administrador" && (
                  <Link 
                    to="/configuracion" 
                    onClick={() => setProfileOpen(false)}
                    className="flex items-center px-4 py-2.5 text-sm text-gray-700 hover:bg-slate-50 transition-colors"
                  >
                    Configuración
                  </Link>
                )}
              </div>
              <div className="py-1">
                <button 
                  onClick={() => {
                    setProfileOpen(false);
                    logout();
                  }}
                  className="flex items-center w-full text-left px-4 py-2.5 text-sm text-red-600 hover:bg-red-50 transition-colors font-medium"
                >
                  Cerrar sesión
                </button>
              </div>
            </div>
          )}
        </div>
      </div>
    </header>
  );
}