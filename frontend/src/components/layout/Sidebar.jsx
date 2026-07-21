import { NavLink } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import { MENU_POR_ROL } from "../../constants/roles";

export default function Sidebar() {
  const { rol, logout } = useAuth();
  const opciones = MENU_POR_ROL[rol] ?? [];

  return (
    <aside className="fixed left-0 top-0 h-screen w-60 bg-inverse-surface flex flex-col py-6 px-4 z-20">
      <div className="mb-10 px-2 flex items-center gap-3">
        <div className="w-8 h-8 rounded bg-primary-container flex items-center justify-center">
          <span className="material-symbols-outlined text-on-primary-container text-[20px]">
            school
          </span>
        </div>
        <div>
          <h1 className="font-bold text-headline-md text-on-primary-fixed leading-none">
            GRADUM
          </h1>
          <p className="text-label-caps text-secondary-fixed-dim opacity-70 mt-1">
            Academic Management
          </p>
        </div>
      </div>

      <nav className="flex-1 flex flex-col gap-1">
        {opciones.map((item) =>
          item.path ? (
            <NavLink
              key={item.label}
              to={item.path}
              end={item.path === "/"}
              className={({ isActive }) =>
                `flex items-center gap-3 px-4 py-2 rounded-lg text-body-md transition-colors ${
                  isActive
                    ? "bg-primary-container text-on-primary-container font-semibold"
                    : "text-on-surface-variant hover:bg-secondary-fixed hover:text-on-primary-fixed-variant"
                }`
              }
            >
              <span className="material-symbols-outlined text-[20px]">{item.icon}</span>
              {item.label}
            </NavLink>
          ) : (
            <button
              key={item.label}
              disabled
              className="flex items-center gap-3 px-4 py-2 rounded-lg text-body-md text-secondary-fixed-dim opacity-40 cursor-not-allowed text-left"
            >
              <span className="material-symbols-outlined text-[20px]">{item.icon}</span>
              {item.label}
            </button>
          )
        )}
      </nav>

      <div className="mt-auto pt-4 border-t border-secondary/30">
        <button
          onClick={logout}
          className="flex items-center gap-3 px-4 py-2 w-full text-left text-secondary-fixed-dim hover:text-on-primary-fixed transition-colors rounded-lg"
        >
          <span className="material-symbols-outlined text-[20px]">logout</span>
          Cerrar sesión
        </button>
      </div>
    </aside>
  );
}