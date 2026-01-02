using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MarioGame.UI
{
    public class MenuItem
    {
        public string Text { get; set; }
        public System.Action OnSelect { get; set; }

        public MenuItem(string text, System.Action onSelect)
        {
            Text = text;
            OnSelect = onSelect;
        }
    }

    public class Menu
    {
        private List<MenuItem> _items;
        private int _selectedIndex;
        private SpriteFont _font;
        private SpriteBatch _spriteBatch;

        public Menu(SpriteFont font, SpriteBatch spriteBatch)
        {
            _items = new List<MenuItem>();
            _selectedIndex = 0;
            _font = font;
            _spriteBatch = spriteBatch;
        }

        public void AddItem(MenuItem item)
        {
            _items.Add(item);
        }

        public void MoveUp()
        {
            _selectedIndex--;
            if (_selectedIndex < 0)
                _selectedIndex = _items.Count - 1;
        }

        public void MoveDown()
        {
            _selectedIndex++;
            if (_selectedIndex >= _items.Count)
                _selectedIndex = 0;
        }

        public void SelectCurrent()
        {
            if (_selectedIndex >= 0 && _selectedIndex < _items.Count)
            {
                _items[_selectedIndex].OnSelect?.Invoke();
            }
        }

        public void Draw(Vector2 startPosition, float spacing)
        {
            Vector2 position = startPosition;

            for (int i = 0; i < _items.Count; i++)
            {
                Color color = (i == _selectedIndex) ? Color.Yellow : Color.White;
                string prefix = (i == _selectedIndex) ? "> " : "  ";

                if (_font != null)
                {
                    _spriteBatch.DrawString(_font, prefix + _items[i].Text, position, color);
                }

                position.Y += spacing;
            }
        }

        public void Clear()
        {
            _items.Clear();
            _selectedIndex = 0;
        }
    }
}