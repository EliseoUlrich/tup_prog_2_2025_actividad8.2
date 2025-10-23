using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio1.Models.Exportadores
{
    public class CSVExportador: IExportador
    {
        public string Exportar(Multa m)
        {
            return $"{m.Patente};{m.Vencimiento:dd/MM/yyyy};{m.Importe:f2}";
        }
        public bool Importar(string data, Multa m)
        {
            string[] campos = data.Split(';');
            if (campos.Length != 3) return false;
            string patente = campos[0];
            DateTime vencimiento = DateTime.ParseExact(campos[1], "dd/MM/yyyy", null);
            double importe = double.Parse(campos[2]);
            m.Patente = patente;
            m.Vencimiento = vencimiento;
            m.Importe = importe;
            return true;
        }
    }
}
