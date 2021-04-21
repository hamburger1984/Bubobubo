using System;
using System.Buffers;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Shared;

namespace Problem_749
{
    /*
     * A positive integer, n, is a near power sum if there exists a positive integer, k, such that the sum of the kth
     * powers of the digits in its decimal representation is equal to either n+1 or n-1.
     * For example 35 is a near power sum number because 3² + 5² = 34.
     *
     * Define S(d) to be the sum of all near power sum numbers of d digits or less. Then S(2) = 110 and S(6) = 2562701.
     *
     * Find S(16).
     */

    class Program
    {
        private static LUT<long> Powers;

        static void Main(string[] args)
        {
            int maxDigits = 10; // TODO: 16
            var upperLimit = (long) Math.Pow(10, maxDigits);
            var maxNumber = upperLimit - 1L;

            Log.Info($"Searching power sums for {maxDigits} digits, 0..{maxNumber}");

            var sw = Stopwatch.StartNew();
            Powers = new LUT<long>(10, maxDigits * 2, delegate((uint col, uint row) position)
            {
                if (position.row == 0) return 1;

                if (position.col == 0) return 0;

                return IntPow(position.col, position.row);
            });

            Log.Debug($"{sw.Elapsed} Precalculated 0..{maxDigits * 2} power of 0..9");

            var step = Math.Min(maxNumber / 1000L, 100_000_000L);
            var steps = (int) (maxNumber / step);
            var totalLock = new object();
            var totalSum = 0L;
            var solved = 0;

            Parallel.For(0, steps, i =>
            {
                var pool = ArrayPool<uint>.Shared;
                var digits = pool.Rent(maxDigits);

                var number = i * step + 1;
                var end = i == steps - 1 ? maxNumber : ((long) i + 1) * step;
                //Log.Debug($"{sw.Elapsed} Start searching {number}..{end}");

                var numDigits = digits.Fill(number, false);

                long localSum = 0L;
                while (true)
                {
                    if (IsPowerSum(number, numDigits, digits))
                    {
                        localSum += number;
                    }

                    number++;

                    numDigits = Math.Max(digits.Fill(number, true), numDigits);

                    if (number > end)
                    {
                        lock (totalLock)
                        {
                            solved++;
                            totalSum += localSum;

                            Console.Write($"\r{sw.Elapsed} ----- {solved} / {steps} .. {totalSum} ----");
                        }

                        break;
                    }
                }

                pool.Return(digits, true);
            });
            Log.Info($"Total {totalSum} from power sums with max {maxDigits} digits");
        }

        static bool IsPowerSum(long number, int numDigits, params uint[] digits)
        {
            var (lower, higher) = (number - 1, number + 1);

            var k = (uint) Math.Max(2, numDigits - 1);
            long lastNumber = 0;
            while (true)
            {
                number = 0;
                if (k < Powers.Rows)
                    for (var d = 0; d < numDigits; d++)
                        number += Powers.Get((digits[d], k));
                else
                    for (var d = 0; d < numDigits; d++)
                        number += IntPow(digits[d], k);

                //Log.Debug($"{lower} {number} {higher} {string.Join(",", digits)}");

                if (number == lower || number == higher)
                {
                    Log.Debug(
                        $"Found {numDigits} digits {number} as {nth(k)} power sum of {string.Join(",", digits.Take(numDigits).Reverse())}");
                    return true;
                }

                if (number <= lastNumber || number > higher)
                {
                    //Log.Debug($"!!!! not increasing .. stop");
                    return false;
                }

                lastNumber = number;
                k++;
            }
        }

        static string nth(uint n)
        {
            if (n == 1) return "1-st";
            if (n == 2) return "2-nd";
            if (n == 3) return "3-rd";
            return $"{n}-th";
        }

        // https://stackoverflow.com/questions/383587/how-do-you-do-integer-exponentiation-in-c
        static long IntPow(long x, uint pow)
        {
            long ret = 1;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                    ret *= x;
                x *= x;
                pow >>= 1;
            }

            return ret;
        }
    }
}