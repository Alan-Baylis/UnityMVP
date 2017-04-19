using Becerra.MVP.Views;
using UnityEngine;

namespace Becerra.MVP.Examples
{
    public class AnimalView : View<Animal>
    {
        [Header("References")]
        public SpriteRenderer spriteRenderer;

        [Header("Resources")]
        public Sprite catIcon;

        public override void Clean()
        {

        }

        protected override void RefreshImplementation(Animal model)
        {
            if (model.Id.Contains("Cat"))
            {
                spriteRenderer.sprite = catIcon;
            }
        }
    }
}

