import type { DashboardResumen } from "../types";
import { http } from "./http";

export function getResumen() {
  return http.get<DashboardResumen>("/dashboard/resumen");
}
