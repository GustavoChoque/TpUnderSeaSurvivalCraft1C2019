using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Sound;
using TGC.Core.Terrain;
using TGC.Core.Textures;
using TGC.Group.Model;
using TGC.Group.Model.Camara;
using TGC.Group.Model.Sprites;

namespace LosTiburones.Model
{
    public class Escenario
    {   //DECLARO VARIABLES 'GLOBALES'
        GameModel GModel;
        
        private TgcPlane piso, agua;
        private TgcMesh coralBrain, coral, meshTiburon, fish, pillarCoral, seaShell, spiralWireCoral, treeCoral, yellowFish;
        private TgcMesh arbusto, arbusto2, pasto, planta, planta2, planta3, roca;
        private TgcScene barco;
        private TgcSkyBox skybox;

        private TgcSimpleTerrain terreno;
        private float currentScaleXZ;
        private float currentScaleY;

        private TgcMp3Player musica;

        public List<TgcMesh> objetosEstaticosEnArray = new List<TgcMesh>();
        private List<Pez> pecesAmarillos = new List<Pez>();
        private List<Pez> pecesAzules = new List<Pez>();
        private List<TGCBox> metales = new List<TGCBox>(); //hacer clase metales?
        private Pez pezCircular;

        private Tiburon tiburon;

        //Constantes para velocidades de movimiento
        private const float ROTATION_SPEED = 50f;
        private const float MOVEMENT_SPEED = 50f;

        private Random rnd = new Random();

        private CustomBitmap bitmapCursor;
        private CustomBitmap bitmapCorazon, bitmapTanqueOxigeno, bitmapBarra00;
        private CustomBitmap bitmapBarraVida10, bitmapBarraVida20, bitmapBarraVida30, bitmapBarraVida40, bitmapBarraVida50,
                bitmapBarraVida60, bitmapBarraVida70, bitmapBarraVida80, bitmapBarraVida90, bitmapBarraVida100;
        private CustomBitmap bitmapBarraOxigeno10, bitmapBarraOxigeno20, bitmapBarraOxigeno30, bitmapBarraOxigeno40, bitmapBarraOxigeno50,
                bitmapBarraOxigeno60, bitmapBarraOxigeno70, bitmapBarraOxigeno80, bitmapBarraOxigeno90, bitmapBarraOxigeno100;
        private Drawer2D spriteDrawer = new Drawer2D();

        private List<CustomSprite> sprites = new List<CustomSprite>();

        private CustomSprite spriteCursor = new CustomSprite();
        private CustomSprite spriteCorazon = new CustomSprite();
        private CustomSprite spriteBarraVida = new CustomSprite();
        private CustomSprite spriteBarraOxigeno = new CustomSprite();
        private CustomSprite spriteTanqueOxigeno = new CustomSprite();

        private int ScreenWidth = D3DDevice.Instance.Device.Viewport.Width;
        private int ScreenHeight = D3DDevice.Instance.Device.Viewport.Height;

        private Personaje personaje = new Personaje(100, 100);
        private float tiempoBajoElAgua = 0;
        float tiempoQueAguantaBajoElAgua = 10;
        float tiempoRestanteBajoElAgua;

        public List<TGCBox> Metales { get => metales; }

