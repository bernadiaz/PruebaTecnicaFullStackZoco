import { Link } from "react-router-dom";
import { formatDate, formatDateTime } from "../lib/format";
import type { ClienteListItem } from "../types";
import { EstadoBadge } from "./EstadoBadge";

export function ClienteTable({ clientes }: { clientes: ClienteListItem[] }) {
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
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100">
          {clientes.map((cliente) => (
            <tr
              key={cliente.id}
              className={cliente.seguimientoVencido ? "bg-rose-50" : "bg-white"}
            >
              <td className="px-4 py-3">
                <Link to={`/clientes/${cliente.id}`} className="font-medium text-indigo-700 hover:underline">
                  {cliente.nombre}
                </Link>
                {cliente.seguimientoVencido && (
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
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
