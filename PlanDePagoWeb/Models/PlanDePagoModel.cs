using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanDePagoCredito.Models
{
    public class PlanDePagoModel
    {
        [Required(ErrorMessage = "El capital es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El capital debe ser un número positivo.")]
        public double Capital { get; set; }

        [Required(ErrorMessage = "La tasa de interés es obligatoria.")]
        [Range(0, 100, ErrorMessage = "La tasa debe estar entre 0 y 100.")]
        public double Tasa { get; set; }

        [Required(ErrorMessage = "El número de meses es obligatorio.")]
        [Range(1, 360, ErrorMessage = "El número de meses debe estar entre 1 y 360.")]
        public int Meses { get; set; }

        [Required(ErrorMessage = "La fecha de desembolso es obligatoria.")]
        public DateTime FechaDesembolso { get; set; }

        public double PagoMensual { get; set; }

        public List<Cuota> Cuotas { get; set; } = new List<Cuota>();
    }

    public class Cuota
    {
        public int Numero { get; set; }
        public DateTime FechaPago { get; set; }
        public double Interes { get; set; }
        public double AbonoCapital { get; set; }
        public double PagoTotal { get; set; }
        public double SaldoPendiente { get; set; }
    }
}