        public void Init(GameModel gmodel)
        {
            this.GModel = gmodel;
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

            this.cargoPisos();

            this.cargoSkybox();

            this.cargoHeightmap();

            this.cargoMusica();

            this.cargoMeshes();

            this.generoObjetosEstaticosSueltos();

            this.generoObjetosEstaticosEnArray();

            this.generoPecesAmarillos();

            this.generoPecesAzules();

            this.generoMetales();

            this.generoHUD();

            tiburon.Init(gmodel);

        }
        public void Update()
        {
            //Capturar Input teclado
            //if (Input.keyPressed(Key.F))
            //{
            //    BoundingBox = !BoundingBox;
            //}

            var Input=this.GModel.Input;
            //Capturar Input teclado
            if (Input.keyPressed(Key.P))
            {
                if (musica.getStatus().Equals(TgcMp3Player.States.Playing))
                {
                    musica.pause();
                }
                else if (musica.getStatus().Equals(TgcMp3Player.States.Paused))
                {
                    musica.resume();
                }
            }

            //Reseteo el juego si apreto R
            //if (Input.keyPressed(Key.R))
            //{
            //this.Dispose();
            //this.Init();
            //}

            //-----------movimientos-------------
            tiburon.moverse(this);


            //-----------
            //Muevo los peces amarillos

            pezCircular.Update(GModel.ElapsedTime);

            pecesAmarillos.ForEach(pez =>
            {
                pez.Position += new TGCVector3(5f * pez.MoveSpeed * GModel.ElapsedTime * pez.CurrentMoveDir, 0, 0);

                if (rnd.Next(0, 2000) > rnd.Next(1998, 1999) //cada tanto
                    || this.fueraDelMapa(pez)) //si toco los bordes
                {
                    //reboto
                    pez.CurrentMoveDir *= -1;
                    pez.Rotation += new TGCVector3(FastMath.PI, 0, 0);
                }

                pez.Transform = TGCMatrix.Scaling(pez.Scale) * TGCMatrix.RotationYawPitchRoll(pez.Rotation.X, pez.Rotation.Y, pez.Rotation.Z) * TGCMatrix.Translation(pez.Position);
            }
            );

            //-----------
            //Muevo los peces azules

            pecesAzules.ForEach(pez =>
            {
                pez.Position += new TGCVector3(0, 0, 2f * pez.MoveSpeed * GModel.ElapsedTime * pez.CurrentMoveDir);

                if (rnd.Next(0, 2000) > rnd.Next(1998, 1999) //cada tanto
                    || this.fueraDelMapa(pez)) //si toco los bordes
                {
                    //reboto
                    pez.CurrentMoveDir *= -1;
                    pez.Rotation += new TGCVector3(FastMath.PI, 0, 0);
                }

                pez.Transform = TGCMatrix.Scaling(pez.Scale) * TGCMatrix.RotationYawPitchRoll(pez.Rotation.X, pez.Rotation.Y, pez.Rotation.Z) * TGCMatrix.Translation(pez.Position);
            }
            );

            //-----Skybox
            skybox.Center = GModel.Camara.Position;
            //----------

            //REFRESCO EL TAMAÑO DE LA PANTALLA
            ScreenWidth = D3DDevice.Instance.Device.Viewport.Width;
            ScreenHeight = D3DDevice.Instance.Device.Viewport.Height;

            //CALCULO TIEMPO RESTANTE DE OXIGENO Y VIDA
            tiempoRestanteBajoElAgua = (tiempoQueAguantaBajoElAgua - tiempoBajoElAgua);

            this.actualizoValoresSaludOxigeno(personaje);

            //this.rotoMetales();

        }

        public void Render()
        {
            //Dibuja un texto por pantalla
            //DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);
            GModel.DrawText.drawText("Con la tecla P se activa o desactiva la música.", 0, 30, Color.OrangeRed);
            GModel.DrawText.drawText("Con clic izquierdo subimos la camara [Actual]: " + TGCVector3.PrintVector3(GModel.Camara.Position), 0, 40, Color.OrangeRed);

            //DIBUJO MENSAJES DE VIDA Y OXIGENO
            if (personaje.Vivo)
            {
                //OXIGENO RESTANTE
                if (tiempoBajoElAgua > 0 && tiempoRestanteBajoElAgua >= 0)
                {
                    GModel.DrawText.drawText("Te quedan " + tiempoRestanteBajoElAgua.ToString() + "segundos de oxigeno", 0, 50, Color.Blue);
                }
                else if (tiempoRestanteBajoElAgua <= 0)
                {
                    GModel.DrawText.drawText("Sufriendo daño por falta de oxigeno", 0, 50, Color.Red);
                }

                // VIDA RESTANTE
                if ((this.fueraDelMapa(GModel.camaraInterna)))
                {
                    GModel.DrawText.drawText("Sufriendo daño por estar fuera del mapa", 0, 60, Color.Red);
                }
            }
            else
            {
                GModel.DrawText.drawText("Te moriste", ScreenWidth / 2, ScreenHeight / 2, Color.Red);
            }

            //Render de BoundingBox, muy útil para debug de colisiones.
            /*
            if (BoundingBox)
            {
                barco.BoundingBox.Render();
                coralBrain.BoundingBox.Render();
                coral.BoundingBox.Render();
                shark.BoundingBox.Render();
                fish.BoundingBox.Render();
                pillarCoral.BoundingBox.Render();
                seaShell.BoundingBox.Render();
                spiralWireCoral.BoundingBox.Render();
                treeCoral.BoundingBox.Render();
                yellowFish.BoundingBox.Render();
            }
            */

            //--------skybox---------
            skybox.Render();
            //---------------
            //----heighmap
            terreno.Render();

            //------------------------------------
            agua.Render();
            //piso.Render();
            coral.Render();
            //meshTiburon.Render();
            tiburon.Render();
            coralBrain.Render();
            barco.RenderAll();
            fish.Render();
            pillarCoral.Render();
            seaShell.Render();
            spiralWireCoral.Render();
            treeCoral.Render();
            yellowFish.Render();
            arbusto.Render();
            arbusto2.Render();
            pasto.Render();
            planta.Render();
            planta2.Render();
            planta3.Render();
            roca.Render();

            objetosEstaticosEnArray.ForEach(obj => obj.Render());

            pecesAmarillos.ForEach(obj => obj.Render());
            pezCircular.Render();
            pecesAzules.ForEach(obj => obj.Render());

            metales.ForEach(obj => obj.Render());

            //SPRITES
            spriteDrawer.BeginDrawSprite();
            sprites.ForEach(sprite => spriteDrawer.DrawSprite(sprite));
            spriteDrawer.EndDrawSprite();

        }

