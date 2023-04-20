using App.ConstanciaElecontronica.Entities.model;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.Request
{
    public class RequestFirmaEntregaInformacion
    {
        public FirmaPaciente Firma { get; set; }
        public Contacto Contacto { get; set; }
    }
}
