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
using TGC.Group.Model.Menu;
using TGC.Group.Model.Camara;

namespace LosTiburones.Model
{
    public class InterfazDeCrafting
    {
        Personaje personaje;
        bool activo;
        private GameModel GModel;
        private bool recienActivo = false;
        private Drawer2D drawer2D;
        private CustomSprite sprite;
        private CustomSprite oxigeno, botiquin;
        private CustomSprite boton, boton2;
        private TgcText2D texto, texto2;

        private DXGui gui = new DXGui();
        public void Init(GameModel gmodel, Personaje personaje)
        {
            this.GModel = gmodel;
            this.personaje = personaje;


            /*
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
            texto.Text = "2 CoralBrain";
            texto.Align = TgcText2D.TextAlign.RIGHT;

            texto.Position = new Point((int)posX + 100, (int)posY + 70);
            texto.Size = new Size(100, 50);
            texto.Color = Color.Gold;




            boton.Scaling = new TGCVector2(0.3f, 0.3f);
            boton.Position = new TGCVector2(posX + 300, posY + 50);

            botiquin.Scaling = new TGCVector2(0.1f, 0.1f);
            botiquin.Position = new TGCVector2(posX + 50, posY + 100);


            texto2 = new TgcText2D();
            texto2.Text = "2 CoralBrain, 1 SeaShell";
            texto2.Align = TgcText2D.TextAlign.RIGHT;

            texto2.Position = new Point((int)posX + 100, (int)posY + 120);
            texto2.Size = new Size(160, 50);
            texto2.Color = Color.Gold;


            boton2.Scaling = new TGCVector2(0.3f, 0.3f);
            boton2.Position = new TGCVector2(posX + 300, posY + 110);
            */


            //--------------Usando GUI de Tgc----------------



            gui.Create(GModel.MediaDir);
            gui.InitDialog(false, false);

            int W = D3DDevice.Instance.Width;
            int H = D3DDevice.Instance.Height;
          
            int dy = H - 50;
            int dy2 = dy;
            int dx = W / 2;

            int posEnX = (W / 2) - (dx / 2);
            int posEnY = (H / 2) - (dy / 2);
            int x0 = posEnX + 150;
            int y0 = posEnY + 50;
            int x1 = x0;
            int y1 = y0;

            GUIItem frame = gui.InsertIFrame("", posEnX, posEnY, dx, dy, Color.FromArgb(140, 240, 140));
            frame.c_font = Color.FromArgb(0, 0, 0);
            frame.scrolleable = true;
            //----------------------
            GUIItem item = gui.InsertImage("OxygenReducida.png", x1, y1 + 30, GModel.MediaDir);
            gui.InsertItem("2 CoralBrain", x1 += 50, y1 + 20);
            gui.InsertButton(1, "Craftear", x1 += 300, y1, 120, 60);
            //-----------------------------------
            x1 = x0;
            y1 = y1 + item.image_height + 20;
            gui.InsertImage("botiquinReducido.png", x1, y1 + 30, GModel.MediaDir);
            gui.InsertItem("2CoralBrain,1SeaShell", x1 += 50, y1 + 20);
            gui.InsertButton(2, "Craftear", x1 += 300, y1, 120, 60);
            //---------------------
            /*x1 = x0;
            y1 = y1 + item.image_height+20;
            gui.InsertImage("OxygenReducida.png", x1, y1 + 30, GModel.MediaDir);
            gui.InsertItem("2 CoralBrain", x1 += 50, y1 + 20);
            gui.InsertButton(3, "Craftear", x1 += 300, y1, 120, 60);*/
            //-------------------





            /*Luego agremar los demas objetos crafteables y ver si existe o si se puede hacer una grilla para 
             * ubicar mejor los sprites*/

            activo = false;
        }

        public void Update()
        {
            if (activo && recienActivo)
            {

                var camaraInterna = (TgcFpsCamera)GModel.Camara;
                camaraInterna.LockCam = false;
                recienActivo = false;

            }
            else if (!activo && recienActivo)
            {
                var camaraInterna = (TgcFpsCamera)GModel.Camara;
                camaraInterna.LockCam = true;
                recienActivo = false;
            }
        }

        public void Render()
        {

            if (activo)
            {
                
                gui_render();

            }


            /*if (activo)
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

            }*/
            /*
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
            */

        }

        public void Dispose()
        {

            /*texto.Dispose();
            texto2.Dispose();*/
        }


        public void gui_render()
        {
            GuiMessage msg = gui.Update(GModel.ElapsedTime, GModel.Input);
            switch (msg.message)
            {
                case MessageType.WM_COMMAND:

                    switch (msg.id)
                    {
                        case 1:
                            craftingOxigeno();
                            break;
                        case 2:
                            craftingBotiquin();
                            break;
                    }


                    break;

            }

            gui.Render();


        }



        public void activar()
        {
            activo = !activo;
            recienActivo = true;
        }
        /* Luego cambiar, en vez de sumar y restar en las variables cantidades de los objetos, 
         * agregar el objeto removerlo de la lista de objetos del pesonaje*/
        private void craftingOxigeno()
        {
            if (personaje.Inventario.tieneObjetoYCantidad("BrainCoral", 2))
            {
                personaje.Inventario.agregaObjeto(new TanqueOxigeno());
                personaje.Inventario.sacarObjetoYCantidad("BrainCoral", 2);
            }
        }

        private void craftingBotiquin()
        {
            if (personaje.Inventario.tieneObjetoYCantidad("BrainCoral", 2) &&
                personaje.Inventario.tieneObjetoYCantidad("SeaShell", 1))
            {
                personaje.Inventario.agregaObjeto(new Botiquin());
                personaje.Inventario.sacarObjetoYCantidad("BrainCoral", 2);
                personaje.Inventario.sacarObjetoYCantidad("SeaShell", 1);

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
