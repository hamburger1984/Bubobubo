using System;
using System.Collections.Generic;
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
        static void Main(string[] args)
        {
            uint maxDigits = 16;
           
            long maxNumber = IntPow(10, maxDigits) - 1;
            Log.Info($"Searching power sums for {maxDigits} digits, 0..{maxNumber}");

            var steps = 100;
            var step = maxNumber / steps;

            var subSums = Enumerable.Range(0, steps).AsParallel()
                .Select(i => SearchPowerSums(i * (step + 1), i == steps - 1 ? maxNumber : (i + 1) * step)).ToArray();

            var totalSum = 0L;
            foreach (var s in subSums)
                totalSum += s;

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

                number /= 10;
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
            do
            {
                //number = digits.AsParallel().Select(d => IntPow(d, k)).Sum();

                number = 0;
                foreach (var d in digits)
                    number += IntPow(d, k);
                // Log.Debug($" .. {string.Join(",", digits.Reverse())} .. k {k} -> {number}");

                if (number == lower || number == higher)
                {
                    Log.Debug($"Found {number} as {k}-th power sum of {string.Join(",", digits)}");
                    return true;
                }

                if (number <= lastNumber)
                {
                    //Log.Debug($"!!!! not increasing .. stop");
                    return false;
                }

                lastNumber = number;

                k++;
            } while (number < higher);

            return false;
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