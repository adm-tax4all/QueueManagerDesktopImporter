using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Configuration;
using Newtonsoft.Json;

namespace Queue_Manager
{
    public partial class Form1 : Form
    {
        public static string __password = Properties.Settings.Default.Password;
        public static string __folderini = Properties.Settings.Default.PastaPendentes;
        public static string __folderfim = Properties.Settings.Default.PastaProcessado;
        public static string __foldererro = Properties.Settings.Default.PastaErro;
        public static string __URL = Properties.Settings.Default.URLQueueXML;
        public Form1()
        {

            InitializeComponent();
            tabControl1.Dock = DockStyle.Fill;
            //button2.Dock = DockStyle.Right;
            this.WindowState = FormWindowState.Maximized;

            LoadGrid();
            Fill();
        }
        public bool LoadSingleXML(string strXML, string path)
        {
            XDocument data = XDocument.Parse(strXML);
            //XDocument data = XDocument.Load(code);
            var ns = data.Root.Name.Namespace;
            try
            {
                var cteProc = data.Element(ns + "cteProc");
                var nfeProc = data.Element(ns + "nfeProc");
                if (cteProc == null && nfeProc == null)
                {
                    return true;
                }

                string xNome = "";
                string CNPJ = "";
                string nNF = "";
                string strdata = "";
                string ambiente = "";
                if (nfeProc == null)
                {
                    var CTe = cteProc.Element(ns + "CTe");
                    var infCte = CTe.Element(ns + "infCte");
                    var ide = infCte.Element(ns + "ide");
                    var emit = infCte.Element(ns + "emit");
                    nNF = ide.Descendants().SingleOrDefault(x => x.Name == ns + "nCT")?.Value.Trim();
                    CNPJ = emit.Descendants().SingleOrDefault(x => x.Name == ns + "CNPJ")?.Value.Trim();
                    xNome = emit.Descendants().SingleOrDefault(x => x.Name == ns + "xNome")?.Value.Trim();

                    string dhEmi = ide.Descendants().SingleOrDefault(x => x.Name == ns + "dhEmi")?.Value.Trim();
                    strdata = dhEmi.Substring(8, 2) + "/" + dhEmi.Substring(5, 2) + "/" + dhEmi.Substring(0, 4);
                    string tpAmb = ide.Descendants().SingleOrDefault(x => x.Name == ns + "tpAmb")?.Value.Trim();
                    ambiente = "";
                    switch (tpAmb)
                    {
                        case "1":
                            // código 1
                            ambiente = "Produção";
                            break;
                        case "2":
                            // código 2
                            ambiente = "Homologação";
                            break;
                    }
                }
                else
                {
                    var NFe = nfeProc.Element(ns + "NFe");
                    var infNFe = NFe.Element(ns + "infNFe");
                    var ide = infNFe.Element(ns + "ide");
                    var emit = infNFe.Element(ns + "emit");
                    nNF = ide.Descendants().SingleOrDefault(x => x.Name == ns + "nNF")?.Value.Trim();
                    CNPJ = emit.Descendants().SingleOrDefault(x => x.Name == ns + "CNPJ")?.Value.Trim();
                    xNome = emit.Descendants().SingleOrDefault(x => x.Name == ns + "xNome")?.Value.Trim();

                    string dhEmi = ide.Descendants().SingleOrDefault(x => x.Name == ns + "dhEmi")?.Value.Trim();
                    strdata = dhEmi.Substring(8, 2) + "/" + dhEmi.Substring(5, 2) + "/" + dhEmi.Substring(0, 4);
                    string tpAmb = ide.Descendants().SingleOrDefault(x => x.Name == ns + "tpAmb")?.Value.Trim();
                    ambiente = "";
                    switch (tpAmb)
                    {
                        case "1":
                            // código 1
                            ambiente = "Produção";
                            break;
                        case "2":
                            // código 2
                            ambiente = "Homologação";
                            break;
                    }
                }



                LoadRow(xNome, CNPJ, nNF, strdata, ambiente, path);

            }
            catch
            { }
            return true;
        }
        public void LoadRow(string razao, string cnpj, string nf, string data, string amb, string caminho)
        {
            dataGridView1.Rows
                            .Add(new object[] { "F", razao, cnpj, nf, data, amb, caminho, "Pendente" });


        }
        public void SaveGrid()
        {
            //colocar pasta Tempo como Hidden
            string file = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + @"\Temp\temp.xml";


            DirectoryInfo di = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory.ToString() + @"\Temp");
            di.Create();
            di.Attributes |= FileAttributes.Hidden;

