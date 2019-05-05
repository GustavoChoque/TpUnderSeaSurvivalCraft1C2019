using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Group.Model.Sprites;
using System.Windows.Forms;
using TGC.Core.Input;
using System.Drawing;
using TGC.Core.Text;
using TGC.Group.Model;

namespace LosTiburones.Model
{
    public class InterfazDeCrafting
    {
        Personaje personaje;
        bool activo;
        private GameModel GModel;
        private Drawer2D drawer2D;
        private CustomSprite sprite;
        private CustomSprite oxigeno, botiquin;
        private CustomSprite boton, boton2;
        private TgcText2D texto, texto2;


        public void Init(GameModel gmodel, Personaje personaje)
        {
            this.GModel = gmodel;
            this.personaje = personaje;



            drawer2D = new Drawer2D();

            sprite = new CustomSprite();
            oxigeno = new CustomSprite();
            boton = new CustomSprite();

            botiquin = new CustomSprite();
            boton2 = new CustomSprite();


            sprite.Bitmap = new CustomBitmap(GModel.MediaDir + "\\Texturas\\5.png", D3DDevice.Instance.Device);
            oxigeno.Bitmap = new CustomBitmap(GModel.MediaDir + "\\Bitmaps\\Oxygen.png", D3DDevice.Instance.Device);
            boton.Bitmap = new CustomBitmap(GModel.MediaDir + "\\Texturas\\Botones\\BotonCrafteo.png", D3DDevice.Instance.Device);

            botiquin.Bitmap = new CustomBitmap(GModel.MediaDir + "\\Bitmaps\\Botiquin.png", D3DDevice.Instance.Device);
            boton2.Bitmap = new CustomBitmap(GModel.MediaDir + "\\Texturas\\Botones\\BotonCrafteo.png", D3DDevice.Instance.Device);


            //Ubicarlo centrado en la pantalla

            var textureSize = sprite.Bitmap.Size;

            var posX = FastMath.Max(D3DDevice.Instance.Width / 2 - textureSize.Width * 0.5f / 2, 0);
            var posY = FastMath.Max(D3DDevice.Instance.Height / 2 - textureSize.Height * 0.5f / 2, 0);

            //sprite.Position = new TGCVector2(FastMath.Max(D3DDevice.Instance.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(D3DDevice.Instance.Height / 2 - textureSize.Height / 2, 0));
            // sprite.Position = new TGCVector2(FastMath.Max(D3DDevice.Instance.Width / 2 - textureSize.Width * 0.5f / 2, 0), FastMath.Max(D3DDevice.Instance.Height / 2 - textureSize.Height * 0.5f / 2, 0));
            sprite.Position = new TGCVector2(posX, posY);


            sprite.Scaling = new TGCVector2(0.5f, 0.5f);
            sprite.Color = Color.Red;


            oxigeno.Scaling = new TGCVector2(0.1f, 0.1f);
            //oxigeno.Position = new TGCVector2(FastMath.Max(D3DDevice.Instance.Width / 2 - textureSize.Width * 0.5f / 2, 0)+50, FastMath.Max(D3DDevice.Instance.Height / 2 - textureSize.Height * 0.5f / 2, 0)+50);
            oxigeno.Position = new TGCVector2(posX + 50, posY + 50);


            texto = new TgcText2D();
            texto.Text = "2CoralBrain";
            texto.Align = TgcText2D.TextAlign.RIGHT;

            texto.Position = new Point((int)posX + 100, (int)posY + 70);
            texto.Size = new Size(100, 50);
            texto.Color = Color.Gold;




            boton.Scaling = new TGCVector2(0.3f, 0.3f);
            boton.Position = new TGCVector2(posX + 300, posY + 50);

            botiquin.Scaling = new TGCVector2(0.1f, 0.1f);
            botiquin.Position = new TGCVector2(posX + 50, posY + 100);


            texto2 = new TgcText2D();
            texto2.Text = "2CoralBrain, 1SeaShell";
            texto2.Align = TgcText2D.TextAlign.RIGHT;

            texto2.Position = new Point((int)posX + 100, (int)posY + 120);
            texto2.Size = new Size(160, 50);
            texto2.Color = Color.Gold;


            boton2.Scaling = new TGCVector2(0.3f, 0.3f);
            boton2.Position = new TGCVector2(posX + 300, posY + 110);

            /*Luego agremar los demas objetos crafteables y ver si existe o si se puede hacer una grilla para 
             * ubicar mejor los sprites*/

            activo = false;
        }

        public void Update()
        {



        }

        public void Render()
        {




            if (activo)
            {
                drawer2D.BeginDrawSprite();
                drawer2D.DrawSprite(sprite);
                drawer2D.DrawSprite(oxigeno);

                drawer2D.DrawSprite(boton);
                drawer2D.DrawSprite(botiquin);
                drawer2D.DrawSprite(boton2);
                drawer2D.EndDrawSprite();
                texto.render();
                texto2.render();

            }

            var Input = GModel.Input;
            if (Input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT) && sobreSprite(boton, 0.3f))
            {

                //GModel.DrawText.drawText("crafteo exito", 500, 500, Color.OrangeRed);
                craftingOxigeno();

            }

            if (Input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT) && sobreSprite(boton2, 0.3f))
            {

                //GModel.DrawText.drawText("crafteo exito", 500, 500, Color.OrangeRed);
                craftingBotiquin();

            }


        }

        public void Dispose()
        {

            texto.Dispose();
            texto2.Dispose();
        }

        public void activar()
        {

            activo = !activo;
        }
        /* Luego cambiar, en vez de sumar y restar en las variables cantidades de los objetos, 
         * agregar el objeto removerlo de la lista de objetos del pesonaje*/
        private void craftingOxigeno()
        {
            if (personaje.inventario.Exists(o => o.nombre.Equals("BrainCoral") && o.cantidad >= 2))
            {
                personaje.inventario.Find(o => o.nombre.Equals("TanqueOxigeno")).cantidad++;
                personaje.inventario.Find(o => o.nombre.Equals("BrainCoral")).cantidad -= 2;
            }


        }

        private void craftingBotiquin()
        {
            if (personaje.inventario.Exists(o1 => o1.nombre.Equals("BrainCoral") && o1.cantidad >= 2) &&
                personaje.inventario.Exists(o2 => o2.nombre.Equals("SeaShell") && o2.cantidad >= 1))
            {
                personaje.inventario.Find(o => o.nombre.Equals("Botiquin")).cantidad++;
                personaje.inventario.Find(o1 => o1.nombre.Equals("BrainCoral")).cantidad -= 2;
                personaje.inventario.Find(o2 => o2.nombre.Equals("SeaShell")).cantidad--;

            }

        }


        private bool sobreSprite(CustomSprite sprite, float escalaSprite)
        {
            /* me dice si me encuentro por encima del sprite, (buscar luego porque me detecta en Y que esta 
             * corrido un poco para arriba)*/
            return
                Cursor.Position.X > sprite.Position.X &&
                Cursor.Position.X < sprite.Position.X + sprite.Bitmap.Size.Width * escalaSprite &&
                Cursor.Position.Y > sprite.Position.Y &&
                Cursor.Position.Y < sprite.Position.Y + sprite.Bitmap.Size.Height * escalaSprite;

        }
    }
}
