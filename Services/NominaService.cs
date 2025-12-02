using NominaCaribe.Models;
using NominaCaribe.Repositories;

namespace NominaCaribe.Services
{
    public class NominaService
    {
        private readonly EmpleadoRepository _empleadoRepo;
        private readonly NominaRepository _nominaRepo;

        public NominaService(EmpleadoRepository empleadoRepo, NominaRepository nominaRepo)
        {
            _empleadoRepo = empleadoRepo;
            _nominaRepo = nominaRepo;
        }

        public void ProcesarNominaMensual(int mes, int anio)
        {
            var empleados = _empleadoRepo.ObtenerTodos().Where(e => e.Activo).ToList();

            if (!empleados.Any())
            {
                Console.WriteLine("No hay empleados activos para procesar nómina.");
                return;
            }

            Console.WriteLine($"\nProcesando nómina {mes:00}/{anio}...\n");

            int procesados = 0;
            foreach (var empleado in empleados)
            {
                try
                {
                    var nomina = new Nomina
                    {
                        EmpleadoId = empleado.Id,
                        Mes = mes,
                        Anio = anio,
                        SalarioBruto = empleado.SalarioBase
                    };

                    nomina.CalcularDeducciones();
                    _nominaRepo.Agregar(nomina);

                    Console.WriteLine($"✓ {empleado.NombreCompleto} - RD${nomina.SalarioNeto:N2}");
                    procesados++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Error procesando {empleado.NombreCompleto}: {ex.Message}");
                }
            }

            Console.WriteLine($"\n✓ Nómina procesada: {procesados} de {empleados.Count} empleados");
        }

        public void MostrarReporteMensual(int mes, int anio)
        {
            var nominas = _nominaRepo.ObtenerPorPeriodo(mes, anio);

            if (!nominas.Any())
            {
                Console.WriteLine($"\nNo hay nóminas registradas para {mes:00}/{anio}");
                return;
            }

            Console.WriteLine($"\n============================================================================");
            Console.WriteLine($"       REPORTE DE NÓMINA - SERVICIOS CORPORATIVOS CARIBE SRL            ");
            Console.WriteLine($"                        Período: {mes:00}/{anio}                                  ");
            Console.WriteLine($" =============================================================================\n");

            decimal totalBruto = 0, totalDeducciones = 0, totalNeto = 0;

            Console.WriteLine($"{"Empleado",-30} {"Bruto",15} {"AFP",12} {"ARS",12} {"ISR",12} {"Neto",15}");
            Console.WriteLine(new string('─', 110));

            foreach (var nomina in nominas)
            {
                var empleado = _empleadoRepo.ObtenerPorId(nomina.EmpleadoId);
                if (empleado == null) continue;

                Console.WriteLine($"{empleado.NombreCompleto,-30} " +
                                $"RD${nomina.SalarioBruto,12:N2} " +
                                $"RD${nomina.DeduccionAFP,9:N2} " +
                                $"RD${nomina.DeduccionARS,9:N2} " +
                                $"RD${nomina.DeduccionISR,9:N2} " +
                                $"RD${nomina.SalarioNeto,12:N2}");

                totalBruto += nomina.SalarioBruto;
                totalDeducciones += nomina.TotalDeducciones;
                totalNeto += nomina.SalarioNeto;
            }

            Console.WriteLine(new string('═', 110));
            Console.WriteLine($"{"TOTALES:",-30} " +
                            $"RD${totalBruto,12:N2} " +
                            $"{"",-12} {"",-12} {"",-12} " +
                            $"RD${totalNeto,12:N2}");

            Console.WriteLine($"\nTotal de Deducciones: RD${totalDeducciones:N2}");
            Console.WriteLine($"Total Pagado por la Empresa: RD${totalNeto:N2}");
            Console.WriteLine($"Cantidad de Empleados: {nominas.Count}");
        }

        public void ExportarReporteCSV(int mes, int anio, string rutaArchivo)
        {
            var nominas = _nominaRepo.ObtenerPorPeriodo(mes, anio);

            if (!nominas.Any())
            {
                Console.WriteLine("No hay datos para exportar.");
                return;
            }

            using var writer = new StreamWriter(rutaArchivo);
            writer.WriteLine("Empleado,Cedula,Departamento,Salario Bruto,AFP,ARS,ISR,Total Deducciones,Salario Neto");

            foreach (var nomina in nominas)
            {
                var empleado = _empleadoRepo.ObtenerPorId(nomina.EmpleadoId);
                if (empleado == null) continue;

                writer.WriteLine($"{empleado.NombreCompleto}," +
                               $"{empleado.Cedula}," +
                               $"{empleado.Departamento}," +
                               $"{nomina.SalarioBruto:F2}," +
                               $"{nomina.DeduccionAFP:F2}," +
                               $"{nomina.DeduccionARS:F2}," +
                               $"{nomina.DeduccionISR:F2}," +
                               $"{nomina.TotalDeducciones:F2}," +
                               $"{nomina.SalarioNeto:F2}");
            }

            Console.WriteLine($"\n✓ Reporte exportado exitosamente: {rutaArchivo}");
        }
    }
}