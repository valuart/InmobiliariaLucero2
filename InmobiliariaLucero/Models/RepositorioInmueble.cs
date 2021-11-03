using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaLucero.Models
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
	{
		public RepositorioInmueble(IConfiguration configuration) : base(configuration)
		{

		}


		public int Alta(Inmueble i)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Inmueble ( IdPropie, Direccion,Tipo, Precio, Estado) " +
					$"VALUES (@idPropietario, @direccion, @tipo, @precio, @estado); " +
					$"SELECT SCOPE_IDENTITY();";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idPropietario", i.IdPropie);
					command.Parameters.AddWithValue("@direccion", i.Direccion);
					command.Parameters.AddWithValue("@tipo", i.Tipo);
					command.Parameters.AddWithValue("@precio", i.Precio);
					command.Parameters.AddWithValue("@estado", i.Estado);		
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					i.Id = res;
					connection.Close();
				}  
			}
			return res;
		}
		public int Baja(int id)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"DELETE FROM Inmueble WHERE Id = @idInmueble";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idInmueble", id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public int Modificacion(Inmueble i)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"UPDATE Inmueble SET IdPropie=@idPropietario, Direccion=@direccion, Tipo=@tipo, Precio=@precio, Estado=@estado " +
					$"WHERE Id = @idInmueble ";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@IdPropietario", i.IdPropie);
					command.Parameters.AddWithValue("@direccion", i.Direccion);
					command.Parameters.AddWithValue("@tipo", i.Tipo);
					command.Parameters.AddWithValue("@precio", i.Precio);
					command.Parameters.AddWithValue("@estado", i.Estado);
					command.Parameters.AddWithValue("@IdInmueble", i.Id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Inmueble> ObtenerTodos()
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT i.Id, IdPropie, Direccion, Tipo, Precio, Estado," +
					" p.Nombre, p.Apellido " +
					" FROM Inmueble i INNER JOIN Propietario p ON i.IdPropie=p.Id ";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble i = new Inmueble
						{
							Id = reader.GetInt32(0),
							IdPropie = reader.GetInt32(1),
							Direccion = reader.GetString(2),						
							Tipo = reader.GetString(3),
							Precio = reader.GetDecimal(4),
							Estado = reader.GetBoolean(5),
							Propietario = new Propietario
							{
								Id = reader.GetInt32(1),
								Nombre = reader.GetString(6),
								Apellido = reader.GetString(7),
							},

						};
						res.Add(i);
					}
					connection.Close();
				}
			}
			return res;
		}
		public Inmueble ObtenerPorId(int id)
		{
			Inmueble i = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT i.Id, IdPropie, Direccion, Tipo, Precio, Estado, p.Nombre, p.Apellido " +
					$" FROM Inmueble i INNER JOIN Propietario p ON i.Id=p.Id" +
					$" WHERE i.Id = @idInmueble";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@idInmueble", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						i = new Inmueble
						{
							Id = reader.GetInt32(0),
							IdPropie = reader.GetInt32(1),
							Direccion = reader.GetString(2),
							Tipo = reader.GetString(3),
							Precio = reader.GetDecimal(4),
							Estado = reader.GetBoolean(5),
							
							Propietario = new Propietario
							{
								Id = reader.GetInt32(1),
								Nombre = reader.GetString(6),
								Apellido = reader.GetString(7),
							}
						};
					}
					connection.Close();
				}
			}
			return i;
		}
		public IList<Inmueble> ObtenerTodosPorPropietarioId(int IdPropie)
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT i.Id, IdPropie, Direccion, Tipo, Precio, Estado, " +
					" p.Nombre, p.Apellido" +
					" FROM Inmueble i INNER JOIN Propietario p ON i.IdPropie = p.Id" +
					$" WHERE i.IdPropie = @IdPropietario";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@idPropietario", SqlDbType.Int).Value = IdPropie;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble i = new Inmueble
						{
							Id = reader.GetInt32(0),
							IdPropie = reader.GetInt32(1),
							Propietario = new Propietario
							{
								Id = reader.GetInt32(1),
								Nombre = reader.GetString(2),
								Apellido = reader.GetString(3),
							},
							Direccion = reader.GetString(4),
							Tipo = reader.GetString(5),
							Precio = reader.GetDecimal(6),
							Estado = reader.GetBoolean(7),
							
						};
						res.Add(i);
					}
					connection.Close();
				}
			}
			return res;
		}
		public IList<Inmueble> ObtenerTodosDisponibles(DateTime fechaInicio, DateTime fechaFin)
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT i.Id, IdPropie, Direccion, Tipo, Precio, Estado, p.Nombre, p.Apellido" +
					$" FROM Inmueble i INNER JOIN Propietario p ON i.IdPropie = p.Id " +
					$"WHERE i.Id IN ( SELECT IdInmueble FROM Contrato c WHERE Estado = 1" +
						$"AND((FechaInicio < @fechaInicio)AND(FechaFin < @fechaInicio))" +
						$"OR((FechaInicio > @fechaFin)AND(FechaFin > @fechaFin))" +
						$"AND((FechaInicio < @fechaInicio)AND(FechaFin > @fechaFin))" +
						$"OR ((FechaInicio > @fechaInicio)AND(FechaFin < @fechaFin))" +
						$"AND(FechaInicio NOT BETWEEN @fechaInicio AND @fechaFin)" +
						$"AND(FechaFin NOT BETWEEN @fechaInicio AND @fechaFin))" +
					$"OR i.Id NOT IN(SELECT IdInmu FROM Contrato);";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@fechaInicio", SqlDbType.DateTime).Value = fechaInicio;
					command.Parameters.Add("@fechaFin", SqlDbType.DateTime).Value = fechaFin;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble i = new Inmueble
						{
							Id = reader.GetInt32(0),
							IdPropie = reader.GetInt32(1),
							Propietario = new Propietario
							{
								Id = reader.GetInt32(1),
								Nombre = reader.GetString(2),
								Apellido = reader.GetString(3),
							},
							Direccion = reader.GetString(4),
							Tipo = reader.GetString(5),
							Precio = reader.GetDecimal(6),
							Estado = reader.GetBoolean(7),
						};
						res.Add(i);
					}
					connection.Close();
				}
			}
			return res;
		}
		public List<Inmueble> BuscarPorPropietario(int id)
		{
			List<Inmueble> res = new List<Inmueble>();
			Inmueble inm = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT Id, IdPropie, Direccion, Tipo, Precio, Estado, " +
					$" FROM Inmuebles" +
					$" WHERE IdPropie = @idPropietario AND Disponible = 1";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@idPropietario", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						inm = new Inmueble
						{
							Id = reader.GetInt32(0),
							Propietario = new Propietario
							{
								Id = reader.GetInt32(1),
								Nombre = reader.GetString(2),
								Apellido = reader.GetString(3),
							},
							Direccion = reader.GetString(4),
							Tipo = reader.GetString(5),
							Precio = reader.GetDecimal(6),
							Estado = reader.GetBoolean(7),
			
						};
						res.Add(inm);
					}
					connection.Close();
				}
			}
			return res;
		}

	}
}
