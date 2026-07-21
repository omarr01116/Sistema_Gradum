import { useState } from "react";
import { useNavigate } from "react-router-dom";
import InputField from "../components/ui/InputField";
import Button from "../components/ui/Button";
import FormMessage from "../components/ui/FormMessage";
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
    <div className="flex h-screen w-screen overflow-hidden bg-background">
      {/* Panel institucional (solo desktop) */}
      <div className="hidden lg:flex lg:w-[60%] relative bg-primary-container overflow-hidden">
        <div className="relative z-10 flex flex-col justify-between p-section-gap w-full h-full text-on-primary">
          <div className="flex items-center gap-3">
            <span className="material-symbols-outlined text-4xl">school</span>
            <h1 className="font-bold text-display-lg tracking-tight">GRADUM</h1>
          </div>
          <div className="max-w-xl">
            <h2 className="font-bold text-display-lg mb-6 leading-tight">
              Impulsando la excelencia en investigación académica.
            </h2>
            <p className="text-body-md text-primary-fixed-dim">
              Plataforma integral para gestionar proyectos de tesis, hitos,
              documentos y la relación entre tesistas y asesores.
            </p>
          </div>
          <div className="flex gap-4">
            <span className="material-symbols-outlined text-primary-fixed-dim">verified</span>
            <span className="material-symbols-outlined text-primary-fixed-dim">workspace_premium</span>
            <span className="material-symbols-outlined text-primary-fixed-dim">assured_workload</span>
          </div>
        </div>
      </div>

      {/* Formulario */}
      <div className="w-full lg:w-[40%] bg-surface flex flex-col justify-center px-8 sm:px-12 lg:px-16 xl:px-24 overflow-y-auto">
        <div className="lg:hidden flex items-center gap-3 mb-12 justify-center">
          <span className="material-symbols-outlined text-primary text-3xl">school</span>
          <h1 className="font-bold text-headline-md text-primary">GRADUM</h1>
        </div>

        <div className="w-full max-w-md mx-auto">
          <div className="mb-10 text-center lg:text-left">
            <h2 className="font-semibold text-headline-md text-on-surface mb-2">
              Iniciar Sesión
            </h2>
            <p className="text-body-sm text-on-surface-variant">
              Accede a tu panel de gestión académica.
            </p>
          </div>

          <form onSubmit={handleSubmit} className="space-y-6">
            <InputField
              label="Usuario"
              name="nombreUsuario"
              icon="person"
              value={formData.nombreUsuario}
              onChange={handleChange}
              placeholder="usuario"
              required
            />
            <InputField
              label="Contraseña"
              name="password"
              type="password"
              icon="lock"
              value={formData.password}
              onChange={handleChange}
              placeholder="••••••••"
              required
            />

            <Button type="submit" disabled={isSubmitting} fullWidth>
              {isSubmitting ? "Ingresando..." : "Iniciar Sesión"}
              {!isSubmitting && (
                <span className="material-symbols-outlined text-[18px]">
                  arrow_forward
                </span>
              )}
            </Button>
          </form>

          <FormMessage type="error" message={message} />
        </div>
      </div>
    </div>
  );
}