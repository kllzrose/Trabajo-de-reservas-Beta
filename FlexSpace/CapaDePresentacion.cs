using System;
using System.Collections.Generic;
using CapaDeDatos;
using CapaDeNegocio;

namespace CapaDePresentacion
{
    public class Program
    {
        public static void Main()
        {
            FlexSpaceNegocio negocio = new FlexSpaceNegocio();
            int opcion = 0;

            do
            {
                Console.WriteLine("\n=== SISTEMA DE GESTIÓN FLEXSPACE ===");
                Console.WriteLine("1. Registrar Nueva Reserva");
                Console.WriteLine("2. Cancelar Reserva");
                Console.WriteLine("3. Consultar Reservas Activas por Puesto");
                Console.WriteLine("4. Listar Clientes Sancionados");
                Console.WriteLine("5. Salir");
                Console.Write("Seleccione una opción: ");

                if (!int.TryParse(Console.ReadLine(), out opcion))
                {
                    Console.WriteLine("Opción no válida.");
                    continue;
                }

                switch (opcion)
                {
                    case 1:
                        OpcionRegistrarReserva(negocio);
                        break;

                    case 2:
                        OpcionCancelarReserva(negocio);
                        break;

                    case 3:
                        OpcionConsultarReservasActivas(negocio);
                        break;

                    case 4:
                        OpcionListarSancionados(negocio);
                        break;

                    case 5:
                        Console.WriteLine("Saliendo de FlexSpace...");
                        break;

                    default:
                        Console.WriteLine("Opción inválida.");
                        break;
                }

            } while (opcion != 5);
        }

        private static void OpcionRegistrarReserva(FlexSpaceNegocio negocio)
        {
            try
            {
                Console.WriteLine("\n--- REGISTRAR NUEVA RESERVA ---");
                Console.Write("ID Cliente: ");
                int clienteId = int.Parse(Console.ReadLine());

                Console.Write("ID Puesto: ");
                int puestoId = int.Parse(Console.ReadLine());

                Console.Write("Fecha y Hora Inicio (yyyy-MM-dd HH:mm): ");
                DateTime inicio = DateTime.Parse(Console.ReadLine());

                Console.Write("Fecha y Hora Fin (yyyy-MM-dd HH:mm): ");
                DateTime fin = DateTime.Parse(Console.ReadLine());

                decimal costoCalculado;
                string error;

                bool resultado = negocio.RegistrarReserva(clienteId, puestoId, inicio, fin, out costoCalculado, out error);

                if (resultado)
                {
                    Console.WriteLine("\n>>> RESERVA REGISTRADA CON ÉXITO <<<");
                    Console.WriteLine($"Costo Total Calculado: ${costoCalculado:F2}");
                }
                else
                {
                    Console.WriteLine($"\n[ERROR]: No se pudo registrar la reserva. {error}");
                }
            }
            catch (ClienteSancionadoException ex)
            {
                Console.WriteLine($"\n[RECHAZADO]: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR DE ENTRADA]: Formato invalido o error de sistema ({ex.Message}).");
            }
        }

        private static void OpcionCancelarReserva(FlexSpaceNegocio negocio)
        {
            Console.WriteLine("\n--- CANCELAR RESERVA ---");
            Console.Write("Ingrese el ID de la Reserva a cancelar: ");

            if (int.TryParse(Console.ReadLine(), out int reservaId))
            {
                string mensaje;
                bool ok = negocio.CancelarReserva(reservaId, out mensaje);
                Console.WriteLine($"\nResultado: {mensaje}");
            }
            else
            {
                Console.WriteLine("ID inválido.");
            }
        }

        private static void OpcionConsultarReservasActivas(FlexSpaceNegocio negocio)
        {
            Console.WriteLine("\n--- CONSULTAR RESERVAS ACTIVAS POR PUESTO ---");
            Console.Write("Ingrese el Código del Puesto (ej. P01): ");
            string codigo = Console.ReadLine();

            List<Reserva> lista = negocio.ConsultarReservasActivasPorPuesto(codigo);

            if (lista.Count == 0)
            {
                Console.WriteLine("No se encontraron reservas activas/futuras para este puesto.");
                return;
            }

            Console.WriteLine($"\nReservas Activas para el Puesto '{codigo}':");
            foreach (Reserva r in lista)
            {
                Console.WriteLine($"ID Reserva: {r.Id} | Cliente ID: {r.ClienteId} | Inicio: {r.FechaInicio:dd/MM/yyyy HH:mm} | Fin: {r.FechaFin:dd/MM/yyyy HH:mm} | Total: ${r.CostoTotal:F2}");
            }
        }

        private static void OpcionListarSancionados(FlexSpaceNegocio negocio)
        {
            Console.WriteLine("\n--- LISTADO DE CLIENTES SANCIONADOS ---");
            List<Cliente> sancionados = negocio.ListarClientesSancionados();

            if (sancionados.Count == 0)
            {
                Console.WriteLine("No hay clientes con sanciones activas.");
                return;
            }

            foreach (Cliente c in sancionados)
            {
                Console.WriteLine($"ID: {c.Id} | Nombre: {c.Nombre} | Email: {c.Email} | Tipo: {c.TipoCliente} | Sanciones Activas: {c.SancionesActivas}");
            }
        }
    }
}