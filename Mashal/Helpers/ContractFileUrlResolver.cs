using AutoMapper;
using Core.DTOs.App;
using Core.Models;

namespace lawyer.Api.Helpers
{
    public class ContractFileUrlResolver : IValueResolver<ContractTemplate, ContractTemplateDto, string?>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContractFileUrlResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Resolve(ContractTemplate source, ContractTemplateDto destination, string? destMember, ResolutionContext context)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null || string.IsNullOrEmpty(source.FileUrl))
                return null;

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}{source.FileUrl}";
        }

    }
}
