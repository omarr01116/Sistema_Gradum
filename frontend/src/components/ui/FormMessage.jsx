export default function FormMessage({ type, message }) {
  if (!message) return null;

  const styles =
    type === "success"
      ? "bg-primary-fixed text-on-primary-fixed"
      : "bg-error-container text-on-error-container";

  return (
    <div className={`mt-4 p-3 rounded-lg font-medium text-body-sm ${styles}`}>
      {message}
    </div>
  );
}