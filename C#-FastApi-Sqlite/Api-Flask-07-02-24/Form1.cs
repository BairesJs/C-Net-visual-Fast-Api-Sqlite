using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace Api_Flask_07_02_24
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string postDataUrl = "http://127.0.0.1:8000/datos/";
                var newData = new
                {
                    nombre = textBox1.Text,
                    direccion = textBox2.Text,
                    telefono = textBox3.Text,
                    email = textBox4.Text
                };
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(newData);

                using (var httpClient = new HttpClient())
                {
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    var response = httpClient.PostAsync(postDataUrl, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Datos insertados correctamente");
                    }
                    else
                    {
                        MessageBox.Show("Error al insertar datos. Código de estado: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar datos en el servidor: " + ex.Message);
            }

            ///////////////////////////////////////////////////////////

            try
            {
                string nombreBuscado = ""; // Obtener el texto ingresado en textBox6 para la búsqueda por nombre

                // Realizar la solicitud GET para obtener los datos más recientes
                string getDataUrl = "http://127.0.0.1:8000/datos";

                using (var httpClient = new HttpClient())
                {
                    var getDataResponse = httpClient.GetAsync(getDataUrl).Result;

                    if (getDataResponse.IsSuccessStatusCode)
                    {
                        string responseData = getDataResponse.Content.ReadAsStringAsync().Result;
                        Newtonsoft.Json.Linq.JObject result = Newtonsoft.Json.Linq.JObject.Parse(responseData);
                        List<List<object>> dataList = result["datos"].ToObject<List<List<object>>>();

                        // Filtrar los datos por nombre en tiempo real mientras el usuario escribe
                        List<List<object>> resultadosBusqueda = dataList
                            .Where(d => d[1].ToString().IndexOf(nombreBuscado, StringComparison.OrdinalIgnoreCase) >= 0)
                            .ToList();

                        // Crear DataTable y agregar columnas con nombres específicos
                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("Id");
                        dataTable.Columns.Add("Nombre");
                        dataTable.Columns.Add("Dirección");
                        dataTable.Columns.Add("Teléfono");
                        dataTable.Columns.Add("Email");

                        // Agregar filas al DataTable
                        foreach (var dataItem in resultadosBusqueda)
                        {
                            dataTable.Rows.Add(dataItem.ToArray());
                        }

                        // Actualizar el DataSource del DataGridView en el hilo de la interfaz de usuario
                        dataGridView1.Invoke((MethodInvoker)delegate {
                            dataGridView1.DataSource = dataTable;
                        });
                    }
                    else
                    {
                        MessageBox.Show("Error al obtener los datos. Código de estado: " + getDataResponse.StatusCode);
                    }
                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox3.Text = "";
                    textBox4.Text = "";

                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    textBox4.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar la búsqueda: " + ex.Message);
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Deshabilitar controles y ocultar TextBox5
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            button1.Enabled = false;
            textBox5.Visible = true;

            try
            {
                // Realizar la solicitud GET para obtener los datos más recientes
                string getDataUrl = "http://127.0.0.1:8000/datos/";

                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage getDataResponse = httpClient.GetAsync(getDataUrl).Result;

                    if (getDataResponse.IsSuccessStatusCode)
                    {
                        string responseData = getDataResponse.Content.ReadAsStringAsync().Result;
                        Newtonsoft.Json.Linq.JObject result = Newtonsoft.Json.Linq.JObject.Parse(responseData);
                        List<List<object>> dataList = result["datos"].ToObject<List<List<object>>>();

                        // Verificar si hay datos en dataList
                        if (dataList.Count > 0)
                        {
                            // Limpiar el DataGridView
                            dataGridView1.Rows.Clear();
                            dataGridView1.Columns.Clear();

                            // Crear DataTable y agregar columnas
                            DataTable dataTable = new DataTable();
                            foreach (string columnName in new[] { "id", "nombre", "direccion", "telefono", "email" })
                            {
                                dataTable.Columns.Add(columnName);
                            }

                            // Agregar filas al DataTable
                            foreach (List<object> dataItem in dataList)
                            {
                                dataTable.Rows.Add(dataItem.ToArray());
                            }

                            // Actualizar el DataSource del DataGridView en el hilo de la interfaz de usuario
                            dataGridView1.Invoke((MethodInvoker)delegate
                            {
                                dataGridView1.DataSource = dataTable;

                                // Mostrar los encabezados de las columnas
                                foreach (DataGridViewColumn column in dataGridView1.Columns)
                                {
                                    column.HeaderText = column.Name;
                                }
                            });
                        }
                        else
                        {
                            MessageBox.Show("No se encontraron datos para actualizar.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error al obtener los datos para actualizar. Código de estado: " + getDataResponse.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener los datos del servidor: " + ex.Message);
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Realizar la solicitud GET para obtener los datos más recientes
            string getDataUrl = "http://127.0.0.1:8000/datos/";

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage getDataResponse = httpClient.GetAsync(getDataUrl).Result; // Síncrono

                if (getDataResponse.IsSuccessStatusCode)
                {
                    string responseData = getDataResponse.Content.ReadAsStringAsync().Result; // Síncrono
                    Newtonsoft.Json.Linq.JObject result = Newtonsoft.Json.Linq.JObject.Parse(responseData);
                    List<List<object>> dataList = result["datos"].ToObject<List<List<object>>>();

                    // Verificar si hay datos en dataList
                    if (dataList.Count > 0)
                    {
                        // Limpiar el DataGridView
                        dataGridView1.Invoke((MethodInvoker)delegate
                        {
                            dataGridView1.DataSource = null;
                            dataGridView1.Rows.Clear();
                            dataGridView1.Columns.Clear();
                        });

                        // Crear DataTable y agregar columnas
                        DataTable dataTable = new DataTable();
                        foreach (string columnName in new[] { "id", "nombre", "direccion", "telefono", "email" })
                        {
                            dataTable.Columns.Add(columnName);
                        }

                        // Agregar filas al DataTable
                        foreach (List<object> dataItem in dataList)
                        {
                            dataTable.Rows.Add(dataItem.ToArray());
                        }

                        // Actualizar el DataSource del DataGridView en el hilo de la interfaz de usuario
                        dataGridView1.Invoke((MethodInvoker)delegate
                        {
                            dataGridView1.DataSource = dataTable;

                            // Mostrar los encabezados de las columnas
                            foreach (DataGridViewColumn column in dataGridView1.Columns)
                            {
                                column.HeaderText = column.Name;
                            }
                        });
                    }
                    else
                    {
                        MessageBox.Show("No se encontraron datos para actualizar.");
                    }
                }
                else
                {
                    MessageBox.Show("Error al obtener los datos para actualizar. Código de estado: " + getDataResponse.StatusCode);
                }
            }


            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = true;
            textBox1.Focus();
            button1.Enabled = true;
            button2.Enabled = false;

        }


        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string nombreBuscado = textBox6.Text.Trim(); // Obtener el texto ingresado en textBox6 para la búsqueda por nombre

                // Realizar la solicitud GET para obtener los datos más recientes
                string getDataUrl = "http://127.0.0.1:8000/datos";

                using (var httpClient = new HttpClient())
                {
                    var getDataResponse = httpClient.GetAsync(getDataUrl).Result;

                    if (getDataResponse.IsSuccessStatusCode)
                    {
                        string responseData = getDataResponse.Content.ReadAsStringAsync().Result;
                        Newtonsoft.Json.Linq.JObject result = Newtonsoft.Json.Linq.JObject.Parse(responseData);
                        List<List<object>> dataList = result["datos"].ToObject<List<List<object>>>();

                        // Filtrar los datos por nombre en tiempo real mientras el usuario escribe
                        List<List<object>> resultadosBusqueda = dataList
                            .Where(d => d[1].ToString().IndexOf(nombreBuscado, StringComparison.OrdinalIgnoreCase) >= 0)
                            .ToList();

                        // Crear DataTable y agregar columnas con nombres específicos
                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("Id");
                        dataTable.Columns.Add("Nombre");
                        dataTable.Columns.Add("Dirección");
                        dataTable.Columns.Add("Teléfono");
                        dataTable.Columns.Add("Email");

                        // Agregar filas al DataTable
                        foreach (var dataItem in resultadosBusqueda)
                        {
                            dataTable.Rows.Add(dataItem.ToArray());
                        }

                        // Actualizar el DataSource del DataGridView en el hilo de la interfaz de usuario
                        dataGridView1.Invoke((MethodInvoker)delegate {
                            dataGridView1.DataSource = dataTable;
                        });
                    }
                    else
                    {
                        MessageBox.Show("Error al obtener los datos. Código de estado: " + getDataResponse.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar la búsqueda: " + ex.Message);
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                int dato_id;
                if (!int.TryParse(textBox7.Text.Trim(), out dato_id))
                {
                    MessageBox.Show("Ingrese un ID válido para el registro a borrar.");
                    return;
                }

                string urlBorrar = $"http://127.0.0.1:8000/datos/{dato_id}";

                using (var client = new HttpClient())
                {
                    var response = client.DeleteAsync(urlBorrar).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Registro con ID {dato_id} borrado exitosamente.");
                    }
                    else
                    {
                        MessageBox.Show("Error al borrar el registro. Código de estado: " + response.StatusCode);
                    }
                }

                // Actualizar el DataGridView u otra lógica necesaria después de borrar el registro...
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al borrar el registro: " + ex.Message);
            }

            ///////////////////////////////////////////////////////////

            try
            {
                // Realizar la solicitud GET para obtener los datos más recientes
                string getDataUrl = "http://127.0.0.1:8000/datos";

                using (var httpClient = new HttpClient())
                {
                    var getDataResponse = httpClient.GetAsync(getDataUrl).Result;

                    if (getDataResponse.IsSuccessStatusCode)
                    {
                        string responseData = getDataResponse.Content.ReadAsStringAsync().Result;
                        Newtonsoft.Json.Linq.JObject result = Newtonsoft.Json.Linq.JObject.Parse(responseData);
                        List<List<object>> dataList = result["datos"].ToObject<List<List<object>>>();

                        // Crear DataTable y agregar columnas con nombres específicos
                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("Id");
                        dataTable.Columns.Add("Nombre");
                        dataTable.Columns.Add("Direccion");
                        dataTable.Columns.Add("Telefono");
                        dataTable.Columns.Add("Email");

                        // Agregar filas al DataTable
                        foreach (var dataItem in dataList)
                        {
                            dataTable.Rows.Add(dataItem.ToArray());
                        }

                        // Actualizar el DataSource del DataGridView en el hilo de la interfaz de usuario
                        dataGridView1.Invoke((MethodInvoker)delegate {
                            dataGridView1.DataSource = dataTable;
                        });

                        // Asignar nombres de columna a las columnas del DataGridView
                        dataGridView1.Invoke((MethodInvoker)delegate {
                            dataGridView1.Columns[0].HeaderText = "Id";
                            dataGridView1.Columns[1].HeaderText = "Nombre";
                            dataGridView1.Columns[2].HeaderText = "Direccion";
                            dataGridView1.Columns[3].HeaderText = "Telefono";
                            dataGridView1.Columns[4].HeaderText = "Email";
                        });
                    }
                    else
                    {
                        MessageBox.Show("Error al obtener los datos para actualizar. Código de estado: " + getDataResponse.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar los datos del DataGridView: " + ex.Message);
            }

            textBox7.Text = "";

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        




    }
    }

       
  

