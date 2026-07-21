import { useState, useEffect, useCallback } from "react";
import Button from "../../components/ui/Button";
import InputField from "../../components/ui/InputField";
import FormMessage from "../../components/ui/FormMessage";
import { getDocumentosByProyecto, subirDocumento, descargarVersion } from "../../services/documentoService";
import { useAuth } from "../../context/AuthContext";
import { getErrorMessage } from "../../utils/errors";

export default function DocumentosPanel({ proyecto }) {
  const { rol } = useAuth();
  const [documentos, setDocumentos] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const [subiendo, setSubiendo] = useState(false);
  const [nuevaCategoria, setNuevaCategoria] = useState("");
  const [archivo, setArchivo] = useState(null);

  const cargarDocumentos = useCallback(async () => {
    try {
      setLoading(true);
      const data = await getDocumentosByProyecto(proyecto.id);
      setDocumentos(data);
    } catch (err) {
      setError("No se pudieron cargar los documentos.");
    } finally {
      setLoading(false);
    }
  }, [proyecto.id]);

  useEffect(() => {
    cargarDocumentos();
  }, [cargarDocumentos]);

  const handleUpload = async (e) => {
    e.preventDefault();
    if (!archivo || !nuevaCategoria) return;

    setError("");
    setSuccess("");
    setSubiendo(true);

    try {
      const formData = new FormData();
      formData.append("Categoria", nuevaCategoria);
      formData.append("Archivo", archivo);

      await subirDocumento(proyecto.id, formData);
      setSuccess("Documento subido correctamente.");
      setNuevaCategoria("");
      setArchivo(null);
      // Reset input file
      document.getElementById("fileInput").value = "";
      cargarDocumentos();
    } catch (err) {
      setError(getErrorMessage(err, "Error al subir documento."));
    } finally {
      setSubiendo(false);
    }
  };

  if (loading) return <p className="p-4 text-body-sm">Cargando documentos...</p>;

  return (
    <div className="p-4 space-y-6">
      <FormMessage type="error" message={error} />
      <FormMessage type="success" message={success} />

      {rol !== "Administrador" && (
        <form onSubmit={handleUpload} className="bg-surface-container-low p-4 rounded-lg space-y-3">
          <h4 className="font-semibold text-body-sm">Subir nuevo documento o versión</h4>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
            <InputField 
              label="Categoría (ej: Tesis, Contrato, Avance 1)" 
              value={nuevaCategoria} 
              onChange={(e) => setNuevaCategoria(e.target.value)} 
              required 
            />
            <div className="flex flex-col gap-1">
              <label className="text-label-sm text-on-surface-variant px-1">Archivo</label>
              <input 
                id="fileInput"
                type="file" 
                onChange={(e) => setArchivo(e.target.files[0])} 
                className="text-body-sm file:mr-4 file:py-2 file:px-4 file:rounded file:border-0 file:text-body-sm file:font-semibold file:bg-primary-container file:text-on-primary-container hover:file:bg-primary-container/80 cursor-pointer"
                required
              />
            </div>
          </div>
          <Button type="submit" disabled={subiendo || !archivo || !nuevaCategoria}>
            {subiendo ? "Subiendo..." : "Subir archivo"}
          </Button>
        </form>
      )}

      <div className="space-y-3">
        <h4 className="font-semibold text-body-sm">Archivos del proyecto</h4>
        {documentos.length === 0 ? (
          <p className="text-body-sm text-on-surface-variant italic">No hay documentos cargados.</p>
        ) : (
          <div className="grid grid-cols-1 gap-3">
            {documentos.map((doc) => (
              <div key={doc.id} className="border border-outline-variant rounded-lg p-3 flex justify-between items-center hover:bg-surface-container-lowest transition-colors">
                <div>
                  <p className="font-medium text-body-md text-on-surface">{doc.categoria}</p>
                  <p className="text-label-sm text-on-surface-variant">
                    {doc.nombreArchivoOriginal} (v{doc.versionActual})
                  </p>
                </div>
                <div className="flex gap-2">
                  <Button variant="secondary" onClick={() => descargarVersion(doc.id, doc.versionActual, doc.nombreArchivoOriginal)}>
                    <span className="material-symbols-outlined text-[18px]">download</span>
                  </Button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
