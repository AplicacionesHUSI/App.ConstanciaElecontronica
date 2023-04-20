using System;
using System.Collections.Generic;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.model
{
    public class Patient
    {
        public int IdCliente { get; set; }
        public string NomCliente { get; set; }
        public string ApeCliente { get; set; }
        public byte IdTipoDoc { get; set; }
        public string TipoDoc { get; set; }
        public string NumDocumento { get; set; }
        public short IdSexo { get; set; }
        public short? IdFactorRh { get; set; }
        public short? IdGrupSangre { get; set; }
        public DateTime FecNacimiento { get; set; }
        public int? IdLugarNac { get; set; }
        public short IdEstadoCivil { get; set; }
        public int? IdLugarCliente { get; set; }
        public short? IdZona { get; set; }
        public string DirCasa { get; set; }
        public string DirOficina { get; set; }
        public string TelCasa { get; set; }
        public string TelOficina { get; set; }
        public short? IdOcupacion { get; set; }
        public short? IdAfiliaciontipo { get; set; }
        public byte? IdNivel { get; set; }
        public string NomPadre { get; set; }
        public string NomMadre { get; set; }
        public string NroHistoria { get; set; }
        public double? ValSaldo { get; set; }
        public int? IdReligion { get; set; }
        public bool IndHabilitado { get; set; }
        public int? IdLugarDocu { get; set; }
        public bool? IndMuerto { get; set; }
        public int? IdClienteMadre { get; set; }      
        public string CodDipoNac { get; set; }
        public string CodDipoCliente { get; set; }
        public string CodUsuario { get; set; }
        public int? Idtercero { get; set; }
        public string CorreoE { get; set; }
        public string TelCelular { get; set; }
        public bool? IndAuditoria { get; set; }
        public DateTime CreateDate { get; set; }
        public bool? IndPacExtranjero { get; set; }
        public bool? IndHabeasData { get; set; }
        public List<Atencion> Atenciones { get; set; }
        public string Eps { get; set; }

    }
}
