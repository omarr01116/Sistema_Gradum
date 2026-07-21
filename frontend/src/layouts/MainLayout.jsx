import Sidebar from "../components/layout/Sidebar";
import Header from "../components/layout/Header";

export default function MainLayout({ children }) {
  return (
    <div className="min-h-screen bg-slate-50 font-sans text-gray-800 flex">
      <Sidebar />
      <div className="flex-1 ml-64 flex flex-col min-h-screen">
        <Header />
        <main className="flex-1 p-8 overflow-y-auto">
          <div className="w-full mx-auto max-w-7xl">
             {children}
          </div>
        </main>
      </div>
    </div>
  );
}