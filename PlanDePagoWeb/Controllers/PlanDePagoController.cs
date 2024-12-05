using Microsoft.AspNetCore.Mvc;
using PlanDePagoCredito.Models;
using System;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Text;

namespace PlanDePagoCredito.Controllers
{
    public class PlanDePagoController : Controller
    {
        public IActionResult Reiniciar()
        {
            var modeloVacio = new PlanDePagoModel
            {
                Capital = 0,
                Tasa = 0,
                Meses = 0,
                FechaDesembolso = DateTime.Now,
                Cuotas = new List<Cuota>()
            };
            return View("Index", modeloVacio);
        }


        // Página de inicio
        public IActionResult Index()
        {
            // Inicializar un modelo vacío y pasarlo a la vista
            var model = new PlanDePagoModel
            {
                Capital = 0,
                Tasa = 0,
                Meses = 0,
                FechaDesembolso = DateTime.Now,
                Cuotas = new List<Cuota>()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult CalcularPlanDePago(PlanDePagoModel model)
        {
            


            if (ModelState.IsValid)
            {
                double tasaMensual = model.Tasa / 100 / 12;
                model.PagoMensual = model.Tasa == 0 ? model.Capital / model.Meses :  (model.Capital * tasaMensual) / (1 - Math.Pow(1 + tasaMensual, -model.Meses));

                double saldoPendiente = model.Capital;
                DateTime fechaPago = model.FechaDesembolso;

                for (int mes = 1; mes <= model.Meses; mes++)
                {
                    DateTime fechaAnterior = fechaPago;
                    fechaPago = fechaPago.AddMonths(1);
                    int diasEntreCuotas = (fechaPago - fechaAnterior).Days;

                    double interesDiario = saldoPendiente * (model.Tasa / 100 / 365) * diasEntreCuotas;
                    double abonoCapital = model.PagoMensual - interesDiario;

                    if (mes == model.Meses)
                    {
                        // Ajustar la última cuota para evitar saldo negativo
                        double ultimoPago = saldoPendiente + interesDiario;
                        abonoCapital = ultimoPago - interesDiario;
                        saldoPendiente = 0;

                        model.Cuotas.Add(new Cuota
                        {
                            Numero = mes,
                            FechaPago = fechaPago,
                            Interes = interesDiario,
                            AbonoCapital = abonoCapital,
                            PagoTotal = ultimoPago,
                            SaldoPendiente = saldoPendiente
                        });
                    }
                    else
                    {
                        saldoPendiente -= abonoCapital;

                        model.Cuotas.Add(new Cuota
                        {
                            Numero = mes,
                            FechaPago = fechaPago,
                            Interes = interesDiario,
                            AbonoCapital = abonoCapital,
                            PagoTotal = model.PagoMensual,
                            SaldoPendiente = saldoPendiente
                        });
                    }
                }

                return View("Index", model); // Retorna la vista con los resultados
            }

            return View("Index", model); // Si el modelo es inválido, regresa el mismo modelo
        }
    }
}