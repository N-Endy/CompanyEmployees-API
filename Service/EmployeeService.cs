using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;
public class EmployeeService : IEmployeeService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
    {
        _ = await _repository.Company.GetCompanyAsync(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);

        var employeeEntity = _mapper.Map<Employee>(employeeForCreation);

        _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
        await _repository.SaveAsync();

        var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

        return employeeToReturn;
    }

    public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);
        
        var employeeFromDb = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges) ?? throw new EmployeeNotFoundException(id);

        var employeeDto = _mapper.Map<EmployeeDto>(employeeFromDb);

        return employeeDto;
    }

    public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId, bool trackChanges)
    {
        var employees = await _repository.Employee.GetEmployeesAsync(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);

        var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, trackChanges);
        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

        return employeesDto;
    }

    public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges)?? throw new CompanyNotFoundException(companyId);

        var employeeToDelete = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges)?? throw new EmployeeNotFoundException(id);

        _repository.Employee.DeleteEmployee(employeeToDelete);
        await _repository.SaveAsync();
    }

    public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdateDto, bool compTrackChanges, bool empTrackChanges)
    {
        _ = await _repository.Company.GetCompanyAsync(companyId, compTrackChanges)?? throw new CompanyNotFoundException(companyId);

        var employeeFromDb = await _repository.Employee.GetEmployeeAsync(companyId, id, empTrackChanges)?? throw new EmployeeNotFoundException(id);

        _mapper.Map(employeeForUpdateDto, employeeFromDb);
        await _repository.SaveAsync();
    }

    public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
    {
        _ = await _repository.Company.GetCompanyAsync(companyId, compTrackChanges) ?? throw new CompanyNotFoundException(companyId);

        var employeeFromDb = await _repository.Employee.GetEmployeeAsync(companyId, id, empTrackChanges)?? throw new EmployeeNotFoundException(id);

        var employeeToBePatch = _mapper.Map<EmployeeForUpdateDto>(employeeFromDb);

        return (employeeToBePatch, employeeFromDb);
    }

    public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
    {
        _mapper.Map(employeeToPatch, employeeEntity);
        await _repository.SaveAsync();
    }
}