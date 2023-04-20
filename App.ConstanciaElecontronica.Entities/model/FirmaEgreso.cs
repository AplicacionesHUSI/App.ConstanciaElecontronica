using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.model
{
    public class FirmaEgreso
    {
        public int IdFirma { get; set; }
        public string UsrWindowsReg { get; set; }
        public bool IndMismoPaciente { get; set; }
        [JsonProperty(PropertyName = "NomResponsable")]
        public string Responsable { get; set; }
        public int IdTipoDocResponsable { get; set; }
        public string NumDocResponsable { get; set; }
        public DateTime FecRegistro { get; set; }
        public string Atencion { get; set; }
     
        public string Image { get; set; }
        public string Parentesco { get; set; }
    }
}
