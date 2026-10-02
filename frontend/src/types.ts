export type EstadoCliente =
  | "Prospecto"
  | "Contactado"
  | "Interesado"
  | "NoInteresado"
  | "Cliente";

export type TipoContacto = "Llamada" | "WhatsApp" | "Correo" | "Reunion" | "Otro";

export interface Asesor {
  id: number;
  nombre: string;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface ClienteListItem {
  id: number;
  nombre: string;
  cuit: string;
  telefono: string;
  email: string | null;
  estado: EstadoCliente;
  estadoNombre: string;
  asesorId: number;
  asesorNombre: string;
  proximoContacto: string | null;
  fechaActualizacion: string;
  seguimientoVencido: boolean;
}

export interface ClienteDetail extends ClienteListItem {
  fechaCreacion: string;
}

export interface ClientePayload {
  nombre: string;
  cuit: string;
  telefono: string;
  email: string | null;
  estado: EstadoCliente;
  asesorId: number;
}

export interface Gestion {
  id: number;
  tipoContacto: TipoContacto;
  tipoContactoNombre: string;
  comentario: string;
  estadoResultante: EstadoCliente;
  estadoResultanteNombre: string;
  fechaGestion: string;
  proximoContacto: string | null;
}

export interface GestionPayload {
  tipoContacto: TipoContacto;
  comentario: string;
  estadoResultante: EstadoCliente;
  proximoContacto: string | null;
}

export interface DashboardResumen {
  totalClientes: number;
  prospectos: number;
  interesados: number;
  seguimientosVencidos: number;
}

export const ESTADOS: { value: EstadoCliente; label: string }[] = [
  { value: "Prospecto", label: "Prospecto" },
  { value: "Contactado", label: "Contactado" },
  { value: "Interesado", label: "Interesado" },
  { value: "NoInteresado", label: "No interesado" },
  { value: "Cliente", label: "Cliente" }
];

export const TIPOS_CONTACTO: { value: TipoContacto; label: string }[] = [
  { value: "Llamada", label: "Llamada" },
  { value: "WhatsApp", label: "WhatsApp" },
  { value: "Correo", label: "Correo" },
  { value: "Reunion", label: "Reunión" },
  { value: "Otro", label: "Otro" }
];
