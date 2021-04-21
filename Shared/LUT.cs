using System;
using System.Buffers;
using System.Threading.Tasks;

namespace Shared
{
    public class LUT<T>
    {
        private readonly T[] _cells;
        private readonly int _columns;
        private readonly int _rows;

        public LUT(int columns, int rows, Func<(uint col, uint row), T> factory)
        {
            _cells = new T[columns * rows];
            _columns = columns;
            _rows = rows;

            Parallel.For(0, rows, r =>
            {
                var pool = ArrayPool<T>.Shared;
                var row = pool.Rent(columns);
                for (var c = 0u; c < columns; c++) row[c] = factory((c, (uint) r));

                Array.Copy(row, 0, _cells, r * columns, columns);

                pool.Return(row);
            });
        }

        public int Columns => _columns;

        public int Rows => _rows;

        public T Get((uint col, uint row) position)
        {
            var index = position.col + position.row * _columns;
            if (index < 0 || index >= _cells.Length)
                throw new ArgumentOutOfRangeException($"Cannot access position {position}");

            return _cells[index];
        }
    }
}