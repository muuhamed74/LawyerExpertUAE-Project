using AutoMapper;
using Core.DTOs.App;
using Core.Models;

namespace lawyer.Api.Helpers.Resolvers
{
    public class ContractImageResolver : IValueResolver<ContractTemplate, ContractTemplateDto, string?>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContractImageResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Resolve(ContractTemplate source, ContractTemplateDto destination, string? destMember, ResolutionContext context)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null || string.IsNullOrEmpty(source.ImageUrl))
                return null;

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}{source.ImageUrl}";
        }
    }
}
