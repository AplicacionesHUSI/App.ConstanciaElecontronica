using System;
using System.Collections.Generic;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.model
{
    public class FirmaPaciente
    {
        public int IdAtencion { get; set; }
        public string tipo { get; set; }
        public string UsrWindowsReg { get; set; }
        public bool IndMismoPaciente { get; set; }
        public string NomResponsable { get; set; }
        public byte? IdTipoDocResponsable { get; set; }
        public string NumDocResponsable { get; set; }
        public string Parentesco { get; set; }
        public string Image { get; set; }
        public string Image2 { get; set; }

        public DateTime FecRegistro { get; set;}
        public string SingImage { get; set; }
    }
}
