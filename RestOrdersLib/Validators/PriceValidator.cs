using System.ComponentModel.DataAnnotations;

namespace RestOrdersLib.Validators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PriceValidator : ValidationAttribute
    {
        internal static string ErrorMessageMockup => "The price cannot be higher than 5000.";
        decimal maxPrice;
        public PriceValidator(string maxPrice) : base(() => ErrorMessageMockup) {
            decimal.TryParse(maxPrice, out this.maxPrice);
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return false;
            }

            return ((decimal)value) < maxPrice;
        }
    }
}