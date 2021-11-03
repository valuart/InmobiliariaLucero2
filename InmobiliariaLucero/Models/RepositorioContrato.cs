using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaLucero.Models
{
	public class RepositorioContrato : RepositorioBase, IRepositorioContrato
	{
		public RepositorioContrato(IConfiguration configuration) : base(configuration)
		{

		}

		public int Alta(Contrato c)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Contrato (IdInmu, IdInqui, FechaInicio, FechaFin, Monto, Estado) " +
					$"VALUES (@idInmueble, @idInquilino, @fechaInicio, @fechaFin, @monto, @estado);" +
					$"SELECT SCOPE_IDENTITY();";//devuelve el id insertado
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idInmueble", c.IdInmu);
					command.Parameters.AddWithValue("@idInquilino", c.IdInqui);
					command.Parameters.AddWithValue("@fechaInicio", c.FechaInicio);
					command.Parameters.AddWithValue("@fechaFin", c.FechaFin);
					command.Parameters.AddWithValue("@monto", c.Monto);
					command.Parameters.AddWithValue("@estado", c.Estado);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					c.Id = res;
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
				string sql = $"DELETE FROM Contrato WHERE Id= @idContrato";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idContrato", id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public int Modificacion(Contrato c)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"UPDATE Contrato SET IdInmu=@idInmueble, IdInqui=@idInquilino, FechaInicio=@fechaInicio, FechaFin=@fechaFin, Monto=@monto, Estado=@estado " +
					$"WHERE Id = @idContrato";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{

					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idInmueble", c.IdInmu);
					command.Parameters.AddWithValue("@idInquilino", c.IdInqui);
					command.Parameters.AddWithValue("@fechaInicio", c.FechaInicio);
					command.Parameters.AddWithValue("@fechaFin", c.FechaFin);
					command.Parameters.AddWithValue("@monto", c.Monto);
					command.Parameters.AddWithValue("@estado", c.Estado);
					command.Parameters.AddWithValue("@idContrato", c.Id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Contrato> ObtenerTodos()
		{
			IList<Contrato> res = new List<Contrato>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT c.Id, IdInmu,IdInqui,FechaInicio, FechaFin, Monto, c.Estado, inq.Nombre, inq.Apellido , i.Direccion " +
					$" FROM Contrato c INNER JOIN Inmueble i ON c.IdInmu = i.Id " +
					$"INNER JOIN Inquilino inq ON c.IdInqui = inq.Id ";

				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato c = new Contrato
						{
							Id = reader.GetInt32(0),
							IdInmu = reader.GetInt32(1),
							IdInqui = reader.GetInt32(2),
							FechaInicio = reader.GetDateTime(3),
							FechaFin = reader.GetDateTime(4),
							Monto = reader.GetDecimal(5),
							Estado = reader.GetBoolean(6),
							Inmueble = new Inmueble
							{
								Id = reader.GetInt32(1),
								Direccion = reader.GetString(7),
								
							},
							Inquilino = new Inquilino
							{
								Id = reader.GetInt32(2),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),

							}
						};
						res.Add(c);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Contrato ObtenerPorId(int id)
		{
			Contrato c = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT Id, IdInmu, IdInqui, FechaInicio, FechaFin, Monto, Estado FROM Contrato" +
					$" WHERE Id=@idContrato";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@idContrato", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						c = new Contrato
						{
							Id = reader.GetInt32(0),
							IdInmu = reader.GetInt32(1),
							IdInqui = reader.GetInt32(2),
							FechaInicio = reader.GetDateTime(3),
							FechaFin = reader.GetDateTime(4),
							Monto = reader.GetDecimal(5),
							Estado = reader.GetBoolean(6),
						};
					}
					connection.Close();
				}
			}
			return c;
		}
		public IList<Contrato> ObtenerPorInmuebleId(int id)
		{
			IList<Contrato> res = new List<Contrato>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT Id, IdInmu, IdInqui, FechaInicio, FechaFin, Monto, Estado, " +
					$"i.Direccion, inq.Nombre, inq.Apellido " +
					$" FROM Contrato c INNER JOIN Inmueble i ON c.IdInmu = i.Id " +
					$"INNER JOIN Inquilino inq ON c.IdInqui = inq.Id " +
					$"WHERE IdInmu = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@idInmueble", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato c = new Contrato
						{
							Id = reader.GetInt32(0),
							IdInmu = reader.GetInt32(1),
							IdInqui = reader.GetInt32(2),
							FechaInicio = reader.GetDateTime(3),
							FechaFin = reader.GetDateTime(4),
							Monto = reader.GetDecimal(5),
							Estado = reader.GetBoolean(6),
							Inmueble = new Inmueble
							{
								Id = reader.GetInt32(1),
								Direccion = reader.GetString(7),

							},
							Inquilino = new Inquilino
							{
								Id = reader.GetInt32(2),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),

							}
						};
						res.Add(c);
					}
					connection.Close();
				}
			}
			return res;
		}

		public IList<Contrato> ObtenerPorContratoId(int id)
		{
			IList<Contrato> res = new List<Contrato>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT Id, IdInmu, IdInqui, FechaInicio, FechaFin, Monto, Estado, " +
					$"i.Direccion, inq.Nombre, inq.Apellido " +
					$" FROM Contrato c INNER JOIN Inmueble i ON c.IdInmu = i.Id " +
					$"INNER JOIN Inquilino inq ON c.IdInqui = inq.Id " +
					$"WHERE IdInmu = @idInmueble";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@idContrato", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato c = new Contrato
						{
							Id = reader.GetInt32(0),
							IdInmu = reader.GetInt32(1),
							IdInqui = reader.GetInt32(2),
							FechaInicio = reader.GetDateTime(3),
							FechaFin = reader.GetDateTime(4),
							Monto = reader.GetDecimal(5),
							Estado = reader.GetBoolean(6),
							Inmueble = new Inmueble
							{
								Id = reader.GetInt32(1),
								Direccion = reader.GetString(7),
							},
							Inquilino = new Inquilino
							{
								Id = reader.GetInt32(2),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							},
														
						};
						res.Add(c);
					}
					connection.Close();
				}
			}
			return res;
		}
		public IList<Contrato> ObtenerTodosVigentes(DateTime fechaInicio, DateTime fechaFin)
		{
			IList<Contrato> res = new List<Contrato>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT Id, IdInmu, IdInqui, FechaInicio, FechaFin, Monto, c.Estado, i.Direccion, inq.Nombre, inq.Apellido  " +
					$" FROM Contrato c INNER JOIN Inmueble i ON c.IdInmu = i.Id " +
					$"INNER JOIN Inquilino inq ON c.IdInqui = inq.Id " +
					$"WHERE c.Estado = 1" +
					$"AND((FechaInicio < @fechaInicio)AND(FechaFin > @fechaFin))" +
					$"OR((FechaInicio BETWEEN @fechaInicio AND @fechaFin)AND(FechaFin BETWEEN @fechaInicio AND @fechaFin))" +
					$"OR((FechaInicio < @fechaInicio)AND(FechaFin BETWEEN @fechaInicio AND @fechaFin))" +
					$"OR((FechaInicio BETWEEN @fechaInicio AND @fechaFin)AND(FechaFin > @fechaFin));";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@fechaInicio", SqlDbType.DateTime).Value = fechaInicio;
					command.Parameters.Add("@fechaFin", SqlDbType.DateTime).Value = fechaFin;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato c = new Contrato
						{ 
						Id = reader.GetInt32(0),
							IdInmu = reader.GetInt32(1),
							IdInqui = reader.GetInt32(2),
							FechaInicio = reader.GetDateTime(3),
							FechaFin = reader.GetDateTime(4),
						    Monto = reader.GetDecimal(5),
							Estado = reader.GetBoolean(6),
							Inmueble = new Inmueble
							{
								Id = reader.GetInt32(1),
								Direccion = reader.GetString(7),
							},
							Inquilino = new Inquilino
							{
								Id = reader.GetInt32(2),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							},
														
						};
					res.Add(c);
					}
					connection.Close();
				}
			}
			return res;
		}

	}

}


