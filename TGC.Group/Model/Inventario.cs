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


        private GameModel GModel;
        private Drawer2D drawer2D;
        private CustomSprite sprite;
        private TgcText2D texto, texto2,texto3, texto4;
        private bool activo;

        public void Init(GameModel gmodel, Personaje per)
        {
            
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
            //---------texto para inventario-------
            texto = new TgcText2D();
            texto.Text = this.personaje.inventario[0].nombre + "-----" + this.personaje.inventario[0].cantidad;
            texto.Align = TgcText2D.TextAlign.RIGHT;
            //ver luego como hacer que el sprite no tape el texto
            texto.Position = new Point((int)FastMath.Max(D3DDevice.Instance.Width / 2 - textureSize.Width * 0.5f / 2, 0), 150);
            texto.Size = new Size(300, 100);
            texto.Color = Color.Gold;

            texto2 = new TgcText2D();
            texto2.Text = this.personaje.inventario[1].nombre + "-----" + this.personaje.inventario[1].cantidad;
            texto2.Align = TgcText2D.TextAlign.RIGHT;
            //ver luego como hacer que el sprite no tape el texto
            texto2.Position = new Point((int)FastMath.Max(D3DDevice.Instance.Width / 2 - textureSize.Width * 0.5f / 2, 0), 160);
            texto2.Size = new Size(300, 100);
            texto2.Color = Color.Gold;

            texto3 = new TgcText2D();
            texto3.Text = this.personaje.inventario[2].nombre + "-----" + this.personaje.inventario[2].cantidad;
            texto3.Align = TgcText2D.TextAlign.RIGHT;

            texto3.Position = new Point((int)FastMath.Max(D3DDevice.Instance.Width / 2 - textureSize.Width * 0.5f / 2, 0), 170);
            texto3.Size = new Size(300, 100);
            texto3.Color = Color.Gold;

            texto4 = new TgcText2D();
            texto4.Text = this.personaje.inventario[3].nombre + "-----" + this.personaje.inventario[3].cantidad;
            texto4.Align = TgcText2D.TextAlign.RIGHT;

            texto4.Position = new Point((int)FastMath.Max(D3DDevice.Instance.Width / 2 - textureSize.Width * 0.5f / 2, 0), 180);
            texto4.Size = new Size(300, 100);
            texto4.Color = Color.Gold;

        }
        public void Update()
        {
            var Input = GModel.Input;
            if (Input.keyPressed(Key.I))
            {
                activo = !activo;
            }

            texto.Text = this.personaje.inventario[0].nombre + "-----" + this.personaje.inventario[0].cantidad;
            texto2.Text = this.personaje.inventario[1].nombre + "-----" + this.personaje.inventario[1].cantidad;
            texto3.Text = this.personaje.inventario[2].nombre + "-----" + this.personaje.inventario[2].cantidad;
            texto4.Text = this.personaje.inventario[3].nombre + "-----" + this.personaje.inventario[3].cantidad;



        }

        public void Render()
        {
            

            if (activo)
            {
                drawer2D.BeginDrawSprite();
                drawer2D.DrawSprite(sprite);
                drawer2D.DrawLine(new TGCVector2(1, 1), new TGCVector2(1, 200), Color.Red, 5, true);
                drawer2D.EndDrawSprite();
                texto.render();
                texto2.render();
                texto3.render();
                texto4.render();
            }
           

        }
        public void Dispose()
        {
            sprite.Dispose();
            texto.Dispose();
            texto2.Dispose();
            texto3.Dispose();
            texto4.Dispose();

        }
    }
}
