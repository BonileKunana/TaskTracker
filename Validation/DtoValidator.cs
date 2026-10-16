
using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.Validation;

public static class DtoValidator
{
	public static Dictionary<string, string[]> Validate<T>(T model)
	{
		var context = new ValidationContext(model!);
		var results = new List<ValidationResult>();

		Validator.TryValidateObject(
			model!,
			context,
			results,
			validateAllProperties: true);

		return results
			.SelectMany(result =>
				(result.MemberNames.Any()
					? result.MemberNames
					: new[] { string.Empty })
				.Select(member => new
				{
					Member = member,
					Error = result.ErrorMessage ?? "Invalid value."
				}))
			.GroupBy(error => error.Member)
			.ToDictionary(
				group => group.Key,
				group => group.Select(error => error.Error).ToArray());
	}
}
