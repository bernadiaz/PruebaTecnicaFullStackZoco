import { FormEvent, useEffect, useState } from "react";
import { getClientes } from "../api/clientes";
import { getResumen } from "../api/dashboard";
import { ApiError } from "../api/http";
import { Alert } from "../components/Alert";
import { ClienteTable } from "../components/ClienteTable";
import { MetricCards } from "../components/MetricCards";
import type { ClienteListItem, DashboardResumen, EstadoCliente } from "../types";
import { ESTADOS } from "../types";

export function HomePage() {
  const [search, setSearch] = useState("");
  const [estado, setEstado] = useState<EstadoCliente | "">("");
  const [clientes, setClientes] = useState<ClienteListItem[]>([]);
  const [resumen, setResumen] = useState<DashboardResumen | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  async function load(nextSearch = search, nextEstado = estado) {
    setLoading(true);
    setError(null);
    try {
      const [lista, metrics] = await Promise.all([
        getClientes(nextSearch, nextEstado),
        getResumen()
      ]);
      setClientes(lista);
      setResumen(metrics);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo cargar el listado.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  function onSubmit(event: FormEvent) {
    event.preventDefault();
    void load();
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Seguimiento comercial</h1>
        <p className="mt-1 text-slate-600">
          Clientes, estados y próximos contactos en un mismo lugar.
        </p>
      </div>

      {resumen && <MetricCards resumen={resumen} />}

      <form onSubmit={onSubmit} className="flex flex-wrap items-end gap-3 rounded-xl border border-slate-200 bg-white p-4">
        <label className="flex min-w-64 flex-1 flex-col gap-1 text-sm">
          <span className="text-slate-600">Buscar</span>
          <input
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Nombre, CUIT o teléfono"
            className="rounded-md border border-slate-300 px-3 py-2"
          />
        </label>
        <label className="flex w-56 flex-col gap-1 text-sm">
          <span className="text-slate-600">Estado</span>
          <select
            value={estado}
            onChange={(event) => setEstado(event.target.value as EstadoCliente | "")}
            className="rounded-md border border-slate-300 px-3 py-2"
          >
            <option value="">Todos</option>
            {ESTADOS.map((item) => (
              <option key={item.value} value={item.value}>
                {item.label}
              </option>
            ))}
          </select>
        </label>
        <button
          type="submit"
          className="rounded-md bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-700"
        >
          Filtrar
        </button>
      </form>

      {error && <Alert variant="error">{error}</Alert>}
      {loading ? (
        <Alert variant="info">Cargando clientes...</Alert>
      ) : (
        <ClienteTable clientes={clientes} />
      )}
    </div>
  );
}
