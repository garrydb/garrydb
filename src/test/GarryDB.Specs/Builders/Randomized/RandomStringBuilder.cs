using System.Linq;
using System.Text;

namespace GarryDB.Specs.Builders.Randomized
{
    internal sealed class RandomStringBuilder : TestDataBuilder<string>
    {
        private const string Characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private bool lettersOnly;

        private int minimumLength;
        private int maximumLength;
        private bool startsWithLetter;

        public RandomStringBuilder()
        {
            WithMinimumLength(5);
            WithMaximumLength(new RandomIntegerBuilder().WithMinimum(5).WithMaximum(15).Build());
        }

        public RandomStringBuilder ThatStartsWithLetter
        {
            get
            {
                startsWithLetter = true;

                return this;
            }
        }

        public RandomStringBuilder WithLettersOnly
        {
            get
            {
                lettersOnly = true;

                return this;
            }
        }

        protected override void OnPreBuild()
        {
            if (maximumLength < minimumLength)
            {
                WithMaximumLength(minimumLength + 10);
            }
        }

        protected override string OnBuild()
        {
            var stringBuilder = new StringBuilder();
            if (startsWithLetter)
            {
                stringBuilder.Append(BuildRandomString(Characters.Substring(10), 1));
                maximumLength--;
            }

            int numberOfCharacters = new RandomIntegerBuilder().WithMinimum(minimumLength).WithMaximum(maximumLength).Build();

            string characters = !lettersOnly ? Characters : Characters.Substring(10);

            stringBuilder.Append(BuildRandomString(characters, numberOfCharacters));

            return stringBuilder.ToString();
        }

        public RandomStringBuilder WithMinimumLength(int length)
        {
            minimumLength = length;

            return this;
        }

        public RandomStringBuilder WithMaximumLength(int length)
        {
            maximumLength = length;

            return this;
        }

        private string BuildRandomString(string characters, int length)
        {
            char[] value = Enumerable.Range(0, length)
                                     .Select(x => new RandomIntegerBuilder().WithMinimum(0)
                                                                            .WithMaximum(characters.Length)
                                                                            .Build())
                                     .Select(x => characters[x])
                                     .ToArray();

            return new string(value);
        }
    }
}
