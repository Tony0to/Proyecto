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
    public partial class FormVentas : Form
    {
        private string conexionSQL = ConfigurationManager.ConnectionStrings["Proyecto.Properties.Settings.velasConnectionString"].ConnectionString;

        public FormVentas()
        {
            InitializeComponent();
        }

        private void ListarVentas()
        {
            using (SqlConnection conn = new SqlConnection(conexionSQL))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Ventas", conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridViewVentas.DataSource = dt;
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            // Verificar si el ID de Producto y la Cantidad son válidos
            if (int.TryParse(txtIDProducto.Text, out int idProducto) && int.TryParse(txtCantidad.Text, out int cantidadVenta))
            {
                using (SqlConnection conn = new SqlConnection(conexionSQL))
                {
                    conn.Open();

                    // Primero, verificar el stock disponible para el producto
                    SqlCommand checkStockCmd = new SqlCommand("SELECT Stock FROM Productos WHERE ID_Producto = @ID_Producto", conn);
                    checkStockCmd.Parameters.AddWithValue("@ID_Producto", idProducto);
                    object result = checkStockCmd.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int stockActual))
                    {
                        // Verificar si hay suficiente stock
                        if (stockActual >= cantidadVenta)
                        {
                            // Proceder con la venta si el stock es suficiente
                            SqlCommand cmd = new SqlCommand("INSERT INTO Ventas (Fecha_Venta, ID_Cliente, ID_Producto, Cantidad, Total_Venta, Metodo_Pago) VALUES (@Fecha, @Cliente, @Producto, @Cantidad, @Total, @Metodo)", conn);
                            cmd.Parameters.AddWithValue("@Fecha", txtFecha.Text);
                            cmd.Parameters.AddWithValue("@Cliente", txtIDCliente.Text);
                            cmd.Parameters.AddWithValue("@Producto", idProducto);
                            cmd.Parameters.AddWithValue("@Cantidad", cantidadVenta);
                            cmd.Parameters.AddWithValue("@Total", txtTotal.Text);
                            cmd.Parameters.AddWithValue("@Metodo", txtMetodo.Text);
                            cmd.ExecuteNonQuery();

                            // Actualizar el stock en la tabla de productos
                            SqlCommand updateStockCmd = new SqlCommand("UPDATE Productos SET Stock = Stock - @Cantidad WHERE ID_Producto = @ID_Producto", conn);
                            updateStockCmd.Parameters.AddWithValue("@Cantidad", cantidadVenta);
                            updateStockCmd.Parameters.AddWithValue("@ID_Producto", idProducto);
                            updateStockCmd.ExecuteNonQuery();

                            MessageBox.Show("Venta agregada correctamente.");
                            ListarVentas(); // Actualizar la lista de ventas
                        }
                        else
                        {
                            // Si no hay suficiente stock, mostrar un mensaje de error
                            MessageBox.Show("No hay suficiente stock disponible para realizar esta venta.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Producto no encontrado o stock inválido.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, ingresa un ID de producto y una cantidad válidos.");
            }
        }


        private void btnActualizar_Click(object sender, EventArgs e)
        {
            // Verifica que el campo de ID de la venta no esté vacío
            if (!string.IsNullOrEmpty(txtIDVenta.Text))
            {
                using (SqlConnection conn = new SqlConnection(conexionSQL))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "UPDATE Ventas SET Fecha_Venta=@Fecha, ID_Cliente=@Cliente, ID_Producto=@Producto, Cantidad=@Cantidad, Total_Venta=@Total, Metodo_Pago=@Metodo WHERE ID_Venta=@ID",
                        conn
                    );

                    cmd.Parameters.AddWithValue("@ID", txtIDVenta.Text);
                    cmd.Parameters.AddWithValue("@Fecha", txtFecha.Text);
                    cmd.Parameters.AddWithValue("@Cliente", txtIDCliente.Text);
                    cmd.Parameters.AddWithValue("@Producto", txtIDProducto.Text); // Campo adicional para ID_Producto
                    cmd.Parameters.AddWithValue("@Cantidad", txtCantidad.Text);   // Campo adicional para Cantidad
                    cmd.Parameters.AddWithValue("@Total", txtTotal.Text);
                    cmd.Parameters.AddWithValue("@Metodo", txtMetodo.Text);

                    // Ejecuta la actualización
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Venta actualizada correctamente.");
                        ListarVentas(); // Llama al método para actualizar la lista de ventas
                    }
                    else
                    {
                        MessageBox.Show("No se encontró ninguna venta con ese ID.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, ingresa el ID de la venta que deseas actualizar.");
            }
        }


        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtIDVenta.Text))
            {
                using (SqlConnection conn = new SqlConnection(conexionSQL))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Ventas WHERE ID_Venta=@ID", conn);
                    cmd.Parameters.AddWithValue("@ID", txtIDVenta.Text);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Venta eliminado correctamente.");
                        ListarVentas();
                    }
                    else
                    {
                        MessageBox.Show("No se encontró ninguna venta con ese ID.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, ingresa el ID del cliente que deseas eliminar.");
            }
        }

        private void FormVentas_Load(object sender, EventArgs e)
        {
            // TODO: esta línea de código carga datos en la tabla 'velasDataSet.Productos' Puede moverla o quitarla según sea necesario.
            this.productosTableAdapter.Fill(this.velasDataSet.Productos);
            // TODO: esta línea de código carga datos en la tabla 'velasDataSet.Clientes' Puede moverla o quitarla según sea necesario.
            this.clientesTableAdapter.Fill(this.velasDataSet.Clientes);
            // TODO: esta línea de código carga datos en la tabla 'velasDataSet.Ventas' Puede moverla o quitarla según sea necesario.
            this.ventasTableAdapter.Fill(this.velasDataSet.Ventas);

        }

        private void txtCliente_TextChanged(object sender, EventArgs e)
        {
            // Verifica que el texto en el TextBox no esté vacío
            if (!string.IsNullOrEmpty(txtIDCliente.Text))
            {
                // Intenta convertir el texto en un número
                if (int.TryParse(txtIDCliente.Text, out int idCliente))
                {
                    // Conexión a la base de datos para buscar el nombre del producto
                    using (SqlConnection conn = new SqlConnection(conexionSQL))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("SELECT Nombre_Cliente FROM Clientes WHERE ID_Cliente = @ID_Cliente", conn);
                        cmd.Parameters.AddWithValue("@ID_Cliente", idCliente);

                        // Ejecuta la consulta y obtiene el nombre del producto
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            // Si se encuentra el producto, muestra el nombre en el Label
                            lblNombreCliente.Text = result.ToString();
                        }
                        else
                        {
                            // Si no se encuentra el producto, muestra un mensaje en el Label
                            lblNombreCliente.Text = "Cliente no Registrado";
                        }
                    }
                }
                else
                {
                    lblNombreCliente.Text = "ID no válido";
                }
            }
            else
            {
                // Si el TextBox está vacío, borra el texto del Label
                lblNombreCliente.Text = string.Empty;
            }
        }

        private void txtIDProducto_TextChanged(object sender, EventArgs e)
        {
            // Verifica que el texto en el TextBox no esté vacío
            if (!string.IsNullOrEmpty(txtIDProducto.Text))
            {
                // Intenta convertir el texto en un número
                if (int.TryParse(txtIDProducto.Text, out int idProducto))
                {
                    // Conexión a la base de datos para buscar los detalles del producto
                    using (SqlConnection conn = new SqlConnection(conexionSQL))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(
                            "SELECT Nombre_Producto, Stock, Precio FROM Productos WHERE ID_Producto = @ID_Producto", conn);
                        cmd.Parameters.AddWithValue("@ID_Producto", idProducto);

                        // Ejecuta la consulta y usa un DataReader para obtener los detalles del producto
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Si se encuentra el producto, muestra el nombre, stock y precio en el mismo Label
                                string nombre = reader["Nombre_Producto"].ToString();
                                string stock = reader["Stock"].ToString();
                                string precio = reader["Precio"].ToString();
                                lblNombreProducto.Text = $"Producto: {nombre} | Stock: {stock} | Precio: ${precio}";
                            }
                            else
                            {
                                // Si no se encuentra el producto, muestra un mensaje en el Label
                                lblNombreProducto.Text = "Producto no encontrado";
                            }
                        }
                    }
                }
                else
                {
                    lblNombreProducto.Text = "ID no válido";
                }
            }
            else
            {
                // Si el TextBox está vacío, borra el texto del Label
                lblNombreProducto.Text = string.Empty;
            }
        }


        private void txtFecha_Click(object sender, EventArgs e)
        {
            txtFecha.Text = DateTime.Now.ToString("yyyy/MM/dd");
        }

        private decimal ObtenerPrecioProducto(int idProducto)
        {
            decimal precio = 0;
            using (SqlConnection conn = new SqlConnection(conexionSQL))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Precio FROM Productos WHERE ID_Producto = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", idProducto);

                var resultado = cmd.ExecuteScalar();
                if (resultado != null)
                {
                    precio = Convert.ToDecimal(resultado);
                }
            }
            return precio;
        }

        private void txtTotal_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtIDProducto.Text, out int idProducto) && int.TryParse(txtCantidad.Text, out int cantidad))
            {
                // Obtener el precio del producto
                decimal precioProducto = ObtenerPrecioProducto(idProducto);

                // Calcular el total
                decimal totalVenta = precioProducto * cantidad;

                // Mostrar el total en txtTotal
                txtTotal.Text = totalVenta.ToString("F2"); // F2 para mostrar 2 decimales
            }
            else
            {
                txtTotal.Text = "0.00";
            }
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            if (dgvProductos.Visible == false)
            {
                dgvProductos.Visible = true;
            }else
            {
                dgvProductos.Visible = false;
            }
        }
    }
}
