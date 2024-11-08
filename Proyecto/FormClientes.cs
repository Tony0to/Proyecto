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
    public partial class FormClientes : Form
    {
        private string conexionSQL = ConfigurationManager.ConnectionStrings["Proyecto.Properties.Settings.velasConnectionString"].ConnectionString;

        public FormClientes()
        {
            InitializeComponent();
            ListarClientes();

        }
        private void ListarClientes()
        {
            using (SqlConnection conn = new SqlConnection(conexionSQL))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Clientes", conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridViewClientes.DataSource = dt;
            }
        }

        private void FormClientes_Load(object sender, EventArgs e)
        {
            // TODO: esta línea de código carga datos en la tabla 'velasDataSet.Clientes' Puede moverla o quitarla según sea necesario.
            this.clientesTableAdapter.Fill(this.velasDataSet.Clientes);
            this.clientesTableAdapter.Fill(this.velasDataSet.Clientes);
        }


        private void btnAgregar_Click_1(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(conexionSQL))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Clientes (Nombre_Cliente, Telefono, Email, Direccion) VALUES (@Nombre, @Telefono, @Email, @Direccion)", conn);
                cmd.Parameters.AddWithValue("@Nombre", txtNombreCliente.Text);
                cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Cliente agregado correctamente.");
                ListarClientes(); // Actualizar la lista
            }
        }

        private void btnActualizar_Click_1(object sender, EventArgs e)
        {
            // Verifica que el campo de ID del cliente no esté vacío
            if (!string.IsNullOrEmpty(txtIDCliente.Text))
            {
                using (SqlConnection conn = new SqlConnection(conexionSQL))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE Clientes SET Nombre_Cliente=@Nombre, Telefono=@Telefono, Email=@Email, Direccion=@Direccion WHERE ID_Cliente=@ID", conn);

                    // Asigna los parámetros
                    cmd.Parameters.AddWithValue("@ID", txtIDCliente.Text); // Usa el ID del cliente desde el campo de texto
                    cmd.Parameters.AddWithValue("@Nombre", txtNombreCliente.Text);
                    cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text);

                    // Ejecuta la actualización
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Cliente actualizado correctamente.");
                        ListarClientes(); // Llama al método para actualizar la lista de clientes
                    }
                    else
                    {
                        MessageBox.Show("No se encontró ningún cliente con ese ID.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, ingresa el ID del cliente que deseas actualizar.");
            }
        }


        private void btnEliminar_Click_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtIDCliente.Text))
            {
                using (SqlConnection conn = new SqlConnection(conexionSQL))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Clientes WHERE ID_Cliente=@ID", conn);
                    cmd.Parameters.AddWithValue("@ID", txtIDCliente.Text);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Cliente eliminado correctamente.");
                        ListarClientes();
                    }
                    else
                    {
                        MessageBox.Show("No se encontró ningún cliente con ese ID.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, ingresa el ID del cliente que deseas eliminar.");
            }
        }

        private void dataGridViewClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Verifica que se ha hecho clic en una fila válida
            {
                // Accede a las celdas utilizando índices
                txtIDCliente.Text = dataGridViewClientes.Rows[e.RowIndex].Cells[0].Value?.ToString(); // ID_Cliente
                txtNombreCliente.Text = dataGridViewClientes.Rows[e.RowIndex].Cells[1].Value?.ToString(); // Nombre_Cliente
                txtTelefono.Text = dataGridViewClientes.Rows[e.RowIndex].Cells[2].Value?.ToString(); // Telefono
                txtEmail.Text = dataGridViewClientes.Rows[e.RowIndex].Cells[3].Value?.ToString(); // Email
                txtDireccion.Text = dataGridViewClientes.Rows[e.RowIndex].Cells[4].Value?.ToString(); // Direccion
            }
        }
    }
}
