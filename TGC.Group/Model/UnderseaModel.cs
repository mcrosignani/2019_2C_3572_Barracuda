using System.Drawing;
using TGC.Core.Example;
using TGC.Group.Model.Levels;

namespace TGC.Group.Model
{
    public class UnderseaModel : TgcExample
    {
        private CustomModel currentModel;

        public UnderseaModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            currentModel = new MainMenuModel(this, Camara, Input, MediaDir, ShadersDir, Frustum, DrawText);
            //currentModel = new Level1Model(this, Camara, Input, MediaDir, ShadersDir, Frustum, DrawText);

            currentModel.Init();

            Camara = currentModel.Camera;

            BackgroundColor = Color.Black;
        }

        public override void Update()
        {
            PreUpdate();

            currentModel.Update(ElapsedTime);

            PostUpdate();
        }

        public override void Render()
        {
            PreRender();

            currentModel.Render(ElapsedTime);

            PostRender();
        }

        public override void Dispose()
        {
            currentModel.Dispose();
        }

        public void ChangeLevel()
        {
            currentModel = new Level1Model(this, Camara, Input, MediaDir, ShadersDir, Frustum, DrawText);

            currentModel.Init();

            Camara = currentModel.Camera;
        }
    }
}
