using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using TGC.Group.Model.Sprites;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using Microsoft.DirectX.DirectInput;
using TGC.Group.Model;
using TGC.Core.Text;

namespace LosTiburones.Model
{
    public class Inventario
    {
        private List<ObjetoInventario> objetos;

        public Inventario()
        {
            objetos = new List<ObjetoInventario>();
        }

        //Agrega objetos controlando la no repeticion
        public void agregaObjeto(ObjetoInventario objetoAAgregar)
        {
            ObjetoInventario obj = objetos.Find(objeto => objeto.Nombre.Equals(objetoAAgregar.Nombre));
            if (obj != null)
            {
                obj.Cantidad += objetoAAgregar.Cantidad;
            }
            else
            {
                objetos.Add(objetoAAgregar);
            }
        }

        //Saca una cantidad de un objeto del inventario y si es 0 lo elimina. El metodo NO checkea que el objeto exista y que el resultado de la reduccion sea justo 0
        public void sacarObjetoYCantidad(String nombre, int cantidad)
        {
            ObjetoInventario obj = objetos.Find(objeto => objeto.Nombre.Equals(nombre));
            obj.Cantidad -= cantidad;
            if (obj.Cantidad < 1)
            {
                objetos.Remove(obj);
            }
        }

        //Busca en la lista de objetos por nombre y devuelte true si hay cantidad o mas
        public bool tieneObjetoYCantidad(String nombre, int cantidad)
        {
            return objetos.Exists(objeto => objeto.Nombre.Equals(nombre) && objeto.Cantidad >= cantidad);
        }

        public List<ObjetoInventario> Objetos { get => objetos; }

    }
}
