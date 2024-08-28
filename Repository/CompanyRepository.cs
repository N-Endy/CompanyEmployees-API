using Contracts;
using Entities.Models;

namespace Repository;
internal sealed class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
    public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
        
    }

    public IEnumerable<Company> GetAllCompanies(bool trackChanges) =>
        [.. FindAll(trackChanges).OrderBy(c => c.Name)];

    public Company GetCompany(Guid companyId, bool trackChanges) =>
        FindByCondition(c => c.Id.Equals(companyId), trackChanges)
            .SingleOrDefault();
}
