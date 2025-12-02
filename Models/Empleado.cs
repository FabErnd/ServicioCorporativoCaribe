namespace NominaCaribe.Models
{
    public class Empleado
    {
        public int Id { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public decimal SalarioBase { get; set; }
        public string Departamento { get; set; }
        public DateTime FechaIngreso { get; set; }
        public bool Activo { get; set; }

        public Empleado()
        {
            FechaIngreso = DateTime.Now;
            Activo = true;
        }

        public string NombreCompleto => $"{Nombre} {Apellido}";

        public override string ToString()
        {
            return $"[{Id}] {Cedula} - {NombreCompleto} | Depto: {Departamento} | Salario: RD${SalarioBase:N2}";
        }
    }
}