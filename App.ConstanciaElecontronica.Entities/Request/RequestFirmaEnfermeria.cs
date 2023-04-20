using App.ConstanciaElecontronica.Entities.model;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.Request
{
    public class RequestFirmaEnfermeria
    {
        public FirmaPaciente Firma { get; set; }
        public string Cargo{ get; set; }
        public string FirmaEnfermeria { get; set; }
    }
}
