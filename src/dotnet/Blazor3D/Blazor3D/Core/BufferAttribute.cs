using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomagGroup.Blazor3D.Core
{
    public class BufferAttribute<T>: IBufferAttribute where T : struct
    {
        public BufferAttribute(T[] array, int itemSize)
        {
            Type = typeof(T).Name;
            ItemSize = itemSize;
            _array = array;
        }
        public string Type { get; }
        private T[] _array { get; set; }        // The actual buffer data

        public int ItemSize { get; set; }     // Number of elements per vertex (e.g., 3 for vec3)
        public bool Normalized { get; set; } = false; // Matches THREE.js default
        public Array Array => _array; // allows reading the raw array
    }

    public interface IBufferAttribute
    {
        int ItemSize { get; }
        bool Normalized { get; }
        Array Array { get; } // allows reading the raw array
    }
}
