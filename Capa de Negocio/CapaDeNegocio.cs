using System;
using System.Collections.Generic;
using CapaDeDatos;

namespace CapaDeNegocio
{
    public class ClienteSancionadoException : Exception
    {
        public ClienteSancionadoException(string mensaje) : base(mensaje) { }
    }

    public class FlexSpaceNegocio
    {
        private ClienteDatos _clienteDatos = new ClienteDatos();
        private PuestoDatos _puestoDatos = new PuestoDatos();
        private ReservaDatos _reservaDatos = new ReservaDatos();


        public decimal CalcularTarifa(Cliente cliente, Puesto puesto, DateTime inicio, DateTime fin)
        {
            TimeSpan duracion = fin - inicio;
            decimal horasReservadas = (decimal)duracion.TotalHours;
            decimal subtotalBase = horasReservadas * puesto.TarifaBasePorHora;

            if (cliente.SancionesActivas > 0)
            {
                return subtotalBase * 1.20m;
            }

            decimal acumulado = subtotalBase;

            bool incluyeFinDeSemana = false;
            for (DateTime dia = inicio; dia <= fin; dia = dia.AddHours(1))
            {
                if (dia.DayOfWeek == DayOfWeek.Saturday || dia.DayOfWeek == DayOfWeek.Sunday)
                {
                    incluyeFinDeSemana = true;
                    break;
                }
            }

            if (incluyeFinDeSemana)
            {
                acumulado += (subtotalBase * 0.15m);
            }

            if (horasReservadas >= 5)
            {
                acumulado -= (acumulado * 0.10m);
            }

            if (cliente.TipoCliente == "VIP")
            {
                acumulado -= (acumulado * 0.05m);
            }

            return acumulado;
        }


        public bool RegistrarReserva(int clienteId, int puestoId, DateTime inicio, DateTime fin, out decimal costoCalculado, out string error)
        {
            costoCalculado = 0;
            error = "";

            if (fin <= inicio)
            {
                error = "La fecha de fin debe ser posterior a la fecha de inicio.";
                return false;
            }

            Cliente cliente = _clienteDatos.ObtenerPorId(clienteId);
            if (cliente == null)
            {
                error = "El cliente especificado no existe.";
                return false;
            }

            if (cliente.SancionesActivas >= 3)
            {
                throw new ClienteSancionadoException($"El cliente {cliente.Nombre} está bloqueado por acumular {cliente.SancionesActivas} sanciones activas.");
            }

            Puesto puesto = _puestoDatos.ObtenerPorId(puestoId);
            if (puesto == null)
            {
                error = "El puesto especificado no existe.";
                return false;
            }

            List<Reserva> reservasExistentes = _reservaDatos.ObtenerConfirmadasPorPuesto(puestoId);
            foreach (Reserva r in reservasExistentes)
            {
                if (inicio < r.FechaFin && fin > r.FechaInicio)
                {
                    error = "El puesto ya posee una reserva confirmada en ese rango horario.";
                    return false;
                }
            }

            costoCalculado = CalcularTarifa(cliente, puesto, inicio, fin);

            Reserva nuevaReserva = new Reserva
            {
                ClienteId = clienteId,
                PuestoId = puestoId,
                FechaInicio = inicio,
                FechaFin = fin,
                Estado = "Confirmada",
                CostoTotal = costoCalculado
            };

            return _reservaDatos.Agregar(nuevaReserva);
        }


        public bool CancelarReserva(int reservaId, out string mensaje)
        {
            mensaje = "";
            Reserva reserva = _reservaDatos.ObtenerPorId(reservaId);

            if (reserva == null)
            {
                mensaje = "No existe la reserva especificada.";
                return false;
            }

            if (reserva.Estado == "Cancelada")
            {
                mensaje = "La reserva ya se encuentra cancelada.";
                return false;
            }

            TimeSpan margenAnticipacion = reserva.FechaInicio - DateTime.Now;
            if (margenAnticipacion.TotalHours < 2)
            {
                _clienteDatos.IncrementarSanciones(reserva.ClienteId);
                mensaje = "Reserva cancelada fuera del tiempo límite. Se aplicó +1 Sanción Activa al cliente.";
            }
            else
            {
                mensaje = "Reserva cancelada con éxito sin penalizaciones.";
            }

            return _reservaDatos.ActualizarEstado(reservaId, "Cancelada");
        }

        public List<Reserva> ConsultarReservasActivasPorPuesto(string codigoPuesto)
        {
            if (string.IsNullOrWhiteSpace(codigoPuesto)) return new List<Reserva>();
            return _reservaDatos.ObtenerActivasPorCodigoPuesto(codigoPuesto);
        }

        public List<Cliente> ListarClientesSancionados()
        {
            return _clienteDatos.ObtenerSancionados();
        }
    }
}