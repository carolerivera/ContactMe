using ContactMe.DataAccess.Models;
using ContactMe.Models;

namespace ContactMe.App_Start
{
    /// <summary>
    /// Create maps between view models and data models using AutoMapper.
    /// </summary>
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            AutoMapper.Mapper.Initialize(x => x.CreateMap<ContactModel, Contact>());
        }
    }
}