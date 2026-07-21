import { useAuth } from "../../context/AuthContext";
import { ROLE_LABELS } from "../../constants/roles";
import { Bell, ChevronDown } from "lucide-react";

export default function Header() {
  const { nombreUsuario, rol } = useAuth();
  const iniciales = nombreUsuario ? nombreUsuario.slice(0, 2).toUpperCase() : "U";

  return (
    <header className="h-20 bg-white border-b border-gray-200 flex items-center justify-between px-8 sticky top-0 z-10 w-full">
      <div className="flex-1">
        {/* Placeholder for future left-side items if needed */}
      </div>
      <div className="flex items-center gap-6">
        <button className="relative p-2 text-gray-600 hover:bg-slate-50 rounded-full transition-colors">
          <Bell size={22} />
          <span className="absolute top-1.5 right-1.5 w-2.5 h-2.5 bg-[#7A0B2E] rounded-full border-2 border-white"></span>
        </button>
        <div className="h-8 w-px bg-gray-200" />
        <div className="flex items-center gap-3 cursor-pointer hover:opacity-80">
          <div className="text-right hidden md:block">
            <p className="text-sm font-bold text-gray-800 leading-tight">
              {nombreUsuario}
            </p>
            <p className="text-xs text-gray-500">
              {ROLE_LABELS[rol] ?? rol}
            </p>
          </div>
          <div className="w-10 h-10 rounded-full overflow-hidden bg-gray-200 border border-gray-200 flex items-center justify-center font-bold text-gray-500">
             {iniciales}
          </div>
          <ChevronDown size={16} className="text-gray-400" />
        </div>
      </div>
    </header>
  );
}