export default function PageContainer({ title, subtitle, children }) {
  return (
    <section>
      <div className="mb-container-margin">
        <h2 className="font-bold text-display-lg text-on-background">{title}</h2>
        {subtitle && (
          <p className="text-body-md text-secondary mt-1">{subtitle}</p>
        )}
      </div>
      {children}
    </section>
  );
}