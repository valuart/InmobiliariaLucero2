using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace InmobiliariaLucero.Models
{
    public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
	{
		public RepositorioUsuario(IConfiguration configuration) : base(configuration)
		{

		}
		public int Alta(Usuario u)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Usuario (Nombre, Apellido, Email, Rol, Clave, Avatar) " +
					$"VALUES (@nombre, @apellido, @email, @rol, @clave, @avatar);" +
					$"SELECT SCOPE_IDENTITY();";//devuelve el id insertado
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", u.Nombre);
					command.Parameters.AddWithValue("@apellido", u.Apellido);
					command.Parameters.AddWithValue("@email", u.Email);
					command.Parameters.AddWithValue("@rol", u.Rol);
					command.Parameters.AddWithValue("@clave", u.Clave);
					if (String.IsNullOrEmpty(u.Avatar))
					{
						command.Parameters.AddWithValue("@avatar", DBNull.Value);
					}
					else
					{
						command.Parameters.AddWithValue("@avatar", u.Avatar);
					}
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					u.Id = res;
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
				string sql = $"DELETE FROM Usuario WHERE Id = @idUsuario";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idUsuario", id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public int Modificacion(Usuario u)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"UPDATE Usuario SET Nombre=@nombre, Apellido=@apellido, Email=@email, Rol=@rol, Clave=@clave WHERE IdUsuario = @idUsuario";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", u.Nombre);
					command.Parameters.AddWithValue("@apellido", u.Apellido);
					command.Parameters.AddWithValue("@email", u.Email);
					command.Parameters.AddWithValue("@rol", u.Rol);
					command.Parameters.AddWithValue("@clave", u.Clave);
					//command.Parameters.AddWithValue("@avatar", u.Avatar);			
					command.Parameters.AddWithValue("@idUsuario", u.Id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Usuario> ObtenerTodos()
		{
			IList<Usuario> res = new List<Usuario>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Nombre, Apellido, Email, Rol, Clave, Avatar " +
					$" FROM Usuario";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Usuario u = new Usuario
						{
							Id = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Email = reader.GetString(3),
							Rol = reader.GetInt32(4),
							Clave = reader.GetString(5),
							Avatar = reader["Avatar"].ToString(),
							
						};
						res.Add(u);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Usuario ObtenerPorId(int id)
		{
			Usuario u = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Nombre, Apellido, Email, Rol, Clave, Avatar FROM Usuario" +
					$" WHERE Id=@idUsuario";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@idUsuario", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						u = new Usuario
						{
							Id = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Email = reader.GetString(3),
							Rol = reader.GetInt32(4),
							Clave = reader.GetString(5),
							Avatar = reader["Avatar"].ToString(),
							
						};
					}
					connection.Close();
				}
			}
			return u;
		}

		public Usuario ObtenerPorEmail(string Email)
		{
			Usuario u = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Nombre, Apellido, Email, Rol, Clave, Avatar FROM Usuario" +
					$" WHERE Email=@email";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.Add("@email", SqlDbType.VarChar).Value = Email;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						u = new Usuario
						{
							Id = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Email = reader.GetString(3),
							Rol = reader.GetInt32(4),
							Clave = reader.GetString(5),
							Avatar = reader["Avatar"].ToString(),
							
						};
					}
					connection.Close();
				}
			}
			return u;
		}

		public IList<Usuario> BuscarPorNombre(string Nombre)
		{
			List<Usuario> res = new List<Usuario>();
			Usuario u = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Nombre, Apellido, Email, Rol, Clave, Avatar FROM Usuario " +
					$" WHERE Nombre LIKE %@nombre% OR Apellido LIKE %@nombre";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@nombre", SqlDbType.VarChar).Value = Nombre;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						u = new Usuario
						{
							Id = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Email = reader.GetString(3),
							Rol = reader.GetInt32(4),
							Clave = reader.GetString(5),
							Avatar = reader["Avatar"].ToString(),
						
						};
						res.Add(u);
					}
					connection.Close();
				}
			}
			return res;
		}
	}
}
