using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class FrameBuffer
    {
        enum EdgeBehavior
        {
            None,
            Clamp,
            Wrap
        }
        public FrameBuffer(int width = 0, int height = 0, int bufferCount = 2)
        {
            _width = width;
            _height = height;
            _buffers = new List<CHAR_INFO[]>(bufferCount);
            _bufferCount = bufferCount;
            _edgeBehavior = EdgeBehavior.Clamp;

            RecreateBuffers(this._bufferCount, width, height);
        }

        public static FrameBuffer operator + (FrameBuffer frameBuffer, Sprite stamp)
        {
            for (int y = 0; y < stamp.Height; ++y)
            {
                for (int x = 0; x < stamp.Width; ++x)
                {
                    int xCoord = x + stamp.OffsetX;
                    int yCoord = y + stamp.OffsetY;
                    // Handle wrapping.
                    if (xCoord < 0) xCoord = frameBuffer.Width - Math.Abs(xCoord) % frameBuffer.Width;
                    if (yCoord < 0) yCoord = frameBuffer.Height - Math.Abs(yCoord) % frameBuffer.Height;

                    frameBuffer.SetBufferPixel(
                        xCoord,
                        yCoord,
                        stamp.BufferPixels[x + y * stamp.Width]
                    );
                }
            }
            return frameBuffer;
        }

        public void RecreateBuffers(int bufferCount, int width, int height)
        {
            this._width = width;
            this._height = height;

            this._buffers.RemoveAll((CHAR_INFO[] buffer) => true);

            int bufferSize = this._width * this._height;
            for (int i = 0; i < bufferCount; ++i)
            {
                this.Buffers.Add(new CHAR_INFO[bufferSize]);
            }

            ClearBuffer(' ', (ushort)(CHAR_INFO_ATTRIBUTE.BG_BLACK | CHAR_INFO_ATTRIBUTE.FG_BLACK));
        }

        /*
         * SetBufferPixel * Copies the passed in BufferPixel to the
         * BufferPixel at position (x, y) in the FrameBuffer.
         * 
         * Parameters:
         *     int x - Framebuffer x coordinate.
         *     int y - FrameBuffer y coordinate.
         *     BufferPixel bufferPixel - BufferPixel object to copy to the FrameBuffer.
         *
         *
         */
        public void SetBufferPixel(int x, int y, CHAR_INFO bufferPixel)
        {
            int bufferIndex = y * this.Width + x;
            if (bufferIndex >= this.Buffers[this._currentBuffer].Length)
            {
                if (EdgeBehavior.Clamp == _edgeBehavior)
                {
                    return;
                }
                else
                if (EdgeBehavior.Wrap == _edgeBehavior)
                {
                    bufferIndex = y % this.Height * this.Width + x % this.Width;
                }
            }
            this.Buffers[this._currentBuffer][bufferIndex] = bufferPixel;
        }

        public void ClearBuffer(char character, ushort attributes)
        {
            CHAR_INFO pixel = new CHAR_INFO()
            {
                Char = ' ',
                Attributes = attributes
            };

            int bufferSize = this._width * this._height;
            _buffers[_currentBuffer] = new CHAR_INFO[bufferSize];
            for (int i = 0; i < bufferSize; ++i)
            {
                _buffers[_currentBuffer][i] = pixel;
            }

        }

        List<CHAR_INFO[]> _buffers;
        public List<CHAR_INFO[]> Buffers
        { get { return _buffers; } }


        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

        public CHAR_INFO[] CurrentBuffer
        {
            get
            {
                return _buffers[_currentBuffer];
            }
        }

        private int _width;
        private int _height;
        private int _bufferCount;
        private ConsoleColor _clearColor;
        private int _currentBuffer;
        private EdgeBehavior _edgeBehavior;
    }
}
