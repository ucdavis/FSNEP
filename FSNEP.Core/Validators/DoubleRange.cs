using System;
using NHibernate.Validator.Constraints;
using NHibernate.Validator.Engine;

namespace FSNEP.Core.Validators
{
    [Serializable]
    public class RangeDoubleValidator : IInitializableValidator<RangeDoubleAttribute>
    {
        private double max;
        private double min;

        #region IInitializableValidator<RangeAttribute> Members

        public void Initialize(RangeDoubleAttribute parameters)
        {
            max = parameters.Max;
            min = parameters.Min;
        }

        public bool IsValid(object value, IConstraintValidatorContext validatorContext)
        {
            if (value == null)
            {
                return true;
            }

            try
            {
                double cvalue = Convert.ToDouble(value);
                return cvalue >= min && cvalue <= max;
            }
            catch (InvalidCastException)
            {
                if (value is char)
                {
                    int i = Convert.ToInt32(value);
                    return i >= min && i <= max;
                }
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
        }

        #endregion
    }

    /// <summary>
    /// The annotated elemnt has to be in the appropriate range. Apply on numeric values or string
    /// representation of the numeric value.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [ValidatorClass(typeof(RangeDoubleValidator))]
    public class RangeDoubleAttribute : Attribute, IRuleArgs
    {
        private double max = double.MaxValue;
        private string message = "{validator.range}";
        private double min = double.MinValue;

        public RangeDoubleAttribute(double min, double max)
        {
            this.min = min;
            this.max = max;
        }

        public RangeDoubleAttribute(double min, double max, string message)
        {
            this.min = min;
            this.max = max;
            this.message = message;
        }

        public RangeDoubleAttribute() { }

        public double Min
        {
            get { return min; }
            set { min = value; }
        }

        public double Max
        {
            get { return max; }
            set { max = value; }
        }

        #region IRuleArgs Members

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        #endregion
    }
}