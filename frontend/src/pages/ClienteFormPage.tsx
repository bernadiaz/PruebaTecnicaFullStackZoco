import { FormEvent, useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { getAsesores } from "../api/asesores";
import { createCliente, getCliente, updateCliente } from "../api/clientes";
import { ApiError } from "../api/http";
import { Alert } from "../components/Alert";
import { FieldError } from "../components/FieldError";
import type { Asesor, EstadoCliente } from "../types";
import { ESTADOS } from "../types";

const emptyForm = {
  nombre: "",
  cuit: "",
  telefono: "",
  email: "",
  estado: "Prospecto" as EstadoCliente,
  asesorId: 0
};

export function ClienteFormPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = Boolean(id);
  const [form, setForm] = useState(emptyForm);
  const [asesores, setAsesores] = useState<Asesor[]>([]);
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    async function load() {
      setLoading(true);
      setError(null);
      try {
        const catalogo = await getAsesores();
        setAsesores(catalogo);
        if (id) {
          const cliente = await getCliente(Number(id));
          setForm({
            nombre: cliente.nombre,
            cuit: cliente.cuit,
            telefono: cliente.telefono,
            email: cliente.email ?? "",
            estado: cliente.estado,
            asesorId: cliente.asesorId
          });
        } else if (catalogo[0]) {
          setForm((current) => ({ ...current, asesorId: catalogo[0].id }));
        }
      } catch (err) {
        setError(err instanceof ApiError ? err.message : "No se pudo cargar el formulario.");
      } finally {
        setLoading(false);
      }
    }

    void load();
  }, [id]);

  function firstError(fieldErrors: Record<string, string[]>) {
    return Object.fromEntries(
      Object.entries(fieldErrors).map(([key, messages]) => [key.toLowerCase(), messages[0] ?? ""])
    );
  }

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    setSaving(true);
    setError(null);
    setSuccess(null);
    setErrors({});

    try {
      const payload = {
        nombre: form.nombre,
        cuit: form.cuit,
        telefono: form.telefono,
        email: form.email.trim() ? form.email.trim() : null,
        estado: form.estado,
        asesorId: Number(form.asesorId)
      };

      if (isEdit && id) {
        await updateCliente(Number(id), payload);
        setSuccess("El cliente se actualizó correctamente.");
      } else {
        const created = await createCliente(payload);
        setSuccess("El cliente se creó correctamente.");
        navigate(`/clientes/${created.id}`, { replace: true, state: { created: true } });
      }
    } catch (err) {
      if (err instanceof ApiError) {
        setError(err.message);
        setErrors(firstError(err.errors));
      } else {
        setError("No se pudo guardar el cliente.");
      }
    } finally {
      setSaving(false);
    }
  }

  if (loading) {
    return <Alert variant="info">Cargando formulario...</Alert>;
  }

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <div>
        <Link to="/" className="text-sm text-green-700 hover:underline">
          Volver al listado
        </Link>
        <h1 className="mt-2 text-2xl font-semibold">{isEdit ? "Editar cliente" : "Nuevo cliente"}</h1>
      </div>

      {error && <Alert variant="error">{error}</Alert>}
      {success && <Alert variant="success">{success}</Alert>}

      <form onSubmit={onSubmit} className="space-y-4 rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
        <label className="block text-sm">
          <span className="text-slate-600">Nombre o razón social</span>
          <input
            value={form.nombre}
            onChange={(event) => setForm({ ...form, nombre: event.target.value })}
            className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2"
          />
          <FieldError message={errors.nombre} />
        </label>
        <label className="block text-sm">
          <span className="text-slate-600">CUIT</span>
          <input
            value={form.cuit}
            onChange={(event) => setForm({ ...form, cuit: event.target.value })}
            className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2"
          />
          <FieldError message={errors.cuit} />
        </label>
        <label className="block text-sm">
          <span className="text-slate-600">Teléfono</span>
          <input
            value={form.telefono}
            onChange={(event) => setForm({ ...form, telefono: event.target.value })}
            className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2"
          />
          <FieldError message={errors.telefono} />
        </label>
        <label className="block text-sm">
          <span className="text-slate-600">Correo electrónico</span>
          <input
            type="email"
            value={form.email}
            onChange={(event) => setForm({ ...form, email: event.target.value })}
            className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2"
          />
          <FieldError message={errors.email} />
        </label>
        <label className="block text-sm">
          <span className="text-slate-600">Estado</span>
          <select
            value={form.estado}
            onChange={(event) => setForm({ ...form, estado: event.target.value as EstadoCliente })}
            className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2"
          >
            {ESTADOS.map((item) => (
              <option key={item.value} value={item.value}>
                {item.label}
              </option>
            ))}
          </select>
          <FieldError message={errors.estado} />
        </label>
        <label className="block text-sm">
          <span className="text-slate-600">Asesor responsable</span>
          <select
            value={form.asesorId}
            onChange={(event) => setForm({ ...form, asesorId: Number(event.target.value) })}
            className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2"
          >
            {asesores.map((asesor) => (
              <option key={asesor.id} value={asesor.id}>
                {asesor.nombre}
              </option>
            ))}
          </select>
          <FieldError message={errors.asesorid} />
        </label>
        <div className="flex justify-end gap-3 pt-2">
          <Link to="/" className="rounded-md px-4 py-2 text-sm text-slate-600 hover:bg-slate-100">
            Cancelar
          </Link>
          <button
            type="submit"
            disabled={saving}
            className="rounded-md bg-green-600 px-4 py-2 text-sm font-medium text-white hover:bg-green-700 disabled:opacity-60"
          >
            {saving ? "Guardando..." : "Guardar"}
          </button>
        </div>
      </form>
    </div>
  );
}
