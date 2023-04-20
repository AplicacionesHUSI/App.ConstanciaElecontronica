using System;
using System.Collections.Generic;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.model
{
    public class Instruccion
    {
        public int IdProceNoQX { get; set; }
        public int IdInforme { get; set; }
        public int IdUsuarioReg { get; set; }
        public DateTime FecRegistro { get; set; }
        public string Informe { get; set; }
        public string Personal { get; set; }
        public int? IdFirma { get; set; }
    }
}
