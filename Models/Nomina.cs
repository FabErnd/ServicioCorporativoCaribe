namespace NominaCaribe.Models
{
    public class Nomina
    {
        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal SalarioBruto { get; set; }
        public decimal DeduccionAFP { get; set; }
        public decimal DeduccionARS { get; set; }
        public decimal DeduccionISR { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal SalarioNeto { get; set; }
        public DateTime FechaProceso { get; set; }

        // Constantes de las deducciones
        public const decimal TASA_AFP = 0.0287m; // 2.87%
        public const decimal TASA_ARS = 0.0304m; // 3.04%

        public Nomina()
        {
            FechaProceso = DateTime.Now;
        }

        public void CalcularDeducciones()
        {
            DeduccionAFP = SalarioBruto * TASA_AFP;
            DeduccionARS = SalarioBruto * TASA_ARS;
            DeduccionISR = CalcularISR();
            TotalDeducciones = DeduccionAFP + DeduccionARS + DeduccionISR;
            SalarioNeto = SalarioBruto - TotalDeducciones;
        }

        private decimal CalcularISR()
        {
            // ISR simplificado
            decimal salarioAnual = SalarioBruto * 12;
            decimal isr = 0;

            if (salarioAnual > 416220.01m)
            {
                decimal exceso = salarioAnual - 416220.01m;
                isr = 31216m + (exceso * 0.25m);
            }
            else if (salarioAnual > 624329.01m)
            {
                decimal exceso = salarioAnual - 624329.01m;
                isr = 79776m + (exceso * 0.20m);
            }
            else if (salarioAnual > 867123.01m)
            {
                decimal exceso = salarioAnual - 867123.01m;
                isr = 128235m + (exceso * 0.15m);
            }

            return isr / 12; // ISR mensual
        }

        public string ObtenerPeriodo()
        {
            return $"{Mes:00}/{Anio}";
        }
    }
}