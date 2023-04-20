using App.ConstanciaElecontronica.Entities.model;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.Request
{
    public class RequestFirmaVacuna
    {
        public FirmaPaciente Firma { get; set; }
        public string LugarExpedicion { get; set; }
        public string FirmaVacunador { get; set; }
        public string FirmaResponsable { get; set; }
        public short IdVacuna { get; set; }
        public bool DosisUnica { get; set; }
        public bool Acepta { get; set; }
        public string RazonNoFirma { get; set; }
    }
}
