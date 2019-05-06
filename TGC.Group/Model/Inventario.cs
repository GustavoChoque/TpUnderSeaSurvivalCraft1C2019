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
        Personaje personaje;
        List<ObjetoInventario> objetos;
        List<TgcText2D> textosAMostrar;
        private GameModel GModel;
        private Drawer2D drawer2D;
        private CustomSprite sprite;
        private bool activo;

        public Inventario(GameModel gmodel, Personaje per)
        {
            objetos = new List<ObjetoInventario>();
            textosAMostrar = new List<TgcText2D>();

            this.GModel = gmodel;
            this.personaje = per;
            drawer2D = new Drawer2D();
            sprite = new CustomSprite();
            sprite.Bitmap = new CustomBitmap(GModel.MediaDir + "\\Texturas\\5.png", D3DDevice.Instance.Device);

            //Ubicarlo centrado en la pantalla

            var textureSize = sprite.Bitmap.Size;
            //sprite.Position = new TGCVector2(FastMath.Max(D3DDevice.Instance.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(D3DDevice.Instance.Height / 2 - textureSize.Height / 2, 0));
            sprite.Position = new TGCVector2(FastMath.Max(D3DDevice.Instance.Width / 2 - textureSize.Width * 0.5f / 2, 0), FastMath.Max(D3DDevice.Instance.Height / 2 - textureSize.Height * 0.5f / 2, 0));

            sprite.Scaling = new TGCVector2(0.5f, 0.5f);

            sprite.Color = Color.Blue;


            activo = false;
        }
        public void Update()
        {
            var Input = GModel.Input;
            if (Input.keyPressed(Key.I))
            {
                activo = !activo;
                if (activo)
                {
                    limpiarListaDeTextoAMostrar();
                    llenarListaDeTextoAMostrar();
                }
            }
        }

        public void Render()
        {
            

            if (activo)
            {
                drawer2D.BeginDrawSprite();
                drawer2D.DrawSprite(sprite);
                drawer2D.DrawLine(new TGCVector2(1, 1), new TGCVector2(1, 200), Color.Red, 5, true);
                drawer2D.EndDrawSprite();
                textosAMostrar.ForEach(texto => texto.render());
            }
           

        }
        public void Dispose()
        {
            sprite.Dispose();
            limpiarListaDeTextoAMostrar();

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

        private void limpiarListaDeTextoAMostrar()
        {
            //textosAMostrar.ForEach(texto => texto.Dispose());
            textosAMostrar.Clear();
        }

        private void llenarListaDeTextoAMostrar()
        {
            objetos.ForEach(objeto => textoParaMostrar(objeto));
        }

        private void textoParaMostrar(ObjetoInventario objeto)
        {
            var textureSize = sprite.Bitmap.Size;
            TgcText2D texto = new TgcText2D();
            texto.Text = objeto.Nombre + "-----" + objeto.Cantidad.ToString();
            texto.Align = TgcText2D.TextAlign.RIGHT;
            texto.Position = new Point((int)FastMath.Max(D3DDevice.Instance.Width / 2 - textureSize.Width * 0.5f / 2, 0), 150 + (10 * textosAMostrar.Count));
            texto.Size = new Size(300, 100);
            texto.Color = Color.Gold;
            textosAMostrar.Add(texto);
        }

    }
}
