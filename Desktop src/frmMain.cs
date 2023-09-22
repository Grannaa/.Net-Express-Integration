using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class frmMain : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string url = "http://localhost:3000/";
        public frmMain()
        {
            InitializeComponent();
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                //Utilizar el metodo POST
                var values = new Dictionary<string, string>
                {
                    { "nota", this.txtResult.Text }
                };
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync(url, content);
                /* He escogido este switch porque cubre los dos errores mas comunes
                 * una alternativa seria en lugar de crear un mensaje para cada tipo de error
                 * crear un mensaje genérico de error para las respuestas que no sean las que quiera.
                 * */
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        System.Windows.Forms.MessageBox.Show("Servicio no encontrado. Inténtelo más tarde.", "Error");
                        break;
                    case HttpStatusCode.InternalServerError:
                        System.Windows.Forms.MessageBox.Show("Hubo un error con el servicio. Inténtelo más tarde.", "Error");
                        break;
                    default:
                        var responseString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"{responseString}");
                        break;
                }
                
            }
            catch (HttpRequestException ex)
            {
                System.Windows.Forms.MessageBox.Show("Servicio no disponible. Inténtelo más tarde.", "Error");
            }
            catch (WebException exc)
            {
                System.Windows.Forms.MessageBox.Show("Servicio no disponible. Inténtelo más tarde.", "Error");
            }

        }

        private async void btnLeer_Click(object sender, EventArgs e)
        {
            try
            {
                //Utilizar el metodo GET
                var response = await client.GetAsync(url + "leer");
                var responseString = await response.Content.ReadAsStringAsync();
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        System.Windows.Forms.MessageBox.Show("Servicio no encontrado. Inténtelo más tarde.", "Error");
                        break;
                    case HttpStatusCode.InternalServerError:
                        System.Windows.Forms.MessageBox.Show("Hubo un error con el servicio. Inténtelo más tarde.", "Error");
                        break;
                    default:
                        Console.WriteLine($"{responseString}");
                        //Esta linea es para que el string lo lea como un JSON
                        this.lblNota.Text = desglosarLectura(JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString));
                        break;
                }
                
            }
            catch (HttpRequestException ex)
            {
                System.Windows.Forms.MessageBox.Show("Servicio no disponible. Inténtelo más tarde.", "Error");
            }

        }

        private async void btnBorrar_Click(object sender, EventArgs e)
        {
            try
            {
                var response = await client.DeleteAsync(url + "delete");
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        System.Windows.Forms.MessageBox.Show("Servicio no encontrado. Inténtelo más tarde.", "Error");
                        break;
                    case HttpStatusCode.InternalServerError:
                        System.Windows.Forms.MessageBox.Show("Hubo un error con el servicio. Inténtelo más tarde.", "Error");
                        break;
                }
            }
            catch (HttpRequestException ex)
            {
                System.Windows.Forms.MessageBox.Show("Servicio no disponible. Inténtelo más tarde.", "Error");
            }

        }

        private string desglosarLectura(Dictionary<string, string> notas)
        {
            //Esta funcion sirve para transformar el JSON string a un formato mas estetico: Nota n : esta es la nota n
            string notaDesglosada = "";
            int cont = 0;

            while (notas.Count >= cont)
            {
                string numNota = "Nota " + cont;
                if (notas.ContainsKey(numNota))
                {
                    notaDesglosada += numNota + ": " + notas[numNota] + "\n";
                }
                
                cont++;
            }

            return notaDesglosada;
        }
    }
}
