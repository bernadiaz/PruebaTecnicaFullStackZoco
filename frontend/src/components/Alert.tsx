import type { ReactNode } from "react";

type AlertVariant = "success" | "error" | "info";

const styles: Record<AlertVariant, string> = {
  success: "border-emerald-200 bg-emerald-50 text-emerald-800",
  error: "border-rose-200 bg-rose-50 text-rose-800",
  info: "border-sky-200 bg-sky-50 text-sky-800"
};

export function Alert({
  variant,
  children
}: {
  variant: AlertVariant;
  children: ReactNode;
}) {
  return (
    <div className={`rounded-md border px-4 py-3 text-sm ${styles[variant]}`}>
      {children}
    </div>
  );
}
