using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Ejercicio1.Models;
using Ejercicio1.Models.Exportadores;

namespace Ejercicio1
{
    public partial class FormPrincipal : Form
        //utilice DateTime porque empece el proyecto en un .Net viejo
    {
        List<IExportable> exportables = new List<IExportable>();
        public FormPrincipal()
        {
            InitializeComponent();
        }
        OpenFileDialog openFileDialog1 = new OpenFileDialog();
        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
        private void FormPrincipal_Load(object sender, EventArgs e)
        {

        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            IExportable nuevo = null;

            string patente = tbPatente.Text;
            DateTime vencimiento = dtpVencimiento.Value.Date;
            double importe = Convert.ToDouble(tbImporte.Text);
            nuevo = new Multa(patente, vencimiento, importe);
            exportables.Sort();
            int idx = exportables.BinarySearch(nuevo);
            if (idx >= 0)
            { 
                Multa multa = exportables[idx] as Multa;
                multa.Importe += importe;
                if (multa.Vencimiento < ((Multa)nuevo).Vencimiento) ;
                multa.Vencimiento = ((Multa)nuevo).Vencimiento;
            }
            else
            {
                 exportables.Add(nuevo);
            }
            btnActualizar.PerformClick();
            tbPatente.Clear();
            dtpVencimiento.Value = DateTime.Now;
            tbImporte.Clear();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            lsbVer.Items.Clear();
            foreach(IExportable multa in exportables)
            {
                lsbVer.Items.Add(multa);
            }
        }

        private void btnImportar_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "(csv)|*.csv|(json)|*.json|(txt)|*.txt|(xml)|*.xml";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            { 
                string path = openFileDialog1.FileName;
                int tipo = openFileDialog1.FilterIndex;
                IExportador exportador = (new ExportadorFactory()).GetIsntance(tipo);
                FileStream fs = null;
                StreamReader sr = null;
                try 
                {
                    fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    sr = new StreamReader(fs);
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        string linea = sr.ReadLine();
                        IExportable nuevo = new Multa();
                        if (nuevo.Importar(linea, exportador) == true)
                        {
                            int idx = exportables.BinarySearch(nuevo);
                            if (idx >= 0)
                            { 
                                Multa multa = exportables[idx] as Multa;
                                multa.Importe += ((Multa)nuevo).Importe;
                                if (multa.Vencimiento < ((Multa)nuevo).Vencimiento);
                                multa.Vencimiento = ((Multa)nuevo).Vencimiento;
                            }
                            else
                            {
                                exportables.Add(nuevo);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error! "+ ex.Message);
                }
                finally
                {
                    if (sr != null) sr.Close();
                    if (fs != null) fs.Close();
                }
            }
            btnActualizar.PerformClick();
        }

        private void lsbVer_SelectedIndexChanged(object sender, EventArgs e)
        {
            Multa selected = lsbVer.SelectedItem as Multa;
            if (selected != null)
            {
                tbPatente.Text = selected.Patente;
                dtpVencimiento.Value = selected.Vencimiento;
                tbImporte.Text = selected.Importe.ToString("f2");
            }
        }
    }
}
