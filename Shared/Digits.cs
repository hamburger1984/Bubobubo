using System.Diagnostics.CodeAnalysis;

namespace Shared
{
    public static class Digits
    {
        public static int Fill(this uint[] digits, long number, bool assumeFilledWithPredecessor)
        {
            var index = 0;
            uint digit;
            while (true)
            {
                digit = (uint) (number % 10L);
                digits[index] = digit;
                number /= 10L;
                index++;

                if (number == 0)
                {
                    // index is the total amount of relevant digits now
                    return index;
                }

                // only fill higher digits, if array is now filled with predecessor already
                if (digit != 0 && assumeFilledWithPredecessor)
                    break;
            }

            return 0;
        }
    }
}