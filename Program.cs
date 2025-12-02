using NominaCaribe.Data;
using NominaCaribe.Models;
using NominaCaribe.Repositories;
using NominaCaribe.Services;
using NominaCaribe.UI;

namespace NominaCaribe
{
    class Program
    {
        private static BaseDatos _dbContext;
        private static EmpleadoRepository _empleadoRepo;
        private static NominaRepository _nominaRepo;
        private static NominaService _nominaService;

        static void Main(string[] args)
        {
            InicializarSistema();
            MostrarMenuPrincipal();
        }

        static void InicializarSistema()
        {
            Console.WriteLine("Inicializando sistema...\n");
            _dbContext = new BaseDatos();
            _empleadoRepo = new EmpleadoRepository(_dbContext);
            _nominaRepo = new NominaRepository(_dbContext);
            _nominaService = new NominaService(_empleadoRepo, _nominaRepo);
            Console.WriteLine();
        }

        static void MostrarMenuPrincipal()
        {
            bool salir = false;

            while (!salir)
            {
                MenuHelper.MostrarTitulo();
                Console.WriteLine("1. Gestión de Empleados");
                Console.WriteLine("2. Procesar Nómina");
                Console.WriteLine("3. Reportes");
                Console.WriteLine("4. Salir");
                Console.WriteLine();

                int opcion = MenuHelper.LeerOpcion("Seleccione una opción", 1, 4);

                switch (opcion)
                {
                    case 1:
                        MenuGestionEmpleados();
                        break;
                    case 2:
                        MenuProcesarNomina();
                        break;
                    case 3:
                        MenuReportes();
                        break;
                    case 4:
                        salir = true;
                        Console.WriteLine("\n¡Hasta pronto!");
                        break;
                }
            }
        }

        static void MenuGestionEmpleados()
        {
            bool volver = false;

            while (!volver)
            {
                MenuHelper.MostrarTitulo();
                Console.WriteLine("═══ GESTIÓN DE EMPLEADOS ═══\n");
                Console.WriteLine("1. Agregar Empleado");
                Console.WriteLine("2. Listar Empleados");
                Console.WriteLine("3. Editar Empleado");
                Console.WriteLine("4. Eliminar Empleado");
                Console.WriteLine("5. Volver al Menú Principal");
                Console.WriteLine();

                int opcion = MenuHelper.LeerOpcion("Seleccione una opción", 1, 5);

                switch (opcion)
                {
                    case 1:
                        AgregarEmpleado();
                        break;
                    case 2:
                        ListarEmpleados();
                        break;
                    case 3:
                        EditarEmpleado();
                        break;
                    case 4:
                        EliminarEmpleado();
                        break;
                    case 5:
                        volver = true;
                        break;
                }
            }
        }

        static void AgregarEmpleado()
        {
            MenuHelper.MostrarTitulo();
            Console.WriteLine("═══ AGREGAR NUEVO EMPLEADO ═══\n");

            try
            {
                var empleado = new Empleado
                {
                    Cedula = MenuHelper.LeerTexto("Cédula"),
                    Nombre = MenuHelper.LeerTexto("Nombre"),
                    Apellido = MenuHelper.LeerTexto("Apellido"),
                    SalarioBase = MenuHelper.LeerDecimal("Salario Base (RD$)"),
                    Departamento = MenuHelper.LeerTexto("Departamento", true)
                };

                _empleadoRepo.Agregar(empleado);
                MenuHelper.MostrarExito($"Empleado {empleado.NombreCompleto} agregado exitosamente");
            }
            catch (Exception ex)
            {
                MenuHelper.MostrarError($"Error: {ex.Message}");
            }

            MenuHelper.Pausar();
        }

        static void ListarEmpleados()
        {
            MenuHelper.MostrarTitulo();
            Console.WriteLine("═══ LISTA DE EMPLEADOS ═══\n");

            var empleados = _empleadoRepo.ObtenerTodos();

            if (!empleados.Any())
            {
                Console.WriteLine("No hay empleados registrados.");
            }
            else
            {
                Console.WriteLine($"{"ID",-5} {"Cédula",-15} {"Nombre",-25} {"Departamento",-20} {"Salario",15} {"Estado",10}");
                Console.WriteLine(new string('─', 100));

                foreach (var emp in empleados)
                {
                    string estado = emp.Activo ? "Activo" : "Inactivo";
                    Console.WriteLine($"{emp.Id,-5} {emp.Cedula,-15} {emp.NombreCompleto,-25} " +
                                    $"{emp.Departamento,-20} RD${emp.SalarioBase,12:N2} {estado,10}");
                }

                Console.WriteLine($"\nTotal: {empleados.Count} empleado(s)");
            }

            MenuHelper.Pausar();
        }

