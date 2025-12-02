using System.Data.SQLite;
using NominaCaribe.Models;
using NominaCaribe.Data;

namespace NominaCaribe.Repositories
{
    public class NominaRepository : IRepository<Nomina>
    {
        private readonly BaseDatos _context;

        public NominaRepository(BaseDatos context)
        {
            _context = context;
        }

        public void Agregar(Nomina nomina)
        {
            using var conexion = _context.ObtenerConexion();
            conexion.Open();

            string sql = @"INSERT INTO Nominas (EmpleadoId, Mes, Anio, SalarioBruto, DeduccionAFP, DeduccionARS, 
                          DeduccionISR, TotalDeducciones, SalarioNeto, FechaProceso)
                          VALUES (@EmpleadoId, @Mes, @Anio, @SalarioBruto, @DeduccionAFP, @DeduccionARS, 
                          @DeduccionISR, @TotalDeducciones, @SalarioNeto, @FechaProceso)";

            using var cmd = new SQLiteCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@EmpleadoId", nomina.EmpleadoId);
            cmd.Parameters.AddWithValue("@Mes", nomina.Mes);
            cmd.Parameters.AddWithValue("@Anio", nomina.Anio);
            cmd.Parameters.AddWithValue("@SalarioBruto", nomina.SalarioBruto);
            cmd.Parameters.AddWithValue("@DeduccionAFP", nomina.DeduccionAFP);
            cmd.Parameters.AddWithValue("@DeduccionARS", nomina.DeduccionARS);
            cmd.Parameters.AddWithValue("@DeduccionISR", nomina.DeduccionISR);
            cmd.Parameters.AddWithValue("@TotalDeducciones", nomina.TotalDeducciones);
            cmd.Parameters.AddWithValue("@SalarioNeto", nomina.SalarioNeto);
            cmd.Parameters.AddWithValue("@FechaProceso", nomina.FechaProceso.ToString("yyyy-MM-dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
        }

        public Nomina ObtenerPorId(int id)
        {
            using var conexion = _context.ObtenerConexion();
            conexion.Open();

            string sql = "SELECT * FROM Nominas WHERE Id = @Id";
            using var cmd = new SQLiteCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                return MapearNomina(reader);

            return null;
        }

        public List<Nomina> ObtenerTodos()
        {
            var nominas = new List<Nomina>();

            using var conexion = _context.ObtenerConexion();
            conexion.Open();

            string sql = "SELECT * FROM Nominas ORDER BY Anio DESC, Mes DESC";
            using var cmd = new SQLiteCommand(sql, conexion);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                nominas.Add(MapearNomina(reader));

            return nominas;
        }

        public List<Nomina> ObtenerPorPeriodo(int mes, int anio)
        {
            var nominas = new List<Nomina>();

            using var conexion = _context.ObtenerConexion();
            conexion.Open();

            string sql = "SELECT * FROM Nominas WHERE Mes = @Mes AND Anio = @Anio ORDER BY EmpleadoId";
            using var cmd = new SQLiteCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Mes", mes);
            cmd.Parameters.AddWithValue("@Anio", anio);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                nominas.Add(MapearNomina(reader));

            return nominas;
        }

        public void Actualizar(Nomina nomina)
        {
            using var conexion = _context.ObtenerConexion();
            conexion.Open();

            string sql = @"UPDATE Nominas 
                          SET SalarioBruto = @SalarioBruto, DeduccionAFP = @DeduccionAFP, 
                              DeduccionARS = @DeduccionARS, DeduccionISR = @DeduccionISR,
                              TotalDeducciones = @TotalDeducciones, SalarioNeto = @SalarioNeto
                          WHERE Id = @Id";

            using var cmd = new SQLiteCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Id", nomina.Id);
            cmd.Parameters.AddWithValue("@SalarioBruto", nomina.SalarioBruto);
            cmd.Parameters.AddWithValue("@DeduccionAFP", nomina.DeduccionAFP);
            cmd.Parameters.AddWithValue("@DeduccionARS", nomina.DeduccionARS);
            cmd.Parameters.AddWithValue("@DeduccionISR", nomina.DeduccionISR);
            cmd.Parameters.AddWithValue("@TotalDeducciones", nomina.TotalDeducciones);
            cmd.Parameters.AddWithValue("@SalarioNeto", nomina.SalarioNeto);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using var conexion = _context.ObtenerConexion();
            conexion.Open();

            string sql = "DELETE FROM Nominas WHERE Id = @Id";
            using var cmd = new SQLiteCommand(sql, conexion);
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }

        private Nomina MapearNomina(SQLiteDataReader reader)
        {
            return new Nomina
            {
                Id = Convert.ToInt32(reader["Id"]),
                EmpleadoId = Convert.ToInt32(reader["EmpleadoId"]),
                Mes = Convert.ToInt32(reader["Mes"]),
                Anio = Convert.ToInt32(reader["Anio"]),
                SalarioBruto = Convert.ToDecimal(reader["SalarioBruto"]),
                DeduccionAFP = Convert.ToDecimal(reader["DeduccionAFP"]),
                DeduccionARS = Convert.ToDecimal(reader["DeduccionARS"]),
                DeduccionISR = Convert.ToDecimal(reader["DeduccionISR"]),
                TotalDeducciones = Convert.ToDecimal(reader["TotalDeducciones"]),
                SalarioNeto = Convert.ToDecimal(reader["SalarioNeto"]),
                FechaProceso = DateTime.Parse(reader["FechaProceso"].ToString())
            };
        }
    }
}