using UnityEngine;

namespace BoopBepin
{
	public class CircularBuffer
	{
		private int _pointer;
		private readonly int _length;
		
		private readonly Vector3[] _buffer;
		
		public CircularBuffer(int length)
		{
			_buffer = new Vector3[length];
			_length = length;
		}

		public void Add(Vector3 obj)
		{
			_buffer[_pointer] = obj;
			_pointer = (_pointer + 1) % _length;
		}

		public Vector3 Average()
		{
			var vector = Vector3.zero;
			
			for (var i = 0; i < _length; i++)
				vector += _buffer[i];
			
			return vector / _length;
		}
	}
}
