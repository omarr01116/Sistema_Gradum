import Sidebar from "../components/layout/Sidebar";
import Header from "../components/layout/Header";

export default function MainLayout({ children }) {
  return (
    <div className="flex h-screen overflow-hidden bg-background">
      <Sidebar />
      <div className="flex-1 ml-60 flex flex-col h-screen">
        <Header />
        <main className="flex-1 overflow-y-auto p-section-gap">
          <div className="max-w-[1440px] mx-auto w-full">{children}</div>
        </main>
      </div>
    </div>
  );
}