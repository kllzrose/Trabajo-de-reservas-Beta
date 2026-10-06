using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace CapaDeDatos
{
 
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string TipoCliente { get; set; } 
        public int SancionesActivas { get; set; }
    }

    public class Puesto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string TipoPuesto { get; set; } 
        public decimal TarifaBasePorHora { get; set; }
    }

    public class Reserva
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int PuestoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } 
        public decimal CostoTotal { get; set; }
    }

    public class ClienteDatos
    {
        private string _conexionString = "server=localhost;port=3306;database=flexspace;user=root;password=;";

        public Cliente ObtenerPorId(int id)
        {
            string query = "SELECT Id, Nombre, Email, TipoCliente, SancionesActivas FROM Clientes WHERE Id = @Id";

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                MySqlCommand comando = new MySqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@Id", id);
                conexion.Open();

                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        return new Cliente
                        {
                            Id = lector.GetInt32("Id"),
                            Nombre = lector.GetString("Nombre"),
                            Email = lector.GetString("Email"),
                            TipoCliente = lector.GetString("TipoCliente"),
                            SancionesActivas = lector.GetInt32("SancionesActivas")
                        };
                    }
                }
            }
            return null;
        }

        public List<Cliente> ObtenerSancionados()
        {
            List<Cliente> resultado = new List<Cliente>();
            string query = "SELECT Id, Nombre, Email, TipoCliente, SancionesActivas FROM Clientes WHERE SancionesActivas > 0";

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                MySqlCommand comando = new MySqlCommand(query, conexion);
                conexion.Open();

                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        resultado.Add(new Cliente
                        {
                            Id = lector.GetInt32("Id"),
                            Nombre = lector.GetString("Nombre"),
                            Email = lector.GetString("Email"),
                            TipoCliente = lector.GetString("TipoCliente"),
                            SancionesActivas = lector.GetInt32("SancionesActivas")
                        });
                    }
                }
            }
            return resultado;
        }

        public bool IncrementarSanciones(int clienteId)
        {
            string query = "UPDATE Clientes SET SancionesActivas = SancionesActivas + 1 WHERE Id = @Id";

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                MySqlCommand comando = new MySqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@Id", clienteId);
                conexion.Open();

                return comando.ExecuteNonQuery() > 0;
            }
        }
    }

    public class PuestoDatos
    {
        private string _conexionString = "server=localhost;port=3306;database=flexspace;user=root;password=;";

        public Puesto ObtenerPorId(int id)
        {
            string query = "SELECT Id, Codigo, TipoPuesto, TarifaBasePorHora FROM Puestos WHERE Id = @Id";

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                MySqlCommand comando = new MySqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@Id", id);
                conexion.Open();

                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        return new Puesto
                        {
                            Id = lector.GetInt32("Id"),
                            Codigo = lector.GetString("Codigo"),
                            TipoPuesto = lector.GetString("TipoPuesto"),
                            TarifaBasePorHora = lector.GetDecimal("TarifaBasePorHora")
                        };
                    }
                }
            }
            return null;
        }

        public Puesto ObtenerPorCodigo(string codigo)
        {
            string query = "SELECT Id, Codigo, TipoPuesto, TarifaBasePorHora FROM Puestos WHERE Codigo = @Codigo";

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                MySqlCommand comando = new MySqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@Codigo", codigo);
                conexion.Open();

                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        return new Puesto
                        {
                            Id = lector.GetInt32("Id"),
                            Codigo = lector.GetString("Codigo"),
                            TipoPuesto = lector.GetString("TipoPuesto"),
                            TarifaBasePorHora = lector.GetDecimal("TarifaBasePorHora")
                        };
                    }
                }
            }
            return null;
        }
    }

    public class ReservaDatos
    {
        private string _conexionString = "server=localhost;port=3306;database=flexspace;user=root;password=;";

        public Reserva ObtenerPorId(int id)
        {
            string query = "SELECT Id, ClienteId, PuestoId, FechaInicio, FechaFin, Estado, CostoTotal FROM Reservas WHERE Id = @Id";

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                MySqlCommand comando = new MySqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@Id", id);
                conexion.Open();

                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        return new Reserva
                        {
                            Id = lector.GetInt32("Id"),
                            ClienteId = lector.GetInt32("ClienteId"),
                            PuestoId = lector.GetInt32("PuestoId"),
                            FechaInicio = lector.GetDateTime("FechaInicio"),
                            FechaFin = lector.GetDateTime("FechaFin"),
                            Estado = lector.GetString("Estado"),
                            CostoTotal = lector.GetDecimal("CostoTotal")
                        };
                    }
                }
            }
            return null;
        }

        public List<Reserva> ObtenerConfirmadasPorPuesto(int puestoId)
        {
            List<Reserva> lista = new List<Reserva>();
            string query = "SELECT Id, ClienteId, PuestoId, FechaInicio, FechaFin, Estado, CostoTotal FROM Reservas WHERE PuestoId = @PuestoId AND Estado = 'Confirmada'";

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                MySqlCommand comando = new MySqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@PuestoId", puestoId);
                conexion.Open();

                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(new Reserva
                        {
                            Id = lector.GetInt32("Id"),
                            ClienteId = lector.GetInt32("ClienteId"),
                            PuestoId = lector.GetInt32("PuestoId"),
                            FechaInicio = lector.GetDateTime("FechaInicio"),
                            FechaFin = lector.GetDateTime("FechaFin"),
                            Estado = lector.GetString("Estado"),
                            CostoTotal = lector.GetDecimal("CostoTotal")
                        });
                    }
                }
            }
            return lista;
        }

        public List<Reserva> ObtenerActivasPorCodigoPuesto(string codigoPuesto)
        {
            List<Reserva> lista = new List<Reserva>();
            string query = @"SELECT r.Id, r.ClienteId, r.PuestoId, r.FechaInicio, r.FechaFin, r.Estado, r.CostoTotal 
                            FROM Reservas r 
                            INNER JOIN Puestos p ON r.PuestoId = p.Id 
                            WHERE p.Codigo = @Codigo AND r.Estado = 'Confirmada' AND r.FechaFin > NOW()"
            ;

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                MySqlCommand comando = new MySqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@Codigo", codigoPuesto);
                conexion.Open();

                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(new Reserva
                        {
                            Id = lector.GetInt32("Id"),
                            ClienteId = lector.GetInt32("ClienteId"),
                            PuestoId = lector.GetInt32("PuestoId"),
                            FechaInicio = lector.GetDateTime("FechaInicio"),
                            FechaFin = lector.GetDateTime("FechaFin"),
                            Estado = lector.GetString("Estado"),
                            CostoTotal = lector.GetDecimal("CostoTotal")
                        });
                    }
                }
            }
            return lista;
        }

        public bool Agregar(Reserva r)
        {
            string query = @"INSERT INTO Reservas (ClienteId, PuestoId, FechaInicio, FechaFin, Estado, CostoTotal) 
                            VALUES (@ClienteId, @PuestoId, @FechaInicio, @FechaFin, @Estado, @CostoTotal)"
            ;

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                MySqlCommand comando = new MySqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@ClienteId", r.ClienteId);
                comando.Parameters.AddWithValue("@PuestoId", r.PuestoId);
                comando.Parameters.AddWithValue("@FechaInicio", r.FechaInicio);
                comando.Parameters.AddWithValue("@FechaFin", r.FechaFin);
                comando.Parameters.AddWithValue("@Estado", r.Estado);
                comando.Parameters.AddWithValue("@CostoTotal", r.CostoTotal);

                conexion.Open();
                return comando.ExecuteNonQuery() > 0;
            }
        }

        public bool ActualizarEstado(int reservaId, string nuevoEstado)
        {
            string query = "UPDATE Reservas SET Estado = @Estado WHERE Id = @Id";

            using (MySqlConnection conexion = new MySqlConnection(_conexionString))
            {
                MySqlCommand comando = new MySqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@Estado", nuevoEstado);
                comando.Parameters.AddWithValue("@Id", reservaId);

                conexion.Open();
                return comando.ExecuteNonQuery() > 0;
            }
        }
    }
}