        static void EditarEmpleado()
        {
            MenuHelper.MostrarTitulo();
            Console.WriteLine("═══ EDITAR EMPLEADO ═══\n");

            int id = MenuHelper.LeerOpcion("Ingrese el ID del empleado", 1, int.MaxValue);
            var empleado = _empleadoRepo.ObtenerPorId(id);

            if (empleado == null)
            {
                MenuHelper.MostrarError("Empleado no encontrado");
                MenuHelper.Pausar();
                return;
            }

            Console.WriteLine($"\nEmpleado actual: {empleado}");
            Console.WriteLine("\nIngrese los nuevos datos (Enter para mantener el valor actual):\n");

            try
            {
                string nombre = MenuHelper.LeerTexto($"Nombre [{empleado.Nombre}]", true);
                if (!string.IsNullOrWhiteSpace(nombre)) empleado.Nombre = nombre;

                string apellido = MenuHelper.LeerTexto($"Apellido [{empleado.Apellido}]", true);
                if (!string.IsNullOrWhiteSpace(apellido)) empleado.Apellido = apellido;

                Console.Write($"Salario Base [{empleado.SalarioBase:N2}] (0 para mantener): ");
                if (decimal.TryParse(Console.ReadLine(), out decimal salario) && salario > 0)
                    empleado.SalarioBase = salario;

                string depto = MenuHelper.LeerTexto($"Departamento [{empleado.Departamento}]", true);
                if (!string.IsNullOrWhiteSpace(depto)) empleado.Departamento = depto;

                _empleadoRepo.Actualizar(empleado);
                MenuHelper.MostrarExito("Empleado actualizado exitosamente");
            }
            catch (Exception ex)
            {
                MenuHelper.MostrarError($"Error: {ex.Message}");
            }

            MenuHelper.Pausar();
        }

        static void EliminarEmpleado()
        {
            MenuHelper.MostrarTitulo();
            Console.WriteLine("═══ ELIMINAR EMPLEADO ═══\n");

            int id = MenuHelper.LeerOpcion("Ingrese el ID del empleado", 1, int.MaxValue);
            var empleado = _empleadoRepo.ObtenerPorId(id);

            if (empleado == null)
            {
                MenuHelper.MostrarError("Empleado no encontrado");
                MenuHelper.Pausar();
                return;
            }

            Console.WriteLine($"\nEmpleado: {empleado}");

            if (MenuHelper.Confirmar("\n¿Está seguro de eliminar este empleado?"))
            {
                try
                {
                    _empleadoRepo.Eliminar(id);
                    MenuHelper.MostrarExito("Empleado eliminado exitosamente");
                }
                catch (Exception ex)
                {
                    MenuHelper.MostrarError($"Error: {ex.Message}");
                }
            }

            MenuHelper.Pausar();
        }

        static void MenuProcesarNomina()
        {
            MenuHelper.MostrarTitulo();
            Console.WriteLine("═══ PROCESAR NÓMINA ═══\n");

            int mes = MenuHelper.LeerOpcion("Mes", 1, 12);
            int anio = MenuHelper.LeerOpcion("Año", 2020, 2030);

            if (MenuHelper.Confirmar($"\n¿Procesar nómina para {mes:00}/{anio}?"))
            {
                try
                {
                    _nominaService.ProcesarNominaMensual(mes, anio);
                }
                catch (Exception ex)
                {
                    MenuHelper.MostrarError($"Error: {ex.Message}");
                }
            }

            MenuHelper.Pausar();
        }

        static void MenuReportes()
        {
            MenuHelper.MostrarTitulo();
            Console.WriteLine("═══ REPORTES ═══\n");
            Console.WriteLine("1. Ver Reporte en Pantalla");
            Console.WriteLine("2. Exportar a CSV");
            Console.WriteLine("3. Volver");
            Console.WriteLine();

            int opcion = MenuHelper.LeerOpcion("Seleccione una opción", 1, 3);

            if (opcion == 3) return;

            int mes = MenuHelper.LeerOpcion("\nMes", 1, 12);
            int anio = MenuHelper.LeerOpcion("Año", 2020, 2030);

            try
            {
                if (opcion == 1)
                {
                    _nominaService.MostrarReporteMensual(mes, anio);
                }
                else
                {
                    string archivo = $"Nomina_{mes:00}_{anio}.csv";
                    _nominaService.ExportarReporteCSV(mes, anio, archivo);
                }
            }
            catch (Exception ex)
            {
                MenuHelper.MostrarError($"Error: {ex.Message}");
            }

            MenuHelper.Pausar();
        }
    }
}