        public void Dispose()
        {
            //------------------------
            skybox.Dispose();
            terreno.Dispose();
            agua.Dispose();
            //piso.Dispose();
            coral.Dispose();
            //meshTiburon.Dispose();
            tiburon.Dispose();
            coralBrain.Dispose();
            barco.DisposeAll();
            fish.Dispose();
            pillarCoral.Dispose();
            seaShell.Dispose();
            spiralWireCoral.Dispose();
            treeCoral.Dispose();
            yellowFish.Dispose();
            arbusto.Dispose();
            arbusto2.Dispose();
            pasto.Dispose();
            planta.Dispose();
            planta2.Dispose();
            planta3.Dispose();
            roca.Dispose();

            objetosEstaticosEnArray.ForEach(obj => obj.Dispose());

            pecesAmarillos.ForEach(obj => obj.Dispose());
            pezCircular.Dispose();

            pecesAzules.ForEach(obj => obj.Dispose());

            metales.ForEach(obj => obj.Dispose());

        }


        private Boolean fueraDelMapa(TgcFpsCamera camara)
        {
            return camara.Position.X > 5000 || camara.Position.X < -5000 || camara.Position.Z > 5000 || camara.Position.Z < -5000;
        }

        private Boolean fueraDelMapa(Pez pez)
        {
            return pez.Position.X > 5000 || pez.Position.X < -5000 || pez.Position.Z > 5000 || pez.Position.Z < -5000;
        }

        private Boolean dentroDelMapa(TgcFpsCamera camara)
        {
            return camara.Position.X <= 5000 && GModel.Camara.Position.X >= -5000 && GModel.Camara.Position.Z <= 5000 && GModel.Camara.Position.Z >= -5000;
        }

        private Boolean bajoElAgua(TgcFpsCamera camara)
        {
            return camara.Position.Y < 0;
        }

        private Boolean sobreElAgua(TgcFpsCamera camara)
        {
            return camara.Position.Y >= 0;
        }

