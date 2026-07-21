const VARIANTS = {
  success: "bg-green-100 text-green-700",
  danger: "bg-red-100 text-red-700",
  neutral: "bg-gray-100 text-gray-600",
  info: "bg-blue-100 text-blue-700",     // nuevo — estado "Activo"
  warning: "bg-amber-100 text-amber-700", // nuevo — estado "Sustentado"
};

export default function Badge({ variant = "neutral", children }) {
  return (
    <span className={`inline-block px-2.5 py-1 rounded-full text-label-caps font-semibold ${VARIANTS[variant]}`}>
      {children}
    </span>
  );
}