const ARGENTINA_TIME_ZONE = "America/Argentina/Buenos_Aires";

export function formatDate(value?: string | null) {
  if (!value) {
    return "—";
  }

  return new Date(value).toLocaleDateString("es-AR", {
    timeZone: ARGENTINA_TIME_ZONE
  });
}

export function formatDateTime(value?: string | null) {
  if (!value) {
    return "—";
  }

  return new Date(value).toLocaleString("es-AR", {
    timeZone: ARGENTINA_TIME_ZONE,
    dateStyle: "short",
    timeStyle: "short"
  });
}

export function toDateInputValue(value?: string | null) {
  if (!value) {
    return "";
  }

  return value.slice(0, 10);
}
