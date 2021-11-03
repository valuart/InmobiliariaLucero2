using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaLucero.Models
{
    public class RepositorioInquilino : RepositorioBase, IRepositorio<Inquilino>
	{
		public RepositorioInquilino(IConfiguration configuration) : base(configuration)
		{

		}


		public int Alta(Inquilino inq)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Inquilino (Nombre, Apellido, Dni, Telefono, Email) " +
					$"VALUES (@nombre, @apellido, @dni, @telefono, @email);" +
					$"SELECT SCOPE_IDENTITY();";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", inq.Nombre);
					command.Parameters.AddWithValue("@apellido", inq.Apellido);
					command.Parameters.AddWithValue("@dni", inq.Dni);
					command.Parameters.AddWithValue("@telefono", inq.Telefono);
					command.Parameters.AddWithValue("@email", inq.Email);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					inq.Id = res;
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
				string sql = $"DELETE FROM Inquilino WHERE Id = {id}";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public int Modificacion(Inquilino inq)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"UPDATE Inquilino SET " +
					$"Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email " +
					$"WHERE Id = @idInquilino";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", inq.Nombre);
					command.Parameters.AddWithValue("@apellido", inq.Apellido);
					command.Parameters.AddWithValue("@dni", inq.Dni);
					command.Parameters.AddWithValue("@telefono", inq.Telefono);
					command.Parameters.AddWithValue("@email", inq.Email);
					command.Parameters.AddWithValue("@idInquilino", inq.Id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Inquilino> ObtenerTodos()
		{
			IList<Inquilino> res = new List<Inquilino>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email FROM Inquilino" +
					$" ORDER BY Apellido, Nombre";

				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inquilino inq = new Inquilino
						{
							Id = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Dni = reader.GetString(3),
							Telefono = reader.GetString(4),
							Email = reader.GetString(5),
						};
						res.Add(inq);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Inquilino ObtenerPorId(int id)
		{
			Inquilino inq = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email FROM Inquilino" +
					$" WHERE Id=@idInquilino";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.Add("@idInquilino", SqlDbType.Int).Value = id;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						inq = new Inquilino
						{
							Id = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Dni = reader.GetString(3),
							Telefono = reader.GetString(4),
							Email = reader.GetString(5),
						};
						return inq;
					}
					connection.Close();
				}
			}
			return inq;
		}
	}
}

