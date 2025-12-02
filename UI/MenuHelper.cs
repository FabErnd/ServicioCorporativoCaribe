namespace NominaCaribe.UI
{
    public static class MenuHelper
    {
        public static void MostrarTitulo()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==============================================================");
            Console.WriteLine("   SISTEMA DE NÓMINA - SERVICIOS CORPORATIVOS CARIBE SRL   ");
            Console.WriteLine("==============================================================");
            Console.ResetColor();
            Console.WriteLine();
        }

        public static int LeerOpcion(string mensaje, int min, int max)
        {
            while (true)
            {
                Console.Write($"{mensaje} [{min}-{max}]: ");
                if (int.TryParse(Console.ReadLine(), out int opcion) && opcion >= min && opcion <= max)
                    return opcion;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Opción inválida. Intente nuevamente.");
                Console.ResetColor();
            }
        }

        public static string LeerTexto(string mensaje, bool opcional = false)
        {
            while (true)
            {
                Console.Write($"{mensaje}: ");
                string valor = Console.ReadLine()?.Trim();

                if (!string.IsNullOrWhiteSpace(valor) || opcional)
                    return valor ?? "";

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Este campo es obligatorio.");
                Console.ResetColor();
            }
        }

        public static decimal LeerDecimal(string mensaje)
        {
            while (true)
            {
                Console.Write($"{mensaje}: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal valor) && valor >= 0)
                    return valor;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Debe ingresar un número válido mayor o igual a cero.");
                Console.ResetColor();
            }
        }

        public static bool Confirmar(string mensaje)
        {
            Console.Write($"{mensaje} (S/N): ");
            var respuesta = Console.ReadLine()?.Trim().ToUpper();
            return respuesta == "S" || respuesta == "SI";
        }

        public static void Pausar()
        {
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        public static void MostrarExito(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ {mensaje}");
            Console.ResetColor();
        }

        public static void MostrarError(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ {mensaje}");
            Console.ResetColor();
        }
    }
}