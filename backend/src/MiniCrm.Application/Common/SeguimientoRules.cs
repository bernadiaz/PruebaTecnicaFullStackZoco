namespace MiniCrm.Application.Common;

public static class SeguimientoRules
{
    public static bool EstaVencido(DateTime? proximoContacto, DateTime? fechaReferencia = null)
    {
        var hoy = (fechaReferencia ?? ArgentinaTime.Today).Date;
        return proximoContacto.HasValue && proximoContacto.Value.Date < hoy;
    }
}
