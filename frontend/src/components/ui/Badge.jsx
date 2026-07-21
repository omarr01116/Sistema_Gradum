export default function Badge({ label, variant = "neutral", children }) {
  const styles = {
    active: "bg-blue-100 text-blue-700",
    correcciones: "bg-red-100 text-red-700",
    finished: "bg-green-100 text-green-700",
    aldia: "border border-green-500 text-green-700 bg-transparent",
    warning: "bg-yellow-100 text-yellow-700",
    neutral: "bg-gray-100 text-gray-700",
    highPrio: "bg-red-100 text-red-700 font-bold",
    update: "bg-emerald-100 text-emerald-700",
    success: "bg-green-100 text-green-700",
    danger: "bg-red-100 text-red-700",
    info: "bg-blue-100 text-blue-700",
  };
  
  return (
    <span className={`inline-flex items-center rounded-full px-3 py-1 text-xs font-semibold ${styles[variant] || styles.neutral}`}>
      {label || children}
    </span>
  );
}