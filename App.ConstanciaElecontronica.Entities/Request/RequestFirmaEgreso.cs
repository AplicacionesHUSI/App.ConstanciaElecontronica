using App.ConstanciaElecontronica.Entities.model;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.Request
{
    public class RequestFirmaEgreso
    {
        public FirmaEgreso Firma { get; set; }
        public List<Instruccion> Instrucciones { get; set; }
    }
}
