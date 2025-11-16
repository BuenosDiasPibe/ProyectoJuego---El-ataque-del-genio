using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ProyectoJuego
{
    public class SceneManager
    {
        private Stack<Scene> sceneManager;
        public Dictionary<GameState, Action> actionByState = new();
        public ContentManager contentManager;
        public GraphicsDeviceManager graphics;

        public SceneManager(ContentManager contentManager, GraphicsDeviceManager graphics)
        {
            sceneManager = new();
            this.contentManager = contentManager;
            this.graphics = graphics;
        }
        public void AddScene(Scene scene)
        {
            scene.LoadContent();
            sceneManager.Push(scene);
        }
        public void RemoveScene()
        {
            //Maybe unnesesary, but it works so fuck it we ball
            GetScene().UnloadContent();
            sceneManager.Pop();
            sceneManager.Peek().LoadContent();
        }
        public Scene GetScene()
        {
            return sceneManager.Peek();
        }
        public void RemoveAllScenes(){
            sceneManager.Clear();
        }
        public bool hasScenes(){
            return sceneManager.Count > 0;
        }
    }
}
