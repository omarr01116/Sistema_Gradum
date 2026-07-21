import MainLayout from "../layouts/MainLayout";
import PageContainer from "../components/layout/PageContainer";
import { useAuth } from "../context/AuthContext";

export default function Dashboard() {
  const { rol } = useAuth();

  return (
    <MainLayout>
      <PageContainer
        title="Dashboard Overview"
        subtitle="Progreso académico e indicadores generales del periodo actual."
      >
        <div className="border border-dashed border-outline-variant rounded-xl flex items-center justify-center bg-surface-bright/50 min-h-[400px]">
          <div className="text-center max-w-md px-4">
            <div className="w-16 h-16 bg-surface-container rounded-full flex items-center justify-center mx-auto mb-4 text-primary">
              <span className="material-symbols-outlined text-[32px]">
                space_dashboard
              </span>
            </div>
            <h3 className="font-semibold text-headline-md text-on-surface mb-2">
              Workspace listo
            </h3>
            <p className="text-body-sm text-on-surface-variant">
              Los indicadores de <strong>GET /api/Dashboard</strong> se
              conectan en el próximo commit
              {rol === "Asesor" ? " (próximas entregas)." : " (KPIs generales)."}
            </p>
          </div>
        </div>
      </PageContainer>
    </MainLayout>
  );
}