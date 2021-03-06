﻿using System;
using System.Collections.Generic;
using DDW.Display;
using DDW.Input;
using DDW.V2D.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#if !(WINDOWS_PHONE)
using Microsoft.Xna.Framework.Net;
using V2DRuntime.Network;
#endif
using Microsoft.Xna.Framework.GamerServices;

namespace DDW.V2D 
{
    public abstract class V2DGame : Microsoft.Xna.Framework.Game
    {
        public static V2DGame instance;
        public static Stage stage;
        public static ContentManager contentManager;
        public const string ROOT_NAME = V2DWorld.ROOT_NAME;
        public static string currentRootName = V2DWorld.ROOT_NAME;

        public static PlayerIndex activeController = PlayerIndex.One;

        protected GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        protected bool keyDown = false;
        protected bool isFullScreen = false;

#if !(WINDOWS_PHONE)
        private bool wasTrialMode;
		public List<NetworkGamer> gamers = new List<NetworkGamer>();
#endif

        protected V2DGame()
        {
            if (instance != null)
            {
                throw new Exception("There can be only one game class.");
            }

#if !(WINDOWS_PHONE)
            Components.Add(new GamerServicesComponent(this));
            SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(OnGamerSignIn);
#endif
            instance = this;
            graphics = new GraphicsDeviceManager(this);
            contentManager = Content;
            Content.RootDirectory = "Content";
			stage = new V2DStage();

            GetCursor();
        }
        
        public virtual bool HasCursor { get { return false; } }

        private Cursor cursor;
        public Cursor GetCursor()
        {
            if (HasCursor && cursor == null)
            {
                cursor = new Cursor(this);
                Components.Add(cursor);
            }
            return cursor;
        }

        protected virtual void CreateScreens()
        {
            //screenPaths.Add(symbolImports[i]);
        }
        public virtual void AddingScreen(Screen screen) { }
        public virtual void RemovingScreen(Screen screen) { }
        protected override void Initialize()
        {
            base.Initialize();

            stage.Initialize();
            CreateScreens();
            stage.SetScreen(0);
        }

        public void SetSize(int width, int height)
        {
            if (width > 0 && height > 0)
            {
                graphics.SynchronizeWithVerticalRetrace = true;
                graphics.PreferredBackBufferWidth = width;
                graphics.PreferredBackBufferHeight = height;
                graphics.IsFullScreen = this.isFullScreen;
                graphics.ApplyChanges();
                stage.SetBounds(0, 0, width, height);
            }
        }

        protected override void LoadContent()
        {
			spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        protected override void UnloadContent()
        {
        }
        public virtual void ExitGame()
        {
            this.Exit();
        }

#if !(WINDOWS_PHONE)

		public virtual void ExitToMainMenu()
        {
			NetworkManager.Instance.LeaveSession();
        }
        public virtual void AllLevelsComplete()
        {
        }

        private bool unlockWhenSignedIn = false;
        public virtual void UnlockTrial(int playerIndex)
        {
            //return;
            //Console.WriteLine(((PlayerIndex)(playerIndex)).ToString());

            //Console.WriteLine("TRACE: " + playerIndex.ToString());

            SignedInGamer gamer = GamerCanPurchase(playerIndex);

            if (gamer != null)
            {
                if (gamer.Privileges.AllowPurchaseContent)
                {
                    ShowMarketPlace(playerIndex);
                }
                else
                {
                    unlockWhenSignedIn = true;
                    if (gamer != null)
                    {
                        if (stage != null && stage.GetCurrentScreen() != null)
                        {
                            stage.GetCurrentScreen().SignInToLive(playerIndex);
                        }
                        else
                        {
                            ShowSignIn(playerIndex);
                        }
                    }
                    else
                    {
                        ShowSignIn(playerIndex);
                    }
                }
            }
            else
            {
                unlockWhenSignedIn = true;
                ShowSignIn(playerIndex);
            }
        }

        protected void OnGamerSignIn(object sender, SignedInEventArgs e)
        {
            if (unlockWhenSignedIn)
            {
                if (!Guide.IsVisible)
                {
                    unlockWhenSignedIn = false;
                    UnlockTrial((int)(e.Gamer.PlayerIndex));
                }
            }
        }

        public virtual SignedInGamer GetSignedInGamer(int playerIndex)
        {
            SignedInGamer result = null;

            for (int i = 0; i < Gamer.SignedInGamers.Count; i++)
            {
                if (Gamer.SignedInGamers[i].PlayerIndex == (PlayerIndex)(playerIndex))
                {
                    result = Gamer.SignedInGamers[i];
                    break;
                }
            }

            if (result == null)
            {
                ShowSignIn(playerIndex);
            }

            return result;
        }
        public virtual SignedInGamer GamerCanPurchase(int playerIndex)
        {
            //Console.WriteLine(((PlayerIndex)(playerIndex)).ToString());

            SignedInGamer result = null;

            PlayerIndex pi = (PlayerIndex)(playerIndex);

            //for (PlayerIndex pi = PlayerIndex.One; pi < PlayerIndex.Four; pi++)
            //{
                if (Gamer.SignedInGamers[pi] != null &&
                    Gamer.SignedInGamers[pi].Privileges.AllowPurchaseContent)
                {
                    result = Gamer.SignedInGamers[pi];
                }
            //}
            return result;
        }
        public virtual void FullGameUnlocked()
        {
        }

        public virtual void ShowMarketPlace(int playerIndex)
        {
            //Console.WriteLine((PlayerIndex)(playerIndex));
            try
            {
                //Guide.ShowMarketplace(V2DGame.activeController);
                Guide.ShowMarketplace((PlayerIndex)(playerIndex));
            }
            catch (Exception){}
        }

        public virtual void ShowSignIn(int playerIndex)
        {
            if (!Guide.IsVisible)
            {
                try
                {
                    Guide.ShowSignIn(1, false);
                }
	            catch (Exception){}
            }
        }
        public static bool CanPlayerBuyGame(PlayerIndex player)
        {
            bool result = false;
            SignedInGamer gamer = Gamer.SignedInGamers[player];
            if (gamer != null)
            {
                result = gamer.Privileges.AllowPurchaseContent;
            }
            return result;
        }


		public virtual void AddGamer(NetworkGamer gamer, int gamerIndex)
		{
			if (!gamers.Contains(gamer))
			{
				gamers.Add(gamer);
			}
		}
		public virtual void RemoveGamer(NetworkGamer gamer)
		{
			if (gamers.Contains(gamer))
			{
				gamers.Remove(gamer);
			}
		}

#endif

        protected override void Update(GameTime gameTime)
        {
			stage.Update(gameTime);
            base.Update(gameTime);

#if !(WINDOWS_PHONE)
            if (!Guide.IsTrialMode && wasTrialMode)
            {
                FullGameUnlocked();
            }
            wasTrialMode = Guide.IsTrialMode;
#endif
        }

        protected override void Draw(GameTime gameTime)
		{
            stage.Draw(spriteBatch);

            base.Draw(gameTime);
        }

    }
}
