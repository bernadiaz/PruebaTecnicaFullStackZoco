namespace MiniCrm.Application.Common;

public static class SeguimientoRules
{
    public static bool EstaVencido(DateTime? proximoContacto, DateTime? fechaReferencia = null)
    {
        var hoy = (fechaReferencia ?? DateTime.Today).Date;
        return proximoContacto.HasValue && proximoContacto.Value.Date < hoy;
    }
}
