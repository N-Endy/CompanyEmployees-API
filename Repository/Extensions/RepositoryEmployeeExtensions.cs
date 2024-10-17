using Entities.Models;

namespace Repository;
public static class RepositoryEmployeeExtensions
{
    public static IQueryable<Employee> FilterEmployees(this IQueryable<Employee> employees, uint minAge, uint MaxAge) =>
        employees.Where(e => e.Age >= minAge && e.Age <= MaxAge);

    public static IQueryable<Employee> Search(this IQueryable<Employee> employees, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return employees;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return employees.Where(e => e.Name != null && e.Name.ToLower().Contains(lowerCaseTerm));
    }
}