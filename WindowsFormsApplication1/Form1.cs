using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Drawing.Imaging;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Image image;
        
        public Form1()
        {
            InitializeComponent();
        }

        async void MakeRequest()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "afc64e5450324b0d9ad0a147faa4a43c");

            // Request parameters
            queryString["maxCandidates"] = "1";
            queryString["visualFeatures"] = "Categories";
            //queryString["details"] = "{string}";
            var uri = "https://api.projectoxford.ai/vision/v1.0/analyze?" + queryString;

            HttpResponseMessage response;

            // Request body
            ImageConverter _imageConverter = new ImageConverter();
            byte[] byteData = (byte[])_imageConverter.ConvertTo(image, typeof(byte[]));

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                textBox1.Text = await response.Content.ReadAsStringAsync();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MakeRequest();
            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            var location = "";
            ofd.Filter = "jpg (*.jpg)|*.jpg|bmp (*.bmp)|*.bmp|png (*.png)|*.png";

            if (ofd.ShowDialog() == DialogResult.OK && ofd.FileName.Length > 0)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Image = Image.FromFile(ofd.FileName);
                image = Image.FromFile(ofd.FileName);
                location = ofd.FileName;
                
                if (ImageFormat.Jpeg.Equals(image.RawFormat))
                {
                    textBox1.Text = "exito";
                }
                else if(ImageFormat.Png.Equals(image.RawFormat))
                {
                    textBox1.Text = "png";
                }
                else
                {
                    textBox1.Text = "falso";
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MakeRequest();
        }
    }
}