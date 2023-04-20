using System;
using System.Collections.Generic;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.model
{
    public class Personal
    {
        public int IdPersonal { get; set; }

        public string NomPersonal { get; set; }

        public string ApePersonal { get; set; }

        public byte IdTipoDoc { get; set; }
        public string NumDocumento { get; set; }

        public string FirmaPersonal { get; set; }
        public string TarjetaProfesional { get; set; }

        public Int16 IdProfesion { get; set; }

        public Int16 IdUsuario { get; set; }
    }
}
