using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmptyAPI.DTOs
{
    public class DataFilterDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AcceptorEmail { get; set; }

    }

    public class DataFilterDtoValidator : AbstractValidator<DataFilterDto>
    {
        public DataFilterDtoValidator()
        {
            RuleFor(s => s.StartDate).NotEmpty().WithMessage("can't be empty");
            RuleFor(s => s.EndDate).NotEmpty().WithMessage("can't be empty");
            RuleFor(s => s.AcceptorEmail).NotEmpty().WithMessage("Email adress is required").EmailAddress().WithMessage("A valid email is required");
            RuleFor(c => c).Custom((c, context) =>
            {
                string[] arr = c.AcceptorEmail.Split('@');
                if (arr[1].ToLower().Trim() != "code.edu.az")
                {
                    context.AddFailure("AcceptorEmail", "only domain name code.edu.az");
                }
            });
            RuleFor(c => c).Custom((c, context) =>
            {
                double time = (c.EndDate - c.StartDate).TotalMilliseconds;
                if (time < 0)
                {
                    context.AddFailure("EndData", "wrong date");
                }
            });
        }

    }

    public enum Filter
    {
        Segment = 1,
        Country,
        Product,
        Discount
    }
}
