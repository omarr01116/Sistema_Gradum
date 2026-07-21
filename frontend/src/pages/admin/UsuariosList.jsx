import { useState, useEffect } from "react";
import MainLayout from "../../layouts/MainLayout";
import PageContainer from "../../components/layout/PageContainer";
import DataTable from "../../components/ui/DataTable";
import Button from "../../components/ui/Button";
import Modal from "../../components/ui/Modal";
import InputField from "../../components/ui/InputField";
import FormMessage from "../../components/ui/FormMessage";
import Badge from "../../components/ui/Badge";
import { getUsuarios, crearUsuario, actualizarUsuario, desactivarUsuario } from "../../services/userService";
import { getAsesores } from "../../services/asesorService";
import { ROLE_LABELS } from "../../constants/roles";
import { getErrorMessage } from "../../utils/errors";

export default function UsuariosList() {
  const [usuarios, setUsuarios] = useState([]);
  const [asesores, setAsesores] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  
  const [modalOpen, setModalOpen] = useState(false);
  const [usuarioEditando, setUsuarioEditando] = useState(null);
  const [formData, setFormData] = useState({ nombreUsuario: "", password: "", rol: "Asesor", asesorId: "" });

  const cargarDatos = async () => {
    try {
      setLoading(true);
      const [uData, aData] = await Promise.all([getUsuarios(), getAsesores()]);
      setUsuarios(uData);
      setAsesores(aData);
    } catch (err) {
      setError("No se pudieron cargar los usuarios.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    cargarDatos();
  }, []);

  const handleOpenModal = (usuario = null) => {
    if (usuario) {
      setUsuarioEditando(usuario);
      setFormData({ 
        nombreUsuario: usuario.nombreUsuario, 
        password: "", 
        rol: usuario.rol, 
        asesorId: usuario.asesorId || "" 
      });
    } else {
      setUsuarioEditando(null);
      setFormData({ nombreUsuario: "", password: "", rol: "Asesor", asesorId: "" });
    }
    setModalOpen(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    try {
      const data = { ...formData, asesorId: formData.asesorId || null };
      if (usuarioEditando) {
        await actualizarUsuario(usuarioEditando.id, data);
      } else {
        await crearUsuario(data);
      }
      setModalOpen(false);
      cargarDatos();
    } catch (err) {
      setError(getErrorMessage(err, "Error al guardar usuario."));
    }
  };

  const columns = [
    { key: "nombreUsuario", label: "Usuario" },
    { key: "rol", label: "Rol", render: (u) => ROLE_LABELS[u.rol] || u.rol },
    { key: "asesorVinculado", label: "Asesor Vinculado", render: (u) => {
      const a = asesores.find(as => as.id === u.asesorId);
      return a ? `${a.nombres} ${a.apellidos}` : "—";
    }},
    { key: "acciones", label: "Acciones", render: (u) => (
      <div className="flex gap-2 justify-end">
        <Button variant="secondary" onClick={() => handleOpenModal(u)}>Editar</Button>
        {u.activo !== false && (
          <Button variant="danger" onClick={async () => {
             if(window.confirm("¿Seguro que deseas desactivar este usuario?")) {
               try {
                 await desactivarUsuario(u.id);
                 cargarDatos();
               } catch (err) {
                 setError(getErrorMessage(err, "Error al desactivar usuario."));
               }
             }
          }}>Desactivar</Button>
        )}
      </div>
    )}
  ];

  return (
    <MainLayout>
      <PageContainer title="Gestión de Usuarios" subtitle="Administra las cuentas de acceso al sistema.">
        <div className="mb-4 flex justify-between items-center">
          <FormMessage type="error" message={error} />
          <Button onClick={() => handleOpenModal()}>Nuevo Usuario</Button>
        </div>

        <DataTable columns={columns} data={usuarios} loading={loading} />

        <Modal isOpen={modalOpen} onClose={() => setModalOpen(false)} title={usuarioEditando ? "Editar Usuario" : "Nuevo Usuario"}>
          <form onSubmit={handleSubmit} className="space-y-4">
            <InputField 
              label="Nombre de Usuario" 
              value={formData.nombreUsuario} 
              onChange={(e) => setFormData({...formData, nombreUsuario: e.target.value})} 
              required 
              disabled={!!usuarioEditando}
            />
            {!usuarioEditando && (
              <InputField 
                label="Contraseña" 
                type="password"
                value={formData.password} 
                onChange={(e) => setFormData({...formData, password: e.target.value})} 
                required 
              />
            )}
            <div className="flex flex-col gap-1">
              <label className="text-label-sm text-on-surface-variant px-1">Rol</label>
              <select 
                className="bg-surface-container border border-outline-variant rounded-lg px-3 py-2 text-body-md"
                value={formData.rol}
                onChange={(e) => setFormData({...formData, rol: e.target.value})}
              >
                {Object.entries(ROLE_LABELS).map(([val, label]) => (
                  <option key={val} value={val}>{label}</option>
                ))}
              </select>
            </div>
            {formData.rol === "Asesor" && (
              <div className="flex flex-col gap-1">
                <label className="text-label-sm text-on-surface-variant px-1">Vincular con Asesor</label>
                <select 
                  className="bg-surface-container border border-outline-variant rounded-lg px-3 py-2 text-body-md"
                  value={formData.asesorId}
                  onChange={(e) => setFormData({...formData, asesorId: e.target.value})}
                  required
                >
                  <option value="">Seleccione un asesor...</option>
                  {asesores.map(a => (
                    <option key={a.id} value={a.id}>{a.nombres} {a.apellidos} ({a.especialidad})</option>
                  ))}
                </select>
              </div>
            )}
            <div className="flex justify-end gap-3 mt-6">
              <Button variant="secondary" type="button" onClick={() => setModalOpen(false)}>Cancelar</Button>
              <Button type="submit">Guardar</Button>
            </div>
          </form>
        </Modal>
      </PageContainer>
    </MainLayout>
  );
}
