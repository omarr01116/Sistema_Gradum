// columns: [{ key, label, render?(row) }]
// data: array de objetos (deben incluir `id`)
export default function DataTable({ columns, data, emptyMessage = "No hay registros." }) {
  if (!data || data.length === 0) {
    return (
      <div className="border border-dashed border-outline-variant rounded-xl p-10 text-center text-body-sm text-on-surface-variant bg-surface-bright/50">
        {emptyMessage}
      </div>
    );
  }

  return (
    <div className="overflow-x-auto border border-outline-variant rounded-xl bg-surface">
      <table className="w-full text-left">
        <thead>
          <tr className="border-b border-outline-variant bg-surface-container-low">
            {columns.map((col) => (
              <th
                key={col.key}
                className="px-4 py-3 text-label-caps text-on-surface-variant uppercase tracking-wide"
              >
                {col.label}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {data.map((row) => (
            <tr
              key={row.id}
              className="border-b border-outline-variant last:border-0 hover:bg-surface-container-low/50 transition-colors"
            >
              {columns.map((col) => (
                <td key={col.key} className="px-4 py-3 text-body-md text-on-surface">
                  {col.render ? col.render(row) : row[col.key]}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}