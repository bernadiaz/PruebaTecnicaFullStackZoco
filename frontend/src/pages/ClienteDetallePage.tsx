import { FormEvent, useEffect, useState } from "react";
import { Link, useLocation, useParams } from "react-router-dom";
import { createGestion, getCliente, getGestiones } from "../api/clientes";
import { ApiError } from "../api/http";
import { Alert } from "../components/Alert";
import { EstadoBadge } from "../components/EstadoBadge";
import { FieldError } from "../components/FieldError";
import { formatDate, formatDateTime, toDateInputValue } from "../lib/format";
import type { ClienteDetail, EstadoCliente, Gestion, TipoContacto } from "../types";
import { ESTADOS, TIPOS_CONTACTO } from "../types";

export function ClienteDetallePage() {
  const { id } = useParams();
  const location = useLocation();
  const [cliente, setCliente] = useState<ClienteDetail | null>(null);
  const [gestiones, setGestiones] = useState<Gestion[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(
    location.state && typeof location.state === "object" && "created" in location.state
      ? "El cliente se creó correctamente."
      : null
  );
  const [saving, setSaving] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [form, setForm] = useState({
    tipoContacto: "Llamada" as TipoContacto,
    comentario: "",
    estadoResultante: "Contactado" as EstadoCliente,
    proximoContacto: ""
  });

  async function load() {
    if (!id) {
      return;
    }

    setLoading(true);
    setError(null);
    try {
      const [detalle, historial] = await Promise.all([
        getCliente(Number(id)),
        getGestiones(Number(id))
      ]);
      setCliente(detalle);
      setGestiones(historial);
      setForm((current) => ({ ...current, estadoResultante: detalle.estado }));
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo cargar el cliente.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id]);

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    if (!id) {
      return;
    }

    setSaving(true);
    setError(null);
    setSuccess(null);
    setErrors({});

    try {
      await createGestion(Number(id), {
        tipoContacto: form.tipoContacto,
        comentario: form.comentario,
        estadoResultante: form.estadoResultante,
        proximoContacto: form.proximoContacto ? form.proximoContacto : null
      });
      setForm((current) => ({ ...current, comentario: "", proximoContacto: "" }));
      setSuccess("La gestión se registró correctamente.");
      await load();
    } catch (err) {
      if (err instanceof ApiError) {
        setError(err.message);
        setErrors(
          Object.fromEntries(
            Object.entries(err.errors).map(([key, messages]) => [key.toLowerCase(), messages[0] ?? ""])
          )
        );
      } else {
        setError("No se pudo registrar la gestión.");
      }
    } finally {
      setSaving(false);
    }
  }

  if (loading) {
    return <Alert variant="info">Cargando detalle del cliente...</Alert>;
  }

  if (!cliente) {
    return error ? <Alert variant="error">{error}</Alert> : null;
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-start justify-between gap-4">
        <div>
          <Link to="/" className="text-sm text-indigo-700 hover:underline">
            Volver al listado
          </Link>
          <h1 className="mt-2 text-2xl font-semibold">{cliente.nombre}</h1>
          <div className="mt-2 flex flex-wrap items-center gap-2">
            <EstadoBadge estado={cliente.estado} label={cliente.estadoNombre} />
            {cliente.seguimientoVencido && (
              <span className="rounded-full bg-rose-100 px-2.5 py-1 text-xs font-medium text-rose-700">
                Seguimiento vencido
              </span>
            )}
          </div>
        </div>
        <Link
          to={`/clientes/${cliente.id}/editar`}
          className="rounded-md border border-slate-300 px-4 py-2 text-sm hover:bg-slate-50"
        >
          Editar cliente
        </Link>
      </div>

      {error && <Alert variant="error">{error}</Alert>}
      {success && <Alert variant="success">{success}</Alert>}

      <section className="grid gap-4 rounded-xl border border-slate-200 bg-white p-6 shadow-sm md:grid-cols-2">
        <p><span className="text-slate-500">CUIT:</span> {cliente.cuit}</p>
        <p><span className="text-slate-500">Teléfono:</span> {cliente.telefono}</p>
        <p><span className="text-slate-500">Correo:</span> {cliente.email ?? "—"}</p>
        <p><span className="text-slate-500">Asesor:</span> {cliente.asesorNombre}</p>
        <p><span className="text-slate-500">Próximo contacto:</span> {formatDate(cliente.proximoContacto)}</p>
        <p><span className="text-slate-500">Última actualización:</span> {formatDateTime(cliente.fechaActualizacion)}</p>
      </section>

      <section className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
        <h2 className="text-lg font-semibold">Nueva gestión</h2>
        <form onSubmit={onSubmit} className="mt-4 grid gap-4 md:grid-cols-2">
          <label className="text-sm">
            <span className="text-slate-600">Tipo de contacto</span>
            <select
              value={form.tipoContacto}
              onChange={(event) => setForm({ ...form, tipoContacto: event.target.value as TipoContacto })}
              className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2"
            >
              {TIPOS_CONTACTO.map((item) => (
                <option key={item.value} value={item.value}>
                  {item.label}
                </option>
              ))}
            </select>
            <FieldError message={errors.tipocontacto} />
          </label>
          <label className="text-sm">
            <span className="text-slate-600">Nuevo estado</span>
            <select
              value={form.estadoResultante}
              onChange={(event) => setForm({ ...form, estadoResultante: event.target.value as EstadoCliente })}
              className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2"
            >
              {ESTADOS.map((item) => (
                <option key={item.value} value={item.value}>
                  {item.label}
                </option>
              ))}
            </select>
            <FieldError message={errors.estadoresultante} />
          </label>
          <label className="text-sm md:col-span-2">
            <span className="text-slate-600">Comentario</span>
            <textarea
              value={form.comentario}
              onChange={(event) => setForm({ ...form, comentario: event.target.value })}
              rows={3}
              className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2"
            />
            <FieldError message={errors.comentario} />
          </label>
          <label className="text-sm">
            <span className="text-slate-600">Próximo contacto (opcional)</span>
            <input
              type="date"
              value={toDateInputValue(form.proximoContacto)}
              onChange={(event) => setForm({ ...form, proximoContacto: event.target.value })}
              className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2"
            />
          </label>
          <div className="flex items-end">
            <button
              type="submit"
              disabled={saving}
              className="rounded-md bg-indigo-600 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-500 disabled:opacity-60"
            >
              {saving ? "Registrando..." : "Registrar gestión"}
            </button>
          </div>
        </form>
      </section>

      <section className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
        <h2 className="text-lg font-semibold">Historial de gestiones</h2>
        {gestiones.length === 0 ? (
          <p className="mt-4 text-sm text-slate-500">Todavía no hay gestiones registradas.</p>
        ) : (
          <ol className="mt-4 space-y-4">
            {gestiones.map((gestion) => (
              <li key={gestion.id} className="border-l-2 border-slate-200 pl-4">
                <p className="text-sm text-slate-500">{formatDateTime(gestion.fechaGestion)}</p>
                <p className="mt-1 font-medium">
                  {gestion.tipoContactoNombre} · {gestion.estadoResultanteNombre}
                </p>
                <p className="mt-1 text-sm text-slate-700">{gestion.comentario}</p>
                {gestion.proximoContacto && (
                  <p className="mt-1 text-sm text-slate-500">
                    Próximo contacto: {formatDate(gestion.proximoContacto)}
                  </p>
                )}
              </li>
            ))}
          </ol>
        )}
      </section>
    </div>
  );
}
