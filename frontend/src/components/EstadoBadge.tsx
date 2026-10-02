import type { EstadoCliente } from "../types";

const styles: Record<EstadoCliente, string> = {
  Prospecto: "bg-slate-100 text-slate-700",
  Contactado: "bg-sky-100 text-sky-800",
  Interesado: "bg-amber-100 text-amber-800",
  NoInteresado: "bg-rose-100 text-rose-800",
  Cliente: "bg-emerald-100 text-emerald-800"
};

export function EstadoBadge({
  estado,
  label
}: {
  estado: EstadoCliente;
  label: string;
}) {
  return (
    <span className={`inline-flex rounded-full px-2.5 py-1 text-xs font-medium ${styles[estado]}`}>
      {label}
    </span>
  );
}
