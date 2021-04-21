using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
            uint maxDigits = 16; // TODO: 16

            Powers = new LUT<long>(10, (int) maxDigits * 2, delegate((uint col, uint row) position)
            {
                if (position.row == 0) return 1;

                if (position.col == 0) return 0;

                return IntPow(position.col, position.row);
            });

            var maxNumber = IntPow(10, maxDigits) - 1;
            Log.Info($"Searching power sums for {maxDigits} digits, 0..{maxNumber}");

            var step = Math.Min(maxNumber / 1000L, 100_000_000L);
            var steps = (int) (maxNumber / step);

            var totalLock = new object();
            var totalSum = 0L;
            var solved = 0;
            var sw = Stopwatch.StartNew();

            Parallel.For(0, steps, i =>
            {
                var start = i * step + 1;
                var end = i == steps - 1 ? maxNumber : ((long) i + 1) * step;

                var localSum = SearchPowerSums(start, end);
                lock (totalLock)
                {
                    solved++;
                    totalSum += localSum;

                    Console.Write($"\r ----- {solved} / {steps} .. {sw.Elapsed} .. {totalSum} ----");
                }
            });

            Log.Info($"Total {totalSum} from power sums with max {maxDigits} digits");
        }

        static long SearchPowerSums(long startNumber, long endNumber)
        {
            var number = startNumber;
            var digits = CreateDigits(number);
            var count = 0;
            var sum = 0L;

            while (true)
            {
                number++;
                if (number > endNumber) break;

                digits = Increment(digits);

                if (IsPowerSum(number, digits))
                {
                    count++;
                    sum += number;
                }
            }

            if (count > 0)
                Log.Debug($"{startNumber}..{endNumber}: Found {count} power sums, their sum is {sum}.");
            return sum;
        }

        static uint[] CreateDigits(long number)
        {
            var result = new List<uint>();

            while (true)
            {
                var digit = number % 10;
                result.Add((uint) digit);

                number -= digit;
                if (number == 0) break;

                number = number / 10L;
            }

            return result.ToArray();
        }

        static uint[] Increment(uint[] digits)
        {
            var digit = 0;
            while (true)
            {
                digits[digit] = (digits[digit] + 1) % 10;
                if (digits[digit] == 0)
                {
                    // enough space for digits?
                    if (digits.Length == digit + 1) digits = Extend(digits);

                    // continue for carry
                }
                else
                    break;

                digit++;
            }

            return digits;
        }

        static uint[] Extend(uint[] array)
        {
            var result = new uint[array.Length + 1];
            Array.Copy(array, result, array.Length);
            return result;
        }

        static bool IsPowerSum(long number, params uint[] digits)
        {
            var (lower, higher) = (number - 1, number + 1);

            var k = (uint) Math.Max(2, digits.Length - 1);
            long lastNumber = 0;
            while (true)
            {
                number = 0;
                if (k < Powers.Rows)
                    foreach (var d in digits)
                        number += Powers.Get((d, k));
                else
                    foreach (var d in digits)
                        number += IntPow(d, k);

                if (number == lower || number == higher)
                {
                    Log.Debug(
                        $"Found {digits.Length} digits number {number} as {nth(k)} power sum of {string.Join(",", digits)}");
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