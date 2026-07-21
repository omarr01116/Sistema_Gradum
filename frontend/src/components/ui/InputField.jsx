import { useState } from "react";

// Reutilizable en cualquier formulario (Login, Cliente, Proyecto, etc).
// icon: nombre de un Material Symbol, ej. "person", "mail", "search".
export default function InputField({
  label,
  name,
  value,
  onChange,
  type = "text",
  placeholder,
  required = false,
  error = "",
  icon,
}) {
  const [showPassword, setShowPassword] = useState(false);
  const isPassword = type === "password";
  const resolvedType = isPassword && showPassword ? "text" : type;

  return (
    <div className="space-y-2">
      <label
        htmlFor={name}
        className="block font-bold text-body-sm text-on-surface"
      >
        {label}
      </label>
      <div className="relative">
        {icon && (
          <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <span className="material-symbols-outlined text-outline-variant text-[18px]">
              {icon}
            </span>
          </div>
        )}
        <input
          id={name}
          name={name}
          type={resolvedType}
          value={value}
          placeholder={placeholder}
          required={required}
          onChange={onChange}
          className={`block w-full py-2 border rounded bg-surface text-on-surface text-body-md
            outline-none focus:ring-1 transition-shadow placeholder:text-outline-variant
            ${icon ? "pl-10" : "pl-3"} ${isPassword ? "pr-10" : "pr-3"}
            ${
              error
                ? "border-error focus:ring-error focus:border-error"
                : "border-outline-variant focus:ring-primary focus:border-primary"
            }`}
        />
        {isPassword && (
          <button
            type="button"
            onClick={() => setShowPassword((prev) => !prev)}
            className="absolute inset-y-0 right-0 pr-3 flex items-center text-outline-variant hover:text-on-surface transition-colors"
          >
            <span className="material-symbols-outlined text-[18px]">
              {showPassword ? "visibility_off" : "visibility"}
            </span>
          </button>
        )}
      </div>
      {error && <p className="text-error text-body-sm">{error}</p>}
    </div>
  );
}