using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

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
        await CheckIfCompanyExists(companyId, trackChanges);

        var employeeEntity = _mapper.Map<Employee>(employeeForCreation);

        _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
        await _repository.SaveAsync();

        var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

        return employeeToReturn;
    }

    public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId, trackChanges);
        
        var employeeFromDb = await GetEmployeeForCompanyAdCheckIfItExists(companyId, id, trackChanges);

        var employeeDto = _mapper.Map<EmployeeDto>(employeeFromDb);

        return employeeDto;
    }

    public async Task<(IEnumerable<EmployeeDto> employees, MetaData metaData)> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
    {
        if (!employeeParameters.ValidAgeRange)
            throw new MaxAgeRangeBadRequestException();

        await CheckIfCompanyExists(companyId, trackChanges);

        var employeesWithMetaData = await _repository.Employee.GetEmployeesAsync(companyId, employeeParameters, trackChanges);
        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesWithMetaData);

        return (employees: employeesDto, metaData: employeesWithMetaData.MetaData);
    }

    public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId, trackChanges);

        var employeeToDelete = await GetEmployeeForCompanyAdCheckIfItExists(companyId, id, trackChanges);

        _repository.Employee.DeleteEmployee(employeeToDelete);
        await _repository.SaveAsync();
    }

    public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdateDto, bool compTrackChanges, bool empTrackChanges)
    {
        await CheckIfCompanyExists(companyId, compTrackChanges);

        var employeeFromDb = await GetEmployeeForCompanyAdCheckIfItExists(companyId, id, empTrackChanges);

        _mapper.Map(employeeForUpdateDto, employeeFromDb);
        await _repository.SaveAsync();
    }

    public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
    {
        await CheckIfCompanyExists(companyId, compTrackChanges);

        var employeeFromDb = await GetEmployeeForCompanyAdCheckIfItExists(companyId, id, empTrackChanges);

        var employeeToBePatch = _mapper.Map<EmployeeForUpdateDto>(employeeFromDb);

        return (employeeToBePatch, employeeFromDb);
    }

    public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
    {
        _mapper.Map(employeeToPatch, employeeEntity);
        await _repository.SaveAsync();
    }

    private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
    {
        _ = await _repository.Company.GetCompanyAsync(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);
    }

    private async Task<Employee> GetEmployeeForCompanyAdCheckIfItExists(Guid companyId, Guid id, bool trackChanges)
    {
        var employee = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges)?? throw new EmployeeNotFoundException(id);

        return employee;
    }
}