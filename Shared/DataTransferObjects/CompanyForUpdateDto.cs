namespace Shared.DataTransferObjects
{
    public record CompanyForUpdateDto
    (
        string Name,
        string Address,
        string Country,
        IEnumerable<EmployeeForUpdateDto> Employees
    );
}