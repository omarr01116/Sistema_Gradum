const VARIANTS = {
  success: "bg-green-100 text-green-700",
  danger: "bg-red-100 text-red-700",
  neutral: "bg-gray-100 text-gray-600",
  warning: "bg-amber-100 text-amber-700",
};

export default function Badge({ label, variant = "neutral" }) {
  return (
    <span
      className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-label-caps font-semibold ${VARIANTS[variant]}`}
    >
      {label}
    </span>
  );
}