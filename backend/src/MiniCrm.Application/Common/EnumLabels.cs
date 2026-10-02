using MiniCrm.Domain.Enums;

namespace MiniCrm.Application.Common;

public static class EnumLabels
{
    public static string For(EstadoCliente estado) => estado switch
    {
        EstadoCliente.Prospecto => "Prospecto",
        EstadoCliente.Contactado => "Contactado",
        EstadoCliente.Interesado => "Interesado",
        EstadoCliente.NoInteresado => "No interesado",
        EstadoCliente.Cliente => "Cliente",
        _ => estado.ToString()
    };

    public static string For(TipoContacto tipo) => tipo switch
    {
        TipoContacto.Llamada => "Llamada",
        TipoContacto.WhatsApp => "WhatsApp",
        TipoContacto.Correo => "Correo",
        TipoContacto.Reunion => "Reunión",
        TipoContacto.Otro => "Otro",
        _ => tipo.ToString()
    };
}
