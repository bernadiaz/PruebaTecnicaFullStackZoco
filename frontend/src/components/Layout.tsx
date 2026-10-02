import { Link, NavLink, Outlet } from "react-router-dom";

export function Layout() {
  return (
    <div className="min-h-screen">
      <header className="border-b border-slate-200 bg-white">
        <div className="mx-auto flex max-w-6xl items-center justify-between px-6 py-4">
          <Link to="/" className="text-lg font-semibold tracking-tight text-slate-900">
            Mini CRM 
          </Link>
          <nav className="flex items-center gap-3">
            <NavLink
              to="/"
              className={({ isActive }) =>
                `rounded-md px-3 py-2 text-sm ${isActive ? "bg-slate-100 font-medium" : "text-slate-600 hover:bg-slate-50"}`
              }
            >
              Clientes
            </NavLink>
            <Link
              to="/clientes/nuevo"
              className="rounded-md bg-green-600 px-3 py-2 text-sm font-medium text-white hover:bg-green-700"
            >
              Nuevo cliente
            </Link>
          </nav>
        </div>
      </header>
      <main className="mx-auto max-w-6xl px-6 py-8">
        <Outlet />
      </main>
    </div>
  );
}
