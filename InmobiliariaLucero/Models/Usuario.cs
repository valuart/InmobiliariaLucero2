﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaLucero.Models
{
      public enum enRoles
          {
         SuperAdministrador = 1,
          Administrador = 2,
          Empleado = 3,
      } 


    public class Usuario
{
    [Key]
    [DisplayName("Codigo")]
    public int Id { get; set; }
    [StringLength(20, MinimumLength = 3, ErrorMessage = "La longitud del {0} deberia ser entre {2} y {1}."), RegularExpression("^[A-Za-z]+$", ErrorMessage = "Solo se permiten letras")]
    public string Nombre { get; set; }
    [StringLength(20, MinimumLength = 2, ErrorMessage = "La longitud del {0} deberia ser entre {2} y {1}."), RegularExpression("^[A-Za-z]+$", ErrorMessage = "Solo se permiten letras")]
    public string Apellido { get; set; }
    [Required, EmailAddress]
    public string Email { get; set; }
    public int Rol { get; set; }
    [Required, DataType(DataType.Password)]
    public string Clave { get; set; }
    public string Avatar { get; set; }
    [NotMapped]
    public IFormFile AvatarFile { get; set; }
        [DisplayName("Rol")]
        public string RolNombre => Rol > 0 ? ((enRoles)Rol).ToString() : "";

        public static IDictionary<int, string> ObtenerRoles()
       {
            SortedDictionary<int, string> roles = new SortedDictionary<int, string>();
            Type tipoEnumRol = typeof(enRoles);
            foreach (var valor in Enum.GetValues(tipoEnumRol))
            {
                roles.Add((int)valor, Enum.GetName(tipoEnumRol, valor));
            }
           return roles;
        } 



    }
}
