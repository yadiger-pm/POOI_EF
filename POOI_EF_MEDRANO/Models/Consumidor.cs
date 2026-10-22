using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POOI_EF_MEDRANO.Models
{
    public class Consumidor
    {
        public string DNI { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string Pais { get; set; }

        public Consumidor()
        {
        }

        public Consumidor(string dni, string nombres, string apellidos, string email, string pais)
        {
            DNI = dni;
            Nombres = nombres;
            Apellidos = apellidos;
            Email = email;
            Pais = pais;
        }
    }

}