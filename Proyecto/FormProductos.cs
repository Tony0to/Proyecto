using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto
{
    public partial class FormProductos : Form
    {
        private string conexionSQL = ConfigurationManager.ConnectionStrings["Proyecto.Properties.Settings.velasConnectionString"].ConnectionString;

        public FormProductos()
        {
            InitializeComponent();
            ListarProductos();
        }
        private void ListarProductos()
        {
            using (SqlConnection conn = new SqlConnection(conexionSQL))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Productos", conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridViewProductos.DataSource = dt;
            }
        }

        private void FormProductos_Load(object sender, EventArgs e)
        {
            // TODO: esta línea de código carga datos en la tabla 'velasDataSet.Productos' Puede moverla o quitarla según sea necesario.
            this.productosTableAdapter.Fill(this.velasDataSet.Productos);
            // TODO: esta línea de código carga datos en la tabla 'velasDataSet.Productos' Puede moverla o quitarla según sea necesario.
            this.productosTableAdapter.Fill(this.velasDataSet.Productos);

        }

        private void buttonAgregarP_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(conexionSQL))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Productos (Nombre_Producto, Descripcion, Precio, Stock) VALUES (@Nombre_Producto, @Descripcion, @Precio, @Stock)", conn);
                cmd.Parameters.AddWithValue("@Nombre_Producto", txtNombreProducto.Text);
                cmd.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text);
                cmd.Parameters.AddWithValue("@Precio", txtPrecioProducto.Text);
                cmd.Parameters.AddWithValue("@Stock", txtStockProducto.Text);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Producto agregado correctamente.");
                ListarProductos(); // Actualizar la lista
            }
        }


        private void dataGridViewProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Verifica que se ha hecho clic en una fila válida
            {
                // Accede a las celdas utilizando índices
                txtIDProducto.Text = dataGridViewProductos.Rows[e.RowIndex].Cells[0].Value?.ToString(); // ID_  Productos
                txtNombreProducto.Text = dataGridViewProductos.Rows[e.RowIndex].Cells[1].Value?.ToString(); // Nombre_Producto
                txtDescripcion.Text = dataGridViewProductos.Rows[e.RowIndex].Cells[2].Value?.ToString(); // Descripcion
                txtPrecioProducto.Text = dataGridViewProductos.Rows[e.RowIndex].Cells[3].Value?.ToString(); // Precio
                txtStockProducto.Text = dataGridViewProductos.Rows[e.RowIndex].Cells[4].Value?.ToString(); // Stock
            }

        }

        private void buttonEliminarP_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtIDProducto.Text))
            {
                using (SqlConnection conn = new SqlConnection(conexionSQL))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Productos WHERE ID_Producto=@ID", conn);
                    cmd.Parameters.AddWithValue("@ID", txtIDProducto.Text);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Producto eliminado correctamente.");
                        ListarProductos();
                    }
                    else
                    {
                        MessageBox.Show("No se encontró ningún Producto con ese ID.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, ingresa el ID del Producto que deseas eliminar.");
            }
        }

        private void buttonActualizarP_Click(object sender, EventArgs e)
        {  // Verifica que el campo de ID del producto no esté vacío
            if (!string.IsNullOrEmpty(txtIDProducto.Text))
            {
                using (SqlConnection conn = new SqlConnection(conexionSQL))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE Productos SET Nombre_Producto=@Nombre, Descripcion=@Descripcion, Precio=@Precio, Stock=@Stock WHERE ID_Producto=@ID", conn);

                    // Asigna los parámetros
                    cmd.Parameters.AddWithValue("@ID", txtIDProducto.Text); // Usa el ID del cliente desde el campo de texto
                    cmd.Parameters.AddWithValue("@Nombre", txtNombreProducto.Text);
                    cmd.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text);
                    cmd.Parameters.AddWithValue("@Precio", txtPrecioProducto.Text);
                    cmd.Parameters.AddWithValue("@Stock", txtStockProducto.Text);

                    // Ejecuta la actualización
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Producto actualizado correctamente.");
                        ListarProductos(); // Llama al método para actualizar la lista de productos
                    }
                    else
                    {
                        MessageBox.Show("No se encontró ningún producto con ese ID.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, ingresa el ID del producto que deseas actualizar.");
            }
        }

    }
}
