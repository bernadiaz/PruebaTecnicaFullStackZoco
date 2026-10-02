import type { Asesor } from "../types";
import { http } from "./http";

export function getAsesores() {
  return http.get<Asesor[]>("/asesores");
}
