using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosTiburones.Model
{
    public class ObjetoInventario
    {
        public ObjetoInventario(String nombre, int cantidad)
        {
            Nombre = nombre;
            Cantidad = cantidad;
        }

        private String nombre;
        private int cantidad;

        public String Nombre { get => nombre; set => nombre = value; }
        public int Cantidad { get => cantidad; set => cantidad = value; }
    }
}
