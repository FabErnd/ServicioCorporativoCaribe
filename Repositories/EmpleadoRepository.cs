using System.Data.SQLite;
using NominaCaribe.Models;
using NominaCaribe.Data;

namespace NominaCaribe.Repositories
{
    public class EmpleadoRepository : IRepository<Empleado>
    {
        private readonly BaseDatos _context;

        public EmpleadoRepository(BaseDatos context)
        {
            _context = context;
        }

        public void Agregar(Empleado empleado)
        {
            if (string.IsNullOrWhiteSpace(empleado.Nombre))
                throw new ArgumentException("El nombre no puede estar vacío");

            if (empleado.SalarioBase <= 0)
                throw new ArgumentException("El salario debe ser mayor a cero");

            if (ExisteCedula(empleado.Cedula))
                throw new InvalidOperationException($"Ya existe un empleado con la cédula {empleado.Cedula}");

            using var conexion = _context.ObtenerConexion();
            conexion.Open();

            string sql = @"INSERT INTO Empleados (Cedula, Nombre, Apellido, SalarioBase, Departamento, FechaIngreso, Activo)
                          VALUES (@Cedula, @Nombre, @Apellido, @SalarioBase, @Departamento, @FechaIngreso, @Activo)";

            using var cmd = new SQLiteCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Cedula", empleado.Cedula);
            cmd.Parameters.AddWithValue("@Nombre", empleado.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", empleado.Apellido);
            cmd.Parameters.AddWithValue("@SalarioBase", empleado.SalarioBase);
            cmd.Parameters.AddWithValue("@Departamento", empleado.Departamento ?? "");
            cmd.Parameters.AddWithValue("@FechaIngreso", empleado.FechaIngreso.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@Activo", empleado.Activo ? 1 : 0);

            cmd.ExecuteNonQuery();
        }

        public Empleado ObtenerPorId(int id)
        {
            using var conexion = _context.ObtenerConexion();
            conexion.Open();

            string sql = "SELECT * FROM Empleados WHERE Id = @Id";
            using var cmd = new SQLiteCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                return MapearEmpleado(reader);

            return null;
        }

        public List<Empleado> ObtenerTodos()
        {
            var empleados = new List<Empleado>();

            using var conexion = _context.ObtenerConexion();
            conexion.Open();

            string sql = "SELECT * FROM Empleados ORDER BY Id";
            using var cmd = new SQLiteCommand(sql, conexion);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                empleados.Add(MapearEmpleado(reader));

            return empleados;
        }

        public void Actualizar(Empleado empleado)
        {
            if (string.IsNullOrWhiteSpace(empleado.Nombre))
                throw new ArgumentException("El nombre no puede estar vacío");

            if (empleado.SalarioBase <= 0)
                throw new ArgumentException("El salario debe ser mayor a cero");

            using var conexion = _context.ObtenerConexion();
            conexion.Open();

            string sql = @"UPDATE Empleados 
                          SET Cedula = @Cedula, Nombre = @Nombre, Apellido = @Apellido, 
                              SalarioBase = @SalarioBase, Departamento = @Departamento, Activo = @Activo
                          WHERE Id = @Id";

            using var cmd = new SQLiteCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Id", empleado.Id);
            cmd.Parameters.AddWithValue("@Cedula", empleado.Cedula);
            cmd.Parameters.AddWithValue("@Nombre", empleado.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", empleado.Apellido);
            cmd.Parameters.AddWithValue("@SalarioBase", empleado.SalarioBase);
            cmd.Parameters.AddWithValue("@Departamento", empleado.Departamento ?? "");
            cmd.Parameters.AddWithValue("@Activo", empleado.Activo ? 1 : 0);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using var conexion = _context.ObtenerConexion();
            conexion.Open();

            string sql = "DELETE FROM Empleados WHERE Id = @Id";
            using var cmd = new SQLiteCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }

        private bool ExisteCedula(string cedula)
        {
            using var conexion = _context.ObtenerConexion();
            conexion.Open();

            string sql = "SELECT COUNT(*) FROM Empleados WHERE Cedula = @Cedula";
            using var cmd = new SQLiteCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Cedula", cedula);

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        private Empleado MapearEmpleado(SQLiteDataReader reader)
        {
            return new Empleado
            {
                Id = Convert.ToInt32(reader["Id"]),
                Cedula = reader["Cedula"].ToString(),
                Nombre = reader["Nombre"].ToString(),
                Apellido = reader["Apellido"].ToString(),
                SalarioBase = Convert.ToDecimal(reader["SalarioBase"]),
                Departamento = reader["Departamento"].ToString(),
                FechaIngreso = DateTime.Parse(reader["FechaIngreso"].ToString()),
                Activo = Convert.ToInt32(reader["Activo"]) == 1
            };
        }
    }
}