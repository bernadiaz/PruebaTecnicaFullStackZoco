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

export function getClientes(
  search?: string,
  estado?: EstadoCliente | "",
  asesorId?: number | "",
  page = 1,
  soloEliminados = false
) {
  const params = new URLSearchParams();
  if (search?.trim()) {
    params.set("search", search.trim());
  }
  if (estado) {
    params.set("estado", estado);
  }
  if (asesorId) {
    params.set("asesorId", String(asesorId));
  }
  params.set("page", String(page));
  if (soloEliminados) {
    params.set("soloEliminados", "true");
  }
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

export function deleteCliente(id: number) {
  return http.delete(`/clientes/${id}`);
}

export function restoreCliente(id: number) {
  return http.post<ClienteDetail>(`/clientes/${id}/restaurar`, {});
}

export function getGestiones(clienteId: number) {
  return http.get<Gestion[]>(`/clientes/${clienteId}/gestiones`);
}

export function createGestion(clienteId: number, payload: GestionPayload) {
  return http.post<Gestion>(`/clientes/${clienteId}/gestiones`, payload);
}
