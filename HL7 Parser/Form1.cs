using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HL7_Parser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // Başlık sütunlarını ekle
            dataGridView1.Columns.Add("KeyColumn", "Key");
            dataGridView1.Columns.Add("ValueColumn", "Value");

            // İstenirse görsel ayarlamalar da yapılabilir (örneğin, başlıkları kalın yapmak)
            dataGridView1.Columns["KeyColumn"].DefaultCellStyle.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            // HL7 Parser nesnesini oluştur
            var parser = new HL7Parser(richTextBox1.Text);
            var parsedData = parser.SegmentAnalyze();

            // DataGridView'e veriyi ekle
            dataGridView1.Rows.Clear(); // Önceki verileri temizle

            foreach (var item in parsedData)
            {
                dataGridView1.Rows.Add(item.Key, item.Value);
            }
        }
    }
}
