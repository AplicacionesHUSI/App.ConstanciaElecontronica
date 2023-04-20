using System;
using System.Collections.Generic;
using System.Text;


namespace App.ConstanciaElecontronica.Util
{
    public class Util
    {
        public static int GetIdDoc(string tipoDoc)
        {
            var IdTipoDoc = 0;
            switch (tipoDoc)
            {
                case "Tarjeta de Identidad":
                case "Colombian Identity card":                    
                    IdTipoDoc = 6;
                    break;
                    
                case "Civil registry":
                case "Registro Civil":                
                    IdTipoDoc = 4;
                    break;
                case "Cédula":
                case "Colombian ID":                    
                    IdTipoDoc = 2;
                    break;
                case "Cédula de Extranjería":
                case "Foreigner's identification card":                    
                    IdTipoDoc = 3;
                    break;
                case "Documento Extranjero":
                case "Foreign certificate":
                    IdTipoDoc = 9;
                    break;
                case "Pasaporte":
                case "Passport":
                    
                    IdTipoDoc = 5;
                    break;
                case "Carnet Diplomático":
                case "Diplomat Card":

                    IdTipoDoc = 11;
                    break;
                case "Salvo Conducto en Permanencia":
                case "Safe conduct in Permanence":                    
                    IdTipoDoc = 12;
                    break;
                case "Pasaporte de la ONU":
                case "ONU passport":
                    IdTipoDoc = 13;
                    break;
                case "Permiso Especial Permanencia":
                case "Special permission to reside":                    
                    IdTipoDoc = 14;
                    break;
                case "Permiso Protección Temporal":
                case "Temporary Protection Permit":
                    IdTipoDoc = 15;
                    break;
                case "Menor sin identificación":
                    IdTipoDoc = 7;
                    break;
                case "Adulto Sin Identificación":
                    IdTipoDoc = 8;
                    break;
                case "Nacido Vivo":
                    IdTipoDoc = 10;
                    break;
                default:
                    IdTipoDoc = 8;
                    break;

            }
            return IdTipoDoc;
        }
        public static string GetNameMenu(string rol) {

            var name = string.Empty;
            switch (rol)
            {
                case "HabeasData":
                    name = "Tratamiento de datos personales";
                    break;
                case "Instrucciones":
                    name = "Instrucciones ambulatorias";
                    break;
                case "CuidadosEnfermeria":
                    name = "Cuidados de enfermería";
                    break;
                case "TratamientoFarmacologico":
                    name = "Tratamiento farmacológico";
                    break;
                case "Inyectologia":
                    name = "Inyectología urgencias";
                    break;
                case "EntregaInformacion":
                    name = "Entrega Información";
                    break;
                case "VacunaCovid":
                    name = "Vacunación Covid 19";
                    break;
                case "CartaCompromisoCovid":
                    name = "Carta Compromiso Covid";
                    break;
                case "InstruccionesEgreso":
                    name = "Instrucciones de Egreso";
                    break;
                default:
                    name = "Desconocido";
                    break;

            }
            return name;
        }
    }
}



