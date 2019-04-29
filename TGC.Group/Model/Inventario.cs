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

namespace LosTiburones.Model
{
    public class Inventario
    {
        // List<Objeto> objetosColectables;


        private GameModel GModel;
        private Drawer2D drawer2D;
        private CustomSprite sprite;
        private bool activo;

        public void Init(GameModel gmodel)
        {
            this.GModel = gmodel;
            drawer2D = new Drawer2D();
            sprite = new CustomSprite();
            sprite.Bitmap = new CustomBitmap(GModel.MediaDir + "\\Texturas\\5.png", D3DDevice.Instance.Device);

            //Ubicarlo centrado en la pantalla

            var textureSize = sprite.Bitmap.Size;
            //sprite.Position = new TGCVector2(FastMath.Max(D3DDevice.Instance.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(D3DDevice.Instance.Height / 2 - textureSize.Height / 2, 0));
            sprite.Position = new TGCVector2(FastMath.Max(D3DDevice.Instance.Width / 2 - textureSize.Width * 0.5f / 2, 0), FastMath.Max(D3DDevice.Instance.Height / 2 - textureSize.Height * 0.5f / 2, 0));

            sprite.Scaling = new TGCVector2(0.5f, 0.5f);
            activo = false;


        }
        public void Update()
        {
            var Input = GModel.Input;
            if (Input.keyDown(Key.I))
            {
                activo = !activo;
            }




        }

        public void Render()
        {
            drawer2D.BeginDrawSprite();

            if (activo)
            {
                drawer2D.DrawSprite(sprite);
                drawer2D.DrawLine(new TGCVector2(1, 1), new TGCVector2(1, 200), Color.Red, 5, true);

            }
            drawer2D.EndDrawSprite();

        }
        public void Dispose()
        {
            sprite.Dispose();
        }
    }
}
