import type {
  ClienteDetail,
  ClientePayload,
  EstadoCliente,
  Gestion,
  GestionPayload,
  PagedResult,
  ClienteListItem
} from "../types";
import { http } from "./http";

export function getClientes(search?: string, estado?: EstadoCliente | "", page = 1) {
  const params = new URLSearchParams();
  if (search?.trim()) {
    params.set("search", search.trim());
  }
  if (estado) {
    params.set("estado", estado);
  }
  params.set("page", String(page));
  return http.get<PagedResult<ClienteListItem>>(`/clientes?${params.toString()}`);
}

export function getCliente(id: number) {
  return http.get<ClienteDetail>(`/clientes/${id}`);
}

export function createCliente(payload: ClientePayload) {
  return http.post<ClienteDetail>("/clientes", payload);
}

export function updateCliente(id: number, payload: ClientePayload) {
  return http.put<ClienteDetail>(`/clientes/${id}`, payload);
}

export function getGestiones(clienteId: number) {
  return http.get<Gestion[]>(`/clientes/${clienteId}/gestiones`);
}

export function createGestion(clienteId: number, payload: GestionPayload) {
  return http.post<Gestion>(`/clientes/${clienteId}/gestiones`, payload);
}
