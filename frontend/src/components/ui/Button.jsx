export default function Button({
  children,
  disabled = false,
  type = "button",
  onClick,
  variant = "primary",
  fullWidth = false,
}) {
  const variants = {
    primary:
      "bg-primary text-on-primary hover:bg-primary-container disabled:bg-outline-variant disabled:text-outline",
    secondary:
      "bg-surface-container text-on-surface hover:bg-surface-container-high disabled:bg-outline-variant/30 disabled:text-outline/50",
    danger:
      "bg-red-600 text-white hover:bg-red-700 disabled:bg-red-300 disabled:text-white/80",
  };

  return (
    <button
      type={type}
      disabled={disabled}
      onClick={onClick}
      className={`inline-flex items-center justify-center gap-2 font-semibold text-body-md px-4 py-2.5 rounded-lg transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary disabled:cursor-not-allowed ${
        fullWidth ? "w-full" : ""
      } ${variants[variant]}`}
    >
      {children}
    </button>
  );
}