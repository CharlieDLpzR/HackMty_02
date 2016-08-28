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
using kMeans;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace WindowsFormsApplication1
{

    public partial class Form1 : Form
    {
        Image[] image = new Image[10];
        Dictionary<string, double>[] imageDetails;
        string[] categoryArr = new string[86];
        string[] images;
        int imageUpIndex = 0;
        int imageSendIndex = 0;
        kMeansAlg kProcess = new kMeansAlg();
        int pos;

        public Form1()
        {
            InitializeComponent();
            string line;
            int count = 0;
            StreamReader file = new StreamReader(@"../../Categories.txt");
            while ((line = file.ReadLine()) != null)
            {
                categoryArr[count] = line;
                count++;
            }
            file.Close();
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
            byte[] byteData = (byte[])_imageConverter.ConvertTo(image[pos], typeof(byte[]));
            imageSendIndex++;

            Console.WriteLine("ENTRA");

            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                images[pos] = await response.Content.ReadAsStringAsync();

                JObject json = new JObject();
                json = JObject.Parse(images[pos]);
                Console.WriteLine((json));
                imageDetails[pos] = new Dictionary<string, double>();
                for (int i = 0; i < json["categories"].Count(); i++)
                {
                    imageDetails[pos][(string)json["categories"][i]["name"]] = (double)(json["categories"][i]["score"]);

                    //Console.WriteLine(json["categories"]);
                }



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

                dataGridView1.Rows.Add();
                dataGridView1.Rows[imageUpIndex].Cells[0].Value = new Bitmap(ofd.FileName);
                ((DataGridViewImageColumn)dataGridView1.Columns[0]).ImageLayout = DataGridViewImageCellLayout.Zoom;
                dataGridView1.Rows[imageUpIndex].Height = 100;
                image[imageUpIndex] = Image.FromFile(ofd.FileName);
                location = ofd.FileName;


                if (ImageFormat.Jpeg.Equals(image[imageUpIndex].RawFormat))
                {
                    textBox1.Text = "exito";
                }
                else if (ImageFormat.Png.Equals(image[imageUpIndex].RawFormat))
                {
                    textBox1.Text = "png";
                }
                else
                {
                    textBox1.Text = "falso";
                }
                imageUpIndex++;

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            imageDetails = new Dictionary<string, double>[imageUpIndex];
            images = new string[imageUpIndex];
            for (int i = 0; i < imageUpIndex; i++)
            {
                pos = i;
                MakeRequest();
            }

            
            

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var dgv = new DataGridView();
            dgv.RowTemplate.MinimumHeight = 100;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int[] c = new int[imageUpIndex];

            for(int i = 0; i < imageUpIndex; i++)
            {
                Console.WriteLine(imageDetails[i]);
            }

            c = kProcess.kMeans(categoryArr, imageDetails, 15, 2);

            Console.WriteLine("Ends K Means");

            for (int i = 0; i < imageUpIndex; i++)
            {
                Console.WriteLine(c[i]);
            }
        }
    }
}

