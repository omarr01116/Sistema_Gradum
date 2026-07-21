export default function Card({ children, className = "" }) {
  return (
    <div className={`bg-white border border-gray-200 shadow-sm rounded-xl ${className}`}>
      {children}
    </div>
  );
}
