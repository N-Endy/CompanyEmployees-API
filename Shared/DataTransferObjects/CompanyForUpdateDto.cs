namespace Shared.DataTransferObjects
{
    public record CompanyForUpdateDto : CompanyForManipulationDto
    {
        IEnumerable<EmployeeForUpdateDto>? Employees { get; init; }
    }
}