import { useState, useEffect } from "react";
import MainLayout from "../../layouts/MainLayout";
import PageContainer from "../../components/layout/PageContainer";
import Button from "../../components/ui/Button";
import InputField from "../../components/ui/InputField";
import FormMessage from "../../components/ui/FormMessage";
import { getConfiguracion, actualizarConfiguracion } from "../../services/configuracionService";

export default function ConfiguracionPage() {
  const [limite, setLimite] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [guardando, setGuardando] = useState(false);

  useEffect(() => {
    const cargar = async () => {
      try {
        const data = await getConfiguracion("LIMITE_PROYECTOS_ASESOR");
        setLimite(data.valor);
      } catch (err) {
        setError("No se pudo cargar la configuración.");
      } finally {
        setLoading(false);
      }
    };
    cargar();
  }, []);

  const handleSave = async (e) => {
    e.preventDefault();
    setError("");
    setSuccess("");
    setGuardando(true);
    try {
      await actualizarConfiguracion("LIMITE_PROYECTOS_ASESOR", limite);
      setSuccess("Configuración actualizada correctamente.");
    } catch (err) {
      setError("Error al guardar la configuración.");
    } finally {
      setGuardando(false);
    }
  };

  return (
    <MainLayout>
      <PageContainer title="Configuración del Sistema" subtitle="Ajusta los parámetros globales de GRADUM.">
        <div className="max-w-md">
          <FormMessage type="error" message={error} />
          <FormMessage type="success" message={success} />

          {loading ? (
            <p className="text-body-sm">Cargando...</p>
          ) : (
            <form onSubmit={handleSave} className="bg-surface-container-low p-6 rounded-xl border border-outline-variant space-y-6">
              <div>
                <h3 className="font-semibold text-body-lg mb-2">Parámetros de Asignación</h3>
                <InputField 
                  label="Límite de proyectos por asesor" 
                  type="number"
                  value={limite} 
                  onChange={(e) => setLimite(e.target.value)} 
                  required 
                  helperText="Define cuántos proyectos activos puede tener un asesor simultáneamente."
                />
              </div>
              
              <div className="pt-4 border-t border-outline-variant flex justify-end">
                <Button type="submit" disabled={guardando}>
                  {guardando ? "Guardando..." : "Guardar Cambios"}
                </Button>
              </div>
            </form>
          )}
        </div>
      </PageContainer>
    </MainLayout>
  );
}
