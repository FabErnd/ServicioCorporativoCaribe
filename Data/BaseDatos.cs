using System.Data.SQLite;

namespace NominaCaribe.Data
{
    public class BaseDatos
    {
        private readonly string baseDatos;

        public BaseDatos(string dbPath = "nomina_caribe.db")
        {
            baseDatos = $"Data Source={dbPath};Version=3;";
            InicializarBaseDatos();
        }

        public SQLiteConnection ObtenerConexion()
        {
            return new SQLiteConnection(baseDatos);
        }

        private void InicializarBaseDatos()
        {
            using var conexion = ObtenerConexion();
            conexion.Open();

            string crearTablaEmpleados = @"
                CREATE TABLE IF NOT EXISTS Empleados (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Cedula TEXT NOT NULL UNIQUE,
                    Nombre TEXT NOT NULL,
                    Apellido TEXT NOT NULL,
                    SalarioBase REAL NOT NULL,
                    Departamento TEXT,
                    FechaIngreso TEXT NOT NULL,
                    Activo INTEGER NOT NULL DEFAULT 1
                )";

            string crearTablaNominas = @"
                CREATE TABLE IF NOT EXISTS Nominas (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    EmpleadoId INTEGER NOT NULL,
                    Mes INTEGER NOT NULL,
                    Anio INTEGER NOT NULL,
                    SalarioBruto REAL NOT NULL,
                    DeduccionAFP REAL NOT NULL,
                    DeduccionARS REAL NOT NULL,
                    DeduccionISR REAL NOT NULL,
                    TotalDeducciones REAL NOT NULL,
                    SalarioNeto REAL NOT NULL,
                    FechaProceso TEXT NOT NULL,
                    FOREIGN KEY (EmpleadoId) REFERENCES Empleados(Id),
                    UNIQUE(EmpleadoId, Mes, Anio)
                )";

            using var cmd = new SQLiteCommand(crearTablaEmpleados, conexion);
            cmd.ExecuteNonQuery();

            cmd.CommandText = crearTablaNominas;
            cmd.ExecuteNonQuery();

            Console.WriteLine("✓ Base de datos inicializada correctamente");
        }
    }
}