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
        private TgcText2D textoErrorCrafteo;
        private TgcText2D textoCrafteoExitoso;
        private bool renderizoTextoError = false;
        private bool renderizoTextoExito = false;
        private bool huboErrorCrafteo = false;
        private bool huboCrafteoExitoso = false;
        private float acumuloTiempo = 0;

        private DXGui gui = new DXGui();
        public void Init(GameModel gmodel, Personaje personaje)
        {
            this.GModel = gmodel;
            this.personaje = personaje;

            textoErrorCrafteo = new TgcText2D();
            textoErrorCrafteo.Text = "No se pudo craftear el elemento, faltan items.";
            textoErrorCrafteo.Align = TgcText2D.TextAlign.CENTER;
            textoErrorCrafteo.Position = new Point(D3DDevice.Instance.Device.Viewport.Width / 3, D3DDevice.Instance.Device.Viewport.Height / 2);
            textoErrorCrafteo.Size = new Size(500, 500);
            textoErrorCrafteo.Color = Color.Red;

            textoCrafteoExitoso = new TgcText2D();
            textoCrafteoExitoso.Text = "Elemento crafteado con éxito!";
            textoCrafteoExitoso.Align = TgcText2D.TextAlign.CENTER;
            textoCrafteoExitoso.Position = new Point(D3DDevice.Instance.Device.Viewport.Width / 3, D3DDevice.Instance.Device.Viewport.Height / 2);
            textoCrafteoExitoso.Size = new Size(500, 500);
            textoCrafteoExitoso.Color = Color.LawnGreen;
            
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

            //Muestro el error por 2 segundos
            acumuloTiempo = acumuloTiempo + GModel.ElapsedTime;
            if (acumuloTiempo > 2 && huboErrorCrafteo)
            {
                renderizoTextoError = false;
            }
            if (acumuloTiempo > 2 && huboCrafteoExitoso)
            {
                renderizoTextoExito = false;
            }

        }

        public void Render()
        {

            if (activo)
            {
                
                gui_render();

            }

            if (renderizoTextoError)
            {
                textoErrorCrafteo.render();
            }
            else if (renderizoTextoExito)
            {
                textoCrafteoExitoso.render();
            }       

        }

        public void Dispose()
        {
            
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
                        case 3:
                            craftingArma();
                            break;
                        case 4:
                            craftingRedPesca();
                            break;
                    }

                    break;

            }

            gui.Render();


        }

        private void craftingRedPesca()
        {
            if (personaje.Inventario.tieneObjetoYCantidad("Platino", 4))
            {
                personaje.Inventario.agregaObjeto(new RedPesca());
                personaje.Inventario.sacarObjetoYCantidad("Platino", 4);
                huboErrorCrafteo = false;
                renderizoTextoExito = true;
                huboCrafteoExitoso = true;
            }
            else
            {
                renderizoTextoError = true;
                huboErrorCrafteo = true;
            }

            acumuloTiempo = 0;

            //Despues de craftear un item cierro el menu
            var camaraInterna = (TgcFpsCamera)GModel.Camara;
            camaraInterna.LockCam = true;
            recienActivo = false;
            activo = false;
            gui.Reset();
        }

        private void craftingArma()
        {
            if (personaje.Inventario.tieneObjetoYCantidad("Oro", 3))
            {
                personaje.Inventario.agregaObjeto(new Arma());
                personaje.Inventario.sacarObjetoYCantidad("Oro", 3);
                huboErrorCrafteo = false;
                renderizoTextoExito = true;
                huboCrafteoExitoso = true;
            }
            else
            {
                renderizoTextoError = true;
                huboErrorCrafteo = true;
            }

            acumuloTiempo = 0;

            //Despues de craftear un item cierro el menu
            var camaraInterna = (TgcFpsCamera)GModel.Camara;
            camaraInterna.LockCam = true;
            recienActivo = false;
            activo = false;
            gui.Reset();
        }

        public void activar()
        {
            activo = !activo;
            recienActivo = true;

            if (activo)
            {
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
                x1 = x0;
                y1 = y1 + item.image_height + 20;
                gui.InsertImage("ArponReducido.png", x1, y1 + 30, GModel.MediaDir);
                gui.InsertItem("3 Oro", x1 += 50, y1 + 20);
                gui.InsertButton(3, "Craftear", x1 += 300, y1, 120, 60);
                //---------------------
                x1 = x0;
                y1 = y1 + item.image_height + 20;
                gui.InsertImage("RedPescaReducido.png", x1, y1 + 30, GModel.MediaDir); 
                gui.InsertItem("4 Platino", x1 += 50, y1 + 20);
                gui.InsertButton(4, "Craftear", x1 += 300, y1, 120, 60);
            }
        }
        /* Luego cambiar, en vez de sumar y restar en las variables cantidades de los objetos, 
         * agregar el objeto removerlo de la lista de objetos del pesonaje*/
        private void craftingOxigeno()
        {
            if (personaje.Inventario.tieneObjetoYCantidad("BrainCoral", 2))
            {
                personaje.Inventario.agregaObjeto(new TanqueOxigeno());
                personaje.Inventario.sacarObjetoYCantidad("BrainCoral", 2);
                huboErrorCrafteo = false;
                renderizoTextoExito = true;
                huboCrafteoExitoso = true;
            }
            else
            {
                renderizoTextoError = true;
                huboErrorCrafteo = true;
            }

            acumuloTiempo = 0;

            //Despues de craftear un item cierro el menu
            var camaraInterna = (TgcFpsCamera)GModel.Camara;
            camaraInterna.LockCam = true;
            recienActivo = false;
            activo = false;
            gui.Reset();
        }

        private void craftingBotiquin()
        {
            if (personaje.Inventario.tieneObjetoYCantidad("BrainCoral", 2) &&
                personaje.Inventario.tieneObjetoYCantidad("SeaShell", 1))
            {
                personaje.Inventario.agregaObjeto(new Botiquin());
                personaje.Inventario.sacarObjetoYCantidad("BrainCoral", 2);
                personaje.Inventario.sacarObjetoYCantidad("SeaShell", 1);
                huboErrorCrafteo = false;
                renderizoTextoExito = true;
                huboCrafteoExitoso = true;
                acumuloTiempo = 0;
            }
            else
            {
                renderizoTextoError = true;
                huboErrorCrafteo = true;
                acumuloTiempo = 0;
            }

            //Despues de craftear un item cierro el menu
            var camaraInterna = (TgcFpsCamera)GModel.Camara;
            camaraInterna.LockCam = true;
            recienActivo = false;
            activo = false;
            gui.Reset();
        }

        public Boolean Activo { get => activo; }


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
