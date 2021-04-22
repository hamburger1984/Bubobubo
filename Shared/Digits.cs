using System.Diagnostics.CodeAnalysis;

namespace Shared
{
    public static class Digits
    {
        public static int Fill(this uint[] digits, long number, bool assumeFilledWithPredecessor)
        {
            var index = 0;
            while (true)
            {
                var digit = (uint) (number % 10L);

                //if (index == digits.Length)
                //{
                //    Log.Info($"Will try to write {digit} to cell {index} into {string.Join(",", digits)}");
                //}

                digits[index] = digit;
                number /= 10L;
                index++;


                if (number <= 0)
                {
                    // index is the total amount of relevant digits now
                    return index;
                }

                // only fill higher digits, if array is not filled with predecessor already
                if (digit != 0 && assumeFilledWithPredecessor)
                    break;
            }

            return 0;
        }
    }
}