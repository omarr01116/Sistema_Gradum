import { NavLink } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import { MENU_POR_ROL } from "../../constants/roles";
import { 
  LayoutDashboard, 
  Users, 
  FolderKanban, 
  GraduationCap, 
  FileText, 
  Settings, 
  Bell,
  LogOut
} from "lucide-react";

// Mapeo de strings a componentes de Lucide
const ICON_MAP = {
  LayoutDashboard,
  Users,
  FolderKanban,
  GraduationCap,
  FileText,
  Settings,
  Bell,
};

export default function Sidebar() {
  const { rol, logout, nombreUsuario } = useAuth();
  const opciones = MENU_POR_ROL[rol] ?? [];

  return (
    <div className="w-64 h-screen bg-[#5B6275] text-white flex flex-col fixed left-0 top-0 z-20">
      <div className="p-6 flex flex-col items-center border-b border-gray-600/50">
        <div className="w-20 h-20 bg-white rounded-full flex items-center justify-center mb-3 overflow-hidden p-1 shadow-md">
          <img src="/logo.jpg" alt="Gradum Logo" className="w-full h-full object-contain rounded-full" />
        </div>
        <h1 className="text-xl font-bold tracking-wider">GRADUM</h1>
        <p className="text-[10px] text-gray-300 tracking-widest text-center uppercase mt-1">Academic Management</p>
      </div>

      <nav className="flex-1 py-6 flex flex-col gap-2 px-3 overflow-y-auto">
        {opciones.map((item) => {
          const IconComponent = ICON_MAP[item.icon] || FileText;
          return item.path ? (
            <NavLink
              key={item.label}
              to={item.path}
              end={item.path === "/"}
              className={({ isActive }) =>
                `flex items-center gap-3 px-4 py-3 rounded-lg transition-colors ${
                  isActive
                    ? "bg-white/10 border-l-4 border-[#7A0B2E] text-white"
                    : "hover:bg-white/5 text-gray-300"
                }`
              }
            >
              <IconComponent size={20} />
              <span className="font-medium">{item.label}</span>
            </NavLink>
          ) : (
            <button
              key={item.label}
              disabled
              className="flex items-center gap-3 px-4 py-3 rounded-lg hover:bg-white/5 text-gray-300/50 transition-colors cursor-not-allowed w-full text-left"
            >
              <IconComponent size={20} />
              <span className="font-medium">{item.label}</span>
            </button>
          )
        })}
      </nav>

      <div className="px-3 pb-6 border-t border-gray-600/50 pt-4">
        <button
          onClick={logout}
          className="flex items-center w-full gap-3 px-4 py-3 rounded-lg hover:bg-white/5 text-gray-300 transition-colors text-left"
        >
          <LogOut size={20} />
          <span className="font-medium">Cerrar sesión</span>
        </button>
        <div className="mt-2 p-3 bg-white/5 rounded-xl flex items-center gap-3">
          <div className="w-10 h-10 rounded-full bg-gray-400 flex items-center justify-center font-bold text-white uppercase overflow-hidden">
             {nombreUsuario ? nombreUsuario.slice(0, 2) : "U"}
          </div>
          <div className="flex-1 overflow-hidden">
            <p className="text-sm font-semibold truncate">{nombreUsuario}</p>
            <p className="text-[10px] text-gray-400 truncate uppercase tracking-wider">{rol}</p>
          </div>
        </div>
      </div>
    </div>
  );
}