        private void actualizoValoresSaludOxigeno(Personaje personaje)
        {
            //ACTUALIZO LOS VALORES DE SALUD Y OXIGENO
            //SI ME SALGO DEL MAPA, RESTO 1 DE SALUD
            if (this.fueraDelMapa(GModel.camaraInterna))
            {
                personaje.sufriDanio(.1f);
            }

            //SI VUELVO A ENTRAR AL MAPA RECUPERO LA ENERGIA
            if (this.dentroDelMapa(GModel.camaraInterna))
            {
                personaje.recuperaVida(.2f);
            }

            //SI ESTOY MAS DE DIEZ SEGUNDOS BAJO DEL AGUA
            //PIERDO 1 DE OXIGENO
            if (this.bajoElAgua(GModel.camaraInterna))
            {
                tiempoBajoElAgua = tiempoBajoElAgua + GModel.ElapsedTime;
            }
            if (tiempoBajoElAgua >= 10)
            {
                personaje.perdeOxigeno(.1f);
            }

            //SI SALGO A LA SUPERFICIE RECUPERO VIDA
            if (this.sobreElAgua(GModel.camaraInterna))
            {
                tiempoBajoElAgua = 0;
                personaje.recuperaOxigeno(.2f);
            }

            //ACTUALIZO LOS SPRITES DE ENERGIA Y OXIGENO
            if (personaje.Vivo)
            {
                switch (personaje.Health)
                {
                    case float n when (n <= 0):
                        spriteBarraVida.Bitmap = bitmapBarra00;
                        break;
                    case float n when (n >= 1 && n <= 10):
                        spriteBarraVida.Bitmap = bitmapBarraVida10;
                        break;
                    case float n when (n >= 11 && n <= 20):
                        spriteBarraVida.Bitmap = bitmapBarraVida20;
                        break;
                    case float n when (n >= 21 && n <= 30):
                        spriteBarraVida.Bitmap = bitmapBarraVida30;
                        break;
                    case float n when (n >= 31 && n <= 40):
                        spriteBarraVida.Bitmap = bitmapBarraVida40;
                        break;
                    case float n when (n >= 41 && n <= 50):
                        spriteBarraVida.Bitmap = bitmapBarraVida50;
                        break;
                    case float n when (n >= 51 && n <= 60):
                        spriteBarraVida.Bitmap = bitmapBarraVida60;
                        break;
                    case float n when (n >= 61 && n <= 70):
                        spriteBarraVida.Bitmap = bitmapBarraVida70;
                        break;
                    case float n when (n >= 71 && n <= 80):
                        spriteBarraVida.Bitmap = bitmapBarraVida80;
                        break;
                    case float n when (n >= 81 && n <= 90):
                        spriteBarraVida.Bitmap = bitmapBarraVida90;
                        break;
                    case float n when (n >= 91 && n <= 100):
                        spriteBarraVida.Bitmap = bitmapBarraVida100;
                        break;
                }

                switch (personaje.Oxygen)
                {
                    case float n when (n <= 0):
                        spriteBarraOxigeno.Bitmap = bitmapBarra00;
                        break;
                    case float n when (n >= 1 && n <= 10):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno10;
                        break;
                    case float n when (n >= 11 && n <= 20):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno20;
                        break;
                    case float n when (n >= 21 && n <= 30):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno30;
                        break;
                    case float n when (n >= 31 && n <= 40):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno40;
                        break;
                    case float n when (n >= 41 && n <= 50):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno50;
                        break;
                    case float n when (n >= 51 && n <= 60):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno60;
                        break;
                    case float n when (n >= 61 && n <= 70):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno70;
                        break;
                    case float n when (n >= 71 && n <= 80):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno80;
                        break;
                    case float n when (n >= 81 && n <= 90):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno90;
                        break;
                    case float n when (n >= 91 && n <= 100):
                        spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno100;
                        break;
                }
            }
            else
            {
                if (personaje.Health < 0)
                {
                    spriteBarraVida.Bitmap = bitmapBarra00;
                }

                if (personaje.Oxygen < 0)
                {
                    spriteBarraOxigeno.Bitmap = bitmapBarra00;
                }
            }
        }

