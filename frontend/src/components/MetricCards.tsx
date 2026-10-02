import type { DashboardResumen } from "../types";

const cards = [
  { key: "totalClientes", label: "Total de clientes" },
  { key: "prospectos", label: "Prospectos" },
  { key: "interesados", label: "Interesados" },
  { key: "seguimientosVencidos", label: "Seguimientos vencidos" }
] as const;

export function MetricCards({ resumen }: { resumen: DashboardResumen }) {
  return (
    <section className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
      {cards.map((card) => (
        <article
          key={card.key}
          className={`rounded-xl border bg-white p-5 shadow-sm ${
            card.key === "seguimientosVencidos" ? "border-rose-200" : "border-slate-200"
          }`}
        >
          <p className="text-sm text-slate-500">{card.label}</p>
          <p className="mt-2 text-3xl font-semibold">{resumen[card.key]}</p>
        </article>
      ))}
    </section>
  );
}