            using (BinaryWriter bw = new BinaryWriter(File.Open(file, FileMode.Create)))
            {
                bw.Write(dataGridView1.Columns.Count);
                bw.Write(dataGridView1.Rows.Count);
                foreach (DataGridViewRow dgvR in dataGridView1.Rows)
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; ++j)
                    {
                        object val = dgvR.Cells[j].Value;
                        if (val == null)
                        {
                            bw.Write(false);
                            bw.Write(false);
                        }
                        else
                        {
                            bw.Write(true);
                            bw.Write(val.ToString());
                        }
                    }
                }
            }


        }
        public void LoadGrid()
        {

            dataGridView1.Rows.Clear();
            string file = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + @"\Temp\temp.xml";
            if (File.Exists(file))
            {
                using (BinaryReader bw = new BinaryReader(File.Open(file, FileMode.Open)))
                {
                    int n = bw.ReadInt32();
                    int m = bw.ReadInt32();
                    for (int i = 0; i < m; ++i)
                    {
                        dataGridView1.Rows.Add();
                        for (int j = 0; j < n; ++j)
                        {
                            if (bw.ReadBoolean())
                            {
                                dataGridView1.Rows[i].Cells[j].Value = bw.ReadString();
                            }
                            else bw.ReadBoolean();
                        }
                    }
                }
            }


        }
        public void Fill()
        {
            try
            {

                foreach (DataGridViewColumn GridCol in dataGridView1.Columns)
                {
                    for (int j = 0; j < GridCol.DataGridView.ColumnCount; j++)
                    {
                        GridCol.DataGridView.Columns[j].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        GridCol.DataGridView.Columns[j].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        GridCol.DataGridView.Columns[j].FillWeight = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #region Buttons
        private void btn_select_Click(object sender, EventArgs e)
        {

            this.openFileDialog1.InitialDirectory = __folderini;
            this.openFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";

            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.Title = "Arquivos NFe";

            DialogResult dr = this.openFileDialog1.ShowDialog();

            if (dr != DialogResult.Cancel)
            {
                this.dataGridView1.DataSource = null;
                this.dataGridView1.Rows.Clear();

                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    // Read the files
                    foreach (String file in openFileDialog1.FileNames)
                    {
                        LoadSingleXML(File.ReadAllText(file), file);
                    }

                }
                SaveGrid();
            }

            Fill();

        }
        private void btn_consult_Click(object sender, EventArgs e)
        {
            // Set Token
            QM.QueueManager.settokenQM();
            int i = 0;
            try
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string _status = row.Cells["Status"].Value.ToString();

                    if (_status == "Em Processamento")
                    {

                        string URL = row.Cells["colKey"].Value.ToString();
                        string status = "";
                        string errCode = "";
                        string msg = "";
                        QM.QueueManager.getstatus(URL, out status, out errCode, out msg);

                        if (status == "Done")
                        {
                            if (errCode == "0")
                            {
                                string Caminho = row.Cells["ColCaminho"].Value.ToString();
                                row.Cells["status"].Value = "Sucesso";
                                row.Cells["Msg"].Value = "Importação Realizada";

                                string destname = "";

                                string filename = Path.GetFileName(Caminho);
                                destname = __folderfim + filename;

                                if (Caminho != destname)
                                {
                                    if (File.Exists(destname))
                                    {
                                        File.Delete(destname);
                                    }
                                    File.Move(Caminho, destname);
                                }
                                i++;
                            }


                            if (errCode != "0")
                            {
                                string Caminho = row.Cells["ColCaminho"].Value.ToString();
                                row.Cells["status"].Value = "Erro";
                                row.Cells["Msg"].Value = msg;

                                string destname = "";
                                string filename = Path.GetFileName(Caminho);
                                destname = __foldererro + filename;

                                if (Caminho != destname)
                                {
                                    if (File.Exists(destname))
                                    {
                                        File.Delete(destname);
                                    }
                                    File.Move(Caminho, destname);
                                }
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                SaveGrid();
            }
            SaveGrid();
            MessageBox.Show(i.ToString() + " XML(s) consultado(s) com Sucesso");
        }
        private void btn_check_Click(object sender, EventArgs e)
        {

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[0];
                chk.Value = "T";
            }
        }
        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btn_process_Click(object sender, EventArgs e)
        {
            SendBatch();

            //// Set Token
            //QM.QueueManager.settokenQM();
            //int i = 0;
            //try
            //{
            //    foreach (DataGridViewRow row in dataGridView1.Rows)
            //    {
            //        string _status = row.Cells["ColCheck"].Value.ToString();
            //        string _check = row.Cells["Status"].Value.ToString();
            //        if (_status == "T" & (_check == "Pendente" || _check == "Erro"))
            //        {

            //            string Caminho = row.Cells["ColCaminho"].Value.ToString();

            //            StreamReader reader = new StreamReader(Caminho);

            //            string content = reader.ReadToEnd();
            //            reader.Close();
            //            string status = "";
            //            string link = "";
            //            string msg = "";
            //            QM.QueueManager.sendsingleXML(content, out status, out link, out msg);

            //            if (status == "True")
            //            {
            //                row.Cells["status"].Value = "Em Processamento";
            //                row.Cells["colKey"].Value = link;

            //                string destname = Caminho.Replace(__folderini, __folderfim);


            //                i++;
            //            }
            //            else
            //            {
            //                row.Cells["status"].Value = "Pendente";
            //                row.Cells["Msg"].Value = link + " - " + msg;

            //                string destname = Caminho.Replace(__folderini, __foldererro);

            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //    SaveGrid();
            //}
            //SaveGrid();
            //MessageBox.Show(i.ToString() + " XML(s) enviado(s) com Sucesso");
        }

        private void SendBatch()
        {
            var cont = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string _status = row.Cells["ColCheck"].Value.ToString();
                if (_status == "T")
                    cont++;
            }

            if (cont <= 50)
            {
                // Set Token
                QM.QueueManager.settokenQM();
                int i = 0;
                try
                {
                    var im = new QM.XmlImporter();
                    im.Log = true;
                    im.Xmls = new List<string>();

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        string _status = row.Cells["ColCheck"].Value.ToString();
                        string _check = row.Cells["Status"].Value.ToString();
                        if (_status == "T" & (_check == "Pendente" || _check == "Erro"))
                        {
                            string Caminho = row.Cells["ColCaminho"].Value.ToString();  

                            var xml = System.IO.File.ReadAllText(Caminho, System.Text.UTF8Encoding.UTF8);
                            im.Xmls.Add(xml);                           
                        }
                    }

                    string status = "";
                    string link = "";
                    string msg = "";
                    string output = JsonConvert.SerializeObject(im);

                    QM.QueueManager.SendBatchXML(output, out status, out link, out msg);

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        string _status = row.Cells["ColCheck"].Value.ToString();                  

                        if (status == "True" && _status == "T")
                        {
                            row.Cells["status"].Value = "Em Processamento";
                            row.Cells["colKey"].Value = link;
                            i++;
                        }
                        else if (_status == "T")
                        {
                            row.Cells["status"].Value = "Pendente";
                            row.Cells["Msg"].Value = link + " - " + msg;
                        }
                    }   
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    SaveGrid();
                }
                SaveGrid();
                MessageBox.Show(i.ToString() + " XML(s) enviado(s) com Sucesso");
            }
            else
                MessageBox.Show("Limite máximo de envio são 50 xml's");
        }

        #endregion
    }
}
