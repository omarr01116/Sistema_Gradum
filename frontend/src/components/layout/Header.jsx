import { useAuth } from "../../context/AuthContext";
import { ROLE_LABELS } from "../../constants/roles";

export default function Header() {
  const { nombreUsuario, rol } = useAuth();
  const iniciales = nombreUsuario ? nombreUsuario.slice(0, 2).toUpperCase() : "";

  return (
    <header className="h-16 bg-surface border-b border-outline-variant flex justify-between items-center px-6 sticky top-0 z-10">
      <div>
        <h2 className="font-semibold text-headline-md text-on-background leading-none">
          Panel de Control Académico
        </h2>
        <p className="text-body-sm text-secondary mt-1">
          Sistema de gestión GRADUM
        </p>
      </div>

      <div className="flex items-center gap-4">
        <button className="w-8 h-8 rounded-full flex items-center justify-center text-on-surface-variant hover:bg-surface-container hover:text-primary transition-colors">
          <span className="material-symbols-outlined">notifications</span>
        </button>
        <div className="h-8 w-px bg-outline-variant mx-2" />
        <div className="flex items-center gap-3">
          <div className="text-right hidden md:block">
            <div className="font-semibold text-body-md text-on-surface leading-none">
              {nombreUsuario}
            </div>
            <div className="text-label-caps text-primary bg-primary-container/10 px-2 py-0.5 rounded mt-1 inline-block">
              {ROLE_LABELS[rol] ?? rol}
            </div>
          </div>
          <div className="w-10 h-10 rounded-full bg-primary-container flex items-center justify-center text-on-primary-container font-semibold text-body-sm">
            {iniciales}
          </div>
        </div>
      </div>
    </header>
  );
}