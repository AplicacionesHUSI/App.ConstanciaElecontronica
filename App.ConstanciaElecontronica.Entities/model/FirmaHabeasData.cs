using System;
using System.Collections.Generic;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.model
{
    public class FirmaHabeasData
    {
        public int IdFirma { get; set; }
        public int IdCliente { get; set; }
        public string UsrWindowsReg { get; set; }
        public bool IndMismoPaciente { get; set; }
        public string Responsable { get; set; }
        public int IdTipoDocResponsable { get; set; }
        public string NumDocResponsable { get; set; }
        public string Parentesco { get; set; }
        public DateTime FecRegistro { get; set; }        
     
        public string Image { get; set; }
        public string Tipo { get; set; }
        
        public DateTime FecRegistroImage { get; set; }
        public string SingImage { get; set; }
        public int Version { get; set; } = 2;
    }
}
