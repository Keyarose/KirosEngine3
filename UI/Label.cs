using KirosEngine3.Config;
using KirosEngine3.Exceptions;
using KirosEngine3.Math.Vector;
using KirosEngine3.Mesh;
using KirosEngine3.Textures;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.UI
{
    /// <summary>
    /// A UI label that is placed in reference to another UI Element.
    /// </summary>
    public class Label : UIElement
    {
        /// <summary>
        /// The text object for the label.
        /// </summary>
        protected Text _labelText;

        /// <summary>
        /// Flag to enable or disable text wrap.
        /// </summary>
        protected bool _textWrap;

        /// <summary>
        /// The element the label describes.
        /// </summary>
        protected UIElement? _attached;

        /// <summary>
        /// The vertices of the Label.
        /// </summary>
        public TexturedVertex2D[] Vertices { get { return _labelText.Vertices; } }

        /// <summary>
        /// The indices of the label.
        /// </summary>
        public uint[] Indices { get { return _labelText.Indices; } }

        /// <summary>
        /// The text rendered by the label.
        /// </summary>
        public string LabelText { get { return _labelText.Sentence; } set { _labelText.Sentence = value; } }

        /// <summary>
        /// The UI Element the label describes.
        /// </summary>
        public UIElement? Describes { get { return _attached; } }

        /// <summary>
        /// The Label's position.
        /// </summary>
        public override Vec2 Position 
        { 
            get => _labelText.Position; 
            set => _labelText.Position = value;
        }

        /// <summary>
        /// Enable or disable text wrap.
        /// </summary>
        public bool TextWrap
        {
            get => _textWrap;
            set => _textWrap = value;
        }

        /// <summary>
        /// Basic constructor for XML use.
        /// </summary>
        private Label() 
        {
            _labelText = new Text();
        }

        /// <summary>
        /// Construct a Label with the given text.
        /// </summary>
        /// <param name="pos">The position of the label.</param>
        /// <param name="text">The text of the label.</param>
        public Label(Vec2 pos, string text) : this(pos, text, null) { }

        /// <summary>
        /// Construct a Label with the given text to describe the attached UIElement.
        /// </summary>
        /// <param name="pos">The position of the label.</param>
        /// <param name="text">The text of the label.</param>
        /// <param name="attached">The UIElement described by the Label.</param>
        public Label(Vec2 pos, string text, UIElement? attached) : this(pos, new(60, 20), text, attached) { }

        /// <summary>
        /// Construct a Label of the given size with the given text.
        /// </summary>
        /// <param name="pos">The position of the label.</param>
        /// <param name="size">The size of the label.</param>
        /// <param name="text">The text of the label.</param>
        public Label(Vec2 pos, Vec2 size, string text) : this(pos, size, text, null) { }

        /// <summary>
        /// Construct a Label of the given size with the given text.
        /// </summary>
        /// <param name="pos">The position of the label.</param>
        /// <param name="size">The size of the label.</param>
        /// <param name="text">The text of the label.</param>
        /// <param name="attached">The UIElement described by the Label.</param>
        public Label(Vec2 pos, Vec2 size, string text, UIElement? attached)
        {
            _size = size;
            _labelText = new Text(pos, text);
            _attached = attached;
        }

        /// <summary>
        /// Construct a Label of the given size using the provided font and text.
        /// </summary>
        /// <param name="pos">The position of the label.</param>
        /// <param name="size">The size of the label.</param>
        /// <param name="font">The font to be used for the text.</param>
        /// <param name="text">The text of the label.</param>
        public Label(Vec2 pos, Vec2 size, Font font, string text) : this(pos, size, font, text, null) { }

        /// <summary>
        /// Construct a Label of the given size using the provided font and text.
        /// </summary>
        /// <param name="pos">The position of the label.</param>
        /// <param name="size">The size of the label.</param>
        /// <param name="font">The font to be used for the text.</param>
        /// <param name="text">The text of the label.</param>
        /// <param name="attached">The UIElement described by the Label.</param>
        public Label(Vec2 pos, Vec2 size, Font font, string text, UIElement? attached)
        {
            _size = size;
            _labelText = new Text(pos, font, text);
            _attached = attached;
        }

        #region Load
        /// <summary>
        /// Prepare the Label for rendering.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public override bool Init()
        {
            _labelText.Init();

            return true;
        }
        #endregion

        #region Draw
        /// <inheritdoc/>
        public override void Draw()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void DrawDX()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void DrawGL(ViewMatrixes vm, params TextureUnit[] tu)
        {
            if (tu != null && tu.Length > 0)
            {
                _labelText.DrawGL(vm, tu);
            }
            else
            {
                Console.WriteLine("Warning: Label requires a texture unit for the font.");
            }
        }
        #endregion

    }
}
