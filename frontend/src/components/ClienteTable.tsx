import { Link } from "react-router-dom";
import { formatDate, formatDateTime } from "../lib/format";
import type { ClienteListItem } from "../types";
import { EstadoBadge } from "./EstadoBadge";

export function ClienteTable({
  clientes,
  onRestore,
  restoringId
}: {
  clientes: ClienteListItem[];
  onRestore?: (id: number) => void;
  restoringId?: number | null;
}) {
  if (clientes.length === 0) {
    return (
      <div className="rounded-xl border border-dashed border-slate-300 bg-white px-6 py-12 text-center text-slate-500">
        No hay clientes para los filtros seleccionados.
      </div>
    );
  }

  return (
    <div className="overflow-x-auto rounded-xl border border-slate-200 bg-white shadow-sm">
      <table className="min-w-full divide-y divide-slate-200 text-sm">
        <thead className="bg-slate-50 text-left text-slate-600">
          <tr>
            <th className="px-4 py-3 font-medium">Nombre</th>
            <th className="px-4 py-3 font-medium">CUIT</th>
            <th className="px-4 py-3 font-medium">Teléfono</th>
            <th className="px-4 py-3 font-medium">Correo</th>
            <th className="px-4 py-3 font-medium">Estado</th>
            <th className="px-4 py-3 font-medium">Asesor</th>
            <th className="px-4 py-3 font-medium">Próximo contacto</th>
            <th className="px-4 py-3 font-medium">Última actualización</th>
            {onRestore && <th className="px-4 py-3 font-medium">Acciones</th>}
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100">
          {clientes.map((cliente) => (
            <tr
              key={cliente.id}
              className={
                cliente.eliminado
                  ? "bg-slate-50"
                  : cliente.seguimientoVencido
                    ? "bg-rose-50"
                    : "bg-white"
              }
            >
              <td className="px-4 py-3">
                <Link to={`/clientes/${cliente.id}`} className="font-medium text-indigo-700 hover:underline">
                  {cliente.nombre}
                </Link>
                {cliente.eliminado && (
                  <span className="ml-2 rounded-full bg-slate-200 px-2 py-0.5 text-xs font-medium text-slate-700">
                    Dado de baja
                  </span>
                )}
                {!cliente.eliminado && cliente.seguimientoVencido && (
                  <span className="ml-2 rounded-full bg-rose-100 px-2 py-0.5 text-xs font-medium text-rose-700">
                    Vencido
                  </span>
                )}
              </td>
              <td className="px-4 py-3">{cliente.cuit}</td>
              <td className="px-4 py-3">{cliente.telefono}</td>
              <td className="px-4 py-3">{cliente.email ?? "—"}</td>
              <td className="px-4 py-3">
                <EstadoBadge estado={cliente.estado} label={cliente.estadoNombre} />
              </td>
              <td className="px-4 py-3">{cliente.asesorNombre}</td>
              <td className="px-4 py-3">{formatDate(cliente.proximoContacto)}</td>
              <td className="px-4 py-3">{formatDateTime(cliente.fechaActualizacion)}</td>
              {onRestore && (
                <td className="px-4 py-3">
                  {cliente.eliminado && (
                    <button
                      type="button"
                      disabled={restoringId === cliente.id}
                      onClick={() => onRestore(cliente.id)}
                      className="rounded-md border border-emerald-200 bg-emerald-50 px-3 py-1 text-xs font-medium text-emerald-800 hover:bg-emerald-100 disabled:opacity-60"
                    >
                      {restoringId === cliente.id ? "Reactivando..." : "Reactivar"}
                    </button>
                  )}
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
