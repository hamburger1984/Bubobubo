using System;
using System.Linq;
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
            var maxDigits = 16;
            ulong number = 0;
            uint[] digits = {0};
            var count = 0;
            ulong sum = 0L;

            while (true)
            {
                number++;
                digits = Increment(digits);
                if (digits.Length > maxDigits) break;

                if (IsPowerSum(number, digits))
                {
                    count++;
                    sum += number;
                }
            }

            Log.Debug($"Found {count} power sums for {maxDigits} digits - sum is {sum}.");
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

        static bool IsPowerSum(ulong number, params uint[] digits)
        {
            var (lower, higher) = (number - 1, number + 1);

            uint k = 2;
            ulong lastNumber = 0;
            do
            {
                number = 0;
                foreach (var d in digits)
                    number += IntPow(d, k);

                // Log.Debug($" .. {string.Join(",", digits.Reverse())} .. k {k} -> {number}");

                if (number == lower || number == higher)
                {
                    Log.Debug($"Found {number} as {k}-th power sum of {string.Join(",", digits.Reverse())}");
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
        static ulong IntPow(uint x, uint pow)
        {
            ulong ret = 1;
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