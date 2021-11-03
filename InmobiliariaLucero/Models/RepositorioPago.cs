using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace InmobiliariaLucero.Models
{
    public class RepositorioPago : RepositorioBase, IRepositorioPago
	{
		public RepositorioPago(IConfiguration configuration) : base(configuration)
		{

		}

		public int Alta(Pago pa)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Pago (NroPago, FechaPago, Importe, IdCon) " +
					$"VALUES (@nroPago, @fechaPago, @importe, @idcontrato);" +
					$"SELECT SCOPE_IDENTITY();";//devuelve el id insertado
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nroPago", pa.NroPago);
					command.Parameters.AddWithValue("@fechaPago", pa.FechaPago);
					command.Parameters.AddWithValue("@importe", pa.Importe);
					command.Parameters.AddWithValue("@idContrato", pa.IdCon);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					pa.Id= res;
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
				string sql = $"DELETE FROM Pago WHERE Id= @idPago";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idPago", id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public int Modificacion(Pago pa)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"UPDATE Pago SET NroPago=@nroPago, FechaPago=@fechaPago, " +
					$"Importe=@importe, IdCon=@idContrato " +
					$"WHERE Id = @idPago";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{

					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nroPago", pa.NroPago);
					command.Parameters.AddWithValue("@fechaPago", pa.FechaPago);
					command.Parameters.AddWithValue("@importe", pa.Importe);
					command.Parameters.AddWithValue("@idContrato", pa.IdCon);
					command.Parameters.AddWithValue("@idPago", pa.Id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Pago> ObtenerTodos()
		{
			IList<Pago> res = new List<Pago>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{

				string sql = $"SELECT pa.Id, NroPago, FechaPago, Importe, IdCon, " +
					"c.IdInmu, i.Direccion, c.IdInqui, inq.Apellido,  " +
					"c.FechaInicio, c.FechaFin, c.Monto, c.Estado " +
					"FROM Pago pa, Contrato c, Inmueble i, Inquilino inq " +
					"WHERE pa.IdCon = c.Id AND c.IdInmu = i.Id " +
					"AND c.IdInqui = inq.Id; ";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Pago pa = new Pago
						{
							Id = reader.GetInt32(0),
							NroPago = reader.GetInt32(1),
							FechaPago = reader.GetDateTime(2),
							Importe = reader.GetDecimal(3),
							IdCon = reader.GetInt32(4),
							Contrato = new Contrato
							{
								Id = reader.GetInt32(4),
								IdInmu = reader.GetInt32(5),
								Inmueble = new Inmueble
								{
									Id = reader.GetInt32(5),
									Direccion = reader.GetString(6),
								},
								IdInqui = reader.GetInt32(7),
								Inquilino = new Inquilino
								{
									Id = reader.GetInt32(7),
									Apellido = reader.GetString(8)
								},
								FechaInicio = reader.GetDateTime(9),
								FechaFin = reader.GetDateTime(10),
								Monto = reader.GetDecimal(11),
								Estado = reader.GetBoolean(12),
								
							}
						};
						res.Add(pa);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Pago ObtenerPorId(int id)
		{
			Pago pa = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT pa.Id, NroPago, FechaPago, Importe, IdCon, ca.IdInmu, ca.IdInqui, ca.FechaInicio, ca.FechaFin, ca.Monto, ca.Estado  " +
					$"FROM Pago pa INNER JOIN Contrato ca ON pa.IdCon = ca.Id" +
					$" WHERE pa.Id=@idPago";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@idPago", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						pa = new Pago
						{
							Id = reader.GetInt32(0),
							NroPago = reader.GetInt32(1),
							FechaPago = reader.GetDateTime(2),
							Importe = reader.GetDecimal(3),
							IdCon = reader.GetInt32(4),
							Contrato = new Contrato
							{
								Id = reader.GetInt32(4),
								IdInmu = reader.GetInt32(5),
								Inmueble = new Inmueble
								{
									Id = reader.GetInt32(5),
								
								},
								IdInqui = reader.GetInt32(6),
								Inquilino = new Inquilino
								{
									Id = reader.GetInt32(6),
								
								},
								FechaInicio = reader.GetDateTime(7),
								FechaFin = reader.GetDateTime(8),
								Monto = reader.GetDecimal(9),
								Estado = reader.GetBoolean(10),
							}
						};
					}
					connection.Close();
				}
			}
			return pa;
		}
		public IList<Pago> ObtenerTodosPorIdContrato(int Idcontrato)
		{
			IList<Pago> res = new List<Pago>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT pa.Id, NroPago, FechaPago, Importe, IdCon, " +
					$" c.IdInmu, i.Direccion, c.IdInqui, inq.Apellido, c.FechaInicio, c.FechaFin, c.Monto, c.Estado " +
					$" FROM Pago pa, Contrato c, " +
					"Inmueble i, Inquilino inq WHERE pa.IdCon = c.Id " +
					"AND c.IdInmu = i.Id " +
					"AND c.IdInqui = inq.Id AND pa.IdCon = @idContrato; ";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@idContrato", SqlDbType.Int).Value = Idcontrato;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Pago pa = new Pago
						{
							Id = reader.GetInt32(0),
							NroPago = reader.GetInt32(1),
							FechaPago = reader.GetDateTime(2),
							Importe = reader.GetDecimal(3),
							IdCon = reader.GetInt32(4),
							Contrato = new Contrato
							{
								Id = reader.GetInt32(4),
								IdInmu = reader.GetInt32(5),
								Inmueble = new Inmueble
								{
									Id = reader.GetInt32(5),
									Direccion = reader.GetString(6),
								},
								IdInqui = reader.GetInt32(7),
								Inquilino = new Inquilino
								{
									Id = reader.GetInt32(7),
									Apellido = reader.GetString(8)
								},
								FechaInicio = reader.GetDateTime(9),
								FechaFin = reader.GetDateTime(10),
								Monto = reader.GetDecimal(11),
								Estado = reader.GetBoolean(12),

							}

						};
						res.Add(pa);
					}
					connection.Close();
				}
			}
			return res;
		}
	}
}