        private void generoPecesAmarillos()
        {
            pezCircular = new Pez(yellowFish.createMeshInstance("pezPrueba"), new TGCVector3(0, -100, 0), new TGCVector3(0, 1.57f, 0), new MovimientoCircular(new TGCVector3(0, -100, 0), 150, 0.25f));
            //----------------
            //PECES AMARILLOS
            //Se mueven en X
            //Se autoescalan entre 1 y 5
            //Son 49
            //Tienen una velocidad de entre 25 y 75
            //----------------
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    int currentMoveDirection = rnd.Next(0, 2) * 2 - 1; //Devuelve aleatoriamente una direccion de movimiento inicial (-1 o 1)
                    float moveSpeed = (float)(rnd.NextDouble() * 75) + 25;

                    Pez pez = new Pez(yellowFish.createMeshInstance(yellowFish.Name + i + "_" + j), currentMoveDirection, moveSpeed);
                    pez.Position = new TGCVector3(rnd.Next(-3000, 3000), rnd.Next(-290, -50), rnd.Next(-3000, 3000));

                    int scale = rnd.Next(1, 5);
                    pez.Scale = new TGCVector3(scale, scale, scale);

                    //Corrijo que los peces vayan para atras
                    if (pez.CurrentMoveDir.Equals(1))
                    {
                        pez.Rotation += new TGCVector3(FastMath.PI, 0, 0);
                        pez.Transform = TGCMatrix.Scaling(pez.Scale) * TGCMatrix.RotationYawPitchRoll(pez.Rotation.X, pez.Rotation.Y, pez.Rotation.Z) * TGCMatrix.Translation(pez.Position);
                    }
                    else
                    {
                        pez.Transform = TGCMatrix.Scaling(pez.Scale) * TGCMatrix.Translation(pez.Position);
                    }

                    pecesAmarillos.Add(pez);
                }

            }
        }

        private void generoPecesAzules()
        {
            //----------------
            //PECES AZULES
            //Se mueven en Z
            //Se autoescalan entre 10 y 20
            //Son 49
            //Tienen una velocidad de entre 40 y 90
            //----------------
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    int currentMoveDirection = rnd.Next(0, 2) * 2 - 1; //Devuelve aleatoriamente una direccion de movimiento inicial (-1 o 1)
                    float moveSpeed = (float)(rnd.NextDouble() * 90) + 40;

                    Pez pez = new Pez(fish.createMeshInstance(fish.Name + i + "_" + j), currentMoveDirection, moveSpeed);
                    pez.Position = new TGCVector3(rnd.Next(-3000, 3000), rnd.Next(-290, -50), rnd.Next(-3000, 3000));

                    int scale = rnd.Next(10, 20);
                    pez.Scale = new TGCVector3(scale, scale, scale);

                    //Corrijo que los peces vayan para atras
                    if (pez.CurrentMoveDir.Equals(1))
                    {
                        pez.Rotation += new TGCVector3(FastMath.PI, 0, 0);
                        pez.Transform = TGCMatrix.Scaling(pez.Scale) * TGCMatrix.RotationYawPitchRoll(pez.Rotation.X, pez.Rotation.Y, pez.Rotation.Z) * TGCMatrix.Translation(pez.Position);
                    }
                    else
                    {
                        pez.Transform = TGCMatrix.Scaling(pez.Scale) * TGCMatrix.Translation(pez.Position);
                    }

                    pecesAzules.Add(pez);
                }

            }
        }

        private void generoHUD()
        {
            //Cargo cursor
            Microsoft.DirectX.Direct3D.Device d3dDevice = D3DDevice.Instance.Device;
            bitmapCursor = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "cursor_default.png", d3dDevice);
            spriteCursor.Bitmap = bitmapCursor;
            spriteCursor.Scaling = new TGCVector2(1f, 1f);
            spriteCursor.Position = new TGCVector2(ScreenWidth / 2.093f, ScreenHeight / 2.17f);
            sprites.Add(spriteCursor);
            //HUD
            bitmapCorazon = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "Vida.png", D3DDevice.Instance.Device);
            spriteCorazon.Bitmap = bitmapCorazon;
            spriteCorazon.Scaling = new TGCVector2(0.15f, 0.15f);
            spriteCorazon.Position = new TGCVector2(ScreenWidth / 1.1f, ScreenHeight / 1.15f);
            sprites.Add(spriteCorazon);

            bitmapBarraVida100 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida1.0.png", D3DDevice.Instance.Device);
            spriteBarraVida.Bitmap = bitmapBarraVida100;
            spriteBarraVida.Scaling = new TGCVector2(.75f, 1f);
            spriteBarraVida.Position = new TGCVector2(ScreenWidth / 1.7f, ScreenHeight / 1.13f);
            sprites.Add(spriteBarraVida);

            bitmapTanqueOxigeno = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "Oxygen.png", D3DDevice.Instance.Device);
            spriteTanqueOxigeno.Bitmap = bitmapTanqueOxigeno;
            spriteTanqueOxigeno.Scaling = new TGCVector2(0.25f, 0.2f);
            spriteTanqueOxigeno.Position = new TGCVector2(ScreenWidth / 1.125f, ScreenHeight / 1.4f);
            sprites.Add(spriteTanqueOxigeno);

            bitmapBarraOxigeno100 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno1.0.png", D3DDevice.Instance.Device);
            spriteBarraOxigeno.Bitmap = bitmapBarraOxigeno100;
            spriteBarraOxigeno.Scaling = new TGCVector2(.75f, 1f);
            spriteBarraOxigeno.Position = new TGCVector2(ScreenWidth / 1.7f, ScreenHeight / 1.325f);
            sprites.Add(spriteBarraOxigeno);

            //ANIMACION DE LAS BARRAS DE ENERGIA Y OXIGENO
            bitmapBarra00 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "Barra0.0.png", D3DDevice.Instance.Device);

            bitmapBarraVida10 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida0.1.png", D3DDevice.Instance.Device);
            bitmapBarraVida20 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida0.2.png", D3DDevice.Instance.Device);
            bitmapBarraVida30 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida0.3.png", D3DDevice.Instance.Device);
            bitmapBarraVida40 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida0.4.png", D3DDevice.Instance.Device);
            bitmapBarraVida50 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida0.5.png", D3DDevice.Instance.Device);
            bitmapBarraVida60 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida0.6.png", D3DDevice.Instance.Device);
            bitmapBarraVida70 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida0.7.png", D3DDevice.Instance.Device);
            bitmapBarraVida80 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida0.8.png", D3DDevice.Instance.Device);
            bitmapBarraVida90 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida0.9.png", D3DDevice.Instance.Device);
            bitmapBarraVida100 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraVida1.0.png", D3DDevice.Instance.Device);

            bitmapBarraOxigeno10 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno0.1.png", D3DDevice.Instance.Device);
            bitmapBarraOxigeno20 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno0.2.png", D3DDevice.Instance.Device);
            bitmapBarraOxigeno30 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno0.3.png", D3DDevice.Instance.Device);
            bitmapBarraOxigeno40 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno0.4.png", D3DDevice.Instance.Device);
            bitmapBarraOxigeno50 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno0.5.png", D3DDevice.Instance.Device);
            bitmapBarraOxigeno60 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno0.6.png", D3DDevice.Instance.Device);
            bitmapBarraOxigeno70 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno0.7.png", D3DDevice.Instance.Device);
            bitmapBarraOxigeno80 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno0.8.png", D3DDevice.Instance.Device);
            bitmapBarraOxigeno90 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno0.9.png", D3DDevice.Instance.Device);
            bitmapBarraOxigeno100 = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "BarraOxigeno1.0.png", D3DDevice.Instance.Device);
        }

        private void cargoMeshes()
        {
            coral = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\coral-TgcScene.xml").Meshes[0];

            meshTiburon = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\shark-TgcScene.xml").Meshes[0];

            coralBrain = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\brain_coral-TgcScene.xml").Meshes[0];

            barco = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\ship-TgcScene.xml");

            fish = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\fish-TgcScene.xml").Meshes[0];

            pillarCoral = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\pillar_coral-TgcScene.xml").Meshes[0];

            seaShell = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\sea_shell-TgcScene.xml").Meshes[0];

            spiralWireCoral = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\spiral_wire_coral-TgcScene.xml").Meshes[0];

            treeCoral = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\tree_coral-TgcScene.xml").Meshes[0];

            yellowFish = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\yellow_fish-TgcScene.xml").Meshes[0];

            arbusto = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Arbusto\\Arbusto-TgcScene.xml").Meshes[0];

            arbusto2 = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Arbusto2\\Arbusto2-TgcScene.xml").Meshes[0];

            pasto = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Pasto\\Pasto-TgcScene.xml").Meshes[0];

            planta = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Planta\\Planta-TgcScene.xml").Meshes[0];

            planta2 = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Planta2\\Planta2-TgcScene.xml").Meshes[0];

            planta3 = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Planta3\\Planta3-TgcScene.xml").Meshes[0];

            roca = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Roca\\Roca-TgcScene.xml").Meshes[0];
        }

        private void generoObjetosEstaticosSueltos()
        {
            //--------------objetos---------
            coral.Position = new TGCVector3(10, -300, 0);
            coral.Transform = TGCMatrix.Translation(coral.Position);

            tiburon = new Tiburon(new TGCVector3(-650, -100, 1000), meshTiburon);
            tiburon.Transform = TGCMatrix.Translation(tiburon.Position);

            coralBrain.Position = new TGCVector3(-200, -300, 340);
            coralBrain.Transform = TGCMatrix.Translation(coralBrain.Position);

            fish.Position = new TGCVector3(0, -200, 0);
            fish.Transform = TGCMatrix.Translation(fish.Position);

            pillarCoral.Position = new TGCVector3(0, -200, 40);
            pillarCoral.Transform = TGCMatrix.Translation(pillarCoral.Position);

            seaShell.Position = new TGCVector3(500, -200, 40);
            seaShell.Transform = TGCMatrix.Translation(seaShell.Position);

            spiralWireCoral.Position = new TGCVector3(-50, -300, 40);
            spiralWireCoral.Transform = TGCMatrix.Translation(spiralWireCoral.Position);

            TGCVector3 escalaTreeCoral = new TGCVector3(10f, 10f, 10f);
            treeCoral.Position = new TGCVector3(-70, -300, 200);
            treeCoral.Transform = TGCMatrix.Scaling(escalaTreeCoral) * TGCMatrix.Translation(treeCoral.Position);

            yellowFish.Position = new TGCVector3(50, -200, -20);
            yellowFish.Transform = TGCMatrix.Translation(yellowFish.Position);

            arbusto.Position = new TGCVector3(70, -300, -30);
            arbusto.AlphaBlendEnable = true;
            arbusto.Transform = TGCMatrix.Translation(arbusto.Position);

            arbusto2.Position = new TGCVector3(60, -300, -20);
            arbusto2.Transform = TGCMatrix.Translation(arbusto2.Position);

            pasto.Position = new TGCVector3(50, -300, -20);
            pasto.Transform = TGCMatrix.Translation(pasto.Position);

            planta.Position = new TGCVector3(40, -300, -20);
            planta.Transform = TGCMatrix.Translation(planta.Position);

            planta2.Position = new TGCVector3(30, -300, -20);
            planta2.Transform = TGCMatrix.Translation(planta2.Position);

            planta3.Position = new TGCVector3(20, -300, -20);
            planta3.Transform = TGCMatrix.Translation(planta3.Position);

            roca.Position = new TGCVector3(10, -300, -20);
            roca.Transform = TGCMatrix.Translation(roca.Position);
        }

        private void generoObjetosEstaticosEnArray()
        {
            var rows = 5;
            var cols = 5;
            //----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = coralBrain.createMeshInstance(coralBrain.Name + i + "_" + j);
                    instance.Position = new TGCVector3(rnd.Next(-5000, 5000), -300, rnd.Next(-5000, 5000));
                    instance.Transform = TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = coral.createMeshInstance(coral.Name + i + "_" + j);

                    instance.Position = new TGCVector3(rnd.Next(-5000, 5000), -300, rnd.Next(-5000, 5000));
                    //escalado random
                    var escalaObjeto = rnd.Next(1, 3);
                    instance.Scale = new TGCVector3(escalaObjeto, escalaObjeto, escalaObjeto);
                    instance.Transform = TGCMatrix.Scaling(instance.Scale) * TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = pillarCoral.createMeshInstance(pillarCoral.Name + i + "_" + j);
                    instance.Position = new TGCVector3(rnd.Next(-5000, 5000), -300, rnd.Next(-5000, 5000));
                    instance.Transform = TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }

            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = seaShell.createMeshInstance(seaShell.Name + i + "_" + j);
                    instance.Position = new TGCVector3(rnd.Next(-5000, 5000), -300, rnd.Next(-5000, 5000));
                    instance.Transform = TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = treeCoral.createMeshInstance(treeCoral.Name + i + "_" + j);
                    instance.Position = new TGCVector3(rnd.Next(-5000, 5000), -300, rnd.Next(-5000, 5000));
                    instance.Transform = TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = spiralWireCoral.createMeshInstance(spiralWireCoral.Name + i + "_" + j);
                    instance.Position = new TGCVector3(rnd.Next(-5000, 5000), -300, rnd.Next(-5000, 5000));
                    instance.Transform = TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = arbusto.createMeshInstance(arbusto.Name + i + "_" + j);
                    instance.Position = new TGCVector3(rnd.Next(-5000, 5000), -300, rnd.Next(-5000, 5000));
                    instance.AlphaBlendEnable = true;
                    instance.Transform = TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = arbusto2.createMeshInstance(arbusto2.Name + i + "_" + j);
                    instance.Position = new TGCVector3(rnd.Next(-5000, 5000), -300, rnd.Next(-5000, 5000));
                    instance.AlphaBlendEnable = true;
                    instance.Transform = TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = pasto.createMeshInstance(pasto.Name + i + "_" + j);
                    instance.Position = new TGCVector3(rnd.Next(-5000, 5000), -300, rnd.Next(-5000, 5000));
                    instance.AlphaBlendEnable = true;
                    instance.Transform = TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = planta.createMeshInstance(planta.Name + i + "_" + j);
                    instance.Position = new TGCVector3(rnd.Next(-5000, 5000), -300, rnd.Next(-5000, 5000));
                    instance.Transform = TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = planta2.createMeshInstance(planta2.Name + i + "_" + j);
                    instance.Position = new TGCVector3(rnd.Next(-5000, 5000), -300, rnd.Next(-5000, 5000));
                    instance.Transform = TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = planta3.createMeshInstance(planta3.Name + i + "_" + j);
                    instance.Position = new TGCVector3(rnd.Next(-5000, 5000), -300, rnd.Next(-5000, 5000));
                    instance.Transform = TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
            //-----------------
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var instance = roca.createMeshInstance(roca.Name + i + "_" + j);
                    instance.Position = new TGCVector3(rnd.Next(-5000, 5000), -300, rnd.Next(-5000, 5000));
                    instance.Transform = TGCMatrix.Translation(instance.Position);
                    objetosEstaticosEnArray.Add(instance);
                }

            }
        }

        private void cargoSkybox()
        {
            skybox = new TgcSkyBox();
            skybox.Center = TGCVector3.Empty;
            skybox.Size = new TGCVector3(10000, 10000, 10000);

            var texturesPath = GModel.MediaDir + "Texturas\\SkyBox\\";

            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "lostatseaday_up.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "lostatseaday_dn.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "lostatseaday_lf.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "lostatseaday_rt.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "lostatseaday_ft.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "lostatseaday_bk.jpg");
            skybox.SkyEpsilon = 25f;
            skybox.Init();
        }

        private void cargoHeightmap()
        {
            terreno = new TgcSimpleTerrain();
            //var path = MediaDir + "Texturas\\Heighmaps\\heighmap.jpg";
            var path = GModel.MediaDir + "Texturas\\Heighmaps\\heighmap1.jpg";
            //var textu = MediaDir + "Texturas\\Grass.jpg";
            var textu = GModel.MediaDir + "Texturas\\pasto.jpg";
            currentScaleXZ = 100f;
            currentScaleY = 50f;
            //terreno.loadHeightmap(path, currentScaleXZ, currentScaleY, new TGCVector3(0, -130, 0));
            terreno.loadHeightmap(path, currentScaleXZ, currentScaleY, new TGCVector3(0, -195, 0));
            terreno.loadTexture(textu);
            terreno.AlphaBlendEnable = true;
        }

        
        private void cargoPisos()
        {
            var aguaTextura = TgcTexture.createTexture(D3DDevice.Instance.Device, GModel.MediaDir + "Texturas\\agua20.jpg");
            agua = new TgcPlane(new TGCVector3(-20000, 0, -20000), new TGCVector3(40000, 0, 40000), TgcPlane.Orientations.XZplane, aguaTextura);

            var pisoTextura = TgcTexture.createTexture(D3DDevice.Instance.Device, GModel.MediaDir + "Texturas\\pasto.jpg");
            piso = new TgcPlane(new TGCVector3(-5000, -300, -5000), new TGCVector3(10000, 0, 10000), TgcPlane.Orientations.XZplane, pisoTextura);
        }

        private void cargoMusica()
        {
            musica = new TgcMp3Player();
            musica.FileName = GModel.MediaDir + "\\Music\\AbandonShip.mp3";
            musica.play(true);
        }



        private void generoMetales()
        {
            var texturaOro = TgcTexture.createTexture(GModel.MediaDir + "\\Texturas\\oro.jpg");
            var texturaRubi = TgcTexture.createTexture(GModel.MediaDir + "\\Texturas\\ruby.jpg");
            var texturaPlatino = TgcTexture.createTexture(GModel.MediaDir + "\\Texturas\\platinum.jpg");

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    var side = rnd.Next(0, 20);
                    var instance = TGCBox.fromSize(new TGCVector3(side, side / 4, side / 2), texturaOro);
                    instance.Position = new TGCVector3(rnd.Next(-5000, 5000), -300 + side / 8, rnd.Next(-5000, 5000));
                    instance.Transform = TGCMatrix.Translation(instance.Position);
                    metales.Add(instance);
                }

            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var side = rnd.Next(0, 20);
                    var instance = TGCBox.fromSize(new TGCVector3(side, side, side), texturaRubi);
                    instance.Position = new TGCVector3(rnd.Next(-5000, 5000), -300 + side / 2, rnd.Next(-5000, 5000));
                    instance.Transform = TGCMatrix.Translation(instance.Position);
                    metales.Add(instance);
                }

            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var side = rnd.Next(0, 20);
                    var instance = TGCBox.fromSize(new TGCVector3(side, side, side), texturaPlatino);
                    instance.Position = new TGCVector3(rnd.Next(-5000, 5000), -300 + side / 2, rnd.Next(-5000, 5000));
                    instance.Transform = TGCMatrix.Translation(instance.Position);
                    metales.Add(instance);
                }
            }

        }

        public float MovementSpeed { get => MOVEMENT_SPEED; }
        public Random GetRandom { get => rnd; }



    }
}
