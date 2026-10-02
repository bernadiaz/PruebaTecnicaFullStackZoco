namespace MiniCrm.Application.DTOs;

public record DashboardResumenDto(
    int TotalClientes,
    int Prospectos,
    int Interesados,
    int SeguimientosVencidos);
