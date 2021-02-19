using System.Collections;
using System.Collections.Generic;
using BlockGame.Backend;
using BlockGame.Backend.World;
using UnityEngine;
using UnityEngine.UI;

namespace BlockGame.Components.UI
{
    public class SelectedBlockThumbnail : MonoBehaviour
    {
        [SerializeField] public Texture2D blockTexture;
        [SerializeField] public Text text;
        private Image _image;
        private GameData _gameData;

        private float _animationScale;

        private const float TextureScaleFactor = 16f;

        // Start is called before the first frame update
        void Awake ()
        {
            _image = GetComponent<Image>();
            _gameData = FindObjectOfType<GameData>();
            GameEvents.ChangeInventorySelection += GameEventsOnChangeInventorySelection;
        }

        private void GameEventsOnChangeInventorySelection (short newblockid)
        {
            var block = _gameData.blockRegistry.ById(newblockid);
            var coords = TextureScaleFactor * (Vector2) block.TexCoords.GetFrontal();
            coords.y = 240f - coords.y;
            var sprite = Sprite.Create(blockTexture, new Rect(coords, Vector2.one * TextureScaleFactor),
                Vector2.zero);
            _image.sprite = sprite;
            text.text = block.blockName;

            StartCoroutine(nameof(Animation));
        }

        private IEnumerator Animation ()
        {
            _animationScale = 1.5f;

            while (_animationScale > 1f)
            {
                _animationScale *= 0.96f;
                transform.localScale = Vector3.one * _animationScale;
                yield return new WaitForSeconds(0.01f);
            }

            transform.localScale = Vector3.one;
        }
    }
}